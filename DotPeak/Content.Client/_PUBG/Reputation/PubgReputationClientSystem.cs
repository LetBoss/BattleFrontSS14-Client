// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Reputation.PubgReputationClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Reputation;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Reputation;

public sealed class PubgReputationClientSystem : EntitySystem
{
  public int Reputation { get; private set; } = 100;

  public event Action<int>? OnReputationUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgReputationStateMessage>(new EntityEventHandler<PubgReputationStateMessage>(this.OnReputationState), (Type[]) null, (Type[]) null);
  }

  public void RequestState()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgReputationRequestMessage());
  }

  public void ClearState()
  {
    this.Reputation = 100;
    Action<int> reputationUpdated = this.OnReputationUpdated;
    if (reputationUpdated == null)
      return;
    reputationUpdated(this.Reputation);
  }

  private void OnReputationState(PubgReputationStateMessage msg)
  {
    this.Reputation = msg.Reputation;
    Action<int> reputationUpdated = this.OnReputationUpdated;
    if (reputationUpdated == null)
      return;
    reputationUpdated(this.Reputation);
  }
}
