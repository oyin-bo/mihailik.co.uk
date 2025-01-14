using System;

namespace Mihailik.InternetExplorer.Protocols
{
    [Serializable]
	public struct ResponseInfo
	{
		public ResponseInfo(
            string mimeType,
            byte[] responseBytes )
		{
            MimeType=mimeType;
            ResponseBytes=responseBytes;
		}

        public string MimeType;
        public byte[] ResponseBytes;
	}
}
