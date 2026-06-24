// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.MiniGameLobbyPlayerInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[NetSerializable]
[Serializable]
public sealed class MiniGameLobbyPlayerInfo
{
  public NetUserId UserId { get; }

  public string Name { get; }

  public bool IsLeader { get; }

  public MiniGameLobbyPlayerInfo(NetUserId userId, string name, bool isLeader)
  {
    this.UserId = userId;
    this.Name = name;
    this.IsLeader = isLeader;
  }
}
