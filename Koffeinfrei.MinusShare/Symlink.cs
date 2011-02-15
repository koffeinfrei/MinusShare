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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Koffeinfrei.MinusShare
{
    /// <summary>
    ///   Creates symlinks on windows
    /// </summary>
    /// <remarks>
    ///   credits to http://www.experts-exchange.com/Programming/Languages/.NET/Visual_Basic.NET/A_3664-Creating-Shortcuts-in-NET.html
    /// </remarks>
    public class Symlink
    {
        // egl1044
        private const string ClsidShellLink = "00021401-0000-0000-C000-000000000046";
        private const string ClsidFolderShortcut = "0AFACED1-E828-11D1-9187-B532F1E9575D";
        private readonly StringBuilder dataBuffer;

        public enum LinkType
        {
            File,
            Folder
        }

        public enum WindowStyle
        {
            Normal = 1,
            Maximized = 3,
            ShowMinNoActive = 7
        }

        private IShellLinkW psl;
        private IPersistFile ppf;

        public Symlink(LinkType shortcutlinkType)
        {
            dataBuffer = new StringBuilder(260);

            // Get a pointer to the IShellLink interface.
            switch (shortcutlinkType)
            {
                case LinkType.File:
                    psl = (IShellLinkW) Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ClsidShellLink)), true);
                    break;
                case LinkType.Folder:
                    psl = (IShellLinkW) Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ClsidFolderShortcut)), true);
                    break;
            }
        }


        ~Symlink()
        {
            Release();
        }

        public void Save(string pszFileName)
        {
            // Get a pointer to the IPersistFile interface.
            ppf = (IPersistFile) psl;
            ppf.Save(pszFileName, true);
        }

        public void Load(string pszFileName)
        {
            // Get a pointer to the IPersistFile interface.
            ppf = (IPersistFile) psl;
            ppf.Load(pszFileName, 0);
            psl.Resolve(IntPtr.Zero, 0);
        }

        public int SetPath(string pszFile)
        {
            return psl.SetPath(pszFile);
        }

        public int SetDescription(string pszName)
        {
            return psl.SetDescription(pszName);
        }

        public int SetWorkingDirectory(string pszDir)
        {
            return psl.SetWorkingDirectory(pszDir);
        }

        public int SetIconLocation(string pszIconPath, int iconIndex)
        {
            return psl.SetIconLocation(pszIconPath, iconIndex);
        }

        public int SetArguments(string pszArgs)
        {
            return psl.SetArguments(pszArgs);
        }

        public int SetShowCmd(WindowStyle showCmd)
        {
            return psl.SetShowCmd((int) showCmd);
        }

        public int SetHotKey(short wHotKey)
        {
            return psl.SetHotkey(wHotKey);
        }

        public string GetPath()
        {
            psl.GetPath(dataBuffer, dataBuffer.Capacity, IntPtr.Zero, 0);
            return dataBuffer.ToString();
        }

        public string GetArguments()
        {
            psl.GetArguments(dataBuffer, dataBuffer.Capacity);
            return dataBuffer.ToString();
        }

        public string GetDescription()
        {
            psl.GetDescription(dataBuffer, dataBuffer.Capacity);
            return dataBuffer.ToString();
        }

        public ShortcutIconInfo GetIconLocation()
        {
            int iconIndex = 0;
            psl.GetIconLocation(dataBuffer, dataBuffer.Capacity, ref iconIndex);
            return new ShortcutIconInfo(dataBuffer.ToString(), iconIndex);
        }

        public string GetWorkingDirectory()
        {
            psl.GetWorkingDirectory(dataBuffer, dataBuffer.Capacity);
            return dataBuffer.ToString();
        }

        public int GetShowCommand()
        {
            int pShowCmd = 0;
            psl.GetShowCmd(ref pShowCmd);
            return pShowCmd;
        }

        public short GetHotkey()
        {
            short pHotKey = 0;
            psl.GetHotkey(ref pHotKey);
            return pHotKey;
        }

        public void Release()
        {
            if (ppf != null)
            {
                Marshal.FinalReleaseComObject(ppf);
                ppf = null;
            }
            if (psl != null)
            {
                Marshal.FinalReleaseComObject(psl);
                psl = null;
            }
        }

        public class ShortcutIconInfo
        {
            private readonly string iconLocation = string.Empty;
            private readonly int iconIndex;

            protected internal ShortcutIconInfo(string iconLocation, int iconIndex)
            {
                this.iconLocation = iconLocation;
                this.iconIndex = iconIndex;
            }

            public string Location
            {
                get { return iconLocation; }
            }

            public int Index
            {
                get { return iconIndex; }
            }
        }
    }

    /// <summary>
    ///   IShellLinkW Interface
    ///   http://msdn.microsoft.com/en-us/library/bb774950(VS.85).aspx
    /// </summary>
    /// <remarks>
    ///   This interface cannot be used to create a link to a URL.
    /// </remarks>
    [ComImport,
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
     Guid("000214F9-0000-0000-C000-000000000046")]
    public interface IShellLinkW
    {
        [PreserveSig]
        int GetPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, int fFlags);

        [PreserveSig]
        int GetIDList(ref IntPtr ppidl);

        [PreserveSig]
        int SetIDList(IntPtr pidl);

        [PreserveSig]
        int GetDescription([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

        [PreserveSig]
        int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [PreserveSig]
        int GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

        [PreserveSig]
        int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        [PreserveSig]
        int GetArguments([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

        [PreserveSig]
        int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        [PreserveSig]
        int GetHotkey(ref short pwHotkey);

        [PreserveSig]
        int SetHotkey(short wHotkey);

        [PreserveSig]
        int GetShowCmd(ref int piShowCmd);

        [PreserveSig]
        int SetShowCmd(int iShowCmd);

        [PreserveSig]
        int GetIconLocation([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, ref int piIcon);

        [PreserveSig]
        int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

        [PreserveSig]
        int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

        [PreserveSig]
        int Resolve(IntPtr hWnd, int fFlags);

        [PreserveSig]
        int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}