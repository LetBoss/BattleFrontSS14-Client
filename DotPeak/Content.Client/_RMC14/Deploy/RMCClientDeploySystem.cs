// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Deploy.RMCClientDeploySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Deploy;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Deploy;

public sealed class RMCClientDeploySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCShowDeployAreaEvent>(new EntityEventHandler<RMCShowDeployAreaEvent>(this.OnShowDeployArea), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RMCHideDeployAreaEvent>(new EntityEventHandler<RMCHideDeployAreaEvent>(this.OnHideDeployArea), (Type[]) null, (Type[]) null);
  }

  private void OnShowDeployArea(RMCShowDeployAreaEvent ev)
  {
    this._overlayManager.AddOverlay((Overlay) new RMCDeployAreaOverlay()
    {
      Box = ev.Box,
      Color = ev.Color
    });
  }

  private void OnHideDeployArea(RMCHideDeployAreaEvent ev)
  {
    this._overlayManager.RemoveOverlay<RMCDeployAreaOverlay>();
  }
}
