using System;

namespace Mihailik.InternetExplorer.Protocols
{
    [Serializable]
	public struct RequestInfo
	{
        public RequestInfo(
            string urlString,
            string verb,
            byte[] verbData )
        {
            UrlString=urlString;
            Verb=verb;
            VerbData=verbData;
        }

        public string UrlString;
        public string Verb;
        public byte[] VerbData;
	}
}
