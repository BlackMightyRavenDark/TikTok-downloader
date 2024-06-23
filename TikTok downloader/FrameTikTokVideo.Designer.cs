
namespace TikTok_downloader
{
	partial class FrameTikTokVideo
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

		#region Код, автоматически созданный конструктором компонентов

		/// <summary> 
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBoxImagePreview = new System.Windows.Forms.PictureBox();
			this.btnDownload = new System.Windows.Forms.Button();
			this.lblVideoTitle = new System.Windows.Forms.Label();
			this.progressBarDownload = new System.Windows.Forms.ProgressBar();
			this.lblChannelName = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImagePreview)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxImagePreview
			// 
			this.pictureBoxImagePreview.BackColor = System.Drawing.Color.Black;
			this.pictureBoxImagePreview.Location = new System.Drawing.Point(14, 7);
			this.pictureBoxImagePreview.Name = "pictureBoxImagePreview";
			this.pictureBoxImagePreview.Size = new System.Drawing.Size(182, 324);
			this.pictureBoxImagePreview.TabIndex = 0;
			this.pictureBoxImagePreview.TabStop = false;
			this.pictureBoxImagePreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxImagePreview_Paint);
			this.pictureBoxImagePreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxImagePreview_MouseDown);
			// 
			// btnDownload
			// 
			this.btnDownload.Location = new System.Drawing.Point(132, 371);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(75, 23);
			this.btnDownload.TabIndex = 1;
			this.btnDownload.Text = "Скачать";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			// 
			// lblVideoTitle
			// 
			this.lblVideoTitle.BackColor = System.Drawing.Color.Black;
			this.lblVideoTitle.ForeColor = System.Drawing.Color.White;
			this.lblVideoTitle.Location = new System.Drawing.Point(7, 339);
			this.lblVideoTitle.Name = "lblVideoTitle";
			this.lblVideoTitle.Size = new System.Drawing.Size(197, 29);
			this.lblVideoTitle.TabIndex = 2;
			this.lblVideoTitle.Text = "lblVideoTitle";
			// 
			// progressBarDownload
			// 
			this.progressBarDownload.Location = new System.Drawing.Point(3, 396);
			this.progressBarDownload.Name = "progressBarDownload";
			this.progressBarDownload.Size = new System.Drawing.Size(204, 18);
			this.progressBarDownload.TabIndex = 3;
			// 
			// lblChannelName
			// 
			this.lblChannelName.BackColor = System.Drawing.Color.Black;
			this.lblChannelName.ForeColor = System.Drawing.Color.Yellow;
			this.lblChannelName.Location = new System.Drawing.Point(7, 376);
			this.lblChannelName.Name = "lblChannelName";
			this.lblChannelName.Size = new System.Drawing.Size(119, 15);
			this.lblChannelName.TabIndex = 4;
			this.lblChannelName.Text = "lblChannelName";
			// 
			// FrameTikTokVideo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.Controls.Add(this.lblChannelName);
			this.Controls.Add(this.progressBarDownload);
			this.Controls.Add(this.lblVideoTitle);
			this.Controls.Add(this.btnDownload);
			this.Controls.Add(this.pictureBoxImagePreview);
			this.Name = "FrameTikTokVideo";
			this.Size = new System.Drawing.Size(210, 417);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImagePreview)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxImagePreview;
		private System.Windows.Forms.Label lblVideoTitle;
		public System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.Label lblChannelName;
		public System.Windows.Forms.ProgressBar progressBarDownload;
	}
}
