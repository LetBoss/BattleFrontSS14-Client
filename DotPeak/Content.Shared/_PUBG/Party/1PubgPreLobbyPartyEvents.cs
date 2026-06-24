// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Party.PubgPreLobbyPartyMemberState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Party;

[NetSerializable]
[Serializable]
public sealed class PubgPreLobbyPartyMemberState
{
  public NetUserId UserId { get; }

  public string Ckey { get; }

  public int Level { get; }

  public int Xp { get; }

  public int MaxXp { get; }

  public bool IsReady { get; }

  public bool InPreLobby { get; }

  public HumanoidCharacterProfile? Profile { get; }

  public Dictionary<string, string> CurrentOutfit { get; }

  public PubgPreLobbyPartyMemberState(
    NetUserId userId,
    string ckey,
    int level,
    int xp,
    int maxXp,
    bool isReady,
    bool inPreLobby,
    HumanoidCharacterProfile? profile,
    Dictionary<string, string>? currentOutfit)
  {
    this.UserId = userId;
    this.Ckey = ckey;
    this.Level = level;
    this.Xp = xp;
    this.MaxXp = maxXp;
    this.IsReady = isReady;
    this.InPreLobby = inPreLobby;
    this.Profile = profile;
    this.CurrentOutfit = currentOutfit != null ? new Dictionary<string, string>((IDictionary<string, string>) currentOutfit) : new Dictionary<string, string>();
  }
}
