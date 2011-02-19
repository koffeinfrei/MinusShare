﻿//  Koffeinfrei Minus Share
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Koffeinfrei.Base;
using Koffeinfrei.MinusShare.Properties;

namespace Koffeinfrei.MinusShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<string> files;

        public MainWindow()
        {
            //ExplorerContextMenu.Add();

            InitializeComponent();

            // setup the UI
            sectionProgress.Visibility = Visibility.Collapsed;
            sectionDone.Visibility = Visibility.Collapsed;

            inputTitle.Focus();
            inputTitle.Text = Properties.Resources.InputTitleDefaultText;

            stackFilesScrollViewer.MaxHeight = SystemParameters.FullPrimaryScreenHeight / 2;

            // setup the file list
            files = new List<string>();
            GetFileList();
            InsertFileList();

            // update check
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
            };

            updater.Check();
        }

        private void GetFileList()
        {
            List<string> fileArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
            foreach (string file in fileArgs)
            {
                if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                {
                    files.AddRange(Directory.EnumerateFiles(file, "*.*", SearchOption.AllDirectories));
                }
                else
                {
                    files.Add(file);
                }
            }
        }

        private void InsertFileList()
        {
            BitmapImage deleteIcon = new BitmapImage();
            deleteIcon.BeginInit();
            deleteIcon.UriSource = new Uri("pack://application:,,,/img/delete.png");
            deleteIcon.EndInit();

            foreach (string s in files)
            {
                StackPanel panel = new StackPanel {Orientation = Orientation.Horizontal};

                Label label = new Label
                {
                    Content = Path.GetFileName(s),
                    Padding = new Thickness(0, 0, 0, 0),
                    Margin = new Thickness(0, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button button = new Button
                {
                    Content = new Image
                    {
                        Source = deleteIcon,
                        Width = 24
                    },
                    Style = (Style) FindResource("MainButton"),
                    ToolTip = Properties.Resources.RemoveFile
                };
                string s1 = s;
                button.Click += (sender, e) =>
                {
                    panel.Children.Remove(button);
                    panel.Children.Remove(label);
                    files.Remove(s1);
                    if (files.Count == 0)
                    {
                        Application.Current.Shutdown();
                    }
                };

                panel.Children.Add(button);
                panel.Children.Add(label);
                stackFiles.Children.Add(panel);
            }
        }

        private void OnGalleryCreated(MinusResult result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                buttonEditLink.Content = result.EditUrl;
                buttonShareLink.Content = result.ShareUrl;
                sectionProgress.Visibility = Visibility.Collapsed;
                sectionDone.Visibility = Visibility.Visible;
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
            string title = inputTitle.Text == Properties.Resources.InputTitleDefaultText
                               ? ""
                               : inputTitle.Text;

            Minus minus1 = new Minus
            {
                GalleryCreated = OnGalleryCreated,
                InfoLogger = OnInfoMessage,
                ErrorLogger = OnErrorMessage
            };
            minus1.AddFiles(files);
            minus1.SetTitle(title);
            minus1.Create();

            // disable controls
            inputTitle.IsEnabled = false;
            buttonShare.IsEnabled = false;
            buttonCancel.IsEnabled = false;
            foreach (UIElement child in stackFiles.Children)
            {
                child.IsEnabled = false;
            }
            sectionProgress.Visibility = Visibility.Visible;
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
    }
}