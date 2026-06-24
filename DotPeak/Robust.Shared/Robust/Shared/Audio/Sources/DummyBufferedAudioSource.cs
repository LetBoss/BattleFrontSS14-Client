// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Sources.DummyBufferedAudioSource
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Audio.Sources;

internal sealed class DummyBufferedAudioSource : 
  DummyAudioSource,
  IBufferedAudioSource,
  IAudioSource,
  IDisposable
{
  public static DummyBufferedAudioSource Instance { get; } = new DummyBufferedAudioSource();

  public int SampleRate { get; set; }

  public void WriteBuffer(int handle, ReadOnlySpan<ushort> data)
  {
  }

  public void WriteBuffer(int handle, ReadOnlySpan<float> data)
  {
  }

  public void QueueBuffers(ReadOnlySpan<int> handles)
  {
  }

  public void EmptyBuffers()
  {
  }

  public void GetBuffersProcessed(Span<int> handles)
  {
  }

  public int GetNumberOfBuffersProcessed() => 0;
}
