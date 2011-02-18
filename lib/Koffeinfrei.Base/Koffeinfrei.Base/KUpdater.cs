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

using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Koffeinfrei.Base
{
    public class KUpdater
    {
        private bool? hasNewerVersion;

        public string NewerVersion { get; set; }

        /// <summary>
        /// Determines whether the server has a more recent version of the application.
        /// </summary>
        /// <param name="versionFileUrl">The version file URL.</param>
        /// <returns>
        /// 	<c>true</c> if the server has a more recent version; otherwise, <c>false</c>.
        /// </returns>
        public bool HasNewerVersion(string versionFileUrl)
        {
            if (!hasNewerVersion.HasValue)
            {
                string serverVersion = new WebClient().DownloadString(versionFileUrl).Trim();

                if (Application.ProductVersion != serverVersion)
                {
                    // strip last .0
                    NewerVersion = Regex.Replace(serverVersion, @"(\d+\.\d+\.\d+)\.\d+", @"$1");
                    hasNewerVersion = true;
                }
                else
                {
                    hasNewerVersion = false;
                }
            }
            return hasNewerVersion.Value;
        }

        /// <summary>
        /// Updates the specified download URL format.
        /// </summary>
        /// <param name="downloadUrlFormat">The download URL format, e.g. 
        /// https://github.com/downloads/koffeinfrei/zueribad-wintray/zueribad_setup_{0}.msi
        /// </param>
        public void Update(string downloadUrlFormat)
        {
            if (!string.IsNullOrEmpty(NewerVersion))
            {
                Process.Start(string.Format(downloadUrlFormat, NewerVersion));
            }
        }
    }
}