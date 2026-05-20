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
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            this.emptyContextMenu.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            this.SuspendLayout();
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
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Item", VisibleIndex = 0, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Nombre", VisibleIndex = 1, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Tipo", VisibleIndex = 2, Visible = true },
            new DevExpress.XtraTreeList.Columns.TreeListColumn() { Caption = "Estado", VisibleIndex = 3, Visible = true }});
            this.treeList1.ContextMenuStrip = null;
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(0, 0);
            this.treeList1.Name = "treeList1";
            this.treeList1.OptionsBehavior.Editable = false;
            this.treeList1.OptionsView.ShowIndicator = false;
            this.treeList1.Size = new System.Drawing.Size(900, 500);
            this.treeList1.TabIndex = 0;
            this.treeList1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList1_MouseDown);
            this.treeList1.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeList1_PopupMenuShowing);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.treeList1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DevExpress XtraTreeList - Demo";
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            this.emptyContextMenu.ResumeLayout(false);
            this.treeContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
