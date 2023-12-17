
namespace AsterixDecoder
{
    partial class MainWindow
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.csvGridView = new System.Windows.Forms.DataGridView();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.title = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressLbl = new System.Windows.Forms.Label();
            this.cSVToolboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCSVFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trajectoriesSimulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTrajectoriesInKMLFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.gmap = new GMap.NET.WindowsForms.GMapControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.infoLbl = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.hourBox = new System.Windows.Forms.TextBox();
            this.fowardPictureBox = new System.Windows.Forms.PictureBox();
            this.playPictureBox = new System.Windows.Forms.PictureBox();
            this.backPictureBox = new System.Windows.Forms.PictureBox();
            this.speedDecisionBox = new System.Windows.Forms.ComboBox();
            this.IsMapas = new System.Windows.Forms.ComboBox();
            this.viewPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.csvGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fowardPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backPictureBox)).BeginInit();
            this.viewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // csvGridView
            // 
            this.csvGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.csvGridView.Location = new System.Drawing.Point(13, 43);
            this.csvGridView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.csvGridView.Name = "csvGridView";
            this.csvGridView.RowHeadersWidth = 51;
            this.csvGridView.RowTemplate.Height = 24;
            this.csvGridView.Size = new System.Drawing.Size(1518, 416);
            this.csvGridView.TabIndex = 3;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(1020, 537);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(511, 57);
            this.pictureBoxLogo.TabIndex = 4;
            this.pictureBoxLogo.TabStop = false;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.BackColor = System.Drawing.Color.Transparent;
            this.title.ForeColor = System.Drawing.Color.Transparent;
            this.title.Location = new System.Drawing.Point(1249, 476);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(46, 17);
            this.title.TabIndex = 5;
            this.title.Text = "label1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(-1, 598);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1548, 10);
            this.progressBar1.TabIndex = 6;
            // 
            // progressLbl
            // 
            this.progressLbl.AutoSize = true;
            this.progressLbl.Font = new System.Drawing.Font("Cascadia Code", 8F);
            this.progressLbl.Location = new System.Drawing.Point(9, 573);
            this.progressLbl.Name = "progressLbl";
            this.progressLbl.Size = new System.Drawing.Size(88, 18);
            this.progressLbl.TabIndex = 10;
            this.progressLbl.Text = "Loading...";
            // 
            // cSVToolboxToolStripMenuItem
            // 
            this.cSVToolboxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importCSVFileToolStripMenuItem});
            this.cSVToolboxToolStripMenuItem.Font = new System.Drawing.Font("Cascadia Code", 9F);
            this.cSVToolboxToolStripMenuItem.Name = "cSVToolboxToolStripMenuItem";
            this.cSVToolboxToolStripMenuItem.Size = new System.Drawing.Size(131, 26);
            this.cSVToolboxToolStripMenuItem.Text = "Initial page";
            this.cSVToolboxToolStripMenuItem.Click += new System.EventHandler(this.cSVToolboxToolStripMenuItem_Click);
            // 
            // importCSVFileToolStripMenuItem
            // 
            this.importCSVFileToolStripMenuItem.Name = "importCSVFileToolStripMenuItem";
            this.importCSVFileToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.importCSVFileToolStripMenuItem.Text = "Import CSV file";
            this.importCSVFileToolStripMenuItem.Click += new System.EventHandler(this.importCSVFileToolStripMenuItem_Click);
            // 
            // trajectoriesSimulatorToolStripMenuItem
            // 
            this.trajectoriesSimulatorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simulateToolStripMenuItem,
            this.saveTrajectoriesInKMLFormatToolStripMenuItem});
            this.trajectoriesSimulatorToolStripMenuItem.Font = new System.Drawing.Font("Cascadia Code", 9F);
            this.trajectoriesSimulatorToolStripMenuItem.Name = "trajectoriesSimulatorToolStripMenuItem";
            this.trajectoriesSimulatorToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.trajectoriesSimulatorToolStripMenuItem.Text = "Trajectories simulator";
            this.trajectoriesSimulatorToolStripMenuItem.Click += new System.EventHandler(this.trajectoriesSimulatorToolStripMenuItem_Click);
            // 
            // simulateToolStripMenuItem
            // 
            this.simulateToolStripMenuItem.Name = "simulateToolStripMenuItem";
            this.simulateToolStripMenuItem.Size = new System.Drawing.Size(371, 26);
            this.simulateToolStripMenuItem.Text = "Simulate";
            this.simulateToolStripMenuItem.Click += new System.EventHandler(this.simulateToolStripMenuItem_Click);
            // 
            // saveTrajectoriesInKMLFormatToolStripMenuItem
            // 
            this.saveTrajectoriesInKMLFormatToolStripMenuItem.Name = "saveTrajectoriesInKMLFormatToolStripMenuItem";
            this.saveTrajectoriesInKMLFormatToolStripMenuItem.Size = new System.Drawing.Size(371, 26);
            this.saveTrajectoriesInKMLFormatToolStripMenuItem.Text = "Save trajectories in KML format";
            this.saveTrajectoriesInKMLFormatToolStripMenuItem.Click += new System.EventHandler(this.saveTrajectoriesInKMLFormatToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cSVToolboxToolStripMenuItem,
            this.trajectoriesSimulatorToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1560, 30);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gmap
            // 
            this.gmap.AutoSize = true;
            this.gmap.BackColor = System.Drawing.Color.Transparent;
            this.gmap.Bearing = 0F;
            this.gmap.CanDragMap = true;
            this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmap.GrayScaleMode = false;
            this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmap.LevelsKeepInMemory = 5;
            this.gmap.Location = new System.Drawing.Point(11, 32);
            this.gmap.Margin = new System.Windows.Forms.Padding(4);
            this.gmap.MarkersEnabled = true;
            this.gmap.MaxZoom = 2;
            this.gmap.MinZoom = 2;
            this.gmap.MouseWheelZoomEnabled = true;
            this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gmap.Name = "gmap";
            this.gmap.NegativeMode = false;
            this.gmap.PolygonsEnabled = true;
            this.gmap.RetryLoadTile = 0;
            this.gmap.RoutesEnabled = true;
            this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmap.ShowTileGridLines = false;
            this.gmap.Size = new System.Drawing.Size(1520, 427);
            this.gmap.TabIndex = 11;
            this.gmap.Zoom = 0D;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // infoLbl
            // 
            this.infoLbl.AutoSize = true;
            this.infoLbl.Font = new System.Drawing.Font("Cascadia Code", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLbl.Location = new System.Drawing.Point(13, 541);
            this.infoLbl.Name = "infoLbl";
            this.infoLbl.Size = new System.Drawing.Size(78, 25);
            this.infoLbl.TabIndex = 12;
            this.infoLbl.Text = "label1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(-21, 463);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1571, 68);
            this.flowLayoutPanel1.TabIndex = 21;
            // 
            // hourBox
            // 
            this.hourBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hourBox.Font = new System.Drawing.Font("Cascadia Code", 11.8F);
            this.hourBox.Location = new System.Drawing.Point(644, 20);
            this.hourBox.Margin = new System.Windows.Forms.Padding(20, 20, 40, 3);
            this.hourBox.Name = "hourBox";
            this.hourBox.Size = new System.Drawing.Size(114, 30);
            this.hourBox.TabIndex = 20;
            // 
            // fowardPictureBox
            // 
            this.fowardPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fowardPictureBox.Location = new System.Drawing.Point(583, 3);
            this.fowardPictureBox.Name = "fowardPictureBox";
            this.fowardPictureBox.Size = new System.Drawing.Size(38, 29);
            this.fowardPictureBox.TabIndex = 15;
            this.fowardPictureBox.TabStop = false;
            this.fowardPictureBox.Click += new System.EventHandler(this.fowardPictureBox_Click);
            // 
            // playPictureBox
            // 
            this.playPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.playPictureBox.Location = new System.Drawing.Point(513, 3);
            this.playPictureBox.Name = "playPictureBox";
            this.playPictureBox.Size = new System.Drawing.Size(64, 29);
            this.playPictureBox.TabIndex = 12;
            this.playPictureBox.TabStop = false;
            this.playPictureBox.Click += new System.EventHandler(this.playPictureBox_Click);
            // 
            // backPictureBox
            // 
            this.backPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.backPictureBox.Location = new System.Drawing.Point(424, 3);
            this.backPictureBox.Name = "backPictureBox";
            this.backPictureBox.Size = new System.Drawing.Size(83, 29);
            this.backPictureBox.TabIndex = 14;
            this.backPictureBox.TabStop = false;
            this.backPictureBox.Click += new System.EventHandler(this.backPictureBox_Click);
            // 
            // speedDecisionBox
            // 
            this.speedDecisionBox.BackColor = System.Drawing.SystemColors.Window;
            this.speedDecisionBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.speedDecisionBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.speedDecisionBox.Font = new System.Drawing.Font("Cascadia Code", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedDecisionBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.speedDecisionBox.FormattingEnabled = true;
            this.speedDecisionBox.Items.AddRange(new object[] {
            "x 0.5",
            "x 1",
            "x 2",
            "x 5",
            "x 10",
            "x 100",
            "x 200"});
            this.speedDecisionBox.Location = new System.Drawing.Point(298, 20);
            this.speedDecisionBox.Margin = new System.Windows.Forms.Padding(20, 20, 20, 3);
            this.speedDecisionBox.Name = "speedDecisionBox";
            this.speedDecisionBox.Size = new System.Drawing.Size(103, 30);
            this.speedDecisionBox.TabIndex = 19;
            this.speedDecisionBox.SelectedIndexChanged += new System.EventHandler(this.speedDecisionBox_SelectedIndexChanged);
            // 
            // IsMapas
            // 
            this.IsMapas.BackColor = System.Drawing.SystemColors.Window;
            this.IsMapas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.IsMapas.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.IsMapas.Font = new System.Drawing.Font("Cascadia Code", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsMapas.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.IsMapas.FormattingEnabled = true;
            this.IsMapas.Items.AddRange(new object[] {
            "Google Maps Satélite",
            "Google Maps Callejero",
            "Google Maps Híbrido",
            "OpenStreetMap",
            "OpenCycleMap"});
            this.IsMapas.Location = new System.Drawing.Point(10, 20);
            this.IsMapas.Margin = new System.Windows.Forms.Padding(10, 20, 3, 3);
            this.IsMapas.Name = "IsMapas";
            this.IsMapas.Size = new System.Drawing.Size(265, 30);
            this.IsMapas.TabIndex = 21;
            this.IsMapas.SelectedIndexChanged += new System.EventHandler(this.IsMapas_SelectedIndexChanged);
            // 
            // viewPanel
            // 
            this.viewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.viewPanel.Controls.Add(this.IsMapas);
            this.viewPanel.Controls.Add(this.speedDecisionBox);
            this.viewPanel.Controls.Add(this.backPictureBox);
            this.viewPanel.Controls.Add(this.playPictureBox);
            this.viewPanel.Controls.Add(this.fowardPictureBox);
            this.viewPanel.Controls.Add(this.hourBox);
            this.viewPanel.Location = new System.Drawing.Point(235, 463);
            this.viewPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.viewPanel.Name = "viewPanel";
            this.viewPanel.Size = new System.Drawing.Size(827, 68);
            this.viewPanel.TabIndex = 9;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1560, 607);
            this.Controls.Add(this.infoLbl);
            this.Controls.Add(this.gmap);
            this.Controls.Add(this.csvGridView);
            this.Controls.Add(this.progressLbl);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.title);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.viewPanel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainWindow";
            this.Text = "Asterix Decoder";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.csvGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fowardPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backPictureBox)).EndInit();
            this.viewPanel.ResumeLayout(false);
            this.viewPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView csvGridView;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label progressLbl;
        private System.Windows.Forms.ToolStripMenuItem cSVToolboxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trajectoriesSimulatorToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.ToolStripMenuItem saveTrajectoriesInKMLFormatToolStripMenuItem;
        public System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label infoLbl;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox hourBox;
        private System.Windows.Forms.PictureBox fowardPictureBox;
        private System.Windows.Forms.PictureBox playPictureBox;
        private System.Windows.Forms.PictureBox backPictureBox;
        private System.Windows.Forms.ComboBox speedDecisionBox;
        private System.Windows.Forms.ComboBox IsMapas;
        private System.Windows.Forms.FlowLayoutPanel viewPanel;
        private System.Windows.Forms.ToolStripMenuItem simulateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importCSVFileToolStripMenuItem;
    }
}

