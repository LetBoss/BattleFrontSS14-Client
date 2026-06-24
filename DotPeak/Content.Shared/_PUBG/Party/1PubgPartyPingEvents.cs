// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Party.PubgPartyPingRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._PUBG.Party;

[NetSerializable]
[Serializable]
public sealed class PubgPartyPingRequestEvent : EntityEventArgs
{
  public MapId MapId { get; }

  public Vector2 Position { get; }

  public PubgPartyPingKind Kind { get; }

  public PubgPartyPingRequestEvent(MapId mapId, Vector2 position, PubgPartyPingKind kind)
  {
    this.MapId = mapId;
    this.Position = position;
    this.Kind = kind;
  }
}
