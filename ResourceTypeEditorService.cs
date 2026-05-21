using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System.Data;

namespace DevExpressTreeListDemo
{
    internal sealed class ResourceTypeEditorService
    {
        private const int ResourceTypeColumnVisibleIndex = 1;
        private const string ResourceTypeColumnName = "TipoRecurso";
        private const string ResourceTypeIdColumnName = "TipoRecursoId";

        private readonly ResourceTypePolicy policy;
        private readonly DataTable allResourceTypesTable;
        private readonly RepositoryItemGridLookUpEdit displayEditor;
        private readonly RepositoryItemGridLookUpEdit editingEditor;

        public ResourceTypeEditorService(ResourceTypePolicy policy, DataTable allResourceTypesTable)
        {
            this.policy = policy;
            this.allResourceTypesTable = allResourceTypesTable;
            displayEditor = CreateEditor(allResourceTypesTable);
            editingEditor = CreateEditor(allResourceTypesTable);
            ConfigurePopupView(displayEditor);
            ConfigurePopupView(editingEditor);
        }

        public void Attach(TreeList treeList, TreeListColumn resourceTypeColumn)
        {
            treeList.RepositoryItems.Add(displayEditor);
            treeList.RepositoryItems.Add(editingEditor);
            treeList.CustomNodeCellEditForEditing += TreeList_CustomNodeCellEditForEditing;
            resourceTypeColumn.ColumnEdit = displayEditor;
        }

        private void TreeList_CustomNodeCellEditForEditing(object sender, GetCustomNodeCellEditEventArgs e)
        {
            if (e.Column == null || e.Column.VisibleIndex != ResourceTypeColumnVisibleIndex || e.Node == null)
                return;

            editingEditor.DataSource = policy.BuildAllowedResourceTypesTable(e.Node, allResourceTypesTable);
            e.RepositoryItem = editingEditor;
        }

        private static RepositoryItemGridLookUpEdit CreateEditor(DataTable table)
        {
            var editor = new RepositoryItemGridLookUpEdit
            {
                TextEditStyle = TextEditStyles.Standard,
                NullText = string.Empty,
                ImmediatePopup = true,
                PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains,
                DataSource = table,
                DisplayMember = ResourceTypeColumnName,
                ValueMember = ResourceTypeIdColumnName
            };

            return editor;
        }

        private static void ConfigurePopupView(RepositoryItemGridLookUpEdit editor)
        {
            GridView popupView = editor.View;
            popupView.Columns.Clear();
            popupView.Columns.AddVisible(ResourceTypeColumnName, "Tipo de recurso");
            popupView.OptionsView.ShowColumnHeaders = false;
            popupView.OptionsView.ShowIndicator = false;
            popupView.OptionsSelection.EnableAppearanceFocusedCell = false;
            popupView.OptionsView.ShowAutoFilterRow = false;
            popupView.OptionsView.ShowGroupPanel = false;
            popupView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
        }
    }
}