using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections;

namespace PresupuestoIA
{
    public partial class ResourceEditForm : Form
    {
        private readonly IList<TipoRecursoDto> resourceTypes;
        private readonly IList<RecursoDto> resources;
        private readonly IList<UnidadDto> units;
        private readonly IList<TipoCalculoDto> calculationTypes;
        private readonly bool requireTypeAndResource;
        private readonly bool allowCreateFromNameChange;
        private List<RecursoDto> filteredResources;
        private string originalRecursoTextoNormalized;

        public int? SelectedTipoRecursoId { get; private set; }
        public int? SelectedRecursoId { get; private set; }
        public int? SelectedUnidadId { get; private set; }
        public int? SelectedTipoCalculoId { get; private set; }
        public string Alias { get; private set; }
        public string RecursoTexto { get; private set; }
        public decimal? RendimientoManoObra { get; private set; }
        public decimal? RendimientoEquipos { get; private set; }
        public decimal? DiasDuracion { get; private set; }
        public decimal? HorasJornal { get; private set; }
        public bool Independiente { get; private set; }
        public bool CreateNewResourceRequested { get; private set; }

        public ResourceEditForm(
            IList<TipoRecursoDto> resourceTypes,
            IList<RecursoDto> resources,
            IList<UnidadDto> units,
            IList<TipoCalculoDto> calculationTypes,
            int? initialTipoRecursoId,
            int? initialRecursoId,
            int? initialUnidadId,
            int? initialTipoCalculoId,
            string initialAlias,
            decimal? initialRendimientoManoObra,
            decimal? initialRendimientoEquipos,
            decimal? initialDiasDuracion,
            decimal? initialHorasJornal,
            bool initialIndependiente,
            bool startBlank,
            bool requireTypeAndResource,
            bool allowCreateFromNameChange)
        {
            this.resourceTypes = resourceTypes ?? new List<TipoRecursoDto>();
            this.resources = resources ?? new List<RecursoDto>();
            this.units = units ?? new List<UnidadDto>();
            this.calculationTypes = calculationTypes ?? new List<TipoCalculoDto>();
            this.requireTypeAndResource = requireTypeAndResource;
            this.allowCreateFromNameChange = allowCreateFromNameChange;

            InitializeComponent();
            LoadLookups();
            LoadInitialValues(
                initialTipoRecursoId,
                initialRecursoId,
                initialUnidadId,
                initialTipoCalculoId,
                initialAlias,
                initialRendimientoManoObra,
                initialRendimientoEquipos,
                initialDiasDuracion,
                initialHorasJornal,
                initialIndependiente,
                startBlank);
        }

