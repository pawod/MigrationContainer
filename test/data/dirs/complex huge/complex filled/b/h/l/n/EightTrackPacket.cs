#region

using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Com.Wodzu.WebAutomation.Helpers;
using Com.Wodzu.WebAutomation.Packet.Http;
using Newtonsoft.Json.Linq;

#endregion

namespace Com.Wodzu.EightTracksGrabber.Helper
{
    public static class EightTrackPacket
    {
        private const string Host = "8tracks.com";
        private const string XActionKey = "XAction";
        private const string XActionValue = "sets\\play";
        private const string CookieKey = "SetCookie";

        private static readonly Regex RequestRegex =
            new Regex(".*?8tracks\\.com\\/sets\\/\\d+\\/(?:next|play|skip)\\?.*?&format=jsonh");

        private static readonly Regex ActonRegex = new Regex("\\/(?:next|skip)\\?");

        public static HttpWebRequest CreateSpoof(this IHttpRequest request)
        {
            var url = ActonRegex.Replace(request.RequestUrl, "/play?");
            // TODO: find more efficient way. dont do regex again
            var spoofedRequest = (HttpWebRequest) WebRequest.Create(url);

            // thx microsoft for this non-intuitive behaviour:
            request.AddBoobs();
            request.Headers.Remove("Host");
            request.Headers.Remove("Connection");
            request.Headers.Remove("Accept");
            request.Headers.Remove("User-Agent");
            request.Headers.Remove("Referer");

            spoofedRequest.Host = request.Host;
            spoofedRequest.KeepAlive = true;
            spoofedRequest.Accept = request.Accept;
            spoofedRequest.UserAgent = request.UserAgent;
            spoofedRequest.Referer = request.Referer;
            spoofedRequest.Headers.Add(request.Headers);

            return spoofedRequest;
        }

        public static IHttpRequest FilterSongRequest(this IRawHttpPacket rawHttpPacket)
        {
            if (!rawHttpPacket.IsRequest()) return null;
            var request = new HttpRequest(ref rawHttpPacket);
            return !request.IsSongRequest() ? null : request;
        }

        public static string GetResponse(this IHttpRequest request)
        {
            var spoofedRequest = request.CreateSpoof();
            var response = (HttpWebResponse) spoofedRequest.GetResponse();

            byte[] bytes;
            using (var stream = response.GetResponseStream())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }

            string result;
            if (response.ContentEncoding.Equals("gzip", StringComparison.Ordinal))
            {
                var decompressed = bytes.DecompressGzip();
                result = decompressed.ParseToString();
            }
            else
            {
                result = bytes.ParseToString();
            }
            return result;
        }

        public static bool IsSongRequest(this IHttpRequest http)
        {
            return (http.Host == Host && RequestRegex.Match(http.RequestUrl).Success);
        }

        public static bool IsSongResponse(this IHttpResponse http)
        {
            return (http.Content.GetType() == typeof (JObject)
                    && http.Headers[XActionKey] == XActionValue
                    && http.Headers[CookieKey] != null
                    && http.Headers[CookieKey].Contains(Host));
        }
    }
}