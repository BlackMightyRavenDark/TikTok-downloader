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
			textBoxDownloadingDirPath.Text = config.DownloadingDirPath;
			textBoxFileNameFormat.Text = config.FileNameFormat;
			textBoxBrowserExePath.Text = config.BrowserExePath;
			numericUpDownMenuFontSize.Value = config.MenuFontSize;

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

		private async void btnSearchVideoByUrl_Click(object sender, EventArgs e)
		{
			btnSearchVideoByUrl.Enabled = false;
			textBoxUrl.Enabled = false;

			if (frameVideo != null)
			{
				frameVideo.Dispose();
				frameVideo = null;
			}

			string url = textBoxUrl.Text;
			if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
			{
				MessageBox.Show("Введите ссылку или ID видео!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxUrl.Enabled = true;
				btnSearchVideoByUrl.Enabled = true;
				return;
			}

			TikTokApi api = new TikTokApi();
			TikTokVideoDetailsResult videoDetailsResult = await Task.Run(() => api.GetVideoDetails(url));
			if (videoDetailsResult.ErrorCode == 200)
			{
				TikTokVideo tikTokVideo = await Task.Run(() => TikTokApi.ParseTikTokInfo(videoDetailsResult.Details));
				await Task.Run(() =>
				{
					FileDownloader d = new FileDownloader() { Url = tikTokVideo.ImagePreviewUrl };
					Stream stream = new MemoryStream();
					if (d.Download(stream) == 200)
					{
						tikTokVideo.ImagePreview = Image.FromStream(stream);
					}
					stream.Close();
				});
				frameVideo = new FrameTikTokVideo(tikTokVideo);
				frameVideo.Parent = panelVideoBkg;
				frameVideo.DownloadButtonPressed += OnDownloadButtonClick;
				frameVideo.ImageMouseDown += (object s, MouseEventArgs args) =>
				{
					if (args.Button == MouseButtons.Right)
					{
						menuImage.Font = new Font(menuImage.Font.Name, config.MenuFontSize);
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
			btnSearchVideoByUrl.Enabled = true;
		}

		private void btnSelectDownloadingDirPath_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Выберите папку для скачивания";
			folderBrowserDialog.SelectedPath =
				(!string.IsNullOrEmpty(config.DownloadingDirPath) && Directory.Exists(config.DownloadingDirPath)) ?
				config.DownloadingDirPath : config.SelfDirPath;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				config.DownloadingDirPath =
					folderBrowserDialog.SelectedPath.EndsWith("\\")
					? folderBrowserDialog.SelectedPath : folderBrowserDialog.SelectedPath + "\\";

				textBoxDownloadingDirPath.Text = config.DownloadingDirPath;
			}
		}

		private void btnSetDefaultFileNameFormat_Click(object sender, EventArgs e)
		{
			textBoxFileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
			config.FileNameFormat = FILENAME_FORMAT_DEFAULT;
		}

		private void btnSelectBrowser_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Выберите браузер";
			ofd.Filter = "EXE-файлы|*.exe";
			string dir = string.IsNullOrEmpty(config.BrowserExePath) ? config.SelfDirPath : Path.GetFullPath(config.BrowserExePath);
			ofd.InitialDirectory = dir;
			if (ofd.ShowDialog() != DialogResult.Cancel)
			{
				config.BrowserExePath = ofd.FileName;
				textBoxBrowserExePath.Text = ofd.FileName;
			}
			ofd.Dispose();
		}

		private void miOpenInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(config.BrowserExePath) || string.IsNullOrWhiteSpace(config.BrowserExePath))
			{
				MessageBox.Show("Браузер не указан!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!File.Exists(config.BrowserExePath))
			{
				MessageBox.Show("Браузер не найден!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = Path.GetFileName(config.BrowserExePath);
			process.StartInfo.WorkingDirectory = Path.GetFullPath(config.BrowserExePath);
			process.StartInfo.Arguments = frameVideo.VideoInfo.VideoUrl;
			process.Start();
		}

		private void textBoxDownloadingDirPath_Leave(object sender, EventArgs e)
		{
			config.DownloadingDirPath = textBoxDownloadingDirPath.Text;
		}

		private void textBoxFileNameFormat_Leave(object sender, EventArgs e)
		{
			config.FileNameFormat = textBoxFileNameFormat.Text;
		}

		private void textBoxBrowserExePath_Leave(object sender, EventArgs e)
		{
			config.BrowserExePath = textBoxBrowserExePath.Text;
		}

		private void numericUpDownMenuFontSize_ValueChanged(object sender, EventArgs e)
		{
			config.MenuFontSize = (int)numericUpDownMenuFontSize.Value;
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
			if (FileDownloader.GetUrlContentLength(tikTokVideo.FileUrlWithoutWatermark, out long fileSize) == 200)
			{
				tikTokVideo.FileSize = fileSize;
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

			if (string.IsNullOrEmpty(config.DownloadingDirPath) || string.IsNullOrWhiteSpace(config.DownloadingDirPath))
			{
				MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				frame.btnDownload.Enabled = true;
				return;
			}

			if (!Directory.Exists(config.DownloadingDirPath))
			{
				MessageBox.Show("Папка для скачивания не существует!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				frame.btnDownload.Enabled = true;
				return;
			}

			if (string.IsNullOrEmpty(config.FileNameFormat) || string.IsNullOrWhiteSpace(config.FileNameFormat))
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
					menu.Font = new Font(menu.Font.Name, config.MenuFontSize);
					Point pt = frame.PointToScreen(new Point(frame.btnDownload.Left + frame.btnDownload.Width, frame.btnDownload.Top));
					menu.Show(pt.X, pt.Y);
				}
				else
				{
					MessageBox.Show("Ссылки для скачивания не найдены!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("Ссылки для скачивания не найдены!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			frame.btnDownload.Text = "Скачать";
			frame.btnDownload.Enabled = true;
		}

		private async void OnMenuDownloadItemClick(object sender, EventArgs e)
		{
			frameVideo.btnDownload.Enabled = false;

			DownloadableItem downloadableItem = (sender as ToolStripMenuItem).Tag as DownloadableItem;
			System.Diagnostics.Debug.WriteLine($"{downloadableItem.Size} | {downloadableItem.Url}");
			string filePath = GetNumberedFileName(config.DownloadingDirPath +
				FixFileName(FormatFileName(config.FileNameFormat, downloadableItem)) + ".mp4");
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
