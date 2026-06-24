// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.AtmosDebugOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.Overlays;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

internal sealed class AtmosDebugOverlaySystem : SharedAtmosDebugOverlaySystem
{
  public readonly Dictionary<EntityUid, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage> TileData = new Dictionary<EntityUid, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage>();
  public AtmosDebugOverlayMode CfgMode;
  public float CfgBase;
  public float CfgScale = 207.855988f;
  public int CfgSpecificGas;
  public bool CfgCBM;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage>(new EntityEventHandler<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage>(this.HandleAtmosDebugOverlayMessage), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage>(new EntityEventHandler<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage>(this.HandleAtmosDebugOverlayDisableMessage), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved), (Type[]) null, (Type[]) null);
    IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
    if (ioverlayManager.HasOverlay<AtmosDebugOverlay>())
      return;
    ioverlayManager.AddOverlay((Overlay) new AtmosDebugOverlay(this));
  }

  private void OnGridRemoved(GridRemovalEvent ev)
  {
    if (!this.TileData.ContainsKey(ev.EntityUid))
      return;
    this.TileData.Remove(ev.EntityUid);
  }

  private void HandleAtmosDebugOverlayMessage(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage message)
  {
    this.TileData[this.GetEntity(message.GridId)] = message;
  }

  private void HandleAtmosDebugOverlayDisableMessage(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage ev)
  {
    this.TileData.Clear();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
    if (!ioverlayManager.HasOverlay<AtmosDebugOverlay>())
      return;
    ioverlayManager.RemoveOverlay<AtmosDebugOverlay>();
  }

  public void Reset(RoundRestartCleanupEvent ev) => this.TileData.Clear();

  public bool HasData(EntityUid gridId) => this.TileData.ContainsKey(gridId);
}
