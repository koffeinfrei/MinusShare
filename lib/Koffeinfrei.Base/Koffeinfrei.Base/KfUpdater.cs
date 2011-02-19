//  Koffeinfrei Base Library
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
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Koffeinfrei.Base
{
    public class KfUpdater
    {
        private readonly string versionFileUrl;
        private readonly string downloadUrlFormat;

        public Action CheckCompleted { get; set; }
        public string NewerVersion { get; private set; }
        public bool HasNewerVersion { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KfUpdater"/> class.
        /// </summary>
        /// <param name="versionFileUrl">The version file URL.</param>
        /// <param name="downloadUrlFormat">The download URL format, e.g. 
        /// https://github.com/downloads/koffeinfrei/zueribad-wintray/zueribad_setup_{0}.msi
        /// </param>
        public KfUpdater(string versionFileUrl, string downloadUrlFormat)
        {
            this.versionFileUrl = versionFileUrl;
            this.downloadUrlFormat = downloadUrlFormat;
        }
        
        /// <summary>
        /// Determines whether the server has a more recent version of the application.
        /// </summary>
        public void Check()
        {
            Uri address = new Uri(versionFileUrl);
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
            webClient.DownloadStringAsync(address);
        }

        /// <summary>
        /// Updates the specified download URL format.
        /// </summary>
        public void Update()
        {
            if (HasNewerVersion)
            {
                Process.Start(string.Format(downloadUrlFormat, NewerVersion));
            }
        }

        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string serverVersion = e.Result.Trim();

            if (Application.ProductVersion != serverVersion)
            {
                // strip last .0
                NewerVersion = Regex.Replace(serverVersion, @"(\d+\.\d+\.\d+)\.\d+", @"$1");
                HasNewerVersion = true;
            }
            else
            {
                NewerVersion = null;
                HasNewerVersion = false;
            }

            CheckCompleted();
        }
    }

}