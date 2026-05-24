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
        private const string ResourceTypeFieldName = "TipoRecurso";
        private const string ResourceTypeIdColumn = "TipoRecursoId";
        private const string ResourceIdColumn = "RecursoId";
        private const string ResourceNameColumn = "Recurso";

        private readonly DataTable allResourcesTable;
        private readonly RepositoryItemGridLookUpEdit displayEditor;
        private readonly RepositoryItemGridLookUpEdit editingEditor;
        private TreeListColumn attachedResourceColumn;
        private TreeList attachedTreeList;

        public event EventHandler<ResourceCreationRequestedEventArgs> ResourceCreationRequested;

        public ResourceEditorService(DataTable allResourcesTable)
        {
            this.allResourcesTable = allResourcesTable;
            displayEditor = CreateEditor(allResourcesTable);
            editingEditor = CreateEditor(allResourcesTable);
            ConfigurePopupView(displayEditor);
            ConfigurePopupView(editingEditor);
            editingEditor.ProcessNewValue += EditingEditor_ProcessNewValue;
        }

        public void Attach(TreeList treeList, TreeListColumn resourceColumn)
        {
            attachedTreeList = treeList;
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
            if (node == null || node.TreeList == null)
                return null;

            var column = node.TreeList.Columns[ResourceTypeFieldName];
            if (column == null)
                return null;

            object value = node.GetValue(column);
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

        private void EditingEditor_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            if (attachedTreeList == null || attachedResourceColumn == null)
                return;

            string typedText = Convert.ToString(e.DisplayValue);
            if (string.IsNullOrWhiteSpace(typedText))
                return;

            TreeListNode node = attachedTreeList.FocusedNode;
            if (node == null)
                return;

            int? tipoRecursoId = GetSelectedResourceTypeId(node);

            // Revert the editor to the previous value to suppress invalid-value handling.
            object previousValue = node.GetValue(attachedResourceColumn);
            e.DisplayValue = previousValue;
            e.Handled = true;

            EventHandler<ResourceCreationRequestedEventArgs> handler = ResourceCreationRequested;
            if (handler == null)
                return;

            TreeList treeListRef = attachedTreeList;
            TreeListColumn columnRef = attachedResourceColumn;
            TreeListNode capturedNode = node;
            string capturedText = typedText.Trim();
            int? capturedTipo = tipoRecursoId;

            // Defer execution so the editor finishes closing before opening the modal form.
            treeListRef.BeginInvoke(new Action(() =>
            {
                var args = new ResourceCreationRequestedEventArgs(capturedNode, capturedText, capturedTipo);
                handler(this, args);
                if (args.CreatedResourceId.HasValue)
                    capturedNode.SetValue(columnRef, args.CreatedResourceId.Value);
            }));
        }
    }

    internal sealed class ResourceCreationRequestedEventArgs : EventArgs
    {
        public ResourceCreationRequestedEventArgs(TreeListNode node, string typedText, int? tipoRecursoId)
        {
            Node = node;
            TypedText = typedText;
            TipoRecursoId = tipoRecursoId;
        }

        public TreeListNode Node { get; }
        public string TypedText { get; }
        public int? TipoRecursoId { get; }
        public int? CreatedResourceId { get; set; }
    }
}
