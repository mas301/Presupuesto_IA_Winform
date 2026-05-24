using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    internal sealed class PartidaCalculationData
    {
        public decimal? RendimientoManoObra { get; set; }
        public decimal? RendimientoEquipos { get; set; }
    }

    public partial class MainForm : Form
    {
        private const int EmpresaId = 1;
        private const int PresupuestoId = 1;
        private const int DefaultGrabacionAutomaticaSegundos = 300;

        // FieldName constants for tree list columns (must match Designer FieldName values)
        internal static class ColumnNames
        {
            public const string Item = "Item";
            public const string TipoRecurso = "TipoRecurso";
            public const string Recurso = "Recurso";
            public const string Independiente = "Independiente";
            public const string Unidad = "Unidad";
            public const string TipoCalculo = "TipoCalculo";
            public const string HorasJornal = "HorasJornal";
            public const string Rendimiento = "Rendimiento";
            public const string Cantidad = "Cantidad";
            public const string PesoUnitario = "PesoUnitario";
            public const string DiasDuracion = "DiasDuracion";
            public const string CantidadTotal = "CantidadTotal";
            public const string ValorUnitario = "ValorUnitario";
            public const string ValorTotal = "ValorTotal";
            public const string Alias = "Alias";
        }

        // TreeListColumn references resolved in ConfigureColumnEditors via FieldName
        private TreeListColumn columnTipoRecurso;
        private TreeListColumn columnRecurso;
        private TreeListColumn columnIndependiente;
        private TreeListColumn columnUnidad;
        private TreeListColumn columnTipoCalculo;
        private TreeListColumn columnHorasJornal;
        private TreeListColumn columnRendimiento;
        private TreeListColumn columnCantidad;
        private TreeListColumn columnPesoUnitario;
        private TreeListColumn columnDiasDuracion;
        private TreeListColumn columnCantidadTotal;
        private TreeListColumn columnValorUnitario;
        private TreeListColumn columnValorTotal;
        private TreeListColumn columnAlias;

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
        private bool columnaCantidadVisible = true;
        private bool columnaPesoUnitarioVisible = true;
        private bool columnaDiasDuracionVisible = true;
        private Dictionary<int, string> resourceNamesById;
        private Dictionary<int, int?> resourceUnitIdsByResourceId;
        private Dictionary<int, int?> resourceCalculationTypeIdsByResourceId;
        private Dictionary<int, RecursoDto> resourcesById;
        private Dictionary<int, string> unitDisplayNamesById;

        [EditableSetting("General", "GrabacionAutomatica", "Intervalo en segundos entre grabaciones automaticas. Si es 0, se desactiva la grabacion automatica.")]
        public int GrabacionAutomatica
        {
            get => grabacionAutomatica;
            set => grabacionAutomatica = value < 0 ? 0 : value;
        }

        [EditableSetting("General", "InicioNumeracion", "Numero inicial para la numeracion de filas. Por defecto es 1. Puede ser 0, 10, etc.")]
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

        [EditableSetting("Columnas", "ColumnaTipoCalculoVisible", "Controla si la columna Tipo Calculo se muestra en el arbol principal.")]
        public bool ColumnaTipoCalculoVisible
        {
            get => columnaTipoCalculoVisible;
            set
            {
                columnaTipoCalculoVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        [EditableSetting("Columnas", "ColumnaRendimientoVisible", "Controla si la columna Rendimiento se muestra en el arbol principal.")]
        public bool ColumnaRendimientoVisible
        {
            get => columnaRendimientoVisible;
            set
            {
                columnaRendimientoVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        [EditableSetting("Columnas", "ColumnaHorasJornalVisible", "Controla si la columna Horas Jornal se muestra en el arbol principal.")]
        public bool ColumnaHorasJornalVisible
        {
            get => columnaHorasJornalVisible;
            set
            {
                columnaHorasJornalVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        [EditableSetting("Columnas", "ColumnaCantidadVisible", "Controla si la columna Cantidad se muestra en el arbol principal.")]
        public bool ColumnaCantidadVisible
        {
            get => columnaCantidadVisible;
            set
            {
                columnaCantidadVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        [EditableSetting("Columnas", "ColumnaPesoUnitarioVisible", "Controla si la columna Peso Unitario se muestra en el arbol principal.")]
        public bool ColumnaPesoUnitarioVisible
        {
            get => columnaPesoUnitarioVisible;
            set
            {
                columnaPesoUnitarioVisible = value;
                ApplyConfiguredColumnVisibility();
            }
        }

        [EditableSetting("Columnas", "ColumnaDiasDuracionVisible", "Controla si la columna DiasDuracion se muestra en el arbol principal.")]
        public bool ColumnaDiasDuracionVisible
        {
            get => columnaDiasDuracionVisible;
            set
            {
                columnaDiasDuracionVisible = value;
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

            columnTipoRecurso = treeList1.Columns[ColumnNames.TipoRecurso];
            columnTipoRecurso.Caption = "Tipo Recurso";
            columnTipoRecurso.OptionsColumn.AllowEdit = true;

            columnRecurso = treeList1.Columns[ColumnNames.Recurso];
            columnRecurso.Caption = "Recurso";
            columnRecurso.OptionsColumn.AllowEdit = true;

            columnIndependiente = treeList1.Columns[ColumnNames.Independiente];
            columnIndependiente.Caption = "Independiente";
            columnIndependiente.OptionsColumn.AllowEdit = false;
            columnIndependiente.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            columnIndependiente.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            columnCantidadTotal = treeList1.Columns[ColumnNames.CantidadTotal];
            columnCantidadTotal.Caption = "CantidadTotal";
            columnCantidadTotal.OptionsColumn.AllowEdit = true;

            columnTipoCalculo = treeList1.Columns[ColumnNames.TipoCalculo];
            columnTipoCalculo.Caption = "Tipo Calculo";
            columnTipoCalculo.OptionsColumn.AllowEdit = false;

            columnHorasJornal = treeList1.Columns[ColumnNames.HorasJornal];
            columnHorasJornal.Caption = "Horas Jornal";
            columnHorasJornal.OptionsColumn.AllowEdit = false;

            columnRendimiento = treeList1.Columns[ColumnNames.Rendimiento];
            columnRendimiento.Caption = "Rendimiento";
            columnRendimiento.OptionsColumn.AllowEdit = true;

            columnCantidad = treeList1.Columns[ColumnNames.Cantidad];
            columnCantidad.Caption = "Cantidad";
            columnCantidad.OptionsColumn.AllowEdit = true;

            columnPesoUnitario = treeList1.Columns[ColumnNames.PesoUnitario];
            columnPesoUnitario.Caption = "Peso Unitario";
            columnPesoUnitario.OptionsColumn.AllowEdit = true;

            columnDiasDuracion = treeList1.Columns[ColumnNames.DiasDuracion];
            columnDiasDuracion.Caption = "DiasDuracion";
            columnDiasDuracion.OptionsColumn.AllowEdit = false;

            ApplyConfiguredColumnVisibility();

            columnValorUnitario = treeList1.Columns[ColumnNames.ValorUnitario];
            columnValorUnitario.Caption = "Valor Unitario";
            columnValorUnitario.OptionsColumn.AllowEdit = true;

            columnValorTotal = treeList1.Columns[ColumnNames.ValorTotal];
            columnValorTotal.Caption = "Valor Total";
            columnValorTotal.OptionsColumn.AllowEdit = false;

            columnUnidad = treeList1.Columns[ColumnNames.Unidad];
            columnUnidad.Caption = "Unidad";
            columnUnidad.OptionsColumn.AllowEdit = false;
            columnUnidad.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            columnUnidad.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            columnAlias = treeList1.Columns[ColumnNames.Alias];

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
            resourceCalculationTypeIdsByResourceId = BuildResourceCalculationTypeMap(resources);
            resourcesById = BuildResourceCatalogMap(resources);
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
            resourceTypeEditorService.Attach(treeList1, columnTipoRecurso);

            resourceEditorService = new ResourceEditorService(resourcesTable);
            resourceEditorService.Attach(treeList1, columnRecurso);
            resourceEditorService.ResourceCreationRequested += ResourceEditorService_ResourceCreationRequested;

            unitEditorService = new UnitEditorService(unitsTable);
            unitEditorService.Attach(treeList1, columnUnidad);

            calculationTypeEditorService = new CalculationTypeEditorService(calculationTypesTable);
            calculationTypeEditorService.Attach(treeList1, columnTipoCalculo);

            RepositoryItemTextEdit decimalEditor = CreateDecimalEditor();
            treeList1.RepositoryItems.Add(decimalEditor);
            columnHorasJornal.ColumnEdit = decimalEditor;
            columnRendimiento.ColumnEdit = decimalEditor;
            columnCantidad.ColumnEdit = decimalEditor;
            columnPesoUnitario.ColumnEdit = decimalEditor;
            columnDiasDuracion.ColumnEdit = decimalEditor;
            columnCantidadTotal.ColumnEdit = decimalEditor;
            columnValorUnitario.ColumnEdit = decimalEditor;
            columnValorTotal.ColumnEdit = decimalEditor;

            var independienteEditor = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            treeList1.RepositoryItems.Add(independienteEditor);
            columnIndependiente.ColumnEdit = independienteEditor;

            ConfigureDecimalColumn(columnHorasJornal);
            ConfigureDecimalColumn(columnRendimiento);
            ConfigureDecimalColumn(columnCantidad);
            ConfigureDecimalColumn(columnPesoUnitario);
            ConfigureDecimalColumn(columnDiasDuracion);
            ConfigureDecimalColumn(columnCantidadTotal);
            ConfigureDecimalColumn(columnValorUnitario);
            ConfigureDecimalColumn(columnValorTotal);

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

            if (columnTipoCalculo != null)
                columnTipoCalculo.Visible = ColumnaTipoCalculoVisible;

            if (columnRendimiento != null)
                columnRendimiento.Visible = ColumnaRendimientoVisible;

            if (columnHorasJornal != null)
                columnHorasJornal.Visible = ColumnaHorasJornalVisible;

            if (columnCantidad != null)
                columnCantidad.Visible = ColumnaCantidadVisible;

            if (columnPesoUnitario != null)
                columnPesoUnitario.Visible = ColumnaPesoUnitarioVisible;

            if (columnDiasDuracion != null)
                columnDiasDuracion.Visible = ColumnaDiasDuracionVisible;
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

        private static Dictionary<int, int?> BuildResourceCalculationTypeMap(List<RecursoDto> resources)
        {
            var map = new Dictionary<int, int?>();
            for (int i = 0; i < resources.Count; i++)
            {
                RecursoDto resource = resources[i];
                if (!map.ContainsKey(resource.RecursoId))
                    map.Add(resource.RecursoId, resource.TipoCalculoId);
            }

            return map;
        }

        private static Dictionary<int, RecursoDto> BuildResourceCatalogMap(List<RecursoDto> resources)
        {
            var map = new Dictionary<int, RecursoDto>();
            for (int i = 0; i < resources.Count; i++)
            {
                RecursoDto resource = resources[i];
                if (!map.ContainsKey(resource.RecursoId))
                    map.Add(resource.RecursoId, resource);
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
            node.SetValue(columnRendimiento, null);
        }
    }
}
