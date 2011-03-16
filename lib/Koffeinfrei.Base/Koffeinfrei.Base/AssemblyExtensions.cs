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
using System.Reflection;

namespace Koffeinfrei.Base
{
    public static class AssemblyExtensions
    {
        public static T GetCustomAttribute<T>(this Assembly assembly) where T : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof (T), false);
            if (attributes.Length == 0)
            {
                return null;
            }

            return (T) attributes[0];
        }
    }
}