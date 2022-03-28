using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TikTok_downloader.Helper;

namespace TikTok_downloader
{
    public partial class Form1 : Form
    {
        private FrameTikTokVideo frameVideo = null;
        private MainConfiguration config;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            config = new MainConfiguration();
            config.Load();
            textBoxDownloadingDirPath.Text = config.downloadingDirPath;
            textBoxFileNameFormat.Text = config.fileNameFormat;
            textBoxBrowserExePath.Text = config.browserExePath;
            numericUpDownMenuFontSize.Value = config.menuFontSize;

            tabControl1.SelectedTab = tabPageSearch;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                config.Save();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterFrame();
        }

        private async void btnSearchVideoByUrlOrId_Click(object sender, EventArgs e)
        {
            btnSearchVideoByUrlOrId.Enabled = false;
            textBoxUrl.Enabled = false;

            if (frameVideo != null)
            {
                frameVideo.Dispose();
                frameVideo = null;
            }

            string urlOrId = textBoxUrl.Text;
            if (string.IsNullOrEmpty(urlOrId) || string.IsNullOrWhiteSpace(urlOrId))
            {
                MessageBox.Show("Введите ссылку или ID видео!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxUrl.Enabled = true;
                btnSearchVideoByUrlOrId.Enabled = true;
                return;
            }

            string videoId = TikTokApi.ExtractVideoIdFromUrl(urlOrId);
            if (string.IsNullOrEmpty(videoId))
            {
                MessageBox.Show("Неправильная ссылка!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxUrl.Enabled = true;
                btnSearchVideoByUrlOrId.Enabled = true;
                return;
            }

            TikTokApi api = new TikTokApi();
            TikTokVideoDetailsResult videoDetailsResult = await Task.Run(() => api.GetVideoDetails(videoId));
            if (videoDetailsResult.ErrorCode == 200)
            {
                TikTokVideo tikTokVideo = await Task.Run(() => TikTokApi.ParseTikTokInfo(videoDetailsResult.Details));
                frameVideo = new FrameTikTokVideo(tikTokVideo);
                frameVideo.Parent = panelVideoBkg;
                frameVideo.DownloadButtonPressed += OnDownloadButtonClick;
                frameVideo.ImageMouseDown += (object s, MouseEventArgs args) =>
                {
                    if (args.Button == MouseButtons.Right)
                    {
                        menuImage.Font = new Font(menuImage.Font.Name, config.menuFontSize);
                        menuImage.Show(Cursor.Position);
                    }
                };
                CenterFrame();

                textBoxUrl.Text = null;
            }
            else
            {
                MessageBox.Show("Видео не найдено!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            textBoxUrl.Enabled = true;
            btnSearchVideoByUrlOrId.Enabled = true;
        }

        private void btnSelectDownloadingDirPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку для скачивания";
            folderBrowserDialog.SelectedPath =
                (!string.IsNullOrEmpty(config.downloadingDirPath) && Directory.Exists(config.downloadingDirPath)) ?
                config.downloadingDirPath : config.selfDirPath;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                config.downloadingDirPath =
                    folderBrowserDialog.SelectedPath.EndsWith("\\")
                    ? folderBrowserDialog.SelectedPath : folderBrowserDialog.SelectedPath + "\\";

                textBoxDownloadingDirPath.Text = config.downloadingDirPath;
            }
        }

        private void btnSetDefaultFileNameFormat_Click(object sender, EventArgs e)
        {
            textBoxFileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
            config.fileNameFormat = FILENAME_FORMAT_DEFAULT;
        }

        private void btnSelectBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выберите браузер";
            ofd.Filter = "EXE-файлы|*.exe";
            string dir = string.IsNullOrEmpty(config.browserExePath) ? config.selfDirPath : Path.GetFullPath(config.browserExePath);
            ofd.InitialDirectory = dir;
            if (ofd.ShowDialog() != DialogResult.Cancel)
            {
                config.browserExePath = ofd.FileName;
                textBoxBrowserExePath.Text = ofd.FileName;
            }
            ofd.Dispose();
        }

        private void miOpenInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(config.browserExePath) || string.IsNullOrWhiteSpace(config.browserExePath))
            {
                MessageBox.Show("Браузер не указан!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(config.browserExePath))
            {
                MessageBox.Show("Браузер не найден!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Path.GetFileName(config.browserExePath);
            process.StartInfo.WorkingDirectory = Path.GetFullPath(config.browserExePath);
            process.StartInfo.Arguments = frameVideo.VideoInfo.VideoUrl;
            process.Start();
        }

        private void textBoxDownloadingDirPath_Leave(object sender, EventArgs e)
        {
            config.downloadingDirPath = textBoxDownloadingDirPath.Text;
        }

        private void textBoxFileNameFormat_Leave(object sender, EventArgs e)
        {
            config.fileNameFormat = textBoxFileNameFormat.Text;
        }

        private void textBoxBrowserExePath_Leave(object sender, EventArgs e)
        {
            config.browserExePath = textBoxBrowserExePath.Text;
        }

        private void numericUpDownMenuFontSize_ValueChanged(object sender, EventArgs e)
        {
            config.menuFontSize = (int)numericUpDownMenuFontSize.Value;
        }

        private void CenterFrame()
        {
            if (frameVideo != null)
            {
                frameVideo.Left = panelVideoBkg.Width / 2 - frameVideo.Width / 2;
            }
        }

        private List<DownloadableItem> GetDownloadableItems(TikTokVideo tikTokVideo)
        {
            List<DownloadableItem> res = new List<DownloadableItem>();
            if (FileDownloader.GetUrlContentLength(tikTokVideo.FileUrl, out long fileSize) == 200)
            {
                res.Add(new DownloadableItem(tikTokVideo.FileUrl, fileSize, true, tikTokVideo));
            }
            if (FileDownloader.GetUrlContentLength(tikTokVideo.FileUrlWithoutWatermark, out fileSize) == 200)
            {
                res.Add(new DownloadableItem(tikTokVideo.FileUrlWithoutWatermark, fileSize, false, tikTokVideo));
            }
            return res;
        }

        private ContextMenuStrip BuildMenuDownloads(IEnumerable<DownloadableItem> items)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (DownloadableItem downloadableItem in items)
            {
                string formattedSize = FormatSize(downloadableItem.Size);
                string itemTitle = downloadableItem.IsLogoPresent ? 
                    $"С логотипом ({formattedSize})" : $"Без логотипа ({formattedSize})";
                ToolStripMenuItem menuItem = new ToolStripMenuItem(itemTitle);
                menuItem.Tag = downloadableItem;
                menuItem.Click += OnMenuDownloadItemClick;
                menu.Items.Add(menuItem);
            }
            return menu;
        }

        private async void OnDownloadButtonClick(object sender)
        {
            FrameTikTokVideo frame = sender as FrameTikTokVideo;
            frame.btnDownload.Enabled = false;
            frame.progressBarDownload.Value = 0;

            if (string.IsNullOrEmpty(config.downloadingDirPath) || string.IsNullOrWhiteSpace(config.downloadingDirPath))
            {
                MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                frame.btnDownload.Enabled = true;
                return;
            }

            if (!Directory.Exists(config.downloadingDirPath))
            {
                MessageBox.Show("Папка для скачивания не существует!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                frame.btnDownload.Enabled = true;
                return;
            }

            if (string.IsNullOrEmpty(config.fileNameFormat) || string.IsNullOrWhiteSpace(config.fileNameFormat))
            {
                MessageBox.Show("Не указан формат имени файла!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                frame.btnDownload.Enabled = true;
                return;
            }

            frame.btnDownload.Text = "Ждите...";
            frame.btnDownload.Refresh();

            List<DownloadableItem> downloadableItems = await Task.Run(() => GetDownloadableItems(frame.VideoInfo));
            if (downloadableItems.Count > 0)
            {
                ContextMenuStrip menu = BuildMenuDownloads(downloadableItems);
                if (menu.Items.Count > 0)
                {
                    menu.Font = new Font(menu.Font.Name, config.menuFontSize);
                    Point pt = frame.PointToScreen(new Point(frame.btnDownload.Left + frame.btnDownload.Width, frame.btnDownload.Top));
                    menu.Show(pt.X, pt.Y);
                }
            }

            frame.btnDownload.Text = "Скачать";
            frame.btnDownload.Enabled = true;
        }

        private async void OnMenuDownloadItemClick(object sender, EventArgs e)
        {
            frameVideo.btnDownload.Enabled = false;

            DownloadableItem downloadableItem = (sender as ToolStripMenuItem).Tag as DownloadableItem;
            System.Diagnostics.Debug.WriteLine($"{downloadableItem.Size} | {downloadableItem.Url}");
            string filePath = GetNumberedFileName(config.downloadingDirPath +
                FixFileName(FormatFileName(config.fileNameFormat, downloadableItem)) + ".mp4");
            int errorCode = await DownloadItem(downloadableItem, filePath);
            if (errorCode != 200)
            {
                MessageBox.Show($"Ошибка {errorCode}", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            frameVideo.btnDownload.Enabled = true;
        }

        private async Task<int> DownloadItem(DownloadableItem downloadableItem, string outputFilePath)
        {
            frameVideo.progressBarDownload.Value = 0;
            Progress<long> progress = new Progress<long>();
            progress.ProgressChanged += (s, n) =>
            {
                double percent = 100.0 / downloadableItem.Size * n;
                frameVideo.progressBarDownload.Value = (int)percent;
                System.Diagnostics.Debug.WriteLine($"{n}: {percent}");
            };

            int res = await Task.Run(() =>
            {
                IProgress<long> progressReporter = progress;
                FileDownloader downloader = new FileDownloader();
                downloader.ProgressUpdateInterval = 3;

                downloader.WorkProgress += (s, n, full) =>
                {
                    progressReporter.Report(n);
                };

                downloader.Url = downloadableItem.Url;

                if (File.Exists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }

                try
                {
                    using (Stream stream = File.OpenWrite(outputFilePath))
                    {
                        int errorCode = downloader.Download(stream);
                        return errorCode;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return ex.HResult;
                }
            }
            );
            return res;
        }
    }
}
