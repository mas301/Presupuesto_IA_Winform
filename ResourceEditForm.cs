using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections;

namespace DevExpressTreeListDemo
{
    public partial class ResourceEditForm : Form
    {
        private readonly IList<TipoRecursoDto> resourceTypes;
        private readonly IList<RecursoDto> resources;
        private readonly IList<UnidadDto> units;
        private readonly IList<TipoCalculoDto> calculationTypes;
        private List<RecursoDto> filteredResources;

        public int? SelectedTipoRecursoId { get; private set; }
        public int? SelectedRecursoId { get; private set; }
        public int? SelectedUnidadId { get; private set; }
        public int? SelectedTipoCalculoId { get; private set; }
        public string Alias { get; private set; }
        public string RecursoTexto { get; private set; }
        public decimal? RendimientoManoObra { get; private set; }
        public decimal? RendimientoEquipos { get; private set; }

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
            bool startBlank)
        {
            this.resourceTypes = resourceTypes ?? new List<TipoRecursoDto>();
            this.resources = resources ?? new List<RecursoDto>();
            this.units = units ?? new List<UnidadDto>();
            this.calculationTypes = calculationTypes ?? new List<TipoCalculoDto>();

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
            bool startBlank)
        {
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

            UpdateFieldVisibilityByResourceType();
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
            TryApplyUnitFromResourceText();
            UpdateFieldVisibilityByResourceType();
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
            }
            else
            {
                RendimientoManoObra = null;
                RendimientoEquipos = null;
            }

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

        private void UpdateFieldVisibilityByResourceType()
        {
            bool isSubpresupuesto = IsSelectedType("Subpresupuesto");
            bool isPartida = IsSelectedType("Partida");

            lblUnidad.Visible = !isSubpresupuesto;
            cmbUnidad.Visible = !isSubpresupuesto;

            bool showTipoCalculo = !isSubpresupuesto && !isPartida;
            lblTipoCalculo.Visible = showTipoCalculo;
            cmbTipoCalculo.Visible = showTipoCalculo;

            lblRendimientoManoObra.Visible = isPartida;
            nudRendimientoManoObra.Visible = isPartida;
            lblRendimientoEquipos.Visible = isPartida;
            nudRendimientoEquipos.Visible = isPartida;
        }

        private bool IsSelectedType(string typeName)
        {
            ResourceTypeOption selectedType = cmbTipoRecurso.SelectedItem as ResourceTypeOption;
            if (selectedType == null)
                return false;

            return string.Equals(selectedType.TipoRecurso, typeName, StringComparison.OrdinalIgnoreCase);
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
