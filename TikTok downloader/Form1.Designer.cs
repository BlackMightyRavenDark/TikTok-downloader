
namespace TikTok_downloader
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSearchVideoByUrl = new System.Windows.Forms.Button();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.numericUpDownMenuFontSize = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSelectBrowser = new System.Windows.Forms.Button();
            this.btnSetDefaultFileNameFormat = new System.Windows.Forms.Button();
            this.btnSelectDownloadingDirPath = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxBrowserExePath = new System.Windows.Forms.TextBox();
            this.textBoxFileNameFormat = new System.Windows.Forms.TextBox();
            this.textBoxDownloadingDirPath = new System.Windows.Forms.TextBox();
            this.tabPageSearch = new System.Windows.Forms.TabPage();
            this.panelVideoBkg = new System.Windows.Forms.Panel();
            this.menuImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miOpenInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMenuFontSize)).BeginInit();
            this.tabPageSearch.SuspendLayout();
            this.menuImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnSearchVideoByUrl);
            this.groupBox1.Controls.Add(this.textBoxUrl);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 98);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Поиск видео по ссылке";
            // 
            // btnSearchVideoByUrl
            // 
            this.btnSearchVideoByUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearchVideoByUrl.Location = new System.Drawing.Point(459, 69);
            this.btnSearchVideoByUrl.Name = "btnSearchVideoByUrl";
            this.btnSearchVideoByUrl.Size = new System.Drawing.Size(75, 23);
            this.btnSearchVideoByUrl.TabIndex = 2;
            this.btnSearchVideoByUrl.Text = "Искать";
            this.btnSearchVideoByUrl.UseVisualStyleBackColor = true;
            this.btnSearchVideoByUrl.Click += new System.EventHandler(this.btnSearchVideoByUrl_Click);
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUrl.Location = new System.Drawing.Point(21, 40);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(513, 20);
            this.textBoxUrl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Введите ссылку на видео:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageSettings);
            this.tabControl1.Controls.Add(this.tabPageSearch);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(560, 559);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageSettings.Controls.Add(this.numericUpDownMenuFontSize);
            this.tabPageSettings.Controls.Add(this.label5);
            this.tabPageSettings.Controls.Add(this.btnSelectBrowser);
            this.tabPageSettings.Controls.Add(this.btnSetDefaultFileNameFormat);
            this.tabPageSettings.Controls.Add(this.btnSelectDownloadingDirPath);
            this.tabPageSettings.Controls.Add(this.label4);
            this.tabPageSettings.Controls.Add(this.label3);
            this.tabPageSettings.Controls.Add(this.label2);
            this.tabPageSettings.Controls.Add(this.textBoxBrowserExePath);
            this.tabPageSettings.Controls.Add(this.textBoxFileNameFormat);
            this.tabPageSettings.Controls.Add(this.textBoxDownloadingDirPath);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(552, 533);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Настройки";
            // 
            // numericUpDownMenuFontSize
            // 
            this.numericUpDownMenuFontSize.Location = new System.Drawing.Point(143, 145);
            this.numericUpDownMenuFontSize.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numericUpDownMenuFontSize.Minimum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownMenuFontSize.Name = "numericUpDownMenuFontSize";
            this.numericUpDownMenuFontSize.Size = new System.Drawing.Size(38, 20);
            this.numericUpDownMenuFontSize.TabIndex = 10;
            this.numericUpDownMenuFontSize.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownMenuFontSize.ValueChanged += new System.EventHandler(this.numericUpDownMenuFontSize_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Размер шрифта в меню:";
            // 
            // btnSelectBrowser
            // 
            this.btnSelectBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectBrowser.Location = new System.Drawing.Point(505, 99);
            this.btnSelectBrowser.Name = "btnSelectBrowser";
            this.btnSelectBrowser.Size = new System.Drawing.Size(41, 23);
            this.btnSelectBrowser.TabIndex = 8;
            this.btnSelectBrowser.Text = "...";
            this.btnSelectBrowser.UseVisualStyleBackColor = true;
            this.btnSelectBrowser.Click += new System.EventHandler(this.btnSelectBrowser_Click);
            // 
            // btnSetDefaultFileNameFormat
            // 
            this.btnSetDefaultFileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDefaultFileNameFormat.Location = new System.Drawing.Point(428, 60);
            this.btnSetDefaultFileNameFormat.Name = "btnSetDefaultFileNameFormat";
            this.btnSetDefaultFileNameFormat.Size = new System.Drawing.Size(118, 23);
            this.btnSetDefaultFileNameFormat.TabIndex = 7;
            this.btnSetDefaultFileNameFormat.Text = "Вернуть как было";
            this.btnSetDefaultFileNameFormat.UseVisualStyleBackColor = true;
            this.btnSetDefaultFileNameFormat.Click += new System.EventHandler(this.btnSetDefaultFileNameFormat_Click);
            // 
            // btnSelectDownloadingDirPath
            // 
            this.btnSelectDownloadingDirPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectDownloadingDirPath.Location = new System.Drawing.Point(505, 21);
            this.btnSelectDownloadingDirPath.Name = "btnSelectDownloadingDirPath";
            this.btnSelectDownloadingDirPath.Size = new System.Drawing.Size(41, 23);
            this.btnSelectDownloadingDirPath.TabIndex = 6;
            this.btnSelectDownloadingDirPath.Text = "...";
            this.btnSelectDownloadingDirPath.UseVisualStyleBackColor = true;
            this.btnSelectDownloadingDirPath.Click += new System.EventHandler(this.btnSelectDownloadingDirPath_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Веб-браузер:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Формат имени файла:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Папка для скачивания:";
            // 
            // textBoxBrowserExePath
            // 
            this.textBoxBrowserExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBrowserExePath.Location = new System.Drawing.Point(9, 101);
            this.textBoxBrowserExePath.Name = "textBoxBrowserExePath";
            this.textBoxBrowserExePath.Size = new System.Drawing.Size(490, 20);
            this.textBoxBrowserExePath.TabIndex = 2;
            this.textBoxBrowserExePath.Leave += new System.EventHandler(this.textBoxBrowserExePath_Leave);
            // 
            // textBoxFileNameFormat
            // 
            this.textBoxFileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileNameFormat.Location = new System.Drawing.Point(9, 62);
            this.textBoxFileNameFormat.Name = "textBoxFileNameFormat";
            this.textBoxFileNameFormat.Size = new System.Drawing.Size(413, 20);
            this.textBoxFileNameFormat.TabIndex = 1;
            this.textBoxFileNameFormat.Leave += new System.EventHandler(this.textBoxFileNameFormat_Leave);
            // 
            // textBoxDownloadingDirPath
            // 
            this.textBoxDownloadingDirPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDownloadingDirPath.Location = new System.Drawing.Point(9, 23);
            this.textBoxDownloadingDirPath.Name = "textBoxDownloadingDirPath";
            this.textBoxDownloadingDirPath.Size = new System.Drawing.Size(490, 20);
            this.textBoxDownloadingDirPath.TabIndex = 0;
            this.textBoxDownloadingDirPath.Leave += new System.EventHandler(this.textBoxDownloadingDirPath_Leave);
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageSearch.Controls.Add(this.panelVideoBkg);
            this.tabPageSearch.Controls.Add(this.groupBox1);
            this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
            this.tabPageSearch.Name = "tabPageSearch";
            this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearch.Size = new System.Drawing.Size(552, 533);
            this.tabPageSearch.TabIndex = 1;
            this.tabPageSearch.Text = "Поиск";
            // 
            // panelVideoBkg
            // 
            this.panelVideoBkg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideoBkg.BackColor = System.Drawing.Color.Black;
            this.panelVideoBkg.Location = new System.Drawing.Point(6, 110);
            this.panelVideoBkg.Name = "panelVideoBkg";
            this.panelVideoBkg.Size = new System.Drawing.Size(540, 417);
            this.panelVideoBkg.TabIndex = 1;
            // 
            // menuImage
            // 
            this.menuImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOpenInBrowserToolStripMenuItem});
            this.menuImage.Name = "menuImage";
            this.menuImage.Size = new System.Drawing.Size(184, 26);
            // 
            // miOpenInBrowserToolStripMenuItem
            // 
            this.miOpenInBrowserToolStripMenuItem.Name = "miOpenInBrowserToolStripMenuItem";
            this.miOpenInBrowserToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.miOpenInBrowserToolStripMenuItem.Text = "Открыть в браузере";
            this.miOpenInBrowserToolStripMenuItem.Click += new System.EventHandler(this.miOpenInBrowserToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 583);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 626);
            this.MinimumSize = new System.Drawing.Size(270, 622);
            this.Name = "Form1";
            this.Text = "TikTok downloader";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMenuFontSize)).EndInit();
            this.tabPageSearch.ResumeLayout(false);
            this.menuImage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearchVideoByUrl;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageSearch;
        private System.Windows.Forms.Panel panelVideoBkg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxBrowserExePath;
        private System.Windows.Forms.TextBox textBoxFileNameFormat;
        private System.Windows.Forms.TextBox textBoxDownloadingDirPath;
        private System.Windows.Forms.Button btnSelectBrowser;
        private System.Windows.Forms.Button btnSetDefaultFileNameFormat;
        private System.Windows.Forms.Button btnSelectDownloadingDirPath;
        private System.Windows.Forms.ContextMenuStrip menuImage;
        private System.Windows.Forms.ToolStripMenuItem miOpenInBrowserToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numericUpDownMenuFontSize;
        private System.Windows.Forms.Label label5;
    }
}

