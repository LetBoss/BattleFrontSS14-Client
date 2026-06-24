// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Sponsor.SponsorSandboxStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Sponsor;

[NetSerializable]
[Serializable]
public sealed class SponsorSandboxStateMessage : EntityEventArgs
{
  public bool AllowSpawnEntities { get; }

  public bool AllowSpawnTiles { get; }

  public bool AllowSpawnDecals { get; }

  public bool AllowEraseEntities { get; }

  public bool AllowEraseTiles { get; }

  public bool AllowSponsorArena { get; }

  public bool AllowSponsorAghost { get; }

  public bool BlockEraseMinds { get; }

  public bool IsMiniGameSandbox { get; }

  public List<string> DisallowedEntityIds { get; }

  public SponsorSandboxStateMessage(
    bool allowSpawnEntities,
    bool allowSpawnTiles,
    bool allowSpawnDecals,
    bool allowEraseEntities,
    bool allowEraseTiles,
    bool allowSponsorArena,
    bool allowSponsorAghost,
    bool blockEraseMinds,
    bool isMiniGameSandbox,
    List<string> disallowedEntityIds)
  {
    this.AllowSpawnEntities = allowSpawnEntities;
    this.AllowSpawnTiles = allowSpawnTiles;
    this.AllowSpawnDecals = allowSpawnDecals;
    this.AllowEraseEntities = allowEraseEntities;
    this.AllowEraseTiles = allowEraseTiles;
    this.AllowSponsorArena = allowSponsorArena;
    this.AllowSponsorAghost = allowSponsorAghost;
    this.BlockEraseMinds = blockEraseMinds;
    this.IsMiniGameSandbox = isMiniGameSandbox;
    this.DisallowedEntityIds = disallowedEntityIds;
  }
}
