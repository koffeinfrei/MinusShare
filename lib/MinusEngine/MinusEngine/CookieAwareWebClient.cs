using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;

namespace BiasedBit.MinusEngine
{
    public class CookieAwareWebClient : WebClient
    {

        [System.Security.SecuritySafeCritical]
        public CookieAwareWebClient()
            : base()
        {
        }

        private CookieContainer m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_container;
            }
            return request;
        }

        public void clearCookies() {
            m_container = new CookieContainer();
        }

        public string getCookieHeader(Uri uri)
        {
            return m_container.GetCookieHeader(uri);
        }

        public void setCookieHeader(Uri uri, string header)
        {
            m_container.SetCookies(uri, header);
        }
    }
}