        private void LoadLookups()
        {
            cmbTipoRecurso.DisplayMember = "TipoRecurso";
            cmbTipoRecurso.ValueMember = "TipoRecursoId";
            var resourceTypeOptions = new List<ResourceTypeOption>
            {
                new ResourceTypeOption { TipoRecursoId = null, TipoRecurso = "Ninguno" }
            };
            for (int i = 0; i < resourceTypes.Count; i++)
            {
                TipoRecursoDto item = resourceTypes[i];
                resourceTypeOptions.Add(new ResourceTypeOption
                {
                    TipoRecursoId = item.TipoRecursoId,
                    TipoRecurso = item.TipoRecurso
                });
            }

            cmbTipoRecurso.DataSource = resourceTypeOptions;

            cmbUnidad.DisplayMember = "DisplayName";
            cmbUnidad.ValueMember = "UnidadId";
            var unitOptions = new List<UnidadDisplayItem>
            {
                new UnidadDisplayItem { UnidadId = null, DisplayName = "Ninguno" }
            };
            unitOptions.AddRange(units
                .Select(u => new UnidadDisplayItem
                {
                    UnidadId = u.UnidadId,
                    DisplayName = !string.IsNullOrWhiteSpace(u.Simbolo)
                        ? u.Simbolo
                        : (!string.IsNullOrWhiteSpace(u.Unidad) ? u.Unidad : u.Codigo)
                })
                .ToList());

            cmbUnidad.DataSource = unitOptions;

            cmbTipoCalculo.DisplayMember = "TipoCalculo";
            cmbTipoCalculo.ValueMember = "TipoCalculoId";
            var calculationTypeOptions = new List<CalculationTypeOption>
            {
                new CalculationTypeOption { TipoCalculoId = null, TipoCalculo = "Ninguno" }
            };
            for (int i = 0; i < calculationTypes.Count; i++)
            {
                TipoCalculoDto item = calculationTypes[i];
                calculationTypeOptions.Add(new CalculationTypeOption
                {
                    TipoCalculoId = item.TipoCalculoId,
                    TipoCalculo = item.TipoCalculo
                });
            }

            cmbTipoCalculo.DataSource = calculationTypeOptions;

            if (!txtRecurso.Multiline)
            {
                txtRecurso.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtRecurso.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        private void LoadInitialValues(
            int? initialTipoRecursoId,
            int? initialRecursoId,
            int? initialUnidadId,
            int? initialTipoCalculoId,
            string initialAlias,
            decimal? initialRendimientoManoObra,
            decimal? initialRendimientoEquipos,
            decimal? initialDiasDuracion,
            decimal? initialHorasJornal,
            bool initialIndependiente,
            bool startBlank)
        {
            btnCrear.Visible = allowCreateFromNameChange && !startBlank;
            btnCrear.Enabled = false;

            if (startBlank)
            {
                cmbTipoRecurso.SelectedIndex = 0;
                ApplyResourceFilter();
                txtRecurso.Text = string.Empty;
                RecursoTexto = string.Empty;
                cmbUnidad.SelectedIndex = 0;
                cmbTipoCalculo.SelectedIndex = 0;
                txtAlias.Text = string.Empty;
                nudRendimientoManoObra.Value = 0m;
                nudRendimientoEquipos.Value = 0m;
                nudDiasDuracion.Value = 0m;
                nudHorasJornal.Value = 0m;
                chkIndependiente.Checked = initialIndependiente;
                originalRecursoTextoNormalized = string.Empty;
                UpdateFieldVisibilityByResourceType();
                return;
            }

            if (initialTipoRecursoId.HasValue)
                cmbTipoRecurso.SelectedValue = initialTipoRecursoId.Value;
            else
                cmbTipoRecurso.SelectedIndex = 0;

            ApplyResourceFilter();

            if (initialRecursoId.HasValue)
            {
                RecursoDto initialResource = resources.FirstOrDefault(r => r.RecursoId == initialRecursoId.Value);
                txtRecurso.Text = initialResource != null ? (initialResource.Recurso ?? string.Empty) : string.Empty;
            }
            else
            {
                txtRecurso.Text = string.Empty;
            }
            RecursoTexto = txtRecurso.Text;
            originalRecursoTextoNormalized = NormalizeResourceInput(txtRecurso.Text);

            if (initialUnidadId.HasValue)
                cmbUnidad.SelectedValue = initialUnidadId.Value;
            else
                cmbUnidad.SelectedIndex = 0;

            if (initialTipoCalculoId.HasValue)
                cmbTipoCalculo.SelectedValue = initialTipoCalculoId.Value;
            else
                cmbTipoCalculo.SelectedIndex = 0;

            txtAlias.Text = initialAlias ?? string.Empty;

            nudRendimientoManoObra.Value = NormalizeDecimal(initialRendimientoManoObra);
            nudRendimientoEquipos.Value = NormalizeDecimal(initialRendimientoEquipos);
            nudDiasDuracion.Value = NormalizeDecimal(initialDiasDuracion);
            nudHorasJornal.Value = NormalizeDecimal(initialHorasJornal);
            chkIndependiente.Checked = initialIndependiente;

            ApplyResourceTypeDefaults();
            UpdateFieldVisibilityByResourceType();
            UpdateCreateButtonState();
        }

        private static decimal NormalizeDecimal(decimal? value)
        {
            if (!value.HasValue)
                return 0m;

            decimal normalized = value.Value;
            if (normalized < 0m)
                return 0m;

            if (normalized > 999999999m)
                return 999999999m;

            return normalized;
        }

        private void cmbTipoRecurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyResourceFilter();
            ApplyResourceTypeDefaults();
            TryApplyUnitFromResourceText();
            UpdateFieldVisibilityByResourceType();
            UpdateCreateButtonState();
        }

        private void ApplyResourceTypeDefaults()
        {
            int? tipoRecursoId = cmbTipoRecurso.SelectedValue as int?;
            if (!tipoRecursoId.HasValue && cmbTipoRecurso.SelectedValue != null)
            {
                int parsed;
                if (int.TryParse(Convert.ToString(cmbTipoRecurso.SelectedValue), out parsed))
                    tipoRecursoId = parsed;
            }

            if (!tipoRecursoId.HasValue)
                return;

            TipoRecursoDto tipoRecurso = null;
            for (int i = 0; i < resourceTypes.Count; i++)
            {
                if (resourceTypes[i].TipoRecursoId == tipoRecursoId.Value)
                {
                    tipoRecurso = resourceTypes[i];
                    break;
                }
            }

            if (tipoRecurso == null)
                return;

            // Solo aplica defaults si el combo aun esta en "Ninguno"
            // para no sobrescribir una seleccion explicita del usuario.
            if (tipoRecurso.UnidadIdDefault.HasValue && (cmbUnidad.SelectedValue as int?) == null)
                cmbUnidad.SelectedValue = tipoRecurso.UnidadIdDefault.Value;

            if (tipoRecurso.TipoCalculoIdDefault.HasValue && (cmbTipoCalculo.SelectedValue as int?) == null)
                cmbTipoCalculo.SelectedValue = tipoRecurso.TipoCalculoIdDefault.Value;
        }

        private void ApplyResourceFilter()
        {
            int? tipoRecursoId = cmbTipoRecurso.SelectedValue as int?;
            if (!tipoRecursoId.HasValue && cmbTipoRecurso.SelectedValue != null)
            {
                int parsed;
                if (int.TryParse(Convert.ToString(cmbTipoRecurso.SelectedValue), out parsed))
                    tipoRecursoId = parsed;
            }

            filteredResources = tipoRecursoId.HasValue
                ? resources.Where(r => r.TipoRecursoId == tipoRecursoId.Value).ToList()
                : new List<RecursoDto>();

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.Add("Ninguno");
            for (int i = 0; i < filteredResources.Count; i++)
            {
                string name = filteredResources[i].Recurso;
                if (!string.IsNullOrWhiteSpace(name))
                    autoComplete.Add(name);
            }

            if (!txtRecurso.Multiline)
                txtRecurso.AutoCompleteCustomSource = autoComplete;
        }

        private void txtRecurso_Leave(object sender, EventArgs e)
        {
            txtRecurso.Text = NormalizeResourceInput(txtRecurso.Text);
            TryApplyUnitFromResourceText();
            UpdateCreateButtonState();
        }

        private void txtRecurso_TextChanged(object sender, EventArgs e)
        {
            UpdateCreateButtonState();
        }

        private void txtAlias_Leave(object sender, EventArgs e)
        {
            txtAlias.Text = NormalizeResourceInput(txtAlias.Text);
        }

        private void TryApplyUnitFromResourceText()
        {
            RecursoDto selected = FindResourceByText(NormalizeResourceInput(txtRecurso.Text));
            if (selected != null && selected.UnidadId.HasValue)
                cmbUnidad.SelectedValue = selected.UnidadId.Value;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            bool isSubpresupuesto = IsSelectedType("Subpresupuesto");
            bool isPartida = IsSelectedType("Partida");

            SelectedTipoRecursoId = cmbTipoRecurso.SelectedValue as int?;
            if (!SelectedTipoRecursoId.HasValue)
                SelectedTipoRecursoId = ParseNullableInt(cmbTipoRecurso.SelectedValue);

            string recursoTextoNormalizado = NormalizeResourceInput(txtRecurso.Text);
            RecursoDto selectedResource = FindResourceByText(recursoTextoNormalizado);
            SelectedRecursoId = selectedResource != null ? (int?)selectedResource.RecursoId : null;
            string recursoTexto = recursoTextoNormalizado;
            RecursoTexto = string.Equals(recursoTexto, "Ninguno", StringComparison.OrdinalIgnoreCase) ? string.Empty : recursoTexto;

            if (requireTypeAndResource)
            {
                if (!SelectedTipoRecursoId.HasValue)
                {
                    MessageBox.Show("Seleccione un Tipo Recurso valido.", "Crear recurso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(RecursoTexto))
                {
                    MessageBox.Show("Ingrese el nombre del recurso.", "Crear recurso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (isSubpresupuesto)
            {
                SelectedUnidadId = null;
                SelectedTipoCalculoId = null;
            }
            else
            {
                SelectedUnidadId = cmbUnidad.SelectedValue as int?;
                if (!SelectedUnidadId.HasValue)
                    SelectedUnidadId = ParseNullableInt(cmbUnidad.SelectedValue);

                SelectedTipoCalculoId = cmbTipoCalculo.SelectedValue as int?;
                if (!SelectedTipoCalculoId.HasValue)
                    SelectedTipoCalculoId = ParseNullableInt(cmbTipoCalculo.SelectedValue);
            }

            Alias = NormalizeResourceInput(txtAlias.Text);

            if (isPartida)
            {
                RendimientoManoObra = nudRendimientoManoObra.Value;
                RendimientoEquipos = nudRendimientoEquipos.Value;
                DiasDuracion = nudDiasDuracion.Value;
                HorasJornal = nudHorasJornal.Value;
            }
            else
            {
                RendimientoManoObra = null;
                RendimientoEquipos = null;
                DiasDuracion = null;
                HorasJornal = null;
            }

            Independiente = chkIndependiente.Checked;

            CreateNewResourceRequested = false;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            SelectedTipoRecursoId = cmbTipoRecurso.SelectedValue as int?;
            if (!SelectedTipoRecursoId.HasValue)
                SelectedTipoRecursoId = ParseNullableInt(cmbTipoRecurso.SelectedValue);

            string recursoTexto = NormalizeResourceInput(txtRecurso.Text);
            RecursoTexto = string.Equals(recursoTexto, "Ninguno", StringComparison.OrdinalIgnoreCase) ? string.Empty : recursoTexto;

            if (!SelectedTipoRecursoId.HasValue)
            {
                MessageBox.Show("Seleccione un Tipo Recurso valido.", "Crear recurso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RecursoTexto))
            {
                MessageBox.Show("Ingrese el nombre del recurso.", "Crear recurso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isSubpresupuesto = IsSelectedType("Subpresupuesto");
            bool isPartida = IsSelectedType("Partida");

            SelectedRecursoId = null;

            if (isSubpresupuesto)
            {
                SelectedUnidadId = null;
                SelectedTipoCalculoId = null;
            }
            else
            {
                SelectedUnidadId = cmbUnidad.SelectedValue as int?;
                if (!SelectedUnidadId.HasValue)
                    SelectedUnidadId = ParseNullableInt(cmbUnidad.SelectedValue);

                SelectedTipoCalculoId = cmbTipoCalculo.SelectedValue as int?;
                if (!SelectedTipoCalculoId.HasValue)
                    SelectedTipoCalculoId = ParseNullableInt(cmbTipoCalculo.SelectedValue);
            }

            Alias = NormalizeResourceInput(txtAlias.Text);

            if (isPartida)
            {
                RendimientoManoObra = nudRendimientoManoObra.Value;
                RendimientoEquipos = nudRendimientoEquipos.Value;
                DiasDuracion = nudDiasDuracion.Value;
                HorasJornal = nudHorasJornal.Value;
            }
            else
            {
                RendimientoManoObra = null;
                RendimientoEquipos = null;
                DiasDuracion = null;
                HorasJornal = null;
            }

            Independiente = chkIndependiente.Checked;

            CreateNewResourceRequested = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private static int? ParseNullableInt(object value)
        {
            if (value == null)
                return null;

            int parsed;
            return int.TryParse(Convert.ToString(value), out parsed) ? (int?)parsed : null;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void PreloadResourceName(string resourceName)
        {
            string text = resourceName ?? string.Empty;
            txtRecurso.Text = text;
            RecursoTexto = text;
            originalRecursoTextoNormalized = NormalizeResourceInput(text);
        }

        private void UpdateFieldVisibilityByResourceType()
        {
            bool isSubpresupuesto = IsSelectedType("Subpresupuesto");
            bool isPartida = IsSelectedType("Partida");

            lblUnidad.Visible = !isSubpresupuesto;
            cmbUnidad.Visible = !isSubpresupuesto;

            bool showTipoCalculo = !isSubpresupuesto;
            lblTipoCalculo.Visible = showTipoCalculo;
            cmbTipoCalculo.Visible = showTipoCalculo;

            lblRendimientoManoObra.Visible = isPartida;
            nudRendimientoManoObra.Visible = isPartida;
            lblRendimientoEquipos.Visible = isPartida;
            nudRendimientoEquipos.Visible = isPartida;
            lblDiasDuracion.Visible = isPartida;
            nudDiasDuracion.Visible = isPartida;
            lblHorasJornal.Visible = isPartida;
            nudHorasJornal.Visible = isPartida;
        }

        private bool IsSelectedType(string typeName)
        {
            ResourceTypeOption selectedType = cmbTipoRecurso.SelectedItem as ResourceTypeOption;
            if (selectedType == null)
                return false;

            return string.Equals(
                NormalizeTypeName(selectedType.TipoRecurso),
                NormalizeTypeName(typeName),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeTypeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Replace("-", string.Empty)
                .Trim();
        }

        private sealed class ResourceTypeOption
        {
            public int? TipoRecursoId { get; set; }
            public string TipoRecurso { get; set; }
        }

        private RecursoDto FindResourceByText(string resourceText)
        {
            string normalized = NormalizeResourceInput(resourceText);
            if (string.IsNullOrEmpty(normalized) || string.Equals(normalized, "Ninguno", StringComparison.OrdinalIgnoreCase))
                return null;

            if (filteredResources == null)
                return null;

            for (int i = 0; i < filteredResources.Count; i++)
            {
                RecursoDto item = filteredResources[i];
                if (string.Equals(item.Recurso, normalized, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        private static string NormalizeResourceInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string normalized = value.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
            return normalized.Trim();
        }

        private void UpdateCreateButtonState()
        {
            if (btnCrear == null)
                return;

            if (!btnCrear.Visible)
            {
                btnCrear.Enabled = false;
                return;
            }

            string current = NormalizeResourceInput(txtRecurso.Text);
            bool hasValidName = !string.IsNullOrWhiteSpace(current) && !string.Equals(current, "Ninguno", StringComparison.OrdinalIgnoreCase);
            bool changed = !string.Equals(current, originalRecursoTextoNormalized ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            btnCrear.Enabled = hasValidName && changed;
        }

        private sealed class UnidadDisplayItem
        {
            public int? UnidadId { get; set; }
            public string DisplayName { get; set; }
        }

        private sealed class CalculationTypeOption
        {
            public int? TipoCalculoId { get; set; }
            public string TipoCalculo { get; set; }
        }
    }
}
