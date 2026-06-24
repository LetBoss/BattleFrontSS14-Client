using System;
using System.IO;
using Robust.Shared.Utility;

namespace Robust.Shared.Audio.AudioLoading;

internal static class AudioLoaderWav
{
	internal readonly struct WavData(WavAudioFormatType audioType, short numChannels, int sampleRate, int byteRate, short blockAlign, short bitsPerSample, ReadOnlyMemory<byte> data)
	{
		public readonly WavAudioFormatType AudioType = audioType;

		public readonly short NumChannels = numChannels;

		public readonly int SampleRate = sampleRate;

		public readonly int ByteRate = byteRate;

		public readonly short BlockAlign = blockAlign;

		public readonly short BitsPerSample = bitsPerSample;

		public readonly ReadOnlyMemory<byte> Data = data;
	}

	internal enum WavAudioFormatType : short
	{
		Unknown,
		PCM
	}

	public static AudioMetadata LoadAudioMetadata(Stream stream)
	{
		WavData wavData = LoadAudioData(stream);
		return new AudioMetadata(TimeSpan.FromSeconds((double)wavData.Data.Length / (double)wavData.BlockAlign / (double)wavData.SampleRate), wavData.NumChannels);
	}

	public static WavData LoadAudioData(Stream stream)
	{
		BinaryReader reader = new BinaryReader(stream, EncodingHelpers.UTF8, leaveOpen: true);
		Span<byte> span = stackalloc byte[4];
		while (true)
		{
			ReadFourCC(reader, span);
			if (((ReadOnlySpan<byte>)span).SequenceEqual("RIFF"u8))
			{
				break;
			}
			SkipChunk(reader);
		}
		return ReadRiffChunk(reader);
	}

	private static void SkipChunk(BinaryReader reader)
	{
		uint num = reader.ReadUInt32();
		reader.BaseStream.Position += num;
	}

	private static void ReadFourCC(BinaryReader reader, Span<byte> fourCc)
	{
		fourCc[0] = reader.ReadByte();
		fourCc[1] = reader.ReadByte();
		fourCc[2] = reader.ReadByte();
		fourCc[3] = reader.ReadByte();
	}

	private static WavData ReadRiffChunk(BinaryReader reader)
	{
		Span<byte> span = stackalloc byte[4];
		reader.ReadUInt32();
		ReadFourCC(reader, span);
		if (!((ReadOnlySpan<byte>)span).SequenceEqual("WAVE"u8))
		{
			throw new InvalidDataException("File is not a WAVE file.");
		}
		ReadFourCC(reader, span);
		if (!((ReadOnlySpan<byte>)span).SequenceEqual("fmt "u8))
		{
			throw new InvalidDataException("Expected fmt chunk.");
		}
		int num = reader.ReadInt32();
		long position = reader.BaseStream.Position + num;
		WavAudioFormatType wavAudioFormatType = (WavAudioFormatType)reader.ReadInt16();
		short numChannels = reader.ReadInt16();
		int sampleRate = reader.ReadInt32();
		int byteRate = reader.ReadInt32();
		short blockAlign = reader.ReadInt16();
		short bitsPerSample = reader.ReadInt16();
		if (wavAudioFormatType != WavAudioFormatType.PCM)
		{
			throw new NotImplementedException("Unable to support audio types other than PCM.");
		}
		reader.BaseStream.Position = position;
		while (true)
		{
			ReadFourCC(reader, span);
			if (((ReadOnlySpan<byte>)span).SequenceEqual("data"u8))
			{
				break;
			}
			SkipChunk(reader);
		}
		num = reader.ReadInt32();
		byte[] array = reader.ReadBytes(num);
		return new WavData(wavAudioFormatType, numChannels, sampleRate, byteRate, blockAlign, bitsPerSample, array);
	}
}
