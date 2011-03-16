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

using System.Reflection;

namespace Koffeinfrei.Base
{
    /// <summary>
    /// Gathers information about the product
    /// </summary>
    public class KfProductInfo
    {
        /// <summary>
        /// Gets or sets the product title as defined in the assembly.
        /// </summary>
        /// <value>The product.</value>
        public string Product { get; private set; }
        /// <summary>
        /// Gets or sets the version in the form <c>x.x.x</c>.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; private set; }
        /// <summary>
        /// Gets or sets the copyright as defined in the assembly.
        /// </summary>
        /// <value>The copyright.</value>
        public string Copyright { get; private set; }

        /// <summary>
        /// Gets the product and version concatenated as "&lt;product&gt; v&lt;version&gt;"
        /// </summary>
        /// <value>The product and version.</value>
        public string ProductAndVersion
        {
            get { return string.Format("{0} v{1}", Product, Version); }
        }

        public KfProductInfo()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            Version = assembly.GetName().Version.ToString(3);

            AssemblyProductAttribute productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            Product = productAttribute != null ? productAttribute.Product : "";

            AssemblyCopyrightAttribute copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            Copyright = copyrightAttribute != null ? copyrightAttribute.Copyright : "";
        }
    }
}