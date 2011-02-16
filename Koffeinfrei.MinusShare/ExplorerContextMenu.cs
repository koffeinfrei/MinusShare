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

namespace Koffeinfrei.MinusShare
{
    public static class ExplorerContextMenu
    {
        private static readonly string LinkFileName =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SendTo), "min.us.lnk");

        public static void Add()
        {
            if (!File.Exists(LinkFileName))
            {
                Symlink symlink = new Symlink(Symlink.LinkType.File);
                symlink.SetPath(Assembly.GetExecutingAssembly().Location);
                symlink.SetDescription("Share on min.us");
                symlink.SetIconLocation(Assembly.GetExecutingAssembly().Location, 0);
                symlink.Save(LinkFileName);
            }
        }

        public static void Remove()
        {
            if (File.Exists(LinkFileName))
            {
                File.Delete(LinkFileName);
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Win32;

//namespace Koffeinfrei.MinusShare
//{
//    public static class ExplorerContextMenu
//    {
//        private const string MenuKeyName = @"folder\shell\koffeinfreiMinusShare";
//        private const string CommandKeyName = MenuKeyName + @"\command";

//        public static void Add()
//        {
//            RegistryKey menuKey = null;
//            RegistryKey cmdKey = null;

//            try
//            {
//                // menu item name
//                menuKey = Registry.ClassesRoot.CreateSubKey(MenuKeyName, 
//                    RegistryKeyPermissionCheck.ReadWriteSubTree);
//                if (menuKey != null)
//                {
//                    menuKey.SetValue(null, "Share on min.us");
//                }

//                // menu item command
//                cmdKey = Registry.ClassesRoot.CreateSubKey(CommandKeyName, 
//                    RegistryKeyPermissionCheck.ReadWriteSubTree);
//                if (cmdKey != null)
//                {
//                    cmdKey.SetValue(null, System.Reflection.Assembly.GetExecutingAssembly().Location + " \"%1\"", 
//                                    RegistryValueKind.ExpandString);
//                }
//            }
//            finally
//            {
//                if (menuKey != null)
//                {
//                    menuKey.Close();
//                }
//                if (cmdKey != null)
//                {
//                    cmdKey.Close();
//                }
//            }

//        }

//        public static void Remove()
//        {
//            // menu item command
//            RegistryKey reg = Registry.ClassesRoot.OpenSubKey(CommandKeyName);
//            if (reg != null)
//            {
//                reg.Close();
//                Registry.ClassesRoot.DeleteSubKey(CommandKeyName);
//            }

//            // menu item name
//            reg = Registry.ClassesRoot.OpenSubKey(MenuKeyName);
//            if (reg != null)
//            {
//                reg.Close();
//                Registry.ClassesRoot.DeleteSubKey(MenuKeyName);
//            }
//        }
//    }
//}