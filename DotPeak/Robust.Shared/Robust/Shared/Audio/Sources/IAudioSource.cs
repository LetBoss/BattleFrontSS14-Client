// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Sources.IAudioSource
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Audio.Effects;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Audio.Sources;

public interface IAudioSource : IDisposable
{
  void Pause();

  void StartPlaying();

  void StopPlaying();

  void Restart();

  bool Playing { get; set; }

  bool Looping { get; set; }

  bool Global { get; set; }

  Vector2 Position { get; set; }

  float Pitch { get; set; }

  float Volume { get; set; }

  float Gain { get; set; }

  float MaxDistance { get; set; }

  float RolloffFactor { get; set; }

  float ReferenceDistance { get; set; }

  float Occlusion { get; set; }

  float PlaybackPosition { get; set; }

  Vector2 Velocity { get; set; }

  void SetAuxiliary(IAuxiliaryAudio? audio);
}
