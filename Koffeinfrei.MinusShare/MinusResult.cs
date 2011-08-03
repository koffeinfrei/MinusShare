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

namespace Koffeinfrei.MinusShare
{
    public class MinusResult
    {
        public class Share
        {
            public string EditUrl { get; set; }
            public string ShareUrl { get; set; }
        }

        public class Gallery
        {
            public string EditorId { get; set; }
            public string EditUrl { get; set; }
            public string ReaderId { get; set; }
            public string ShareUrl { get; set; }
            public int ItemCount { get; set; }
            public string Name { get; set; }
            public bool Deleted { get; set; }
            public bool NotDeleted { get { return !Deleted; } }
        }
    }
}