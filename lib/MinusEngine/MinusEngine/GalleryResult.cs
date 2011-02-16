//   Copyright 2010 Bruno de Carvalho
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BiasedBit.MinusEngine
{
    /// <summary>
    /// Part of MyGalleries Result
    /// </summary>
    public class GalleryResult
    {
        #region Constructors
        public GalleryResult()
        {
        }
        #endregion

        #region Fields
        /// <summary>
        /// The last visit date
        /// </summary>
        [JsonProperty("last_visit")]
        public String LastVisit { get; set; }

        /// <summary>
        /// The gallery title, as assigned by it's creator.
        /// </summary>
        [JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Number of items in the gallery
        /// </summary>
        [JsonProperty("item_count")]
        public int ItemCount { get; set; }

        /// <summary>
        /// Number of clicks on the gallery
        /// </summary>
        [JsonProperty("clicks")]
        public int Clicks { get; set; }

        /// <summary>
        /// Reader id of the gallery
        /// </summary>
        [JsonProperty("reader_id")]
        public String ReaderId { get; set; }

        /// <summary>
        /// editor id of the gallery
        /// </summary>
        [JsonProperty("editor_id")]
        public String EditorId { get; set; }

        #endregion Fields
    }
}
