// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Friends.PubgFriendEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Friends;

[NetSerializable]
[Serializable]
public sealed class PubgFriendEntry
{
  public NetUserId UserId { get; }

  public string Ckey { get; }

  public PubgFriendStatus Status { get; }

  public int PartySize { get; }

  public int PartyMax { get; }

  public DateTime? LastSeenUtc { get; }

  public PubgFriendEntry(
    NetUserId userId,
    string ckey,
    PubgFriendStatus status,
    int partySize,
    int partyMax,
    DateTime? lastSeenUtc)
  {
    this.UserId = userId;
    this.Ckey = ckey;
    this.Status = status;
    this.PartySize = partySize;
    this.PartyMax = partyMax;
    this.LastSeenUtc = lastSeenUtc;
  }
}
