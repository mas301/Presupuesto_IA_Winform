using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    public partial class MainForm : Form
    {
        private const int EmpresaId = 1;
        private const int PresupuestoId = 1;
        private const int ResourceTypeColumnIndex = 1;
        private const int ResourceTypeValueIndex = 1;
        private const int ResourceColumnIndex = 2;
        private const int UnitColumnIndex = 3;
        private const int CalculationTypeColumnIndex = 4;
        private const int HoursPerDayColumnIndex = 5;
        private const int PerformanceColumnIndex = 6;
        private const int CrewColumnIndex = 7;
        private const int QuantityColumnIndex = 8;
        private const int UnitValueColumnIndex = 9;
        private const int TotalValueColumnIndex = 10;
        private const int AliasColumnIndex = 11;
        private const int DefaultGrabacionAutomaticaSegundos = 300;

        private ResourceTypePolicy resourceTypePolicy;
        private ResourceTypeEditorService resourceTypeEditorService;
        private ResourceEditorService resourceEditorService;
        private UnitEditorService unitEditorService;
        private CalculationTypeEditorService calculationTypeEditorService;
        private readonly TreeListItemService treeListItemService;
        private readonly Timer autoSaveTimer;
        private NodeClipboardData clipboardData;
        private bool suppressPersistence;
        private bool hasPendingAutoSaveChanges;
        private DateTime lastAutoSaveUtc;
        private decimal horasJornalPresupuestoActual;
        private int grabacionAutomatica = DefaultGrabacionAutomaticaSegundos;
        private int inicioNumeracion = 1;
        private bool columnaTipoCalculoVisible = true;
        private bool columnaRendimientoVisible = true;
        private bool columnaHorasJornalVisible = true;
        private bool columnaCuadrillaVisible = true;
        private Dictionary<int, string> resourceNamesById;
        private Dictionary<int, int?> resourceUnitIdsByResourceId;
        private Dictionary<int, string> unitDisplayNamesById;

        public int GrabacionAutomatica
        {
            get => grabacionAutomatica;
            set => grabacionAutomatica = value < 0 ? 0 : value;
        }

        public int InicioNumeracion
        {
            get => inicioNumeracion;
            set
            {
                inicioNumeracion = value < 0 ? 0 : value;
                if (treeListItemService != null)
                {
                    treeListItemService.ItemNumberStart = inicioNumeracion;
                    if (treeList1 != null)
                        treeListItemService.AssignItemNumbers();
                }
            }
        }

        public bool ColumnaTipoCalculoVisible
        {
            get => columnaTipoCalculoVisible;
            set
            {
                columnaTipoCalculoVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        public bool ColumnaRendimientoVisible
        {
            get => columnaRendimientoVisible;
            set
            {
                columnaRendimientoVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        public bool ColumnaHorasJornalVisible
        {
            get => columnaHorasJornalVisible;
            set
            {
                columnaHorasJornalVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        public bool ColumnaCuadrillaVisible
        {
            get => columnaCuadrillaVisible;
            set
            {
                columnaCuadrillaVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        public MainForm()
        {
            InitializeComponent();

            treeListItemService = new TreeListItemService(treeList1, null);
            treeListItemService.ItemNumberStart = inicioNumeracion;
            autoSaveTimer = new Timer { Interval = 1000 };
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Start();
            lastAutoSaveUtc = DateTime.UtcNow;
            ConfigureColumnEditors();
            LoadBudgetTree();
            treeList1.ExpandAll();
            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            if (GrabacionAutomatica <= 0 || !hasPendingAutoSaveChanges)
                return;

            if ((DateTime.UtcNow - lastAutoSaveUtc).TotalSeconds < GrabacionAutomatica)
                return;

            try
            {
                SaveBudgetTree();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void MarkPendingAutoSave()
        {
            if (suppressPersistence)
                return;

            hasPendingAutoSaveChanges = true;
            UpdatePendingSaveIndicator(true);
        }

        private void UpdatePendingSaveIndicator(bool pending)
        {
            if (lblPendingSaveStatus == null)
                return;

            lblPendingSaveStatus.Text = pending
                ? "Presupuesto pendiente de grabacion"
                : string.Empty;
        }

        private void ConfigureColumnEditors()
        {
            var resourceTypes = Datos.ObtenerTiposRecurso(EmpresaId, true);
            var resources = Datos.ObtenerRecursos(EmpresaId);
            var units = Datos.ObtenerUnidades();
            List<TipoCalculoDto> calculationTypes;
            try
            {
                calculationTypes = Datos.ObtenerTiposCalculo(EmpresaId);
            }
            catch (DataException)
            {
                calculationTypes = new List<TipoCalculoDto>();
            }

            treeList1.OptionsBehavior.Editable = true;
            for (int i = 0; i < treeList1.Columns.Count; i++)
                treeList1.Columns[i].OptionsColumn.AllowEdit = false;

            var resourceTypeColumn = treeList1.Columns[ResourceTypeColumnIndex];
            resourceTypeColumn.Caption = "Tipo Recurso";
            resourceTypeColumn.OptionsColumn.AllowEdit = true;

            var resourceColumn = treeList1.Columns[ResourceColumnIndex];
            resourceColumn.Caption = "Recurso";
            resourceColumn.OptionsColumn.AllowEdit = true;

            var quantityColumn = treeList1.Columns[QuantityColumnIndex];
            quantityColumn.Caption = "Cantidad";
            quantityColumn.OptionsColumn.AllowEdit = true;

            var calculationTypeColumn = treeList1.Columns[CalculationTypeColumnIndex];
            calculationTypeColumn.Caption = "Tipo Calculo";
            calculationTypeColumn.OptionsColumn.AllowEdit = true;

            var hoursPerDayColumn = treeList1.Columns[HoursPerDayColumnIndex];
            hoursPerDayColumn.Caption = "Horas Jornal";
            hoursPerDayColumn.OptionsColumn.AllowEdit = false;

            var performanceColumn = treeList1.Columns[PerformanceColumnIndex];
            performanceColumn.Caption = "Rendimiento";
            performanceColumn.OptionsColumn.AllowEdit = true;

            var crewColumn = treeList1.Columns[CrewColumnIndex];
            crewColumn.Caption = "Cuadrilla";
            crewColumn.OptionsColumn.AllowEdit = true;

            ApplyConfiguredColumnVisibility();

            var unitValueColumn = treeList1.Columns[UnitValueColumnIndex];
            unitValueColumn.Caption = "Valor Unitario";
            unitValueColumn.OptionsColumn.AllowEdit = true;

            var totalValueColumn = treeList1.Columns[TotalValueColumnIndex];
            totalValueColumn.Caption = "Valor Total";
            totalValueColumn.OptionsColumn.AllowEdit = false;

            var unitColumn = treeList1.Columns[UnitColumnIndex];
            unitColumn.Caption = "Unidad";
            unitColumn.OptionsColumn.AllowEdit = false;
            unitColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unitColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            var resourceTypesTable = new DataTable();
            resourceTypesTable.Columns.Add("EmpresaId", typeof(int));
            resourceTypesTable.Columns.Add("TipoRecursoId", typeof(int));
            resourceTypesTable.Columns.Add("TipoRecurso", typeof(string));
            resourceTypesTable.Columns.Add("Activo", typeof(bool));
            for (int i = 0; i < resourceTypes.Count; i++)
            {
                var item = resourceTypes[i];
                resourceTypesTable.Rows.Add(item.EmpresaId, item.TipoRecursoId, item.TipoRecurso, item.Activo);
            }

            var resourcesTable = new DataTable();
            resourcesTable.Columns.Add("RecursoId", typeof(int));
            resourcesTable.Columns.Add("TipoRecursoId", typeof(int));
            resourcesTable.Columns.Add("Recurso", typeof(string));
            resourcesTable.Columns.Add("UnidadId", typeof(int));
            for (int i = 0; i < resources.Count; i++)
            {
                var item = resources[i];
                resourcesTable.Rows.Add(item.RecursoId, item.TipoRecursoId, item.Recurso, item.UnidadId);
            }

            var unitsTable = new DataTable();
            unitsTable.Columns.Add("UnidadId", typeof(int));
            unitsTable.Columns.Add("Codigo", typeof(string));
            unitsTable.Columns.Add("Unidad", typeof(string));
            unitsTable.Columns.Add("Simbolo", typeof(string));
            unitsTable.Columns.Add("UnidadDisplay", typeof(string));
            for (int i = 0; i < units.Count; i++)
            {
                UnidadDto item = units[i];
                string display = !string.IsNullOrWhiteSpace(item.Simbolo)
                    ? item.Simbolo
                    : (!string.IsNullOrWhiteSpace(item.Unidad) ? item.Unidad : item.Codigo);
                unitsTable.Rows.Add(item.UnidadId, item.Codigo, item.Unidad, item.Simbolo, display);
            }

            resourceNamesById = BuildResourceNameMap(resources);
            resourceUnitIdsByResourceId = BuildResourceUnitMap(resources);
            unitDisplayNamesById = BuildUnitDisplayMap(units);

            var calculationTypesTable = new DataTable();
            calculationTypesTable.Columns.Add("TipoCalculoId", typeof(int));
            calculationTypesTable.Columns.Add("CodigoTipoCalculoId", typeof(string));
            calculationTypesTable.Columns.Add("TipoCalculo", typeof(string));
            for (int i = 0; i < calculationTypes.Count; i++)
            {
                var item = calculationTypes[i];
                calculationTypesTable.Rows.Add(item.TipoCalculoId, item.CodigoTipoCalculoId, item.TipoCalculo);
            }

            resourceTypePolicy = new ResourceTypePolicy(GetResourceTypeNames(resourceTypesTable), BuildResourceTypeMap(resourceTypesTable));
            treeListItemService.SetPolicy(resourceTypePolicy);

            resourceTypeEditorService = new ResourceTypeEditorService(resourceTypePolicy, resourceTypesTable);
            resourceTypeEditorService.Attach(treeList1, resourceTypeColumn);

            resourceEditorService = new ResourceEditorService(resourcesTable);
            resourceEditorService.Attach(treeList1, resourceColumn);

            unitEditorService = new UnitEditorService(unitsTable);
            unitEditorService.Attach(treeList1, unitColumn);

            calculationTypeEditorService = new CalculationTypeEditorService(calculationTypesTable);
            calculationTypeEditorService.Attach(treeList1, calculationTypeColumn);

            RepositoryItemTextEdit decimalEditor = CreateDecimalEditor();
            treeList1.RepositoryItems.Add(decimalEditor);
            hoursPerDayColumn.ColumnEdit = decimalEditor;
            performanceColumn.ColumnEdit = decimalEditor;
            crewColumn.ColumnEdit = decimalEditor;
            quantityColumn.ColumnEdit = decimalEditor;
            unitValueColumn.ColumnEdit = decimalEditor;
            totalValueColumn.ColumnEdit = decimalEditor;

            ConfigureDecimalColumn(hoursPerDayColumn);
            ConfigureDecimalColumn(performanceColumn);
            ConfigureDecimalColumn(crewColumn);
            ConfigureDecimalColumn(quantityColumn);
            ConfigureDecimalColumn(unitValueColumn);
            ConfigureDecimalColumn(totalValueColumn);

            treeList1.CellValueChanged -= treeList1_CellValueChanged;
            treeList1.CellValueChanged += treeList1_CellValueChanged;

            treeList1.ShowingEditor -= treeList1_ShowingEditor;
            treeList1.ShowingEditor += treeList1_ShowingEditor;

            treeList1.CustomColumnDisplayText -= treeList1_CustomColumnDisplayText;
            treeList1.CustomColumnDisplayText += treeList1_CustomColumnDisplayText;

            treeList1.NodeCellStyle -= treeList1_NodeCellStyle;
            treeList1.NodeCellStyle += treeList1_NodeCellStyle;
        }

        private void RefreshHorasJornalPresupuesto()
        {
            try
            {
                horasJornalPresupuestoActual = Datos.ObtenerHorasJornalPresupuesto(EmpresaId, PresupuestoId) ?? 0m;
            }
            catch (DataException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void ApplyConfiguredColumnVisibility()
        {
            if (treeList1 == null || treeList1.Columns == null || treeList1.Columns.Count == 0)
                return;

            if (CalculationTypeColumnIndex < treeList1.Columns.Count)
                treeList1.Columns[CalculationTypeColumnIndex].Visible = ColumnaTipoCalculoVisible;

            if (PerformanceColumnIndex < treeList1.Columns.Count)
                treeList1.Columns[PerformanceColumnIndex].Visible = ColumnaRendimientoVisible;

            if (HoursPerDayColumnIndex < treeList1.Columns.Count)
                treeList1.Columns[HoursPerDayColumnIndex].Visible = ColumnaHorasJornalVisible;

            if (CrewColumnIndex < treeList1.Columns.Count)
                treeList1.Columns[CrewColumnIndex].Visible = ColumnaCuadrillaVisible;
        }

        private static Dictionary<int, string> BuildResourceTypeMap(DataTable table)
        {
            var map = new Dictionary<int, string>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                int id = Convert.ToInt32(row["TipoRecursoId"]);
                string name = Convert.ToString(row["TipoRecurso"]);

                if (!map.ContainsKey(id))
                    map.Add(id, name);
            }

            return map;
        }

        private static Dictionary<int, string> BuildResourceNameMap(List<RecursoDto> resources)
        {
            var map = new Dictionary<int, string>();
            for (int i = 0; i < resources.Count; i++)
            {
                RecursoDto resource = resources[i];
                if (!map.ContainsKey(resource.RecursoId))
                    map.Add(resource.RecursoId, resource.Recurso);
            }

            return map;
        }

        private static Dictionary<int, int?> BuildResourceUnitMap(List<RecursoDto> resources)
        {
            var map = new Dictionary<int, int?>();
            for (int i = 0; i < resources.Count; i++)
            {
                RecursoDto resource = resources[i];
                if (!map.ContainsKey(resource.RecursoId))
                    map.Add(resource.RecursoId, resource.UnidadId);
            }

            return map;
        }

        private static Dictionary<int, string> BuildUnitDisplayMap(List<UnidadDto> units)
        {
            var map = new Dictionary<int, string>();
            for (int i = 0; i < units.Count; i++)
            {
                UnidadDto unit = units[i];
                if (map.ContainsKey(unit.UnidadId))
                    continue;

                string display = !string.IsNullOrWhiteSpace(unit.Simbolo)
                    ? unit.Simbolo
                    : (!string.IsNullOrWhiteSpace(unit.Unidad) ? unit.Unidad : unit.Codigo);

                map.Add(unit.UnidadId, display ?? string.Empty);
            }

            return map;
        }

        private static IEnumerable<string> GetResourceTypeNames(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
                yield return Convert.ToString(table.Rows[i]["TipoRecurso"]);
        }

        private static RepositoryItemTextEdit CreateDecimalEditor()
        {
            var editor = new RepositoryItemTextEdit();
            editor.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            editor.Mask.EditMask = "n5";
            editor.Mask.UseMaskAsDisplayFormat = false;
            return editor;
        }

        private static void ConfigureDecimalColumn(DevExpress.XtraTreeList.Columns.TreeListColumn column)
        {
            column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            column.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            column.Format.FormatString = "n2";
        }

        private sealed class PartidaCalculationData
        {
            public decimal? RendimientoManoObra { get; set; }
            public decimal? RendimientoEquipos { get; set; }
        }

        private static PartidaCalculationData GetPartidaCalculationData(TreeListNode node, bool createIfMissing)
        {
            if (node == null)
                return null;

            PartidaCalculationData data = node.Tag as PartidaCalculationData;
            if (data == null && createIfMissing)
            {
                data = new PartidaCalculationData();
                node.Tag = data;
            }

            return data;
        }

        private decimal? GetPartidaRendimientoManoObra(TreeListNode node)
        {
            PartidaCalculationData data = GetPartidaCalculationData(node, false);
            return data == null ? (decimal?)null : data.RendimientoManoObra;
        }

        private decimal? GetPartidaRendimientoEquipos(TreeListNode node)
        {
            PartidaCalculationData data = GetPartidaCalculationData(node, false);
            return data == null ? (decimal?)null : data.RendimientoEquipos;
        }

        private void SetPartidaCalculationData(TreeListNode node, decimal? rendimientoManoObra, decimal? rendimientoEquipos)
        {
            if (node == null || resourceTypePolicy == null || !resourceTypePolicy.IsPartida(node))
                return;

            PartidaCalculationData data = GetPartidaCalculationData(node, true);
            data.RendimientoManoObra = rendimientoManoObra;
            data.RendimientoEquipos = rendimientoEquipos;
            node.SetValue(PerformanceColumnIndex, null);
            node.SetValue(CrewColumnIndex, null);
        }
    }
}
