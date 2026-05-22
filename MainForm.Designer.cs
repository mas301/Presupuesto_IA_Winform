namespace DevExpressTreeListDemo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private System.Windows.Forms.ContextMenuStrip treeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuAddAbove;
        private System.Windows.Forms.ToolStripMenuItem menuAddBelow;
        private System.Windows.Forms.ToolStripMenuItem menuAddSubItem;
        private System.Windows.Forms.ToolStripSeparator menuEditSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteItem;
        private System.Windows.Forms.ToolStripMenuItem menuCopyItem;
        private System.Windows.Forms.ToolStripMenuItem menuCutItem;
        private System.Windows.Forms.ToolStripMenuItem menuPasteItem;
        private System.Windows.Forms.ToolStripMenuItem menuPasteItemBelow;
        private System.Windows.Forms.ToolStripSeparator menuMoveSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuMoveRight;
        private System.Windows.Forms.ToolStripMenuItem menuMoveLeft;
        private System.Windows.Forms.ToolStripMenuItem menuMoveUp;
        private System.Windows.Forms.ToolStripMenuItem menuMoveDown;
        private System.Windows.Forms.ToolStripSeparator menuBottomSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuModifyResource;
        private System.Windows.Forms.ContextMenuStrip emptyContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuAddRootItem;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Button btnSaveBudgetDetail;
        private System.Windows.Forms.Button btnCreateResource;
        private System.Windows.Forms.Button btnRecalculateAll;
        private System.Windows.Forms.Button btnPropiedades;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Label lblPendingSaveStatus;

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
            this.components = new System.ComponentModel.Container();
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.treeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddAbove = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddBelow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddSubItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuDeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCutItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPasteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPasteItemBelow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuMoveRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBottomSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuModifyResource = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddRootItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topPanel = new System.Windows.Forms.Panel();
            this.btnSaveBudgetDetail = new System.Windows.Forms.Button();
            this.btnCreateResource = new System.Windows.Forms.Button();
            this.btnRecalculateAll = new System.Windows.Forms.Button();
            this.btnPropiedades = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.lblPendingSaveStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            this.topPanel.SuspendLayout();
            this.emptyContextMenu.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.lblPendingSaveStatus);
            this.topPanel.Controls.Add(this.btnMoveLeft);
            this.topPanel.Controls.Add(this.btnMoveDown);
            this.topPanel.Controls.Add(this.btnMoveRight);
            this.topPanel.Controls.Add(this.btnMoveUp);
            this.topPanel.Controls.Add(this.btnPropiedades);
            this.topPanel.Controls.Add(this.btnRecalculateAll);
            this.topPanel.Controls.Add(this.btnCreateResource);
            this.topPanel.Controls.Add(this.btnSaveBudgetDetail);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(12, 6, 12, 4);
            this.topPanel.Size = new System.Drawing.Size(900, 40);
            this.topPanel.TabIndex = 1;
            // 
            // btnCreateResource
            // 
            this.btnCreateResource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(167)))), ((int)(((byte)(84)))));
            this.btnCreateResource.FlatAppearance.BorderSize = 0;
            this.btnCreateResource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateResource.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateResource.ForeColor = System.Drawing.Color.White;
            this.btnCreateResource.Location = new System.Drawing.Point(138, 5);
            this.btnCreateResource.Name = "btnCreateResource";
            this.btnCreateResource.Size = new System.Drawing.Size(116, 28);
            this.btnCreateResource.TabIndex = 1;
            this.btnCreateResource.Text = "Crear recurso";
            this.btnCreateResource.UseVisualStyleBackColor = false;
            this.btnCreateResource.Click += new System.EventHandler(this.btnCreateResource_Click);
            // 
            // btnRecalculateAll
            // 
            this.btnRecalculateAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(126)))), ((int)(((byte)(33)))));
            this.btnRecalculateAll.FlatAppearance.BorderSize = 0;
            this.btnRecalculateAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecalculateAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecalculateAll.ForeColor = System.Drawing.Color.White;
            this.btnRecalculateAll.Location = new System.Drawing.Point(260, 5);
            this.btnRecalculateAll.Name = "btnRecalculateAll";
            this.btnRecalculateAll.Size = new System.Drawing.Size(120, 28);
            this.btnRecalculateAll.TabIndex = 2;
            this.btnRecalculateAll.Text = "Recalcular todo";
            this.btnRecalculateAll.UseVisualStyleBackColor = false;
            this.btnRecalculateAll.Click += new System.EventHandler(this.btnRecalculateAll_Click);
            // 
            // btnPropiedades
            // 
            this.btnPropiedades.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnPropiedades.FlatAppearance.BorderSize = 0;
            this.btnPropiedades.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPropiedades.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPropiedades.ForeColor = System.Drawing.Color.White;
            this.btnPropiedades.Location = new System.Drawing.Point(386, 5);
            this.btnPropiedades.Name = "btnPropiedades";
            this.btnPropiedades.Size = new System.Drawing.Size(110, 28);
            this.btnPropiedades.TabIndex = 3;
            this.btnPropiedades.Text = "Propiedades";
            this.btnPropiedades.UseVisualStyleBackColor = false;
            this.btnPropiedades.Click += new System.EventHandler(this.btnPropiedades_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMoveUp.FlatAppearance.BorderSize = 0;
            this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveUp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveUp.ForeColor = System.Drawing.Color.White;
            this.btnMoveUp.Location = new System.Drawing.Point(502, 5);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(44, 28);
            this.btnMoveUp.TabIndex = 4;
            this.btnMoveUp.Text = "↑";
            this.btnMoveUp.Enabled = false;
            this.btnMoveUp.UseVisualStyleBackColor = false;
            this.btnMoveUp.Click += new System.EventHandler(this.menuMoveUp_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMoveRight.FlatAppearance.BorderSize = 0;
            this.btnMoveRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveRight.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveRight.ForeColor = System.Drawing.Color.White;
            this.btnMoveRight.Location = new System.Drawing.Point(552, 5);
            this.btnMoveRight.Name = "btnMoveRight";
            this.btnMoveRight.Size = new System.Drawing.Size(44, 28);
            this.btnMoveRight.TabIndex = 5;
            this.btnMoveRight.Text = "→";
            this.btnMoveRight.Enabled = false;
            this.btnMoveRight.UseVisualStyleBackColor = false;
            this.btnMoveRight.Click += new System.EventHandler(this.menuMoveRight_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMoveDown.FlatAppearance.BorderSize = 0;
            this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveDown.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveDown.ForeColor = System.Drawing.Color.White;
            this.btnMoveDown.Location = new System.Drawing.Point(602, 5);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(44, 28);
            this.btnMoveDown.TabIndex = 6;
            this.btnMoveDown.Text = "↓";
            this.btnMoveDown.Enabled = false;
            this.btnMoveDown.UseVisualStyleBackColor = false;
            this.btnMoveDown.Click += new System.EventHandler(this.menuMoveDown_Click);
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMoveLeft.FlatAppearance.BorderSize = 0;
            this.btnMoveLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveLeft.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveLeft.ForeColor = System.Drawing.Color.White;
            this.btnMoveLeft.Location = new System.Drawing.Point(652, 5);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(44, 28);
            this.btnMoveLeft.TabIndex = 7;
            this.btnMoveLeft.Text = "←";
            this.btnMoveLeft.Enabled = false;
            this.btnMoveLeft.UseVisualStyleBackColor = false;
            this.btnMoveLeft.Click += new System.EventHandler(this.menuMoveLeft_Click);
            // 
            // lblPendingSaveStatus
            // 
            this.lblPendingSaveStatus.AutoSize = true;
            this.lblPendingSaveStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPendingSaveStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblPendingSaveStatus.Location = new System.Drawing.Point(906, 11);
            this.lblPendingSaveStatus.Name = "lblPendingSaveStatus";
            this.lblPendingSaveStatus.Size = new System.Drawing.Size(0, 15);
            this.lblPendingSaveStatus.TabIndex = 8;
            this.lblPendingSaveStatus.Text = "";
            // 
            // btnSaveBudgetDetail
            // 
            this.btnSaveBudgetDetail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(230)))));
            this.btnSaveBudgetDetail.FlatAppearance.BorderSize = 0;
            this.btnSaveBudgetDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveBudgetDetail.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveBudgetDetail.ForeColor = System.Drawing.Color.White;
            this.btnSaveBudgetDetail.Location = new System.Drawing.Point(12, 5);
            this.btnSaveBudgetDetail.Name = "btnSaveBudgetDetail";
            this.btnSaveBudgetDetail.Size = new System.Drawing.Size(120, 28);
            this.btnSaveBudgetDetail.TabIndex = 0;
            this.btnSaveBudgetDetail.Text = "Grabar detalle";
            this.btnSaveBudgetDetail.UseVisualStyleBackColor = false;
            this.btnSaveBudgetDetail.Click += new System.EventHandler(this.btnSaveBudgetDetail_Click);
            // 
            // emptyContextMenu
            // 
            this.emptyContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddRootItem});
            this.emptyContextMenu.Name = "emptyContextMenu";
            this.emptyContextMenu.Size = new System.Drawing.Size(164, 26);
            // 
            // menuAddRootItem
            // 
            this.menuAddRootItem.Name = "menuAddRootItem";
            this.menuAddRootItem.Size = new System.Drawing.Size(163, 22);
            this.menuAddRootItem.Text = "Agregar item";
            this.menuAddRootItem.Click += new System.EventHandler(this.menuAddRootItem_Click);
            // treeContextMenu
            // 
            this.treeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddAbove,
            this.menuAddBelow,
            this.menuAddSubItem,
            this.menuEditSeparator,
            this.menuCopyItem,
            this.menuCutItem,
            this.menuPasteItem,
            this.menuPasteItemBelow,
            this.menuMoveSeparator,
            this.menuMoveRight,
            this.menuMoveLeft,
            this.menuMoveUp,
            this.menuMoveDown,
            this.menuDeleteItem,
            this.menuBottomSeparator,
            this.menuModifyResource});
            this.treeContextMenu.Name = "treeContextMenu";
            this.treeContextMenu.Size = new System.Drawing.Size(224, 324);
            // 
            // menuAddAbove
            // 
            this.menuAddAbove.Name = "menuAddAbove";
            this.menuAddAbove.Size = new System.Drawing.Size(163, 22);
            this.menuAddAbove.Text = "Agregar item arriba";
            this.menuAddAbove.Click += new System.EventHandler(this.menuAddAbove_Click);
            // 
            // menuAddBelow
            // 
            this.menuAddBelow.Name = "menuAddBelow";
            this.menuAddBelow.Size = new System.Drawing.Size(163, 22);
            this.menuAddBelow.Text = "Agregar item abajo";
            this.menuAddBelow.Click += new System.EventHandler(this.menuAddBelow_Click);
            // 
            // menuAddSubItem
            // 
            this.menuAddSubItem.Name = "menuAddSubItem";
            this.menuAddSubItem.Size = new System.Drawing.Size(163, 22);
            this.menuAddSubItem.Text = "Agregar SubItem";
            this.menuAddSubItem.Click += new System.EventHandler(this.menuAddSubItem_Click);
            // 
            // menuEditSeparator
            // 
            this.menuEditSeparator.Name = "menuEditSeparator";
            this.menuEditSeparator.Size = new System.Drawing.Size(213, 6);
            // 
            // menuDeleteItem
            // 
            this.menuDeleteItem.Name = "menuDeleteItem";
            this.menuDeleteItem.Size = new System.Drawing.Size(216, 22);
            this.menuDeleteItem.Text = "Eliminar Item";
            this.menuDeleteItem.Click += new System.EventHandler(this.menuDeleteItem_Click);
            // 
            // menuCopyItem
            // 
            this.menuCopyItem.Name = "menuCopyItem";
            this.menuCopyItem.Size = new System.Drawing.Size(216, 22);
            this.menuCopyItem.Text = "Copiar Item";
            this.menuCopyItem.Click += new System.EventHandler(this.menuCopyItem_Click);
            // 
            // menuCutItem
            // 
            this.menuCutItem.Name = "menuCutItem";
            this.menuCutItem.Size = new System.Drawing.Size(216, 22);
            this.menuCutItem.Text = "Cortar Item";
            this.menuCutItem.Click += new System.EventHandler(this.menuCutItem_Click);
            // 
            // menuPasteItem
            // 
            this.menuPasteItem.Name = "menuPasteItem";
            this.menuPasteItem.Size = new System.Drawing.Size(216, 22);
            this.menuPasteItem.Text = "Pegar Item Arriba";
            this.menuPasteItem.Click += new System.EventHandler(this.menuPasteItem_Click);
            // 
            // menuPasteItemBelow
            // 
            this.menuPasteItemBelow.Name = "menuPasteItemBelow";
            this.menuPasteItemBelow.Size = new System.Drawing.Size(216, 22);
            this.menuPasteItemBelow.Text = "Pegar Item Abajo";
            this.menuPasteItemBelow.Click += new System.EventHandler(this.menuPasteItemBelow_Click);
            // 
            // menuMoveSeparator
            // 
            this.menuMoveSeparator.Name = "menuMoveSeparator";
            this.menuMoveSeparator.Size = new System.Drawing.Size(220, 6);
            // 
            // menuMoveRight
            // 
            this.menuMoveRight.Name = "menuMoveRight";
            this.menuMoveRight.Size = new System.Drawing.Size(223, 22);
            this.menuMoveRight.Text = "Mover Item a la derecha";
            this.menuMoveRight.Click += new System.EventHandler(this.menuMoveRight_Click);
            // 
            // menuMoveLeft
            // 
            this.menuMoveLeft.Name = "menuMoveLeft";
            this.menuMoveLeft.Size = new System.Drawing.Size(223, 22);
            this.menuMoveLeft.Text = "Mover Item a la Izquierda";
            this.menuMoveLeft.Click += new System.EventHandler(this.menuMoveLeft_Click);
            // 
            // menuMoveUp
            // 
            this.menuMoveUp.Name = "menuMoveUp";
            this.menuMoveUp.Size = new System.Drawing.Size(223, 22);
            this.menuMoveUp.Text = "Mover Item hacia Arriba";
            this.menuMoveUp.Click += new System.EventHandler(this.menuMoveUp_Click);
            // 
            // menuMoveDown
            // 
            this.menuMoveDown.Name = "menuMoveDown";
            this.menuMoveDown.Size = new System.Drawing.Size(223, 22);
            this.menuMoveDown.Text = "Mover Item hacia Abajo";
            this.menuMoveDown.Click += new System.EventHandler(this.menuMoveDown_Click);
            // 
            // menuBottomSeparator
            // 
            this.menuBottomSeparator.Name = "menuBottomSeparator";
            this.menuBottomSeparator.Size = new System.Drawing.Size(220, 6);
            // 
            // menuModifyResource
            // 
            this.menuModifyResource.Name = "menuModifyResource";
            this.menuModifyResource.Size = new System.Drawing.Size(223, 22);
            this.menuModifyResource.Text = "Modificar recurso";
            this.menuModifyResource.Click += new System.EventHandler(this.menuModifyResource_Click);
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Item", VisibleIndex = 0, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Nombre", VisibleIndex = 1, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Tipo", VisibleIndex = 2, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Unidad", VisibleIndex = 3, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Tipo Calculo", VisibleIndex = 4, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Horas Jornal", VisibleIndex = 5, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Rendimiento", VisibleIndex = 6, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Cuadrilla", VisibleIndex = 7, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Cantidad", VisibleIndex = 8, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Valor Unitario", VisibleIndex = 9, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Valor Total", VisibleIndex = 10, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Alias", Visible = false }});
            this.treeList1.ContextMenuStrip = null;
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(0, 40);
            this.treeList1.Name = "treeList1";
            this.treeList1.OptionsBehavior.Editable = false;
            this.treeList1.OptionsView.ShowIndicator = false;
            this.treeList1.Size = new System.Drawing.Size(900, 460);
            this.treeList1.TabIndex = 0;
            this.treeList1.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList1_FocusedNodeChanged);
            this.treeList1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList1_MouseDown);
            this.treeList1.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeList1_PopupMenuShowing);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.treeList1);
            this.Controls.Add(this.topPanel);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DevExpress XtraTreeList - Demo";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            this.emptyContextMenu.ResumeLayout(false);
            this.treeContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
