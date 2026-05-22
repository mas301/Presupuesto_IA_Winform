namespace DevExpressTreeListDemo
{
    partial class ResourceEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTipoRecurso;
        private System.Windows.Forms.ComboBox cmbTipoRecurso;
        private System.Windows.Forms.Label lblRecurso;
        private System.Windows.Forms.TextBox txtRecurso;
        private System.Windows.Forms.Label lblAlias;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Label lblUnidad;
        private System.Windows.Forms.ComboBox cmbUnidad;
        private System.Windows.Forms.Label lblTipoCalculo;
        private System.Windows.Forms.ComboBox cmbTipoCalculo;
        private System.Windows.Forms.Label lblRendimientoManoObra;
        private System.Windows.Forms.NumericUpDown nudRendimientoManoObra;
        private System.Windows.Forms.Label lblRendimientoEquipos;
        private System.Windows.Forms.NumericUpDown nudRendimientoEquipos;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Button btnGrabar;
        private System.Windows.Forms.Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTipoRecurso = new System.Windows.Forms.Label();
            this.cmbTipoRecurso = new System.Windows.Forms.ComboBox();
            this.lblRecurso = new System.Windows.Forms.Label();
            this.txtRecurso = new System.Windows.Forms.TextBox();
            this.lblAlias = new System.Windows.Forms.Label();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.lblUnidad = new System.Windows.Forms.Label();
            this.cmbUnidad = new System.Windows.Forms.ComboBox();
            this.lblTipoCalculo = new System.Windows.Forms.Label();
            this.cmbTipoCalculo = new System.Windows.Forms.ComboBox();
            this.lblRendimientoManoObra = new System.Windows.Forms.Label();
            this.nudRendimientoManoObra = new System.Windows.Forms.NumericUpDown();
            this.lblRendimientoEquipos = new System.Windows.Forms.Label();
            this.nudRendimientoEquipos = new System.Windows.Forms.NumericUpDown();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnGrabar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudRendimientoManoObra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRendimientoEquipos)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTipoRecurso
            // 
            this.lblTipoRecurso.AutoSize = true;
            this.lblTipoRecurso.Location = new System.Drawing.Point(16, 18);
            this.lblTipoRecurso.Name = "lblTipoRecurso";
            this.lblTipoRecurso.Size = new System.Drawing.Size(79, 13);
            this.lblTipoRecurso.TabIndex = 0;
            this.lblTipoRecurso.Text = "Tipo Recurso";
            // 
            // cmbTipoRecurso
            // 
            this.cmbTipoRecurso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoRecurso.FormattingEnabled = true;
            this.cmbTipoRecurso.Location = new System.Drawing.Point(19, 34);
            this.cmbTipoRecurso.Name = "cmbTipoRecurso";
            this.cmbTipoRecurso.Size = new System.Drawing.Size(320, 21);
            this.cmbTipoRecurso.TabIndex = 1;
            this.cmbTipoRecurso.SelectedIndexChanged += new System.EventHandler(this.cmbTipoRecurso_SelectedIndexChanged);
            // 
            // lblRecurso
            // 
            this.lblRecurso.AutoSize = true;
            this.lblRecurso.Location = new System.Drawing.Point(16, 66);
            this.lblRecurso.Name = "lblRecurso";
            this.lblRecurso.Size = new System.Drawing.Size(47, 13);
            this.lblRecurso.TabIndex = 2;
            this.lblRecurso.Text = "Recurso";
            // 
            // txtRecurso
            // 
            this.txtRecurso.Location = new System.Drawing.Point(19, 82);
            this.txtRecurso.Multiline = true;
            this.txtRecurso.Name = "txtRecurso";
            this.txtRecurso.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRecurso.Size = new System.Drawing.Size(320, 52);
            this.txtRecurso.TabIndex = 3;
            this.txtRecurso.TextChanged += new System.EventHandler(this.txtRecurso_TextChanged);
            this.txtRecurso.Leave += new System.EventHandler(this.txtRecurso_Leave);
            // 
            // lblAlias
            // 
            this.lblAlias.AutoSize = true;
            this.lblAlias.Location = new System.Drawing.Point(16, 144);
            this.lblAlias.Name = "lblAlias";
            this.lblAlias.Size = new System.Drawing.Size(29, 13);
            this.lblAlias.TabIndex = 4;
            this.lblAlias.Text = "Alias";
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(19, 160);
            this.txtAlias.Multiline = true;
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAlias.Size = new System.Drawing.Size(320, 52);
            this.txtAlias.TabIndex = 5;
            this.txtAlias.Leave += new System.EventHandler(this.txtAlias_Leave);
            // 
            // lblUnidad
            // 
            this.lblUnidad.AutoSize = true;
            this.lblUnidad.Location = new System.Drawing.Point(16, 224);
            this.lblUnidad.Name = "lblUnidad";
            this.lblUnidad.Size = new System.Drawing.Size(41, 13);
            this.lblUnidad.TabIndex = 6;
            this.lblUnidad.Text = "Unidad";
            // 
            // cmbUnidad
            // 
            this.cmbUnidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnidad.FormattingEnabled = true;
            this.cmbUnidad.Location = new System.Drawing.Point(19, 240);
            this.cmbUnidad.Name = "cmbUnidad";
            this.cmbUnidad.Size = new System.Drawing.Size(320, 21);
            this.cmbUnidad.TabIndex = 7;
            // 
            // lblTipoCalculo
            // 
            this.lblTipoCalculo.AutoSize = true;
            this.lblTipoCalculo.Location = new System.Drawing.Point(16, 272);
            this.lblTipoCalculo.Name = "lblTipoCalculo";
            this.lblTipoCalculo.Size = new System.Drawing.Size(66, 13);
            this.lblTipoCalculo.TabIndex = 8;
            this.lblTipoCalculo.Text = "Tipo Calculo";
            // 
            // cmbTipoCalculo
            // 
            this.cmbTipoCalculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoCalculo.FormattingEnabled = true;
            this.cmbTipoCalculo.Location = new System.Drawing.Point(19, 288);
            this.cmbTipoCalculo.Name = "cmbTipoCalculo";
            this.cmbTipoCalculo.Size = new System.Drawing.Size(320, 21);
            this.cmbTipoCalculo.TabIndex = 9;
            // 
            // lblRendimientoManoObra
            // 
            this.lblRendimientoManoObra.AutoSize = true;
            this.lblRendimientoManoObra.Location = new System.Drawing.Point(16, 320);
            this.lblRendimientoManoObra.Name = "lblRendimientoManoObra";
            this.lblRendimientoManoObra.Size = new System.Drawing.Size(132, 13);
            this.lblRendimientoManoObra.TabIndex = 10;
            this.lblRendimientoManoObra.Text = "RendimientoManoObra";
            // 
            // nudRendimientoManoObra
            // 
            this.nudRendimientoManoObra.DecimalPlaces = 5;
            this.nudRendimientoManoObra.Location = new System.Drawing.Point(19, 336);
            this.nudRendimientoManoObra.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudRendimientoManoObra.Name = "nudRendimientoManoObra";
            this.nudRendimientoManoObra.Size = new System.Drawing.Size(320, 20);
            this.nudRendimientoManoObra.TabIndex = 11;
            // 
            // lblRendimientoEquipos
            // 
            this.lblRendimientoEquipos.AutoSize = true;
            this.lblRendimientoEquipos.Location = new System.Drawing.Point(16, 368);
            this.lblRendimientoEquipos.Name = "lblRendimientoEquipos";
            this.lblRendimientoEquipos.Size = new System.Drawing.Size(105, 13);
            this.lblRendimientoEquipos.TabIndex = 12;
            this.lblRendimientoEquipos.Text = "RendimientoEquipos";
            // 
            // nudRendimientoEquipos
            // 
            this.nudRendimientoEquipos.DecimalPlaces = 5;
            this.nudRendimientoEquipos.Location = new System.Drawing.Point(19, 384);
            this.nudRendimientoEquipos.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudRendimientoEquipos.Name = "nudRendimientoEquipos";
            this.nudRendimientoEquipos.Size = new System.Drawing.Size(320, 20);
            this.nudRendimientoEquipos.TabIndex = 13;
            // 
            // btnCrear
            // 
            this.btnCrear.Enabled = false;
            this.btnCrear.Location = new System.Drawing.Point(102, 425);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(75, 28);
            this.btnCrear.TabIndex = 14;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnGrabar
            // 
            this.btnGrabar.Location = new System.Drawing.Point(183, 425);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(75, 28);
            this.btnGrabar.TabIndex = 15;
            this.btnGrabar.Text = "Grabar";
            this.btnGrabar.UseVisualStyleBackColor = true;
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(264, 425);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 28);
            this.btnCancelar.TabIndex = 16;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // ResourceEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 469);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGrabar);
            this.Controls.Add(this.nudRendimientoEquipos);
            this.Controls.Add(this.lblRendimientoEquipos);
            this.Controls.Add(this.nudRendimientoManoObra);
            this.Controls.Add(this.lblRendimientoManoObra);
            this.Controls.Add(this.cmbTipoCalculo);
            this.Controls.Add(this.lblTipoCalculo);
            this.Controls.Add(this.cmbUnidad);
            this.Controls.Add(this.lblUnidad);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.lblAlias);
            this.Controls.Add(this.txtRecurso);
            this.Controls.Add(this.lblRecurso);
            this.Controls.Add(this.cmbTipoRecurso);
            this.Controls.Add(this.lblTipoRecurso);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResourceEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modificar recurso";
            ((System.ComponentModel.ISupportInitialize)(this.nudRendimientoManoObra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRendimientoEquipos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
