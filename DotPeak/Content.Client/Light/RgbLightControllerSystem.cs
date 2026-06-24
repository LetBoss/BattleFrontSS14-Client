// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.RgbLightControllerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items.Systems;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Light;

public sealed class RgbLightControllerSystem : SharedRgbLightControllerSystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private ItemSystem _itemSystem;
  [Dependency]
  private SharedPointLightSystem _lights;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RgbLightControllerComponent, ComponentHandleState>(new ComponentEventRefHandler<RgbLightControllerComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RgbLightControllerComponent, ComponentShutdown>(new ComponentEventHandler<RgbLightControllerComponent, ComponentShutdown>((object) this, __methodptr(OnComponentShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RgbLightControllerComponent, ComponentStartup>(new ComponentEventHandler<RgbLightControllerComponent, ComponentStartup>((object) this, __methodptr(OnComponentStart)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RgbLightControllerComponent, GotUnequippedEvent>(new ComponentEventHandler<RgbLightControllerComponent, GotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RgbLightControllerComponent, EquipmentVisualsUpdatedEvent>(new ComponentEventHandler<RgbLightControllerComponent, EquipmentVisualsUpdatedEvent>((object) this, __methodptr(OnEquipmentVisualsUpdated)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RgbLightControllerComponent, HeldVisualsUpdatedEvent>(new ComponentEventHandler<RgbLightControllerComponent, HeldVisualsUpdatedEvent>((object) this, __methodptr(OnHeldVisualsUpdated)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentStart(
    EntityUid uid,
    RgbLightControllerComponent rgb,
    ComponentStartup args)
  {
    this.GetOriginalColors(uid, rgb);
    this._itemSystem.VisualsChanged(uid);
  }

  private void OnComponentShutdown(
    EntityUid uid,
    RgbLightControllerComponent rgb,
    ComponentShutdown args)
  {
    if (this.LifeStage(uid, (MetaDataComponent) null) >= 4)
      return;
    this.ResetOriginalColors(uid, rgb);
    this._itemSystem.VisualsChanged(uid);
  }

  private void OnGotUnequipped(
    EntityUid uid,
    RgbLightControllerComponent rgb,
    GotUnequippedEvent args)
  {
    rgb.Holder = new EntityUid?();
    rgb.HolderLayers = (List<string>) null;
  }

  private void OnHeldVisualsUpdated(
    EntityUid uid,
    RgbLightControllerComponent rgb,
    HeldVisualsUpdatedEvent args)
  {
    if (args.RevealedLayers.Count == 0)
    {
      rgb.Holder = new EntityUid?();
      rgb.HolderLayers = (List<string>) null;
    }
    else
    {
      rgb.Holder = new EntityUid?(args.User);
      rgb.HolderLayers = new List<string>();
      SpriteComponent spriteComponent;
      if (!this.TryComp<SpriteComponent>(args.User, ref spriteComponent))
        return;
      foreach (string revealedLayer in args.RevealedLayers)
      {
        int num;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((args.User, spriteComponent)), revealedLayer, ref num, false) && spriteComponent[num] is SpriteComponent.Layer layer)
        {
          ProtoId<ShaderPrototype>? shaderPrototype = layer.ShaderPrototype;
          ProtoId<ShaderPrototype>? nullable = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
          if ((shaderPrototype.HasValue == nullable.HasValue ? (shaderPrototype.HasValue ? (ProtoId<ShaderPrototype>.op_Equality(shaderPrototype.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
            rgb.HolderLayers.Add(revealedLayer);
        }
      }
    }
  }

  private void OnEquipmentVisualsUpdated(
    EntityUid uid,
    RgbLightControllerComponent rgb,
    EquipmentVisualsUpdatedEvent args)
  {
    rgb.Holder = new EntityUid?(args.Equipee);
    rgb.HolderLayers = new List<string>();
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(args.Equipee, ref spriteComponent))
      return;
    foreach (string revealedLayer in args.RevealedLayers)
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((args.Equipee, spriteComponent)), revealedLayer, ref num, false) && spriteComponent[num] is SpriteComponent.Layer layer)
      {
        ProtoId<ShaderPrototype>? shaderPrototype = layer.ShaderPrototype;
        ProtoId<ShaderPrototype>? nullable = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
        if ((shaderPrototype.HasValue == nullable.HasValue ? (shaderPrototype.HasValue ? (ProtoId<ShaderPrototype>.op_Equality(shaderPrototype.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
          rgb.HolderLayers.Add(revealedLayer);
      }
    }
  }

  private void OnHandleState(
    EntityUid uid,
    RgbLightControllerComponent rgb,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is RgbLightControllerState current))
      return;
    this.ResetOriginalColors(uid, rgb);
    rgb.CycleRate = current.CycleRate;
    rgb.Layers = current.Layers;
    this.GetOriginalColors(uid, rgb);
  }

  private void GetOriginalColors(
    EntityUid uid,
    RgbLightControllerComponent? rgb = null,
    PointLightComponent? light = null,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<RgbLightControllerComponent, SpriteComponent, PointLightComponent>(uid, ref rgb, ref sprite, ref light, true))
      return;
    rgb.OriginalLightColor = ((SharedPointLightComponent) light).Color;
    rgb.OriginalLayerColors = new Dictionary<int, Color>();
    int num = sprite.AllLayers.Count<ISpriteLayer>();
    if (rgb.Layers == null)
    {
      rgb.Layers = new List<int>();
      for (int key = 0; key < num; ++key)
      {
        if (sprite[key] is SpriteComponent.Layer layer)
        {
          ProtoId<ShaderPrototype>? shaderPrototype = layer.ShaderPrototype;
          ProtoId<ShaderPrototype>? nullable = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
          if ((shaderPrototype.HasValue == nullable.HasValue ? (shaderPrototype.HasValue ? (ProtoId<ShaderPrototype>.op_Equality(shaderPrototype.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
          {
            rgb.Layers.Add(key);
            rgb.OriginalLayerColors[key] = layer.Color;
          }
        }
      }
    }
    else
    {
      foreach (int key in rgb.Layers.ToArray())
      {
        if (key < num)
        {
          rgb.OriginalLayerColors[key] = sprite[key].Color;
        }
        else
        {
          this.Log.Warning($"RGB light attempted to use invalid sprite index {key} on entity {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
          rgb.Layers.Remove(key);
        }
      }
    }
  }

  private void ResetOriginalColors(
    EntityUid uid,
    RgbLightControllerComponent? rgb = null,
    PointLightComponent? light = null,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<RgbLightControllerComponent, SpriteComponent, PointLightComponent>(uid, ref rgb, ref sprite, ref light, false))
      return;
    this._lights.SetColor(uid, rgb.OriginalLightColor, (SharedPointLightComponent) light);
    if (rgb.Layers == null || rgb.OriginalLayerColors == null)
      return;
    foreach ((int key, Color color) in rgb.OriginalLayerColors)
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, color);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    EntityQueryEnumerator<RgbLightControllerComponent, PointLightComponent, SpriteComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RgbLightControllerComponent, PointLightComponent, SpriteComponent>();
    EntityUid entityUid1;
    RgbLightControllerComponent controllerComponent1;
    PointLightComponent pointLightComponent;
    SpriteComponent spriteComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid1, ref controllerComponent1, ref pointLightComponent, ref spriteComponent1))
    {
      Color currentRgbColor = RgbLightControllerSystem.GetCurrentRgbColor(this._gameTiming.RealTime, (double) controllerComponent1.CreationTick.Value * this._gameTiming.TickPeriod, Entity<RgbLightControllerComponent>.op_Implicit((entityUid1, controllerComponent1)));
      this._lights.SetColor(entityUid1, currentRgbColor, (SharedPointLightComponent) pointLightComponent);
      if (controllerComponent1.Layers != null)
      {
        foreach (int layer1 in controllerComponent1.Layers)
        {
          SpriteComponent.Layer layer2;
          if (this._sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent1)), layer1, ref layer2, false))
            layer2.Color = currentRgbColor;
        }
      }
      SpriteComponent spriteComponent2;
      if (controllerComponent1.HolderLayers != null && this.TryComp<SpriteComponent>(controllerComponent1.Holder, ref spriteComponent2))
      {
        foreach (string holderLayer in controllerComponent1.HolderLayers)
        {
          int num;
          if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((controllerComponent1.Holder.Value, spriteComponent2)), holderLayer, ref num, false))
            this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((controllerComponent1.Holder.Value, spriteComponent2)), num, currentRgbColor);
        }
      }
    }
    EntityQueryEnumerator<MapLightComponent, RgbLightControllerComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<MapLightComponent, RgbLightControllerComponent>();
    EntityUid entityUid2;
    MapLightComponent mapLightComponent;
    RgbLightControllerComponent controllerComponent2;
    while (entityQueryEnumerator2.MoveNext(ref entityUid2, ref mapLightComponent, ref controllerComponent2))
    {
      Color currentRgbColor = RgbLightControllerSystem.GetCurrentRgbColor(this._gameTiming.RealTime, (double) controllerComponent2.CreationTick.Value * this._gameTiming.TickPeriod, Entity<RgbLightControllerComponent>.op_Implicit((entityUid2, controllerComponent2)));
      mapLightComponent.AmbientLightColor = currentRgbColor;
    }
  }

  public static Color GetCurrentRgbColor(
    TimeSpan curTime,
    TimeSpan offset,
    Entity<RgbLightControllerComponent> rgb)
  {
    double totalSeconds = (curTime - offset).TotalSeconds;
    float num = Math.Abs((float) rgb.Owner.Id * 0.09817f);
    double cycleRate = (double) rgb.Comp.CycleRate;
    return Color.FromHsv(new Vector4(MathF.Abs((float) ((totalSeconds * cycleRate + (double) num) % 1.0)), 1f, 1f, 1f));
  }
}
