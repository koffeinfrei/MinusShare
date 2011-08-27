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
using System.IO;
using System.Reflection;
using Koffeinfrei.Base;

namespace Koffeinfrei.MinusShare
{
    public static class ExplorerContextMenu
    {
        private static readonly string LinkFileName =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SendTo), "minus.lnk");

        private static readonly string LegacyLinkFileName =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SendTo), "min.us.lnk");


        public static void Add()
        {
            RemoveLegacy();

            if (!File.Exists(LinkFileName))
            {
                KfSymlink symlink = new KfSymlink(KfSymlink.LinkType.File);
                symlink.SetPath(Assembly.GetExecutingAssembly().Location);
                symlink.SetDescription("Share on minus");
                symlink.SetIconLocation(Assembly.GetExecutingAssembly().Location, 0);
                symlink.Save(LinkFileName);
            }
        }

        public static void Remove()
        {
            RemoveLegacy();

            if (File.Exists(LinkFileName))
            {
                File.Delete(LinkFileName);
            }
        }

        private static void RemoveLegacy()
        {
            if (File.Exists(LegacyLinkFileName))
            {
                File.Delete(LegacyLinkFileName);
            }
        }
    }
}