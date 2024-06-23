using System;
using System.Drawing;
using System.Windows.Forms;

namespace TikTok_downloader
{
    public partial class FrameTikTokVideo : UserControl
    {
        public TikTokVideo VideoInfo { get; private set; }

        public delegate void DownloadButtonPressedDelegate(object sender);
        public delegate void ImageMouseDownDelegate(object sender, MouseEventArgs e);
        public DownloadButtonPressedDelegate DownloadButtonPressed;
        public ImageMouseDownDelegate ImageMouseDown;

        public FrameTikTokVideo(TikTokVideo tikTokVideo)
        {
            InitializeComponent();

            SetVideoInfo(tikTokVideo);
        }

        private void SetVideoInfo(TikTokVideo tikTokVideo)
        {
            VideoInfo = tikTokVideo;
            lblVideoTitle.Text = tikTokVideo.Title;
            lblChannelName.Text = tikTokVideo.Author.UniqueId;
        }

        private void pictureBoxImagePreview_Paint(object sender, PaintEventArgs e)
        {
            if (VideoInfo.ImagePreview != null)
            {
                Rectangle imageRect = new Rectangle(0, 0, VideoInfo.ImagePreview.Width, VideoInfo.ImagePreview.Height);
                Rectangle imageRectResized = imageRect.ResizeTo(pictureBoxImagePreview.ClientSize).CenterIn(pictureBoxImagePreview.ClientRectangle);
                e.Graphics.DrawImage(VideoInfo.ImagePreview, imageRectResized);
            }
            if (VideoInfo.DateCreation > DateTime.MinValue)
            {
                string dateString = VideoInfo.DateCreation.ToString();
                SizeF sz = e.Graphics.MeasureString(dateString, Font);
                float x = pictureBoxImagePreview.Width - sz.Width;
                float y = pictureBoxImagePreview.Height - sz.Height;
                RectangleF r = new RectangleF(x, y, x + sz.Width, y + sz.Height);
                e.Graphics.FillRectangle(Brushes.Black, r);
                e.Graphics.DrawString(dateString, Font, Brushes.White, x, y);
            }
            string t = "Длина: " + VideoInfo.Duration.ToString("m':'ss");
            float yPos = 0.0f;
            SizeF size = e.Graphics.MeasureString(t, Font);
            RectangleF rect = new RectangleF(0.0f, yPos, size.Width, size.Height);
            e.Graphics.FillRectangle(Brushes.Black, rect);
            e.Graphics.DrawString(t, Font, Brushes.White, 0.0f, yPos);
            yPos += size.Height;

            t = "Автор: " + VideoInfo.Author.NickName;
            size = e.Graphics.MeasureString(t, Font);
            rect = new RectangleF(0.0f, yPos, size.Width, size.Height);
            e.Graphics.FillRectangle(Brushes.Black, rect);
            e.Graphics.DrawString(t, Font, Brushes.White, 0.0f, yPos);
            yPos += size.Height;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadButtonPressed?.Invoke(this);
        }

        private void pictureBoxImagePreview_MouseDown(object sender, MouseEventArgs e)
        {
            ImageMouseDown?.Invoke(this, e);
        }
    }
}
