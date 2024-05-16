
using System.Windows.Forms;

namespace PuPuSharp
{
    partial class Entry
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Entry));
            mnuMainMenu = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            mnuImportLayers = new ToolStripMenuItem();
            mnuExportLayers = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            mnuExportModpathTextures = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            lstFields = new ListBox();
            pbImage = new PictureBox();
            lstLayers = new ListBox();
            mnuMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbImage).BeginInit();
            SuspendLayout();
            // 
            // mnuMainMenu
            // 
            mnuMainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolStripMenuItem1 });
            resources.ApplyResources(mnuMainMenu, "mnuMainMenu");
            mnuMainMenu.Name = "mnuMainMenu";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, toolStripSeparator1, mnuImportLayers, mnuExportLayers, toolStripSeparator2, mnuExportModpathTextures, toolStripSeparator3, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(openToolStripMenuItem, "openToolStripMenuItem");
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // mnuImportLayers
            // 
            mnuImportLayers.Name = "mnuImportLayers";
            resources.ApplyResources(mnuImportLayers, "mnuImportLayers");
            mnuImportLayers.Click += mnuImportLayers_Click;
            // 
            // mnuExportLayers
            // 
            mnuExportLayers.Name = "mnuExportLayers";
            resources.ApplyResources(mnuExportLayers, "mnuExportLayers");
            mnuExportLayers.Click += mnuExportLayers_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // mnuExportModpathTextures
            // 
            mnuExportModpathTextures.Name = "mnuExportModpathTextures";
            resources.ApplyResources(mnuExportModpathTextures, "mnuExportModpathTextures");
            mnuExportModpathTextures.Click += mnuExportModpathTextures_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // lstFields
            // 
            resources.ApplyResources(lstFields, "lstFields");
            lstFields.BackColor = System.Drawing.Color.Black;
            lstFields.BorderStyle = BorderStyle.None;
            lstFields.ForeColor = System.Drawing.Color.Cyan;
            lstFields.FormattingEnabled = true;
            lstFields.Items.AddRange(new object[] { resources.GetString("lstFields.Items") });
            lstFields.Name = "lstFields";
            lstFields.SelectedIndexChanged += lstFields_SelectedIndexChanged;
            // 
            // pbImage
            // 
            resources.ApplyResources(pbImage, "pbImage");
            pbImage.BackColor = System.Drawing.Color.Black;
            pbImage.Name = "pbImage";
            pbImage.TabStop = false;
            // 
            // lstLayers
            // 
            resources.ApplyResources(lstLayers, "lstLayers");
            lstLayers.BackColor = System.Drawing.Color.Black;
            lstLayers.BorderStyle = BorderStyle.None;
            lstLayers.ForeColor = System.Drawing.Color.Cyan;
            lstLayers.FormattingEnabled = true;
            lstLayers.Items.AddRange(new object[] { resources.GetString("lstLayers.Items") });
            lstLayers.Name = "lstLayers";
            lstLayers.SelectedIndexChanged += lstLayers_SelectedIndexChanged;
            // 
            // Entry
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lstLayers);
            Controls.Add(pbImage);
            Controls.Add(lstFields);
            Controls.Add(mnuMainMenu);
            MainMenuStrip = mnuMainMenu;
            Name = "Entry";
            mnuMainMenu.ResumeLayout(false);
            mnuMainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mnuMainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ListBox lstFields;
        private PictureBox pbImage;
        private ListBox lstLayers;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mnuExportLayers;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem mnuImportLayers;
        private ToolStripMenuItem mnuExportModpathTextures;
        private ToolStripSeparator toolStripSeparator3;
    }
}
