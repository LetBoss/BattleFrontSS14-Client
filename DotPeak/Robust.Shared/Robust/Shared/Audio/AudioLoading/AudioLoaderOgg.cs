// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioLoading.AudioLoaderOgg
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NVorbis;
using System;
using System.IO;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Audio.AudioLoading;

internal static class AudioLoaderOgg
{
  private const int ReadBufferLength = 32768 /*0x8000*/;

  public static AudioMetadata LoadAudioMetadata(Stream stream)
  {
    using (VorbisReader vorbisReader = new VorbisReader(stream, false))
    {
      vorbisReader.Initialize();
      return new AudioMetadata(vorbisReader.TotalTime, vorbisReader.Channels, vorbisReader.Tags.Title, vorbisReader.Tags.Artist);
    }
  }

  public static AudioLoaderOgg.OggVorbisData LoadAudioData(Stream stream)
  {
    using (VorbisReader reader = new VorbisReader(stream, false))
    {
      reader.Initialize();
      int sampleRate = reader.SampleRate;
      int channels = reader.Channels;
      long totalSamples = reader.TotalSamples;
      long num1 = totalSamples * (long) channels;
      int start = 0;
      short[] numArray = new short[totalSamples * (long) channels];
      Span<float> readBuffer = stackalloc float[32768 /*0x8000*/];
      int num2;
      for (; (long) start < num1; start += num2)
      {
        num2 = AudioLoaderOgg.ReadSamples(numArray.AsSpan<short>(start), readBuffer, channels, reader);
        if (num2 == 0)
          break;
      }
      return new AudioLoaderOgg.OggVorbisData(totalSamples, (long) sampleRate, (long) channels, (ReadOnlyMemory<short>) numArray, reader.Tags.Title, reader.Tags.Artist);
    }
  }

  private static int ReadSamples(
    Span<short> dest,
    Span<float> readBuffer,
    int channels,
    VorbisReader reader)
  {
    if (readBuffer.Length > dest.Length)
      readBuffer = readBuffer.Slice(0, dest.Length);
    int length = reader.ReadSamples(readBuffer) * channels;
    AudioLoaderOgg.ConvertToShort((ReadOnlySpan<float>) readBuffer.Slice(0, length), dest.Slice(0, length));
    return length;
  }

  private static void ConvertToShort(ReadOnlySpan<float> src, Span<short> dst)
  {
    if (src.Length != dst.Length)
      throw new InvalidOperationException("Invalid lengths!");
    int num = src.Length / Vector<short>.Count * Vector<short>.Count;
    Vector<float> vector1 = new Vector<float>((float) short.MaxValue);
    ref readonly float local1 = ref src[0];
    ref short local2 = ref dst[0];
    for (int elementOffset = 0; elementOffset < num; elementOffset += Vector<short>.Count)
    {
      Vector<float> vector2 = Vector.LoadUnsafe<float>(ref local1, (UIntPtr) elementOffset);
      Vector<float> vector3 = Vector.LoadUnsafe<float>(ref local1, (UIntPtr) (elementOffset + Vector<float>.Count));
      Vector<float> vector4 = vector2 * vector1;
      Vector<float> vector5 = vector1;
      Vector<float> vector6 = vector3 * vector5;
      Vector.Narrow(Vector.ConvertToInt32(vector4), Vector.ConvertToInt32(vector6)).StoreUnsafe<short>(ref local2, (UIntPtr) elementOffset);
    }
    for (int index = num; index < src.Length; ++index)
      dst[index] = (short) ((double) src[index] * (double) short.MaxValue);
  }

  internal readonly struct OggVorbisData(
    long totalSamples,
    long sampleRate,
    long channels,
    ReadOnlyMemory<short> data,
    string title,
    string artist)
  {
    public readonly long TotalSamples = totalSamples;
    public readonly long SampleRate = sampleRate;
    public readonly long Channels = channels;
    public readonly ReadOnlyMemory<short> Data = data;
    public readonly string Title = title;
    public readonly string Artist = artist;
  }
}
