// Decompiled with JetBrains decompiler
// Type: Content.Client.Voting.VotingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Voting;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Voting;

public sealed class VotingSystem : EntitySystem
{
  public event Action<VotePlayerListResponseEvent>? VotePlayerListResponse;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<VotePlayerListResponseEvent>(new EntityEventHandler<VotePlayerListResponseEvent>(this.OnVotePlayerListResponseEvent), (Type[]) null, (Type[]) null);
  }

  private void OnVotePlayerListResponseEvent(VotePlayerListResponseEvent msg)
  {
    Action<VotePlayerListResponseEvent> playerListResponse = this.VotePlayerListResponse;
    if (playerListResponse == null)
      return;
    playerListResponse(msg);
  }

  public void RequestVotePlayerList()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new VotePlayerListRequestEvent());
  }
}
