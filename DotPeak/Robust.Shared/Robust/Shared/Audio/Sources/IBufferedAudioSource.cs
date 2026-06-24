// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Sources.IBufferedAudioSource
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Audio.Sources;

internal interface IBufferedAudioSource : IAudioSource, IDisposable
{
  int SampleRate { get; set; }

  int GetNumberOfBuffersProcessed();

  void GetBuffersProcessed(Span<int> handles);

  void WriteBuffer(int handle, ReadOnlySpan<ushort> data);

  void WriteBuffer(int handle, ReadOnlySpan<float> data);

  void QueueBuffers(ReadOnlySpan<int> handles);

  void EmptyBuffers();
}
