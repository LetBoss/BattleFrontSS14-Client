// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Ghost.PubgGhostFollowStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Ghost;

[NetSerializable]
[Serializable]
public sealed class PubgGhostFollowStateEvent : EntityEventArgs
{
  public bool Enabled { get; }

  public PubgGhostFollowStateEvent(bool enabled) => this.Enabled = enabled;
}
