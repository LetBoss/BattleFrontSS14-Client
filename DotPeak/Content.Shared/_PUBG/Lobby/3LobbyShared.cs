// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Lobby.LobbyJoinModeMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Lobby;

[NetSerializable]
[Serializable]
public sealed class LobbyJoinModeMessage : EntityEventArgs
{
  public PubgMatchMode MatchMode { get; }

  public bool PreferFullSquad { get; }

  public LobbyJoinModeMessage(PubgMatchMode matchMode, bool preferFullSquad)
  {
    this.MatchMode = matchMode;
    this.PreferFullSquad = preferFullSquad;
  }
}
