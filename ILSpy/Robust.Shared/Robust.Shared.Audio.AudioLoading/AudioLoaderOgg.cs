using System;
using System.IO;
using System.Numerics;
using NVorbis;

namespace Robust.Shared.Audio.AudioLoading;

internal static class AudioLoaderOgg
{
	internal readonly struct OggVorbisData(long totalSamples, long sampleRate, long channels, ReadOnlyMemory<short> data, string title, string artist)
	{
		public readonly long TotalSamples = totalSamples;

		public readonly long SampleRate = sampleRate;

		public readonly long Channels = channels;

		public readonly ReadOnlyMemory<short> Data = data;

		public readonly string Title = title;

		public readonly string Artist = artist;
	}

	private const int ReadBufferLength = 32768;

	public static AudioMetadata LoadAudioMetadata(Stream stream)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		VorbisReader val = new VorbisReader(stream, false);
		try
		{
			val.Initialize();
			return new AudioMetadata(val.TotalTime, val.Channels, val.Tags.Title, val.Tags.Artist);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static OggVorbisData LoadAudioData(Stream stream)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		VorbisReader val = new VorbisReader(stream, false);
		try
		{
			val.Initialize();
			int sampleRate = val.SampleRate;
			int channels = val.Channels;
			long totalSamples = val.TotalSamples;
			long num = totalSamples * channels;
			int i = 0;
			short[] array = new short[totalSamples * channels];
			Span<float> readBuffer = stackalloc float[32768];
			int num2;
			for (; i < num; i += num2)
			{
				num2 = ReadSamples(array.AsSpan(i), readBuffer, channels, val);
				if (num2 == 0)
				{
					break;
				}
			}
			return new OggVorbisData(totalSamples, sampleRate, channels, array, val.Tags.Title, val.Tags.Artist);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static int ReadSamples(Span<short> dest, Span<float> readBuffer, int channels, VorbisReader reader)
	{
		if (readBuffer.Length > dest.Length)
		{
			readBuffer = readBuffer.Slice(0, dest.Length);
		}
		int num = reader.ReadSamples(readBuffer);
		num *= channels;
		ConvertToShort(readBuffer.Slice(0, num), dest.Slice(0, num));
		return num;
	}

	private static void ConvertToShort(ReadOnlySpan<float> src, Span<short> dst)
	{
		if (src.Length != dst.Length)
		{
			throw new InvalidOperationException("Invalid lengths!");
		}
		int num = src.Length / Vector<short>.Count * Vector<short>.Count;
		Vector<float> vector = new Vector<float>(32767f);
		ref readonly float source = ref src[0];
		ref short destination = ref dst[0];
		for (int i = 0; i < num; i += Vector<short>.Count)
		{
			Vector<float> value = Vector.LoadUnsafe(in source, (nuint)i);
			Vector<float> vector2 = Vector.LoadUnsafe(in source, (nuint)(i + Vector<float>.Count));
			value *= vector;
			Vector<float> value2 = vector2 * vector;
			Vector<int> low = Vector.ConvertToInt32(value);
			Vector<int> high = Vector.ConvertToInt32(value2);
			Vector.Narrow(low, high).StoreUnsafe(ref destination, (nuint)i);
		}
		for (int j = num; j < src.Length; j++)
		{
			dst[j] = (short)(src[j] * 32767f);
		}
	}
}
