// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.AmbientSoundComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Audio;

[NetSerializable]
[Serializable]
public sealed class AmbientSoundComponentState : ComponentState
{
  public bool Enabled { get; init; }

  public float Range { get; init; }

  public float Volume { get; init; }

  public SoundSpecifier Sound { get; init; }
}
