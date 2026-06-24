// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Airdrop.PubgAirdropStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._PUBG.Airdrop;

[NetSerializable]
[Serializable]
public sealed class PubgAirdropStateEvent : EntityEventArgs
{
  public bool Active;
  public Vector2 Position;
  public int RemainingSeconds;
  public MapId MapId;

  public PubgAirdropStateEvent(bool active, Vector2 position, int remainingSeconds, MapId mapId)
  {
    this.Active = active;
    this.Position = position;
    this.RemainingSeconds = remainingSeconds;
    this.MapId = mapId;
  }
}
