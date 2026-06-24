// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioLoading.AudioLoader
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Audio.AudioLoading;

internal static class AudioLoader
{
  public static bool IsLoadableAudioFile(ReadOnlySpan<char> filename)
  {
    ReadOnlySpan<char> extension = Path.GetExtension(filename);
    return extension.SequenceEqual<char>(".wav".AsSpan()) || extension.SequenceEqual<char>(".ogg".AsSpan());
  }

  public static AudioMetadata LoadAudioMetadata(Stream stream, ReadOnlySpan<char> filename)
  {
    ReadOnlySpan<char> extension = Path.GetExtension(filename);
    if (extension.SequenceEqual<char>(".ogg".AsSpan()))
      return AudioLoaderOgg.LoadAudioMetadata(stream);
    if (extension.SequenceEqual<char>(".wav".AsSpan()))
      return AudioLoaderWav.LoadAudioMetadata(stream);
    throw new ArgumentException($"Unknown file type: {extension}");
  }
}
