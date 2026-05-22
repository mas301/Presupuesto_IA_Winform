using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Data;

namespace DevExpressTreeListDemo
{
    internal sealed class ResourceEditorService
    {
        private const int ResourceTypeColumnIndex = 1;
        private const string ResourceTypeIdColumn = "TipoRecursoId";
        private const string ResourceIdColumn = "RecursoId";
        private const string ResourceNameColumn = "Recurso";

        private readonly DataTable allResourcesTable;
        private readonly RepositoryItemGridLookUpEdit displayEditor;
        private readonly RepositoryItemGridLookUpEdit editingEditor;
        private TreeListColumn attachedResourceColumn;

        public ResourceEditorService(DataTable allResourcesTable)
        {
            this.allResourcesTable = allResourcesTable;
            displayEditor = CreateEditor(allResourcesTable);
            editingEditor = CreateEditor(allResourcesTable);
            ConfigurePopupView(displayEditor);
            ConfigurePopupView(editingEditor);
        }

        public void Attach(TreeList treeList, TreeListColumn resourceColumn)
        {
            attachedResourceColumn = resourceColumn;
            treeList.RepositoryItems.Add(displayEditor);
            treeList.RepositoryItems.Add(editingEditor);
            treeList.CustomNodeCellEditForEditing += TreeList_CustomNodeCellEditForEditing;
            resourceColumn.ColumnEdit = displayEditor;
        }

        private void TreeList_CustomNodeCellEditForEditing(object sender, GetCustomNodeCellEditEventArgs e)
        {
            if (e.Column == null || e.Node == null || attachedResourceColumn == null || e.Column != attachedResourceColumn)
                return;

            DataTable filteredResources = BuildFilteredResourcesTable(e.Node);
            editingEditor.DataSource = filteredResources;
            e.RepositoryItem = editingEditor;
        }

        private DataTable BuildFilteredResourcesTable(TreeListNode node)
        {
            var filtered = allResourcesTable.Clone();
            int? tipoRecursoId = GetSelectedResourceTypeId(node);
            if (!tipoRecursoId.HasValue)
                return filtered;

            for (int i = 0; i < allResourcesTable.Rows.Count; i++)
            {
                DataRow row = allResourcesTable.Rows[i];
                if (Convert.ToInt32(row[ResourceTypeIdColumn]) == tipoRecursoId.Value)
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private int? GetSelectedResourceTypeId(TreeListNode node)
        {
            object value = node.GetValue(ResourceTypeColumnIndex);
            if (value == null)
                return null;

            if (value is int resourceTypeId)
                return resourceTypeId;

            if (int.TryParse(Convert.ToString(value), out int parsedTypeId))
                return parsedTypeId;

            return null;
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
                DisplayMember = ResourceNameColumn,
                ValueMember = ResourceIdColumn
            };

            return editor;
        }

        private static void ConfigurePopupView(RepositoryItemGridLookUpEdit editor)
        {
            GridView popupView = editor.View;
            popupView.Columns.Clear();
            popupView.Columns.AddVisible(ResourceNameColumn, "Recurso");
            popupView.OptionsView.ShowIndicator = false;
            popupView.OptionsSelection.EnableAppearanceFocusedCell = false;
            popupView.OptionsView.ShowAutoFilterRow = true;
            popupView.OptionsView.ShowGroupPanel = false;
            popupView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
        }
    }
}
