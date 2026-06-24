// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioLoading.AudioLoaderWav
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Audio.AudioLoading;

internal static class AudioLoaderWav
{
  public static AudioMetadata LoadAudioMetadata(Stream stream)
  {
    AudioLoaderWav.WavData wavData = AudioLoaderWav.LoadAudioData(stream);
    return new AudioMetadata(TimeSpan.FromSeconds((double) wavData.Data.Length / (double) wavData.BlockAlign / (double) wavData.SampleRate), (int) wavData.NumChannels);
  }

  public static unsafe AudioLoaderWav.WavData LoadAudioData(Stream stream)
  {
    BinaryReader reader = new BinaryReader(stream, EncodingHelpers.UTF8, true);
    Span<byte> span = stackalloc byte[4];
    while (true)
    {
      AudioLoaderWav.ReadFourCC(reader, span);
      // ISSUE: reference to a compiler-generated field
      if (!((ReadOnlySpan<byte>) span).SequenceEqual<byte>(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.B81FFE9A0176B5888A4651D91E4AF16BC4D56F4A24EDC8E5B3126B46EBD112B5, 4)))
        AudioLoaderWav.SkipChunk(reader);
      else
        break;
    }
    return AudioLoaderWav.ReadRiffChunk(reader);
  }

  private static void SkipChunk(BinaryReader reader)
  {
    uint num = reader.ReadUInt32();
    reader.BaseStream.Position += (long) num;
  }

  private static void ReadFourCC(BinaryReader reader, Span<byte> fourCc)
  {
    fourCc[0] = reader.ReadByte();
    fourCc[1] = reader.ReadByte();
    fourCc[2] = reader.ReadByte();
    fourCc[3] = reader.ReadByte();
  }

  private static unsafe AudioLoaderWav.WavData ReadRiffChunk(BinaryReader reader)
  {
    Span<byte> span = stackalloc byte[4];
    int num1 = (int) reader.ReadUInt32();
    AudioLoaderWav.ReadFourCC(reader, span);
    // ISSUE: reference to a compiler-generated field
    if (!((ReadOnlySpan<byte>) span).SequenceEqual<byte>(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.B6311D00BA4D5CF0D9459A6FA59565888E6A2A9360EB07ABB78778107E8A44AB, 4)))
      throw new InvalidDataException("File is not a WAVE file.");
    AudioLoaderWav.ReadFourCC(reader, span);
    // ISSUE: reference to a compiler-generated field
    if (!((ReadOnlySpan<byte>) span).SequenceEqual<byte>(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.AEB65FD511E03FFB93D425EE0B2809E226827D1580B444DCFC4F03F22B4FC9B6, 4)))
      throw new InvalidDataException("Expected fmt chunk.");
    int num2 = reader.ReadInt32();
    long num3 = reader.BaseStream.Position + (long) num2;
    AudioLoaderWav.WavAudioFormatType audioType = (AudioLoaderWav.WavAudioFormatType) reader.ReadInt16();
    short numChannels = reader.ReadInt16();
    int sampleRate = reader.ReadInt32();
    int byteRate = reader.ReadInt32();
    short blockAlign = reader.ReadInt16();
    short bitsPerSample = reader.ReadInt16();
    if (audioType != AudioLoaderWav.WavAudioFormatType.PCM)
      throw new NotImplementedException("Unable to support audio types other than PCM.");
    reader.BaseStream.Position = num3;
    while (true)
    {
      AudioLoaderWav.ReadFourCC(reader, span);
      // ISSUE: reference to a compiler-generated field
      if (!((ReadOnlySpan<byte>) span).SequenceEqual<byte>(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u0030E8703AFC36AEC11160A11E1F9DE65A07F4622274FF5197C273FB7EC7C75FCF8, 4)))
        AudioLoaderWav.SkipChunk(reader);
      else
        break;
    }
    int count = reader.ReadInt32();
    byte[] data = reader.ReadBytes(count);
    return new AudioLoaderWav.WavData(audioType, numChannels, sampleRate, byteRate, blockAlign, bitsPerSample, (ReadOnlyMemory<byte>) data);
  }

  internal readonly struct WavData(
    AudioLoaderWav.WavAudioFormatType audioType,
    short numChannels,
    int sampleRate,
    int byteRate,
    short blockAlign,
    short bitsPerSample,
    ReadOnlyMemory<byte> data)
  {
    public readonly AudioLoaderWav.WavAudioFormatType AudioType = audioType;
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
    PCM,
  }
}
