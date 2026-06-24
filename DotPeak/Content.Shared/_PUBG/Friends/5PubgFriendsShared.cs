// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Friends.PubgFriendsStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Friends;

[NetSerializable]
[Serializable]
public sealed class PubgFriendsStateEvent : EntityEventArgs
{
  public List<PubgFriendEntry> Friends { get; }

  public List<PubgFriendRequestEntry> Requests { get; }

  public List<PubgFriendOutgoingEntry> Outgoing { get; }

  public PubgFriendsStateEvent(
    List<PubgFriendEntry> friends,
    List<PubgFriendRequestEntry> requests,
    List<PubgFriendOutgoingEntry> outgoing)
  {
    this.Friends = friends;
    this.Requests = requests;
    this.Outgoing = outgoing;
  }
}
