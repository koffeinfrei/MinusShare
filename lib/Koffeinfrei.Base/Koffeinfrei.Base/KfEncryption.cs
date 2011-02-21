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
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Koffeinfrei.Base
{
    /// <summary>
    /// Handles encryption, maybe used e.g. for storing password in a configuration.
    /// The encryption is provided by the Data Protection API (DPAPI) available in Microsoft Windows 2000 and later operating systems.
    /// </summary>
    /// <remarks>
    /// credits to http://msdn.microsoft.com/de-de/library/system.security.cryptography.protecteddata.aspx, 
    /// http://weblogs.asp.net/jgalloway/archive/2008/04/13/encrypting-passwords-in-a-net-app-config-file.aspx
    /// </remarks>
    public static class KfEncryption
    {
        private static readonly byte[] Entropy = Encoding.Unicode.GetBytes(
            "{23ne4t} -> This salt isn't very random... and not very useful anyway... we3541b sfq234 rfe!@#asdf!");

        public static string EncryptString(string input)
        {
            return EncryptString(input.ToSecureString());
        }

        // TODO: is the use of securestring any good if we need to use string anyway?
        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(input.ToInsecureString()),
                Entropy,
                DataProtectionScope.CurrentUser);
            string encryptedString = Convert.ToBase64String(encryptedData);
            Array.Clear(encryptedData, 0, encryptedData.Length);
            return encryptedString;
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    Entropy,
                    DataProtectionScope.CurrentUser);
                SecureString secureString = Encoding.Unicode.GetString(decryptedData).ToSecureString();
                Array.Clear(decryptedData, 0, decryptedData.Length);
                return secureString;
            }
            catch
            {
                return new SecureString();
            }
        }
    }
}