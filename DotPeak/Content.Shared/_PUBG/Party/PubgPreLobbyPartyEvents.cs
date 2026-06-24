// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Party.PubgPreLobbyPartyStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Party;

[NetSerializable]
[Serializable]
public sealed class PubgPreLobbyPartyStateEvent : EntityEventArgs
{
  public List<PubgPreLobbyPartyMemberState> Members { get; }

  public NetUserId LeaderId { get; }

  public PubgMatchMode? SelectedMode { get; }

  public bool PreferFullSquad { get; }

  public PubgPreLobbyPartyStateEvent(
    List<PubgPreLobbyPartyMemberState> members,
    NetUserId leaderId,
    PubgMatchMode? selectedMode,
    bool preferFullSquad)
  {
    this.Members = members;
    this.LeaderId = leaderId;
    this.SelectedMode = selectedMode;
    this.PreferFullSquad = preferFullSquad;
  }
}
