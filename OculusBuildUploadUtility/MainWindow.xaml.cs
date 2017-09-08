using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OculusBuildUploadUtility {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		Config<BuildData> config;
		string configFilePath = "buildconfig.json";

		public MainWindow() {
			InitializeComponent();
			CenterWindowOnScreen();
			ChannelComboBox.ItemsSource = new List<string>() { "alpha", "beta", "store" };
			config = new Config<BuildData>();
			config.OnConfigDataChanged += RefreshUI;
			config.OnConfigDataChanged += CheckForRequiredFiles;
			BuildData d = config.Load(configFilePath);
			if(d == null) {
				config.Save(new BuildData("appID", "key", "", "", new AppVersion(), "alpha"), configFilePath);
			}

		}

		private void SaveConfigButton_Click(object sender, RoutedEventArgs e) {
			BuildData buildData = new BuildData(AppIDTextBox.Text, KeyTextBox.Text, DirectoryTextBox.Text, ExecutableTextBox.Text, new AppVersion(VersionTextBox.Text), ChannelComboBox.SelectedItem.ToString());
			config.Save(buildData, configFilePath);
		}

		private void LoadConfigButton_Click(object sender, RoutedEventArgs e) {
			config.Load(configFilePath);
		}

		private void UploadButton_Click(object sender, RoutedEventArgs e) {
			BuildData buildData = new BuildData(AppIDTextBox.Text, KeyTextBox.Text, DirectoryTextBox.Text, ExecutableTextBox.Text, new AppVersion(VersionTextBox.Text), ChannelComboBox.SelectedItem.ToString());
			config.Save(buildData, configFilePath);
			CheckForRequiredFiles(config.ConfigData);
			if(canDoUpload) {
				UploadBuild();
			}
		}



		private void UploadBuild() {
			Process process = new Process();
			process.StartInfo.FileName = "ovr-platform-util.exe";
			process.StartInfo.Arguments = "upload-rift-build" + config.ConfigData.ToString();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.EnableRaisingEvents = true;
			process.OutputDataReceived += Process_OutputDataReceived;
			process.Exited += Process_Exited;
			UploadButton.IsEnabled = false;
			process.Start();

			process.BeginOutputReadLine();

		}

		private void Process_Exited(object sender, EventArgs e) {
			if(!this.Dispatcher.CheckAccess()) {
				// Called from a none ui thread, so use dispatcher
				this.Dispatcher.Invoke((Action)delegate {
					UploadButton.IsEnabled = true;
				});
			} else {
				// Called from UI trhead so just update the textbox
				UploadButton.IsEnabled = true;
			};

		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			// Prepend line numbers to each line of the output.
			if(!String.IsNullOrEmpty(e.Data)) {
				if(!this.Dispatcher.CheckAccess()) {
					// Called from a none ui thread, so use dispatcher
					this.Dispatcher.Invoke((Action)delegate {
						OutputTextBox.Text += "\n" + e.Data;

					});
				} else {
					// Called from UI trhead so just update the textbox
					OutputTextBox.Text += "\n" + e.Data;
				};
			}
		}

		private void RefreshUI(BuildData buildData) {
			if(buildData == null) {
				return;
			}
			AppIDTextBox.Text = buildData.AppID;
			KeyTextBox.Text = buildData.SecretKey;
			DirectoryTextBox.Text = buildData.DirectoryName;
			ExecutableTextBox.Text = buildData.ExecutableName;
			VersionTextBox.Text = buildData.AppVersion.ToString();
			if(!ChannelComboBox.Items.Contains(buildData.BuildChannel)) {
				ChannelComboBox.Items.Add(buildData.BuildChannel);
				ChannelComboBox.SelectedItem = buildData.BuildChannel;
			} else {
				ChannelComboBox.SelectedItem = buildData.BuildChannel;
			}
			
		}

		private bool canDoUpload = false;

		private void CheckForRequiredFiles(BuildData data) {
			if(!System.IO.File.Exists("ovr-platform-util.exe")) {
				MessageBox.Show("ovr-platform-util.exe not found!", "Error");
				canDoUpload = false;
				return;
			}
			if(!File.Exists(data.DirectoryName + "/" + data.ExecutableName)) {
				MessageBox.Show(data.DirectoryName + " / " + data.ExecutableName + " not found!", "Error");
				canDoUpload = false;
				return;
			}
			if(!Directory.Exists(data.DirectoryName)) {
				MessageBox.Show(data.DirectoryName + " not found!", "Error");
				canDoUpload = false;
				return;
			}
			canDoUpload = true;
		}

		private void IncrementMajorButton_Click(object sender, RoutedEventArgs e) {
			config.ConfigData.AppVersion.IncrementMajor();
			RefreshUI(config.ConfigData);
		}

		private void IncrementMinorButton_Click(object sender, RoutedEventArgs e) {
			config.ConfigData.AppVersion.IncrementMinor();
			RefreshUI(config.ConfigData);
		}

		private void IncrementPatchButton_Click(object sender, RoutedEventArgs e) {
			config.ConfigData.AppVersion.IncrementPatch();
			RefreshUI(config.ConfigData);
		}

		private void CenterWindowOnScreen() {
			double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
			double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
			double windowWidth = this.Width;
			double windowHeight = this.Height;
			this.Left = (screenWidth / 2) - (windowWidth / 2);
			this.Top = (screenHeight / 2) - (windowHeight / 2);
		}

		private void OutputTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			OutputTextBox.ScrollToEnd();
		}
	}
}
