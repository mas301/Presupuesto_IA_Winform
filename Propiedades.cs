using System;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    public partial class Propiedades : Form
    {
        private readonly SettingsBag settings;

        public Propiedades(object target)
        {
            InitializeComponent();
            settings = new SettingsBag(target);
            propertyGrid1.SelectedObject = settings;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            settings.Apply();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
