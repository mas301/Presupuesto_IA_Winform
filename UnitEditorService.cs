using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using System.Data;

namespace PresupuestoIA
{
    internal sealed class UnitEditorService
    {
        private const string UnitIdColumn = "UnidadId";
        private const string UnitDisplayColumn = "UnidadDisplay";

        private readonly RepositoryItemLookUpEdit displayEditor;

        public UnitEditorService(DataTable unitsTable)
        {
            displayEditor = CreateEditor(unitsTable);
        }

        public void Attach(TreeList treeList, TreeListColumn unitColumn)
        {
            treeList.RepositoryItems.Add(displayEditor);
            unitColumn.ColumnEdit = displayEditor;
        }

        private static RepositoryItemLookUpEdit CreateEditor(DataTable unitsTable)
        {
            var editor = new RepositoryItemLookUpEdit
            {
                TextEditStyle = TextEditStyles.DisableTextEditor,
                NullText = string.Empty,
                DataSource = unitsTable,
                DisplayMember = UnitDisplayColumn,
                ValueMember = UnitIdColumn,
                SearchMode = SearchMode.AutoFilter,
                ImmediatePopup = false,
                ShowHeader = false,
                ShowFooter = false
            };

            editor.Columns.Clear();
            editor.Columns.Add(new LookUpColumnInfo(UnitDisplayColumn, "Unidad"));
            return editor;
        }
    }
}
