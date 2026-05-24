using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace PresupuestoIA
{
    internal sealed class CalculationTypeEditorService
    {
        private readonly RepositoryItemGridLookUpEdit editor;

        public CalculationTypeEditorService(object dataSource)
        {
            editor = new RepositoryItemGridLookUpEdit
            {
                TextEditStyle = TextEditStyles.Standard,
                NullText = string.Empty,
                ImmediatePopup = true,
                PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains,
                DataSource = dataSource,
                DisplayMember = "TipoCalculo",
                ValueMember = "TipoCalculoId"
            };

            ConfigurePopupView(editor);
        }

        public void Attach(TreeList treeList, TreeListColumn calculationTypeColumn)
        {
            treeList.RepositoryItems.Add(editor);
            calculationTypeColumn.ColumnEdit = editor;
        }

        private static void ConfigurePopupView(RepositoryItemGridLookUpEdit editor)
        {
            GridView popupView = editor.View;
            popupView.Columns.Clear();
            popupView.Columns.AddVisible("CodigoTipoCalculoId", "Codigo");
            popupView.Columns.AddVisible("TipoCalculo", "Tipo de calculo");
            popupView.OptionsView.ShowIndicator = false;
            popupView.OptionsSelection.EnableAppearanceFocusedCell = false;
            popupView.OptionsView.ShowAutoFilterRow = true;
            popupView.OptionsView.ShowGroupPanel = false;
            popupView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
        }
    }
}
