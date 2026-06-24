// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Emote.PlayEmoteEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Emote;

[NetSerializable]
[Serializable]
public sealed class PlayEmoteEvent : EntityEventArgs
{
  public NetEntity Entity;
  public EmotePlayMode PlayMode;
  public float? Duration;
  public int? RepeatCount;
  public string RsiPath = string.Empty;
  public string StateName = string.Empty;
  public SoundSpecifier? Sound;
  public float Scale = 1f;
}
