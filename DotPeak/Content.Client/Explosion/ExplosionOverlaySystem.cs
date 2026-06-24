// Decompiled with JetBrains decompiler
// Type: Content.Client.Explosion.ExplosionOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Explosion;
using Content.Shared.Explosion.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Explosion;

public sealed class ExplosionOverlaySystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private IResourceCache _resCache;
  [Dependency]
  private IOverlayManager _overlayMan;
  [Dependency]
  private SharedPointLightSystem _lights;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentInit>(new ComponentEventHandler<ExplosionVisualsComponent, ComponentInit>((object) this, __methodptr(OnExplosionInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentRemove>(new ComponentEventHandler<ExplosionVisualsComponent, ComponentRemove>((object) this, __methodptr(OnCompRemove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<ExplosionVisualsComponent, ComponentHandleState>((object) this, __methodptr(OnExplosionHandleState)), (Type[]) null, (Type[]) null);
    this._overlayMan.AddOverlay((Overlay) new ExplosionOverlay(this._appearance));
  }

  private void OnExplosionHandleState(
    EntityUid uid,
    ExplosionVisualsComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is ExplosionVisualsState current))
      return;
    component.Epicenter = current.Epicenter;
    component.SpaceTiles = current.SpaceTiles;
    component.Tiles.Clear();
    foreach ((NetEntity key, Dictionary<int, List<Vector2i>> dictionary) in current.Tiles)
      component.Tiles[this.GetEntity(key)] = dictionary;
    component.Intensity = current.Intensity;
    component.ExplosionType = current.ExplosionType;
    component.SpaceMatrix = current.SpaceMatrix;
    component.SpaceTileSize = current.SpaceTileSize;
  }

  private void OnCompRemove(
    EntityUid uid,
    ExplosionVisualsComponent component,
    ComponentRemove args)
  {
    ExplosionVisualsTexturesComponent texturesComponent;
    if (!this.TryComp<ExplosionVisualsTexturesComponent>(uid, ref texturesComponent) || this.Deleted(texturesComponent.LightEntity, (MetaDataComponent) null))
      return;
    this.QueueDel(new EntityUid?(texturesComponent.LightEntity));
  }

  private void OnExplosionInit(
    EntityUid uid,
    ExplosionVisualsComponent component,
    ComponentInit args)
  {
    this.EnsureComp<ExplosionVisualsTexturesComponent>(uid);
    ExplosionPrototype explosionPrototype;
    ExplosionVisualsTexturesComponent texturesComponent;
    if (!this._protoMan.TryIndex<ExplosionPrototype>(component.ExplosionType, ref explosionPrototype) || !this.TryComp<ExplosionVisualsTexturesComponent>(uid, ref texturesComponent))
      return;
    if (this._mapSystem.MapExists(new MapId?(component.Epicenter.MapId)))
    {
      EntityUid entityUid = this.Spawn("ExplosionLight", component.Epicenter, (ComponentRegistry) null, new Angle());
      SharedPointLightComponent pointLightComponent = this._lights.EnsureLight(entityUid);
      this._lights.SetRadius(entityUid, (float) component.Intensity.Count, pointLightComponent, (MetaDataComponent) null);
      this._lights.SetEnergy(entityUid, (float) component.Intensity.Count, pointLightComponent);
      this._lights.SetColor(entityUid, explosionPrototype.LightColor, pointLightComponent);
      texturesComponent.LightEntity = entityUid;
    }
    texturesComponent.FireColor = explosionPrototype.FireColor;
    texturesComponent.IntensityPerState = explosionPrototype.IntensityPerState;
    foreach (Robust.Client.Graphics.RSI.State state in this._resCache.GetResource<RSIResource>(explosionPrototype.TexturePath, true).RSI)
    {
      texturesComponent.FireFrames.Add(state.GetFrames((RsiDirection) 0));
      if (texturesComponent.FireFrames.Count == explosionPrototype.FireStates)
        break;
    }
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayMan.RemoveOverlay<ExplosionOverlay>();
  }
}
