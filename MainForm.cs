using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Data;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    public partial class MainForm : Form
    {
        private ResourceTypePolicy resourceTypePolicy;
        private ResourceTypeEditorService resourceTypeEditorService;
        private readonly TreeListItemService treeListItemService;
        private NodeClipboardData clipboardData;
        private DataTable resourceTypesTable;

        public MainForm()
        {
            InitializeComponent();

            treeListItemService = new TreeListItemService(treeList1, null);
            ConfigureResourceTypeColumnEditor();
            BuildTree();
            treeList1.ExpandAll();
        }

        private void ConfigureResourceTypeColumnEditor()
        {
            var resourceTypes = Datos.ObtenerTiposRecurso(1, true);

            treeList1.OptionsBehavior.Editable = true;
            for (int i = 0; i < treeList1.Columns.Count; i++)
                treeList1.Columns[i].OptionsColumn.AllowEdit = false;

            var resourceTypeColumn = treeList1.Columns[1];
            resourceTypeColumn.Caption = "Tipo Recurso";
            resourceTypeColumn.OptionsColumn.AllowEdit = true;

            var table = new DataTable();
            table.Columns.Add("EmpresaId", typeof(int));
            table.Columns.Add("TipoRecursoId", typeof(int));
            table.Columns.Add("TipoRecurso", typeof(string));
            table.Columns.Add("Activo", typeof(bool));
            for (int i = 0; i < resourceTypes.Count; i++)
            {
                var item = resourceTypes[i];
                table.Rows.Add(item.EmpresaId, item.TipoRecursoId, item.TipoRecurso, item.Activo);
            }

            resourceTypesTable = table;
            resourceTypePolicy = new ResourceTypePolicy(GetResourceTypeNames(table));
            treeListItemService.SetPolicy(resourceTypePolicy);

            resourceTypeEditorService = new ResourceTypeEditorService(resourceTypePolicy, table);
            resourceTypeEditorService.Attach(treeList1, resourceTypeColumn);
        }

        private static System.Collections.Generic.IEnumerable<string> GetResourceTypeNames(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
                yield return Convert.ToString(table.Rows[i]["TipoRecurso"]);
        }

        private void BuildTree()
        {
            treeList1.BeginUnboundLoad();

            TreeListNode rootFinance = treeList1.AppendNode(new object[] { string.Empty, "Finanzas", "Departamento", "Activa" }, parentNode: null);
            treeList1.AppendNode(new object[] { string.Empty, "Contabilidad", "Area", "Activa" }, rootFinance);
            treeList1.AppendNode(new object[] { string.Empty, "Tesoreria", "Area", "Activa" }, rootFinance);

            TreeListNode rootOperations = treeList1.AppendNode(new object[] { string.Empty, "Operaciones", "Departamento", "Activa" }, parentNode: null);
            TreeListNode logistics = treeList1.AppendNode(new object[] { string.Empty, "Logistica", "Area", "Activa" }, rootOperations);
            treeList1.AppendNode(new object[] { string.Empty, "Compras", "Equipo", "Activa" }, logistics);
            treeList1.AppendNode(new object[] { string.Empty, "Inventarios", "Equipo", "Pausada" }, logistics);

            TreeListNode rootTechnology = treeList1.AppendNode(new object[] { string.Empty, "Tecnologia", "Departamento", "Activa" }, parentNode: null);
            treeList1.AppendNode(new object[] { string.Empty, "Desarrollo", "Area", "Activa" }, rootTechnology);
            treeList1.AppendNode(new object[] { string.Empty, "QA", "Area", "Activa" }, rootTechnology);

            treeListItemService.AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void treeList1_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            e.Allow = false;
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            TreeListHitInfo hitInfo = treeList1.CalcHitInfo(e.Location);

            if (treeList1.Nodes.Count == 0 && (hitInfo.Node == null || hitInfo.HitInfoType != HitInfoType.Cell))
            {
                emptyContextMenu.Show(treeList1, e.Location);
                return;
            }

            if (treeList1.Nodes.Count > 0 && hitInfo.HitInfoType == HitInfoType.Cell && hitInfo.Node != null)
            {
                treeList1.FocusedNode = hitInfo.Node;
                treeList1.FocusedColumn = hitInfo.Column;
                UpdateContextMenuVisibility(hitInfo.Node);
                treeContextMenu.Show(treeList1, e.Location);
            }
        }

        private void menuAddRootItem_Click(object sender, EventArgs e)
        {
            treeListItemService.AddRootItem();
        }

        private void menuAddAbove_Click(object sender, EventArgs e)
        {
            treeListItemService.AddAbove(treeList1.FocusedNode);
        }

        private void menuAddBelow_Click(object sender, EventArgs e)
        {
            treeListItemService.AddBelow(treeList1.FocusedNode);
        }

        private void menuAddSubItem_Click(object sender, EventArgs e)
        {
            if (!treeListItemService.AddSubItem(treeList1.FocusedNode, out string validationMessage))
            {
                if (!string.IsNullOrEmpty(validationMessage))
                    MessageBox.Show(validationMessage, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuDeleteItem_Click(object sender, EventArgs e)
        {
            treeListItemService.Delete(treeList1.FocusedNode);
        }

        private void menuCopyItem_Click(object sender, EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            clipboardData = treeListItemService.CaptureNode(selectedNode);
        }

        private void menuCutItem_Click(object sender, EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            treeListItemService.Cut(selectedNode, data => clipboardData = data);
        }

        private void menuPasteItem_Click(object sender, EventArgs e)
        {
            if (!treeListItemService.Paste(treeList1.FocusedNode, clipboardData, false, out string validationMessage))
            {
                if (!string.IsNullOrEmpty(validationMessage))
                    MessageBox.Show(validationMessage, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuPasteItemBelow_Click(object sender, EventArgs e)
        {
            if (!treeListItemService.Paste(treeList1.FocusedNode, clipboardData, true, out string validationMessage))
            {
                if (!string.IsNullOrEmpty(validationMessage))
                    MessageBox.Show(validationMessage, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuMoveRight_Click(object sender, EventArgs e)
        {
            treeListItemService.MoveRight(treeList1.FocusedNode);
        }

        private void menuMoveLeft_Click(object sender, EventArgs e)
        {
            treeListItemService.MoveLeft(treeList1.FocusedNode);
        }

        private void menuMoveUp_Click(object sender, EventArgs e)
        {
            treeListItemService.MoveUp(treeList1.FocusedNode);
        }

        private void menuMoveDown_Click(object sender, EventArgs e)
        {
            treeListItemService.MoveDown(treeList1.FocusedNode);
        }

        private void UpdateContextMenuVisibility(TreeListNode targetNode)
        {
            string selectedType = resourceTypePolicy.NormalizeTypeName(targetNode.GetValue(1));
            TreeListNode targetParent = targetNode.ParentNode;
            TreeListNode newParentOnIndent = GetPreviousSibling(targetNode);
            TreeListNode newParentOnOutdent = targetParent == null ? null : targetParent.ParentNode;
            TreeListNode[] nodes = new[] { targetNode };

            bool canPasteHere = clipboardData != null
                && resourceTypePolicy.HasAssignedResourceType(targetNode)
                && resourceTypePolicy.CanBePlacedUnder(targetParent, resourceTypePolicy.NormalizeTypeName(clipboardData.Values[1]));
            menuPasteItem.Visible = canPasteHere;
            menuPasteItemBelow.Visible = canPasteHere;
            menuAddSubItem.Visible = resourceTypePolicy.CanParentAcceptChildren(targetNode) && resourceTypePolicy.HasAssignedResourceType(targetNode);

            menuMoveRight.Visible = treeList1.CanIndentNodes(nodes) && resourceTypePolicy.CanBePlacedUnder(newParentOnIndent, selectedType);
            menuMoveLeft.Visible = treeList1.CanOutdentNodes(nodes) && resourceTypePolicy.CanBePlacedUnder(newParentOnOutdent, selectedType);
        }

        private TreeListNode GetPreviousSibling(TreeListNode node)
        {
            if (node == null)
                return null;

            TreeListNode parent = node.ParentNode;
            int index = treeList1.GetNodeIndex(node);
            if (index <= 0)
                return null;

            return parent == null ? treeList1.Nodes[index - 1] : parent.Nodes[index - 1];
        }
    }
}