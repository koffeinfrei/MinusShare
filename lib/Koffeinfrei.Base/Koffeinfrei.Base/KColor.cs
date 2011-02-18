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

using System.Drawing;

namespace Koffeinfrei.Base
{
    public static class KColor
    {
        public static string GetHexCode(this Color color)
        {
            return string.Format("{0:X}", color.ToArgb()).Substring(2);
        }

        public static Color FromHexCodeAndAlpha(string hexCode, string alpha)
        {
            hexCode = hexCode.Replace("#", "");
            int a = (int) (float.Parse(alpha)*255);
            int r = KInteger.FromHex(hexCode.Substring(0, 2));
            int g = KInteger.FromHex(hexCode.Substring(2, 2));
            int b = KInteger.FromHex(hexCode.Substring(4));
            return Color.FromArgb(a, r, g, b);
        }

        public static Bitmap CreateImage(this Color fillColor, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (SolidBrush backgroundBrush = new SolidBrush(fillColor))
                {
                    graphics.FillRectangle(backgroundBrush, 0, 0, width, height);
                }
            }

            return bitmap;
        }
    }
}