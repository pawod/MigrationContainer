#region

using System;
using Com.Wodzu.EightTracksGrabber.Helper;
using Com.Wodzu.EightTracksGrabber.Messages;
using Com.Wodzu.WebAutomation.Packet.Http;
using Newtonsoft.Json.Linq;
using NLog;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

#endregion

namespace Com.Wodzu.EightTracksGrabber.Core
{
    /// <summary>
    ///     Grabs tracks from 8Tracks.com by sniffing the network traffic between client and service for API requests and
    ///     extracting the resource URLs to the desired song.
    /// </summary>
    public sealed class SongGrabber : FileGrabber
    {
        #region Static Fields and Constants

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        private readonly bool _activeMode;
        private string _referer;
                       // keeps track of the current playlist, since its name is only transmitted on the first request

        /// <summary>
        ///     Initializes a new instance of a SongGrabber.
        /// </summary>
        /// <param name="captureDevice">The device, which is used for.</param>
        /// <param name="activeMode">
        ///     Wether to grab files actively or passively. Active mode filters song requests and resend them
        ///     in order to retreive the file location directly from the server response. Passive mode only filters the responses
        ///     for the request and extracts the file location.
        /// </param>
        public SongGrabber(ICaptureDevice captureDevice, bool activeMode = false)
            : base(captureDevice, GetFilterString(captureDevice))
        {
            _activeMode = activeMode;
        }

        protected override void ProcessPacket(Packet packet)
        {
            try
            {
                var tcp = (TcpPacket) packet.Extract(typeof(TcpPacket));
                if (tcp.PayloadData.Length == 0)
                    return;
                IRawHttpPacket rawHttpPacket = new RawHttpPacket(tcp.PayloadData);

                ISongResponse songResponse = null;
                if (_activeMode)
                {
                    var songRequest = rawHttpPacket.FilterSongRequest();
                    if (songRequest == null) return;

                    Logger.Debug("Captured request: {0}", songRequest.RequestUrl);
                    SetCurrentPlayList(songRequest);
                    if (songRequest.GotBoobs()) return; // filter requests, that have been sent by the grabber itself

                    var response = songRequest.GetResponse();
                    Logger.Debug("Captured response: {0}", response);
                    songResponse = new SongResponse(JObject.Parse(response), _referer);
                }
                if (songResponse == null) return;
                songResponse.DownloadSong(DownloadDirectory.FullName);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to process packet.");
                Logger.Error(ex);
            }
        }

        #region private methods

        private static string GetFilterString(ICaptureDevice captureDevice)
        {
            var device = (LibPcapLiveDevice) captureDevice;

            var type = Type.GetType("Mono.Runtime");
            var address = (type == null)
                ? device.Addresses[1].Addr.ipAddress.ToString()
                : device.Addresses[2].Addr.ipAddress.ToString();

            return string.Format("((tcp dst port 80) and (src net {0})) or ((dst net {0}) and (tcp src port 80))",
                address);
        }

        private void SetCurrentPlayList(IHttpRequest request)
        {
            if (!String.IsNullOrWhiteSpace(request.Referer))
                _referer = request.Referer.Replace(String.Format("http://{0}", request.Host), "");
            Logger.Debug("Current playlist is: \"{0}\"", _referer);
        }

        #endregion private methods
    }
}