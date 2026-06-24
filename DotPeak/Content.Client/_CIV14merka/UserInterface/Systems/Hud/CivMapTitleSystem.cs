// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.UserInterface.Systems.Hud.CivMapTitleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivMapTitleSystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  private CivMapTitleOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivMapTitleEvent>(new EntitySessionEventHandler<CivMapTitleEvent>(this.OnMapTitle), (Type[]) null, (Type[]) null);
    this._overlay = new CivMapTitleOverlay();
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnMapTitle(CivMapTitleEvent msg, EntitySessionEventArgs args)
  {
    this._overlay.Show(msg.Title);
  }
}
