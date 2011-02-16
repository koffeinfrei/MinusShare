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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Koffeinfrei.MinusShare
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> files;
        private readonly Minus minus;

        public MainWindow()
        {
            ExplorerContextMenu.Add();

            InitializeComponent();

            files = Environment.GetCommandLineArgs().Skip(1).ToList();

            InsertFileList();

            minus = new Minus
            {
                GalleryCreated = OnGalleryCreated,
                InfoLogger = OnInfoMessage,
                ErrorLogger = OnErrorMessage
            };
            minus.AddFiles(files);
        }

        private void InsertFileList()
        {
            foreach (string s in files)
            {
                Label label = new Label {Content = s};
                stackFiles.Children.Add(label);
            }
        }

        private void OnGalleryCreated(MinusResult result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                buttonEditLink.Content = result.EditUrl;
                buttonShareLink.Content = result.ShareUrl;
            }));
        }

        private void OnInfoMessage(string message)
        {
            Dispatcher.Invoke(new Action(() => outputStatus.Content = message));
        }

        private void OnErrorMessage(string message)
        {
            // TODO colorize instead of "ERROR: "
            Dispatcher.Invoke(new Action(() => outputStatus.Content = "ERROR: " + message));
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
            minus.SetTitle(inputTitle.Text);
            minus.Create();
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
    }
}