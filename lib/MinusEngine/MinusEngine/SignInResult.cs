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
    /// Result of a LogIn operation.
    /// </summary>
    public class SignInResult
    {
        #region Constructors
        public SignInResult()
        {
        }

        public SignInResult(bool success)
        {
            this.Success = success;
        }
        #endregion

        #region Fields
        /// <summary>
        /// The readonly URL for the gallery.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }


        public string CookieHeaders { get; set; }
        #endregion
    }
}
