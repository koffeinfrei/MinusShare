//  Koffeinfrei Batch Replacer
//  Copyright (C) 2010  Alexis Reigel
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

namespace Koffeinfrei.Base
{
    /// <summary>
    /// A custom <see cref="IFormatProvider"/> that formats a size in its <see cref="long"/> value
    /// as a human readable size string (&lt;value&gt; [B/KB/MB/GB])
    /// </summary>
    /// <remarks>credits to http://flimflan.com/blog/FileSizeFormatProvider.aspx</remarks>
    public class KfFileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof (ICustomFormatter) ? this : null;
        }

        private const string FileSizeFormat = "fs";
        private const Decimal KB = 1024M;
        private const Decimal MB = KB * 1024M;
        private const Decimal GB = MB * 1024M;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith(FileSizeFormat))
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            if (arg is string)
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            Decimal size;

            try
            {
                size = Convert.ToDecimal(arg);
            }
            catch (InvalidCastException)
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            string suffix;

            if (size > GB)
            {
                size /= GB;
                suffix = "GB";
            }
            else if (size > MB)
            {
                size /= MB;
                suffix = "MB";
            }
            else if (size > KB)
            {
                size /= KB;
                suffix = "KB";
            }
            else
            {
                suffix = "B";
            }

            string precision = format.Substring(2);
            if (String.IsNullOrEmpty(precision))
            {
                precision = "2";
            }

            // don't show precision if x.00
            if ((size - (int) size) == 0)
            {
                precision = "0";
            }

            return String.Format("{0:N" + precision + "}{1}", size, suffix);
        }

        private static string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            IFormattable formattableArg = arg as IFormattable;

            if (formattableArg != null)
            {
                return formattableArg.ToString(format, formatProvider);
            }

            return arg.ToString();
        }
    }
}