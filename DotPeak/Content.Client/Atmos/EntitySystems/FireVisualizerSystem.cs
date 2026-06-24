// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.FireVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.Components;
using Content.Shared._RMC14.Atmos;
using Content.Shared.Atmos;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class FireVisualizerSystem : VisualizerSystem<FireVisualsComponent>
{
  [Dependency]
  private PointLightSystem _lights;
  private EntityQuery<RMCFireColorComponent> _fireColorQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._fireColorQuery = ((EntitySystem) this).GetEntityQuery<RMCFireColorComponent>();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<FireVisualsComponent, ComponentInit>(new ComponentEventHandler<FireVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<FireVisualsComponent, ComponentShutdown>(new ComponentEventHandler<FireVisualsComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnShutdown(EntityUid uid, FireVisualsComponent component, ComponentShutdown args)
  {
    if (component.LightEntity.HasValue)
    {
      ((EntitySystem) this).Del(new EntityUid?(component.LightEntity.Value));
      component.LightEntity = new EntityUid?();
    }
    SpriteComponent spriteComponent;
    int num;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) FireVisualLayers.Fire, ref num, false))
      return;
    this.SpriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, true);
  }

  private void OnComponentInit(EntityUid uid, FireVisualsComponent component, ComponentInit args)
  {
    SpriteComponent sprite;
    AppearanceComponent appearance;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref sprite) || !((EntitySystem) this).TryComp<AppearanceComponent>(uid, ref appearance))
      return;
    this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) FireVisualLayers.Fire);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) FireVisualLayers.Fire, false);
    sprite.LayerSetShader((object) FireVisualLayers.Fire, "unshaded");
    if (component.Sprite != null)
      this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) FireVisualLayers.Fire, new ResPath(component.Sprite), new RSI.StateId?());
    this.UpdateAppearance(uid, component, sprite, appearance);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    FireVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateAppearance(uid, component, args.Sprite, args.Component);
  }

  private void UpdateAppearance(
    EntityUid uid,
    FireVisualsComponent component,
    SpriteComponent sprite,
    AppearanceComponent appearance)
  {
    int num1;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) FireVisualLayers.Fire, ref num1, false))
      return;
    bool flag;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) FireVisuals.OnFire, ref flag, appearance);
    float num2;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(uid, (Enum) FireVisuals.FireStacks, ref num2, appearance);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, flag);
    if (!flag)
    {
      if (!component.LightEntity.HasValue)
        return;
      ((EntitySystem) this).Del(new EntityUid?(component.LightEntity.Value));
      component.LightEntity = new EntityUid?();
    }
    else
    {
      if ((double) num2 > (double) component.FireStackAlternateState && !string.IsNullOrEmpty(component.AlternateState))
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, RSI.StateId.op_Implicit(component.AlternateState));
      else
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, RSI.StateId.op_Implicit(component.NormalState));
      Color color = component.LightColor;
      RMCFireColorComponent fireColorComponent;
      if (this._fireColorQuery.TryComp(uid, ref fireColorComponent))
      {
        color = fireColorComponent.Color;
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, color);
      }
      FireVisualsComponent visualsComponent = component;
      visualsComponent.LightEntity.GetValueOrDefault();
      if (!visualsComponent.LightEntity.HasValue)
      {
        EntityUid entityUid = ((EntitySystem) this).Spawn((string) null, new EntityCoordinates(uid, new Vector2()));
        visualsComponent.LightEntity = new EntityUid?(entityUid);
      }
      PointLightComponent pointLightComponent = ((EntitySystem) this).EnsureComp<PointLightComponent>(component.LightEntity.Value);
      ((SharedPointLightSystem) this._lights).SetColor(component.LightEntity.Value, color, (SharedPointLightComponent) pointLightComponent);
      ((SharedPointLightSystem) this._lights).SetRadius(component.LightEntity.Value, Math.Clamp((float) (1.5 + (double) component.LightRadiusPerStack * (double) num2), 0.0f, component.MaxLightRadius), (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
      ((SharedPointLightSystem) this._lights).SetEnergy(component.LightEntity.Value, Math.Clamp((float) (1.0 + (double) component.LightEnergyPerStack * (double) num2), 0.0f, component.MaxLightEnergy), (SharedPointLightComponent) pointLightComponent);
    }
  }
}
