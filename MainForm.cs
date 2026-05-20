
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace DevExpressTreeListDemo
{
    public partial class MainForm : Form
    {
        private NodeClipboardData clipboardData;

        private sealed class NodeClipboardData
        {
            public object[] Values { get; set; }
            public System.Collections.Generic.List<NodeClipboardData> Children { get; set; }
        }

        public MainForm()
        {
            InitializeComponent();
            BuildTree();
            treeList1.ExpandAll();
        }

        private void treeList1_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            e.Allow = false;
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

            AssignItemNumbers();

            treeList1.EndUnboundLoad();
        }

        private void AssignItemNumbers()
        {
            for (int i = 0; i < treeList1.Nodes.Count; i++)
            {
                string rootCode = (i + 1).ToString("D2");
                AssignNodeCode(treeList1.Nodes[i], rootCode);
            }
        }

        private void AssignNodeCode(TreeListNode node, string code)
        {
            node.SetValue(0, code);

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                string childCode = code + "." + (i + 1).ToString("D2");
                AssignNodeCode(node.Nodes[i], childCode);
            }
        }

        private static object[] CreateNewNodeValues()
        {
            return new object[] { string.Empty, "Nuevo item", "Item", "Activa" };
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            TreeListHitInfo hitInfo = treeList1.CalcHitInfo(e.Location);

            // Si la grilla está vacía, solo mostrar el menú vacío si el clic no es sobre ninguna celda
            if (treeList1.Nodes.Count == 0 && (hitInfo.Node == null || hitInfo.HitInfoType != HitInfoType.Cell))
            {
                emptyContextMenu.Show(treeList1, e.Location);
                return;
            }

            // Si hay filas, solo mostrar el menú contextual si el clic es sobre una celda válida
            if (treeList1.Nodes.Count > 0 && hitInfo.HitInfoType == HitInfoType.Cell && hitInfo.Node != null)
            {
                treeList1.FocusedNode = hitInfo.Node;
                treeList1.FocusedColumn = hitInfo.Column;
                menuPasteItem.Enabled = clipboardData != null;
                menuPasteItemBelow.Enabled = clipboardData != null;
                treeContextMenu.Show(treeList1, e.Location);
            }
        }

        private void menuAddRootItem_Click(object sender, System.EventArgs e)
        {
            treeList1.BeginUnboundLoad();
            treeList1.AppendNode(CreateNewNodeValues(), null);
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuAddAbove_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            treeList1.BeginUnboundLoad();
            TreeListNode parent = selectedNode.ParentNode;
            int selIndex = treeList1.GetNodeIndex(selectedNode);
            object[] nodeData = CreateNewNodeValues();
            TreeListNode newNode = treeList1.AppendNode(nodeData, parent);
            treeList1.SetNodeIndex(newNode, selIndex);
            treeList1.FocusedNode = newNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuAddBelow_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            treeList1.BeginUnboundLoad();
            TreeListNode parent = selectedNode.ParentNode;
            int selIndex = treeList1.GetNodeIndex(selectedNode);
            object[] nodeData = CreateNewNodeValues();
            TreeListNode newNode = treeList1.AppendNode(nodeData, parent);
            treeList1.SetNodeIndex(newNode, selIndex + 1);
            treeList1.FocusedNode = newNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuAddSubItem_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            treeList1.BeginUnboundLoad();
            TreeListNode newNode = treeList1.AppendNode(CreateNewNodeValues(), selectedNode);
            selectedNode.Expanded = true;
            treeList1.FocusedNode = newNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuDeleteItem_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            treeList1.BeginUnboundLoad();
            selectedNode.Remove();
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuCopyItem_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            clipboardData = CaptureNode(selectedNode);
        }

        private void menuCutItem_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            clipboardData = CaptureNode(selectedNode);

            treeList1.BeginUnboundLoad();
            selectedNode.Remove();
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuPasteItem_Click(object sender, System.EventArgs e)
        {
            if (clipboardData == null)
                return;

            treeList1.BeginUnboundLoad();

            TreeListNode targetNode = treeList1.FocusedNode;
            TreeListNode targetParent = targetNode == null ? null : targetNode.ParentNode;
            TreeListNode newNode = AppendNodeFromClipboard(targetParent, clipboardData);

            if (targetNode != null)
            {
                int targetIndex = treeList1.GetNodeIndex(targetNode);
                treeList1.SetNodeIndex(newNode, targetIndex);
            }

            treeList1.FocusedNode = newNode;
            treeList1.ExpandAll();
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuPasteItemBelow_Click(object sender, System.EventArgs e)
        {
            if (clipboardData == null)
                return;

            treeList1.BeginUnboundLoad();

            TreeListNode targetNode = treeList1.FocusedNode;
            TreeListNode targetParent = targetNode == null ? null : targetNode.ParentNode;
            TreeListNode newNode = AppendNodeFromClipboard(targetParent, clipboardData);

            if (targetNode != null)
            {
                int targetIndex = treeList1.GetNodeIndex(targetNode);
                treeList1.SetNodeIndex(newNode, targetIndex + 1);
            }

            treeList1.FocusedNode = newNode;
            treeList1.ExpandAll();
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuMoveRight_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            TreeListNode[] nodes = new[] { selectedNode };
            if (!treeList1.CanIndentNodes(nodes))
                return;

            treeList1.BeginUnboundLoad();
            treeList1.IndentNodes(nodes, true);
            treeList1.FocusedNode = selectedNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuMoveLeft_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            TreeListNode[] nodes = new[] { selectedNode };
            if (!treeList1.CanOutdentNodes(nodes))
                return;

            treeList1.BeginUnboundLoad();
            treeList1.OutdentNodes(nodes, true);
            treeList1.FocusedNode = selectedNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuMoveUp_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            int currentIndex = treeList1.GetNodeIndex(selectedNode);
            if (currentIndex <= 0)
                return;

            treeList1.BeginUnboundLoad();
            treeList1.SetNodeIndex(selectedNode, currentIndex - 1);
            treeList1.FocusedNode = selectedNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private void menuMoveDown_Click(object sender, System.EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            TreeListNode parent = selectedNode.ParentNode;
            int currentIndex = treeList1.GetNodeIndex(selectedNode);
            int siblingCount = parent == null ? treeList1.Nodes.Count : parent.Nodes.Count;
            if (currentIndex >= siblingCount - 1)
                return;

            treeList1.BeginUnboundLoad();
            treeList1.SetNodeIndex(selectedNode, currentIndex + 1);
            treeList1.FocusedNode = selectedNode;
            AssignItemNumbers();
            treeList1.EndUnboundLoad();
        }

        private NodeClipboardData CaptureNode(TreeListNode node)
        {
            NodeClipboardData data = new NodeClipboardData
            {
                Values = new object[] { node.GetValue(0), node.GetValue(1), node.GetValue(2), node.GetValue(3) },
                Children = new System.Collections.Generic.List<NodeClipboardData>()
            };

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                data.Children.Add(CaptureNode(node.Nodes[i]));
            }

            return data;
        }

        private TreeListNode AppendNodeFromClipboard(TreeListNode parent, NodeClipboardData data)
        {
            TreeListNode node = treeList1.AppendNode((object[])data.Values.Clone(), parent);

            for (int i = 0; i < data.Children.Count; i++)
            {
                AppendNodeFromClipboard(node, data.Children[i]);
            }

            return node;
        }
    }
}
