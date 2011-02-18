// Koffeinfrei Base Library
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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Koffeinfrei.Base
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// credits to http://stackoverflow.com/questions/1357593/list-of-availible-cultures/1357680#1357680
    /// </remarks>
    public class KfCultureInfo
    {
        public static ReadOnlyCollection<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> list = new List<CultureInfo>();

            string startupDir = Application.StartupPath;
            Assembly asm = Assembly.GetEntryAssembly();

            CultureInfo currentCulture = CultureInfo.CurrentUICulture;
            if (asm != null)
            {
                NeutralResourcesLanguageAttribute attr =
                    Attribute.GetCustomAttribute(asm, typeof (NeutralResourcesLanguageAttribute)) as
                    NeutralResourcesLanguageAttribute;
                if (attr != null)
                {
                    currentCulture = CultureInfo.GetCultureInfo(attr.CultureName);
                }
            }
            list.Add(currentCulture);

            if (asm != null)
            {
                string baseName = asm.GetName().Name;
                foreach (string dir in Directory.GetDirectories(startupDir))
                {
                    // Check that the directory name is a valid culture
                    DirectoryInfo dirinfo = new DirectoryInfo(dir);
                    CultureInfo culture;
                    try
                    {
                        culture = CultureInfo.GetCultureInfo(dirinfo.Name);
                    }
                        // Not a valid culture : skip that directory
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    // Check that the directory contains satellite assemblies
                    if (dirinfo.GetFiles(baseName + ".resources.dll").Length > 0)
                    {
                        list.Add(culture);
                    }
                }
            }
            return list.AsReadOnly();
        }
    }
}