// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.RoundEnd.PubgRoundEndPartyEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.RoundEnd;

[NetSerializable]
[Serializable]
public sealed class PubgRoundEndPartyEntry
{
  public int PartyId { get; }

  public string Username { get; }

  public int Placement { get; }

  public int Kills { get; }

  public int DamageDealt { get; }

  public int DamageTaken { get; }

  public PubgRoundEndPartyEntry(
    int partyId,
    string username,
    int placement,
    int kills,
    int damageDealt,
    int damageTaken)
  {
    this.PartyId = partyId;
    this.Username = username;
    this.Placement = placement;
    this.Kills = kills;
    this.DamageDealt = damageDealt;
    this.DamageTaken = damageTaken;
  }
}
