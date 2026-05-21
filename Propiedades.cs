using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    public partial class Propiedades : Form
    {
        private readonly PropiedadesViewModel viewModel;

        public int GrabacionAutomatica { get; private set; }
        public bool ColumnaTipoCalculoVisible { get; private set; }
        public bool ColumnaRendimientoVisible { get; private set; }
        public bool ColumnaHorasJornalVisible { get; private set; }
        public bool ColumnaCuadrillaVisible { get; private set; }
        public int InicioNumeracion { get; private set; }

        public Propiedades(
            int grabacionAutomatica,
            int inicioNumeracion,
            bool columnaTipoCalculoVisible,
            bool columnaRendimientoVisible,
            bool columnaHorasJornalVisible,
            bool columnaCuadrillaVisible)
        {
            InitializeComponent();
            viewModel = new PropiedadesViewModel
            {
                GrabacionAutomatica = grabacionAutomatica,
                InicioNumeracion = inicioNumeracion,
                ColumnaTipoCalculoVisible = columnaTipoCalculoVisible,
                ColumnaRendimientoVisible = columnaRendimientoVisible,
                ColumnaHorasJornalVisible = columnaHorasJornalVisible,
                ColumnaCuadrillaVisible = columnaCuadrillaVisible
            };
            propertyGrid1.SelectedObject = viewModel;
            GrabacionAutomatica = grabacionAutomatica;
            InicioNumeracion = inicioNumeracion;
            ColumnaTipoCalculoVisible = columnaTipoCalculoVisible;
            ColumnaRendimientoVisible = columnaRendimientoVisible;
            ColumnaHorasJornalVisible = columnaHorasJornalVisible;
            ColumnaCuadrillaVisible = columnaCuadrillaVisible;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (viewModel.GrabacionAutomatica < 0)
            {
                MessageBox.Show("GrabacionAutomatica debe ser mayor o igual a 0.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GrabacionAutomatica = viewModel.GrabacionAutomatica;
            InicioNumeracion = viewModel.InicioNumeracion;
            ColumnaTipoCalculoVisible = viewModel.ColumnaTipoCalculoVisible;
            ColumnaRendimientoVisible = viewModel.ColumnaRendimientoVisible;
            ColumnaHorasJornalVisible = viewModel.ColumnaHorasJornalVisible;
            ColumnaCuadrillaVisible = viewModel.ColumnaCuadrillaVisible;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private sealed class PropiedadesViewModel
        {
            [Category("General")]
            [DisplayName("GrabacionAutomatica")]
            [Description("Intervalo en segundos entre grabaciones automaticas. Si es 0, se desactiva la grabacion automatica.")]
            public int GrabacionAutomatica { get; set; } = 300;

            [Category("General")]
            [DisplayName("InicioNumeracion")]
            [Description("Numero inicial para la numeracion de filas. Por defecto es 1. Puede ser 0, 10, etc.")]
            public int InicioNumeracion { get; set; } = 1;

            [Category("Columnas")]
            [DisplayName("ColumnaTipoCalculoVisible")]
            [Description("Controla si la columna Tipo Calculo se muestra en el arbol principal.")]
            public bool ColumnaTipoCalculoVisible { get; set; } = true;

            [Category("Columnas")]
            [DisplayName("ColumnaRendimientoVisible")]
            [Description("Controla si la columna Rendimiento se muestra en el arbol principal.")]
            public bool ColumnaRendimientoVisible { get; set; } = true;

            [Category("Columnas")]
            [DisplayName("ColumnaHorasJornalVisible")]
            [Description("Controla si la columna Horas Jornal se muestra en el arbol principal.")]
            public bool ColumnaHorasJornalVisible { get; set; } = true;

            [Category("Columnas")]
            [DisplayName("ColumnaCuadrillaVisible")]
            [Description("Controla si la columna Cuadrilla se muestra en el arbol principal.")]
            public bool ColumnaCuadrillaVisible { get; set; } = true;
        }
    }
}
