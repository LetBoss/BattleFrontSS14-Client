// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Party.PubgPartyMemberState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._PUBG.Party;

[NetSerializable]
[Serializable]
public sealed class PubgPartyMemberState
{
  public NetEntity Entity { get; }

  public string Username { get; }

  public int Level { get; }

  public float HpPercent { get; }

  public bool IsDead { get; }

  public bool IsInVoice { get; }

  public MapId MapId { get; }

  public Vector2 Position { get; }

  public int SlotIndex { get; }

  public PubgPartyMemberState(
    NetEntity entity,
    string username,
    int level,
    float hpPercent,
    bool isDead,
    bool isInVoice,
    MapId mapId,
    Vector2 position,
    int slotIndex)
  {
    this.Entity = entity;
    this.Username = username;
    this.Level = level;
    this.HpPercent = hpPercent;
    this.IsDead = isDead;
    this.IsInVoice = isInVoice;
    this.MapId = mapId;
    this.Position = position;
    this.SlotIndex = slotIndex;
  }
}
