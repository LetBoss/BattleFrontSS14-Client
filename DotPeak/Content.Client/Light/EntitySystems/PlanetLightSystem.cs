// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.PlanetLightSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class PlanetLightSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _cfgManager;
  [Dependency]
  private IOverlayManager _overlayMan;
  private bool _ambientOcclusion;

  public bool AmbientOcclusion
  {
    get => this._ambientOcclusion;
    set
    {
      if (this._ambientOcclusion == value)
        return;
      this._ambientOcclusion = value;
      if (value)
        this._overlayMan.AddOverlay((Overlay) new AmbientOcclusionOverlay());
      else
        this._overlayMan.RemoveOverlay<AmbientOcclusionOverlay>();
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GetClearColorEvent>(new EntityEventRefHandler<GetClearColorEvent>((object) this, __methodptr(OnClearColor)), (Type[]) null, (Type[]) null);
    this._cfgManager.OnValueChanged<bool>(CCVars.AmbientOcclusion, (Action<bool>) (val => this.AmbientOcclusion = val), true);
    this._overlayMan.AddOverlay((Overlay) new BeforeLightTargetOverlay());
    this._overlayMan.AddOverlay((Overlay) new RoofOverlay((IEntityManager) this.EntityManager));
    this._overlayMan.AddOverlay((Overlay) new TileEmissionOverlay((IEntityManager) this.EntityManager));
    this._overlayMan.AddOverlay((Overlay) new LightBlurOverlay());
    this._overlayMan.AddOverlay((Overlay) new SunShadowOverlay());
    this._overlayMan.AddOverlay((Overlay) new AfterLightTargetOverlay());
  }

  private void OnClearColor(ref GetClearColorEvent ev) => ev.Color = new Color?(Color.Transparent);

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayMan.RemoveOverlay<BeforeLightTargetOverlay>();
    this._overlayMan.RemoveOverlay<RoofOverlay>();
    this._overlayMan.RemoveOverlay<TileEmissionOverlay>();
    this._overlayMan.RemoveOverlay<LightBlurOverlay>();
    this._overlayMan.RemoveOverlay<SunShadowOverlay>();
    this._overlayMan.RemoveOverlay<AfterLightTargetOverlay>();
  }
}
