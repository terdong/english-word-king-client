using System;
using System.Collections.Generic;
using System.Text;

namespace TeamGehem
{
    public struct UrlMaker
    {
        private StringBuilder SB_;
        public UrlPath Path;

        public static UrlMaker CreateUrlMaker(string address, UrlPath path)
        {
            return new UrlMaker(address, path);
        }

        IDictionary<string, string> url_params_;
        public void AddParams( string key, string value ) { url_params_.Add( key, value ); }

        List<string> protocols;
        public string[] GetProtocols() { return protocols.ToArray(); }

        private string address_;

        //private UrlMaker() { }
        private UrlMaker(string address, UrlPath path)
        {
            SB_ = new StringBuilder();
            address_ = address;
            Path = path;
            url_params_ = new Dictionary<string, string>();

            protocols = new List<string>();
        }

        public override string ToString()
        {
            SB_.Length = 0;

            SB_.Append(address_).Append(Path.ToString());
            if ( url_params_.Count > 0 )
            {
                SB_.Append("/?");
                foreach ( KeyValuePair<string, string> kvp in url_params_ )
                {
                    SB_.AppendFormat("{0}={1}", kvp.Key, kvp.Value);
                }
            }
            return SB_.ToString();
        }
    }
}