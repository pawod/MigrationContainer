using System.IO;
using System.Linq;
using NLog;

namespace Com.Wodzu.EightTracksGrabber.Helper
{
	public static class AudioContainer
	{
		private const long MAX_HEADERSIZE = 128*1024;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private static readonly byte[] LAME = {76, 65, 77, 69};
		private static readonly byte[] ID3 = {73, 68, 51};
		private static readonly byte[] MP4 = {109, 112, 52};

		/// <summary>
		///     Determines the audio container type of a given file by checking its header against some magic numbers.
		/// </summary>
		/// <param name="fileName">The file to determine the audio container type.</param>
		/// <returns>The audio container format if there was a match, else null.</returns>
		public static string GetContainerFormat(string fileName)
		{
			using (var file = File.OpenRead(fileName))
			{
				var buffer = new byte[16];

				while (file.Position < MAX_HEADERSIZE)
				{
					file.Read(buffer, 0, 16);
					if (buffer.ContainsSequence(MP4)) return "m4a";
					if (buffer.ContainsSequence(LAME) || buffer.ContainsSequence(ID3)) return "mp3";
				}
				Logger.Warn("Failed to determine audio container (max header size: {0}).", MAX_HEADERSIZE);
				return null;
			}
		}

		private static bool ContainsSequence(this byte[] source, byte[] pattern)
		{
			for (var i = 0; i < source.Length; i++)
			{
				if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
				{
					return true;
				}
			}
			return false;
		}
	}
}