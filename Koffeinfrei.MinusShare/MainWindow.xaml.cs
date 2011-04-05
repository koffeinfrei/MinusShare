//  Koffeinfrei Minus Share
//  Copyright (C) 2011  Alexis Reigel
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Koffeinfrei.Base;
using Koffeinfrei.MinusShare.Properties;

namespace Koffeinfrei.MinusShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<FileListItem> Files
        {
            get { return (ObservableCollection<FileListItem>) GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof (ObservableCollection<FileListItem>), typeof (MainWindow), new UIPropertyMetadata(null));

        public ObservableCollection<MinusResult.Galleries> Galleries
        {
            get { return (ObservableCollection<MinusResult.Galleries>)GetValue(GalleriesProperty); }
            set { SetValue(GalleriesProperty, value); }
        }

        public static readonly DependencyProperty GalleriesProperty =
            DependencyProperty.Register("Galleries", typeof(ObservableCollection<MinusResult.Galleries>), typeof(MainWindow), new UIPropertyMetadata(null));

        private readonly Minus minus;
        private bool authenticationSettingsChanged;
        private bool galleriesSettingsChanged;

        public MainWindow()
        {
            Files = new ObservableCollection<FileListItem>();

            InitializeComponent();

            // setup the UI
            inputTitle.Focus();
            inputTitle.Text = Properties.Resources.InputTitleDefaultText;

            stackFilesScrollViewer.MaxHeight = SystemParameters.FullPrimaryScreenHeight / 2;
            stackGalleriesScrollViewer.MaxHeight = SystemParameters.FullPrimaryScreenHeight / 2;

            // get the files from the passed arguments
            AddFileList(Environment.GetCommandLineArgs().Skip(1).ToList());

            // update check
            if (Settings.Default.AutoUpdateCheck)
            {
                CheckForUpdates(false);
            }

            // setup minus handling
            minus = new Minus
            {
                InfoLogger = OnInfoMessage,
                ErrorLogger = OnErrorMessage
            };

            // settings change listener
            Settings.Default.PropertyChanged += Default_PropertyChanged;
            authenticationSettingsChanged = true;
            galleriesSettingsChanged = true;
        }

        // TODO: find a nicer way to track settings changes
        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Username":
                case "Password":
                    authenticationSettingsChanged = true;
                    break;
                case "HideDeletedGalleries":
                    galleriesSettingsChanged = true;
                    break;
            }
        }

        private static void CheckForUpdates(bool showResultsAlways)
        {
            KfUpdater updater = new KfUpdater(Settings.Default.VersionUrl, Settings.Default.DownloadUrlFormat);
            updater.CheckCompleted = () =>
            {
                if (updater.HasNewerVersion)
                {
                    MessageBoxResult dialogResult = MessageBox.Show(string.Format(Base.Resources.DialogVersionUpdateQuestionFormat,
                                                                                  updater.NewerVersion),
                                                                    Base.Resources.DialogVersionUpdate,
                                                                    MessageBoxButton.YesNo);

                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        updater.Update();
                    }
                }
                else if (showResultsAlways)
                {
                    MessageBox.Show(Properties.Resources.DialogVersionUpdateNoUpdates, Base.Resources.DialogVersionUpdate, MessageBoxButton.OK);
                }
            };

            updater.Check();
        }

        private void AddFileList(IEnumerable<string> fileList)
        {
            foreach (string file in fileList)
            {
                // recursively get files from directory
                if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                {
                    foreach (string enumeratedFile in Directory.EnumerateFiles(file, "*.*", SearchOption.AllDirectories))
                    {
                        Files.Add(new FileListItem(enumeratedFile));
                    }
                }
                else
                {
                    Files.Add(new FileListItem(file));
                }
            }
        }

        private string GetTitle()
        {
            return inputTitle.Text == Properties.Resources.InputTitleDefaultText
                       ? ""
                       : inputTitle.Text;
        }

        private void OnGalleryCreated(MinusResult.Share result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                buttonEditLink.Content = result.EditUrl;
                buttonShareLink.Content = result.ShareUrl;
                sectionProgress.Visibility = Visibility.Collapsed;
                sectionDone.Visibility = Visibility.Visible;
                buttonCancel.Visibility = Visibility.Collapsed;
                buttonShare.Visibility = Visibility.Collapsed;
            }));
        }
        
        private void OnInfoMessage(string message)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                outputStatus.Foreground = new SolidColorBrush(Colors.Black);
                outputStatus.Content = message;
            }));
        }

        private void OnErrorMessage(string message)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                outputStatus.Foreground = new SolidColorBrush(Colors.Red);
                outputStatus.Content = message;
                imageLoading.Visibility = Visibility.Collapsed;
            }));
        }

        private void buttonEditLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(buttonEditLink.Content.ToString());
        }

        private void buttonShareLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(buttonShareLink.Content.ToString());
        }

        private void buttonShare_Click(object sender, RoutedEventArgs e)
        {
            // disable controls
            inputTitle.IsEnabled = false;
            buttonShare.IsEnabled = false;
            buttonCancel.IsEnabled = false;
            listFiles.IsEnabled = false;

            sectionProgress.Visibility = Visibility.Visible;

            // share
            minus.Login(loginResult =>
            {
                if (loginResult == LoginStatus.Anonymous || loginResult == LoginStatus.Successful)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        minus.AddFiles(Files.Select(x => x.FullName).ToList());
                        minus.SetTitle(GetTitle());
                        // reset galleries -> need reload
                        Galleries = null;
                    }));
                    minus.Share(OnGalleryCreated);
                }
            });
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void inputTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (inputTitle.Text == Properties.Resources.InputTitleDefaultText)
            {
                inputTitle.Text = "";
            }
        }

        private void inputTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(inputTitle.Text))
            {
                inputTitle.Text = Properties.Resources.InputTitleDefaultText;
            }
        }

        private void inputTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputTitle.Text == Properties.Resources.InputTitleDefaultText)
            {
                inputTitle.Text = "";
            }
        }

        private void buttonClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, buttonShareLink.Content);
        }

        private void buttonEditClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, buttonEditLink.Content);
        }

        private void buttonTwitter_Click(object sender, RoutedEventArgs e)
        {
            const string baseUrl = "http://twitter.com/home?status=";
            OpenShareUrl(baseUrl + "{0}",
                         baseUrl + "{1}: {0}");
        }

        private void buttonFacebook_Click(object sender, RoutedEventArgs e)
        {
            const string baseURl = "http://www.facebook.com/sharer.php?u=";
            OpenShareUrl(baseURl + "{0}",
                         baseURl + "{1}: {0}");
        }

        private void buttonIdentica_Click(object sender, RoutedEventArgs e)
        {
            const string baseUrl = "http://identi.ca//index.php?action=bookmarklet&status_textarea=";
            OpenShareUrl(baseUrl + "{0}",
                         baseUrl + "{1}: {0}");
        }

        private void buttonEmail_Click(object sender, RoutedEventArgs e)
        {
            OpenShareUrl("mailto:?body={0}",
                         "mailto:?subject={1}&body={0}");
        }

        private void OpenShareUrl(string urlFormat, string urlWithTitleFormat)
        {
            string title = GetTitle();
            string url = string.IsNullOrEmpty(title)
                             ? string.Format(urlFormat, buttonShareLink.Content)
                             : string.Format(urlWithTitleFormat, buttonShareLink.Content, title);

            Process.Start(url);
        }

        private void stackFilesScrollViewer_Drop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (droppedFiles != null)
            {
                AddFileList(droppedFiles);
            }
            inputTitle.Focus();
        }

        private void buttonCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            CheckForUpdates(true);
        }

        private void buttonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (authenticationSettingsChanged)
            {
                minus.LoginStatus = LoginStatus.None;
            }

            if (galleriesSettingsChanged || authenticationSettingsChanged)
            {
                Galleries = null;
            }

            Settings.Default.Save();
            mainTabControl.SelectedIndex = 0;

            // reset change flags
            authenticationSettingsChanged = false;
            galleriesSettingsChanged = false;
        }

        private void buttonDiscardSettings_Click(object sender, RoutedEventArgs e)
        {
            authenticationSettingsChanged = false;
            galleriesSettingsChanged = false;

            Settings.Default.Reload();
        }

        private void buttonRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            FileListItem item = button.DataContext as FileListItem;
            if (item != null)
            {
                Files.Remove(item);
                if (Files.Count == 0)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void tabItemGalleries_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Galleries == null)
            {
                galleriesNeedLogin.Visibility = Visibility.Collapsed;
                galleriesProgress.Visibility = Visibility.Visible;

                minus.Login(loginResult =>
                {
                    if (loginResult == LoginStatus.Successful)
                    {
                        minus.GetGalleries(galleries => Dispatcher.Invoke(
                            new Action(() =>
                            {
                                Galleries = new ObservableCollection<MinusResult.Galleries>(
                                    Settings.Default.HideDeletedGalleries
                                        ? galleries.Where(gallery => gallery.NotDeleted)
                                        : galleries);

                                galleriesProgress.Visibility = Visibility.Collapsed;
                            })));
                    }
                    else
                    {
                        galleriesProgress.Visibility = Visibility.Collapsed;
                        galleriesNeedLogin.Visibility = Visibility.Visible;
                    }
                });
            }
        }

        private void buttonGalleriesEditLink_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MinusResult.Galleries item = button.DataContext as MinusResult.Galleries;
            if (item != null)
            {
                Process.Start(item.EditUrl);
            }
        }

        private void buttonGalleriesShareLink_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MinusResult.Galleries item = button.DataContext as MinusResult.Galleries;
            if (item != null)
            {
                Process.Start(item.ShareUrl);
            }
        }

        private void tabItemAbout_GotFocus(object sender, RoutedEventArgs e)
        {
            KfProductInfo info = new KfProductInfo();
            aboutVersionText.Text = info.ProductAndVersion;
            aboutCopyrightText.Text = info.Copyright;
        }

        /// <summary>
        /// Genereal link button which has an url as its text (= content)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void buttonLink_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Process.Start(button.Content.ToString());
            }
        }
    }
}