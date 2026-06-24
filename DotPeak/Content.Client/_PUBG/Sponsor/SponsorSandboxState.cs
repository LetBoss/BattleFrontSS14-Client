// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Sponsor.SponsorSandboxState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorSandboxState
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

  public IReadOnlyCollection<string> DisallowedEntityIds { get; }

  public bool Enabled
  {
    get
    {
      return this.AllowSpawnEntities || this.AllowSpawnTiles || this.AllowSpawnDecals || this.AllowEraseEntities || this.AllowEraseTiles;
    }
  }

  public SponsorSandboxState(
    bool allowSpawnEntities,
    bool allowSpawnTiles,
    bool allowSpawnDecals,
    bool allowEraseEntities,
    bool allowEraseTiles,
    bool allowSponsorArena,
    bool allowSponsorAghost,
    bool blockEraseMinds,
    bool isMiniGameSandbox,
    IReadOnlyCollection<string> disallowedEntityIds)
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

  public static SponsorSandboxState Disabled()
  {
    return new SponsorSandboxState(false, false, false, false, false, false, false, false, false, (IReadOnlyCollection<string>) Array.Empty<string>());
  }
}
