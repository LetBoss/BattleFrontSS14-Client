using System;
using System.IO;

namespace Robust.Shared.Audio.AudioLoading;

internal static class AudioLoader
{
	public static bool IsLoadableAudioFile(ReadOnlySpan<char> filename)
	{
		ReadOnlySpan<char> extension = Path.GetExtension(filename);
		if (extension.SequenceEqual(".wav".AsSpan()) || extension.SequenceEqual(".ogg".AsSpan()))
		{
			return true;
		}
		return false;
	}

	public static AudioMetadata LoadAudioMetadata(Stream stream, ReadOnlySpan<char> filename)
	{
		ReadOnlySpan<char> extension = Path.GetExtension(filename);
		if (extension.SequenceEqual(".ogg".AsSpan()))
		{
			return AudioLoaderOgg.LoadAudioMetadata(stream);
		}
		if (extension.SequenceEqual(".wav".AsSpan()))
		{
			return AudioLoaderWav.LoadAudioMetadata(stream);
		}
		throw new ArgumentException($"Unknown file type: {extension}");
	}
}
