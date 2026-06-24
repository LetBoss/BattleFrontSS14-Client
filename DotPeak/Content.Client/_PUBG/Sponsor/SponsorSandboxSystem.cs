// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Sponsor.SponsorSandboxSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Sponsor;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorSandboxSystem : EntitySystem
{
  public SponsorSandboxState State { get; private set; } = SponsorSandboxState.Disabled();

  public event Action<SponsorSandboxState>? OnStateUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<SponsorSandboxStateMessage>(new EntityEventHandler<SponsorSandboxStateMessage>(this.OnState), (Type[]) null, (Type[]) null);
  }

  private void OnState(SponsorSandboxStateMessage msg)
  {
    this.State = new SponsorSandboxState(msg.AllowSpawnEntities, msg.AllowSpawnTiles, msg.AllowSpawnDecals, msg.AllowEraseEntities, msg.AllowEraseTiles, msg.AllowSponsorArena, msg.AllowSponsorAghost, msg.BlockEraseMinds, msg.IsMiniGameSandbox, (IReadOnlyCollection<string>) msg.DisallowedEntityIds);
    Action<SponsorSandboxState> onStateUpdated = this.OnStateUpdated;
    if (onStateUpdated == null)
      return;
    onStateUpdated(this.State);
  }

  public void RequestSponsorArenaTeleport()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SponsorArenaTeleportRequestMessage());
  }

  public void RequestSponsorAghost()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SponsorAghostRequestMessage());
  }
}
