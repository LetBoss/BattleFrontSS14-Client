// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.GlobalSoundEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Audio;

[Virtual]
[NetSerializable]
[Serializable]
public class GlobalSoundEvent : EntityEventArgs
{
  public ResolvedSoundSpecifier Specifier;
  public Robust.Shared.Audio.AudioParams? AudioParams;

  public GlobalSoundEvent(ResolvedSoundSpecifier specifier, Robust.Shared.Audio.AudioParams? audioParams = null)
  {
    this.Specifier = specifier;
    this.AudioParams = audioParams;
  }
}
