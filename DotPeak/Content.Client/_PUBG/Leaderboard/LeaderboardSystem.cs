// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Leaderboard.LeaderboardSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Leaderboard;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Leaderboard;

public sealed class LeaderboardSystem : EntitySystem
{
  public event Action<LeaderboardResponseMessage>? OnLeaderboardReceived;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<LeaderboardResponseMessage>(new EntityEventHandler<LeaderboardResponseMessage>(this.OnLeaderboardResponse), (Type[]) null, (Type[]) null);
  }

  private void OnLeaderboardResponse(LeaderboardResponseMessage msg)
  {
    Action<LeaderboardResponseMessage> leaderboardReceived = this.OnLeaderboardReceived;
    if (leaderboardReceived == null)
      return;
    leaderboardReceived(msg);
  }

  public void RequestLeaderboard(string sortBy = "rating")
  {
    this.RaiseNetworkEvent((EntityEventArgs) new LeaderboardRequestMessage(sortBy));
  }
}
