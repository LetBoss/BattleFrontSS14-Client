// Decompiled with JetBrains decompiler
// Type: Content.Client.Damage.DamageVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Damage;

public sealed class DamageVisualsSystem : VisualizerSystem<DamageVisualsComponent>
{
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<DamageVisualsComponent, ComponentInit>(new ComponentEventHandler<DamageVisualsComponent, ComponentInit>((object) this, __methodptr(InitializeEntity)), (Type[]) null, (Type[]) null);
  }

  private void InitializeEntity(EntityUid entity, DamageVisualsComponent comp, ComponentInit args)
  {
    this.VerifyVisualizerSetup(entity, comp);
    if (!comp.Valid)
      ((EntitySystem) this).RemCompDeferred<DamageVisualsComponent>(entity);
    else
      this.InitializeVisualizer(entity, comp);
  }

  private void VerifyVisualizerSetup(EntityUid entity, DamageVisualsComponent damageVisComp)
  {
    if (damageVisComp.Thresholds.Count < 1)
    {
      ((EntitySystem) this).Log.Error($"ThresholdsLookup were invalid for entity {entity}. ThresholdsLookup: {damageVisComp.Thresholds}");
      damageVisComp.Valid = false;
    }
    else if ((double) damageVisComp.Divisor == 0.0)
    {
      ((EntitySystem) this).Log.Error($"Divisor for {entity} is set to zero.");
      damageVisComp.Valid = false;
    }
    else
    {
      if (damageVisComp.Overlay)
      {
        if (damageVisComp.DamageOverlayGroups == null && damageVisComp.DamageOverlay == null)
        {
          ((EntitySystem) this).Log.Error($"Enabled overlay without defined damage overlay sprites on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
        if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay == null)
        {
          ((EntitySystem) this).Log.Error($"Enabled all damage tracking without a damage overlay sprite on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
        if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay != null)
        {
          ((EntitySystem) this).Log.Warning($"Disabled all damage tracking with a damage overlay sprite on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
        if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
        {
          ((EntitySystem) this).Log.Warning($"Enabled all damage tracking with damage overlay groups on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
      }
      else if (!damageVisComp.Overlay)
      {
        if (damageVisComp.TargetLayers == null)
        {
          ((EntitySystem) this).Log.Error($"Disabled overlay without target layers on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
        if (damageVisComp.DamageOverlayGroups != null || damageVisComp.DamageOverlay != null)
        {
          ((EntitySystem) this).Log.Error($"Disabled overlay with defined damage overlay sprites on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
        if (damageVisComp.DamageGroup == null)
        {
          ((EntitySystem) this).Log.Error($"Disabled overlay without defined damage group on {entity}.");
          damageVisComp.Valid = false;
          return;
        }
      }
      if (damageVisComp.DamageOverlayGroups != null && damageVisComp.DamageGroup != null)
        ((EntitySystem) this).Log.Warning($"Damage overlay sprites and damage group are both defined on {entity}.");
      if (damageVisComp.DamageOverlay == null || damageVisComp.DamageGroup == null)
        return;
      ((EntitySystem) this).Log.Warning($"Damage overlay sprites and damage group are both defined on {entity}.");
    }
  }

  private void InitializeVisualizer(EntityUid entity, DamageVisualsComponent damageVisComp)
  {
    SpriteComponent spriteComponent;
    DamageableComponent damageableComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(entity, ref spriteComponent) || !((EntitySystem) this).TryComp<DamageableComponent>(entity, ref damageableComponent) || !((EntitySystem) this).HasComp<AppearanceComponent>(entity))
      return;
    damageVisComp.Thresholds.Add(FixedPoint2.Zero);
    damageVisComp.Thresholds.Sort();
    if (damageVisComp.Thresholds[0] != 0)
    {
      ((EntitySystem) this).Log.Error($"ThresholdsLookup were invalid for entity {entity}. ThresholdsLookup: {damageVisComp.Thresholds}");
      damageVisComp.Valid = false;
    }
    else
    {
      DamageContainerPrototype containerPrototype;
      if (damageableComponent.DamageContainerID.HasValue && this._prototypeManager.TryIndex<DamageContainerPrototype>(damageableComponent.DamageContainerID, ref containerPrototype))
      {
        if (damageVisComp.DamageOverlayGroups != null)
        {
          foreach (string key in damageVisComp.DamageOverlayGroups.Keys)
          {
            if (!containerPrototype.SupportedGroups.Contains(key))
            {
              ((EntitySystem) this).Log.Error($"Damage key {key} was invalid for entity {entity}.");
              damageVisComp.Valid = false;
              return;
            }
            damageVisComp.LastThresholdPerGroup.Add(key, FixedPoint2.Zero);
          }
        }
        else if (!damageVisComp.Overlay && damageVisComp.DamageGroup != null)
        {
          if (!containerPrototype.SupportedGroups.Contains(damageVisComp.DamageGroup))
          {
            ((EntitySystem) this).Log.Error($"Damage keys were invalid for entity {entity}.");
            damageVisComp.Valid = false;
            return;
          }
          damageVisComp.LastThresholdPerGroup.Add(damageVisComp.DamageGroup, FixedPoint2.Zero);
        }
      }
      else
      {
        List<string> list = this._prototypeManager.EnumeratePrototypes<DamageGroupPrototype>().Select<DamageGroupPrototype, string>((Func<DamageGroupPrototype, int, string>) ((p, _) => p.ID)).ToList<string>();
        if (damageVisComp.DamageOverlayGroups != null)
        {
          foreach (string key in damageVisComp.DamageOverlayGroups.Keys)
          {
            if (!list.Contains(key))
            {
              ((EntitySystem) this).Log.Error($"Damage keys were invalid for entity {entity}.");
              damageVisComp.Valid = false;
              return;
            }
            damageVisComp.LastThresholdPerGroup.Add(key, FixedPoint2.Zero);
          }
        }
        else if (damageVisComp.DamageGroup != null)
        {
          if (!list.Contains(damageVisComp.DamageGroup))
          {
            ((EntitySystem) this).Log.Error($"Damage keys were invalid for entity {entity}.");
            damageVisComp.Valid = false;
            return;
          }
          damageVisComp.LastThresholdPerGroup.Add(damageVisComp.DamageGroup, FixedPoint2.Zero);
        }
      }
      List<Enum> targetLayers = damageVisComp.TargetLayers;
      if (targetLayers != null && targetLayers.Count > 0)
      {
        foreach (Enum targetLayer in damageVisComp.TargetLayers)
        {
          int num;
          if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), targetLayer, ref num, false))
            ((EntitySystem) this).Log.Warning($"Layer at key {targetLayer} was invalid for entity {entity}.");
          else
            damageVisComp.TargetLayerMapKeys.Add(targetLayer);
        }
        if (damageVisComp.TargetLayerMapKeys.Count == 0)
        {
          ((EntitySystem) this).Log.Error($"Target layers were invalid for entity {entity}.");
          damageVisComp.Valid = false;
        }
        else
        {
          foreach (Enum targetLayerMapKey in damageVisComp.TargetLayerMapKeys)
          {
            int num1 = spriteComponent.AllLayers.Count<ISpriteLayer>();
            int num2 = this.SpriteSystem.LayerMapGet(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), targetLayerMapKey);
            if (num2 + 1 != num1)
              ++num2;
            damageVisComp.LayerMapKeyStates.Add((object) targetLayerMapKey, targetLayerMapKey.ToString());
            if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
            {
              foreach ((string key, DamageVisualizerSprite sprite) in damageVisComp.DamageOverlayGroups)
                this.AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), sprite, $"{targetLayerMapKey}_{key}_{damageVisComp.Thresholds[1]}", $"{targetLayerMapKey}{key}", new int?(num2));
              damageVisComp.DisabledLayers.Add((object) targetLayerMapKey, false);
            }
            else if (damageVisComp.DamageOverlay != null)
            {
              this.AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), damageVisComp.DamageOverlay, $"{targetLayerMapKey}_{damageVisComp.Thresholds[1]}", $"{targetLayerMapKey}trackDamage", new int?(num2));
              damageVisComp.DisabledLayers.Add((object) targetLayerMapKey, false);
            }
          }
        }
      }
      else if (damageVisComp.DamageOverlayGroups != null)
      {
        foreach ((string key, DamageVisualizerSprite sprite) in damageVisComp.DamageOverlayGroups)
        {
          this.AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), sprite, $"DamageOverlay_{key}_{damageVisComp.Thresholds[1]}", "DamageOverlay" + key);
          damageVisComp.TopMostLayerKey = "DamageOverlay" + key;
        }
      }
      else
      {
        if (damageVisComp.DamageOverlay == null)
          return;
        this.AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, spriteComponent)), damageVisComp.DamageOverlay, $"DamageOverlay_{damageVisComp.Thresholds[1]}", "DamageOverlay");
        damageVisComp.TopMostLayerKey = "DamageOverlay";
      }
    }
  }

  private void AddDamageLayerToSprite(
    Entity<SpriteComponent?> spriteEnt,
    DamageVisualizerSprite sprite,
    string state,
    string mapKey,
    int? index = null)
  {
    int num = this.SpriteSystem.AddLayer(spriteEnt, (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath(sprite.Sprite), state), index);
    this.SpriteSystem.LayerMapSet(spriteEnt, mapKey, num);
    if (sprite.Color != null)
      this.SpriteSystem.LayerSetColor(spriteEnt, num, Color.FromHex((ReadOnlySpan<char>) sprite.Color, new Color?()));
    this.SpriteSystem.LayerSetVisible(spriteEnt, num, false);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    DamageVisualsComponent damageVisComp,
    ref AppearanceChangeEvent args)
  {
    if (!damageVisComp.Valid)
      return;
    bool flag;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) DamageVisualizerKeys.Disabled, ref flag, args.Component))
      damageVisComp.Disabled = flag;
    if (damageVisComp.Disabled)
      return;
    this.HandleDamage(uid, args.Component, damageVisComp);
  }

  private void HandleDamage(
    EntityUid uid,
    AppearanceComponent component,
    DamageVisualsComponent damageVisComp)
  {
    SpriteComponent spriteComponent;
    DamageableComponent damageableComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent) || !((EntitySystem) this).TryComp<DamageableComponent>(uid, ref damageableComponent))
      return;
    if (damageVisComp.TargetLayers != null && damageVisComp.DamageOverlayGroups != null)
      this.UpdateDisabledLayers(uid, spriteComponent, component, damageVisComp);
    if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null && damageVisComp.TargetLayers == null)
      this.CheckOverlayOrdering(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), damageVisComp);
    bool flag;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) DamageVisualizerKeys.ForceUpdate, ref flag, component) & flag)
      this.ForceUpdateLayers(Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit((uid, damageableComponent, spriteComponent, damageVisComp)));
    else if (damageVisComp.TrackAllDamage)
    {
      this.UpdateDamageVisuals(Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit((uid, damageableComponent, spriteComponent, damageVisComp)));
    }
    else
    {
      DamageVisualizerGroupData visualizerGroupData;
      if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<DamageVisualizerGroupData>(uid, (Enum) DamageVisualizerKeys.DamageUpdateGroups, ref visualizerGroupData, component))
        visualizerGroupData = new DamageVisualizerGroupData(((EntitySystem) this).Comp<DamageableComponent>(uid).DamagePerGroup.Keys.ToList<string>());
      this.UpdateDamageVisuals(visualizerGroupData.GroupList, Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit((uid, damageableComponent, spriteComponent, damageVisComp)));
    }
  }

  private void UpdateDisabledLayers(
    EntityUid uid,
    SpriteComponent spriteComponent,
    AppearanceComponent component,
    DamageVisualsComponent damageVisComp)
  {
    foreach (Enum targetLayerMapKey in damageVisComp.TargetLayerMapKeys)
    {
      bool flag;
      ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, targetLayerMapKey, ref flag, component);
      if (damageVisComp.DisabledLayers[(object) targetLayerMapKey] != flag)
      {
        damageVisComp.DisabledLayers[(object) targetLayerMapKey] = flag;
        if (damageVisComp.TrackAllDamage)
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), $"{targetLayerMapKey}trackDamage", (!flag ? 1 : 0) != 0);
        else if (damageVisComp.DamageOverlayGroups != null)
        {
          foreach (string key in damageVisComp.DamageOverlayGroups.Keys)
            this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), $"{targetLayerMapKey}{key}", (!flag ? 1 : 0) != 0);
        }
      }
    }
  }

  private void CheckOverlayOrdering(
    Entity<SpriteComponent> spriteEnt,
    DamageVisualsComponent damageVisComp)
  {
    if (spriteEnt.Comp[(object) damageVisComp.TopMostLayerKey] == spriteEnt.Comp[spriteEnt.Comp.AllLayers.Count<ISpriteLayer>() - 1])
      return;
    if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
    {
      foreach ((string key, DamageVisualizerSprite sprite) in damageVisComp.DamageOverlayGroups)
      {
        FixedPoint2 threshold = damageVisComp.LastThresholdPerGroup[key];
        this.ReorderOverlaySprite(spriteEnt, damageVisComp, sprite, "DamageOverlay" + key, "DamageOverlay_" + key, threshold);
      }
    }
    else
    {
      if (!damageVisComp.TrackAllDamage || damageVisComp.DamageOverlay == null)
        return;
      this.ReorderOverlaySprite(spriteEnt, damageVisComp, damageVisComp.DamageOverlay, "DamageOverlay", "DamageOverlay", damageVisComp.LastDamageThreshold);
    }
  }

  private void ReorderOverlaySprite(
    Entity<SpriteComponent> spriteEnt,
    DamageVisualsComponent damageVisComp,
    DamageVisualizerSprite sprite,
    string key,
    string statePrefix,
    FixedPoint2 threshold)
  {
    int num1;
    this.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), key, ref num1, false);
    bool visible = spriteEnt.Comp[num1].Visible;
    this.SpriteSystem.RemoveLayer(spriteEnt.AsNullable(), num1, true);
    if (threshold == FixedPoint2.Zero)
      threshold = damageVisComp.Thresholds[1];
    int num2 = this.SpriteSystem.AddLayer(spriteEnt.AsNullable(), (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath(sprite.Sprite), $"{statePrefix}_{threshold}"), new int?(num1));
    this.SpriteSystem.LayerMapSet(spriteEnt.AsNullable(), key, num2);
    this.SpriteSystem.LayerSetVisible(spriteEnt.AsNullable(), num2, visible);
    damageVisComp.TopMostLayerKey = key;
  }

  private void UpdateDamageVisuals(
    Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent> entity)
  {
    DamageableComponent comp1 = entity.Comp1;
    SpriteComponent comp2 = entity.Comp2;
    DamageVisualsComponent comp3 = entity.Comp3;
    FixedPoint2 threshold;
    if (!this.CheckThresholdBoundary(comp1.TotalDamage, comp3.LastDamageThreshold, comp3, out threshold))
      return;
    comp3.LastDamageThreshold = threshold;
    if (comp3.TargetLayers != null)
    {
      foreach (Enum targetLayerMapKey in comp3.TargetLayerMapKeys)
        this.UpdateTargetLayer(Entity<SpriteComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2)), comp3, (object) targetLayerMapKey, threshold);
    }
    else
      this.UpdateOverlay(Entity<SpriteComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2)), threshold);
  }

  private void UpdateDamageVisuals(
    List<string> delta,
    Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent> entity)
  {
    DamageableComponent comp1 = entity.Comp1;
    SpriteComponent comp2 = entity.Comp2;
    DamageVisualsComponent comp3 = entity.Comp3;
    foreach (string str in delta)
    {
      DamageGroupPrototype group;
      FixedPoint2 total;
      FixedPoint2 lastThreshold;
      FixedPoint2 threshold;
      if ((comp3.Overlay || !(str != comp3.DamageGroup)) && this._prototypeManager.TryIndex<DamageGroupPrototype>(str, ref group) && comp1.Damage.TryGetDamageInGroup(group, out total) && comp3.LastThresholdPerGroup.TryGetValue(str, out lastThreshold) && this.CheckThresholdBoundary(total, lastThreshold, comp3, out threshold))
      {
        comp3.LastThresholdPerGroup[str] = threshold;
        if (comp3.TargetLayers != null)
        {
          foreach (Enum targetLayerMapKey in comp3.TargetLayerMapKeys)
            this.UpdateTargetLayer(Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2, comp3)), (object) targetLayerMapKey, str, threshold);
        }
        else
          this.UpdateOverlay(Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2, comp3)), str, threshold);
      }
    }
  }

  private bool CheckThresholdBoundary(
    FixedPoint2 damageTotal,
    FixedPoint2 lastThreshold,
    DamageVisualsComponent damageVisComp,
    out FixedPoint2 threshold)
  {
    threshold = FixedPoint2.Zero;
    damageTotal /= damageVisComp.Divisor;
    int index = damageVisComp.Thresholds.BinarySearch(damageTotal);
    if (index < 0)
    {
      int num = ~index;
      threshold = damageVisComp.Thresholds[num - 1];
    }
    else
      threshold = damageVisComp.Thresholds[index];
    return !(threshold == lastThreshold);
  }

  private void ForceUpdateLayers(
    Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent> entity)
  {
    DamageVisualsComponent comp3 = entity.Comp3;
    if (comp3.DamageOverlayGroups != null)
      this.UpdateDamageVisuals(comp3.DamageOverlayGroups.Keys.ToList<string>(), entity);
    else if (comp3.DamageGroup != null)
    {
      this.UpdateDamageVisuals(new List<string>()
      {
        comp3.DamageGroup
      }, entity);
    }
    else
    {
      if (comp3.DamageOverlay == null)
        return;
      this.UpdateDamageVisuals(entity);
    }
  }

  private void UpdateTargetLayer(
    Entity<SpriteComponent> spriteEnt,
    DamageVisualsComponent damageVisComp,
    object layerMapKey,
    FixedPoint2 threshold)
  {
    if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
    {
      if (damageVisComp.DisabledLayers[layerMapKey])
        return;
      string layerMapKeyState = damageVisComp.LayerMapKeyStates[layerMapKey];
      int spriteLayer;
      this.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), $"{layerMapKey}trackDamage", ref spriteLayer, false);
      this.UpdateDamageLayerState(spriteEnt, spriteLayer, layerMapKeyState ?? "", threshold);
    }
    else
    {
      if (damageVisComp.Overlay)
        return;
      string layerMapKeyState = damageVisComp.LayerMapKeyStates[layerMapKey];
      int spriteLayer;
      this.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), $"{layerMapKey}", ref spriteLayer, false);
      this.UpdateDamageLayerState(spriteEnt, spriteLayer, layerMapKeyState ?? "", threshold);
    }
  }

  private void UpdateTargetLayer(
    Entity<SpriteComponent, DamageVisualsComponent> entity,
    object layerMapKey,
    string damageGroup,
    FixedPoint2 threshold)
  {
    SpriteComponent comp1 = entity.Comp1;
    DamageVisualsComponent comp2 = entity.Comp2;
    if (comp2.Overlay && comp2.DamageOverlayGroups != null)
    {
      if (!comp2.DamageOverlayGroups.ContainsKey(damageGroup) || comp2.DisabledLayers[layerMapKey])
        return;
      string layerMapKeyState = comp2.LayerMapKeyStates[layerMapKey];
      int spriteLayer;
      this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp1)), $"{layerMapKey}{damageGroup}", ref spriteLayer, false);
      this.UpdateDamageLayerState(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp1)), spriteLayer, $"{layerMapKeyState}_{damageGroup}", threshold);
    }
    else
    {
      if (comp2.Overlay)
        return;
      string layerMapKeyState = comp2.LayerMapKeyStates[layerMapKey];
      int spriteLayer;
      this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp1)), $"{layerMapKey}", ref spriteLayer, false);
      this.UpdateDamageLayerState(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp1)), spriteLayer, $"{layerMapKeyState}_{damageGroup}", threshold);
    }
  }

  private void UpdateOverlay(Entity<SpriteComponent> spriteEnt, FixedPoint2 threshold)
  {
    int spriteLayer;
    this.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), "DamageOverlay", ref spriteLayer, false);
    this.UpdateDamageLayerState(spriteEnt, spriteLayer, "DamageOverlay", threshold);
  }

  private void UpdateOverlay(
    Entity<SpriteComponent, DamageVisualsComponent> entity,
    string damageGroup,
    FixedPoint2 threshold)
  {
    SpriteComponent comp1 = entity.Comp1;
    DamageVisualsComponent comp2 = entity.Comp2;
    if (comp2.DamageOverlayGroups == null || !comp2.DamageOverlayGroups.ContainsKey(damageGroup))
      return;
    int spriteLayer;
    this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp1)), "DamageOverlay" + damageGroup, ref spriteLayer, false);
    this.UpdateDamageLayerState(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp1)), spriteLayer, "DamageOverlay_" + damageGroup, threshold);
  }

  private void UpdateDamageLayerState(
    Entity<SpriteComponent> spriteEnt,
    int spriteLayer,
    string statePrefix,
    FixedPoint2 threshold)
  {
    if (threshold == 0)
    {
      this.SpriteSystem.LayerSetVisible(spriteEnt.AsNullable(), spriteLayer, false);
    }
    else
    {
      if (!spriteEnt.Comp[spriteLayer].Visible)
        this.SpriteSystem.LayerSetVisible(spriteEnt.AsNullable(), spriteLayer, true);
      this.SpriteSystem.LayerSetRsiState(spriteEnt.AsNullable(), spriteLayer, RSI.StateId.op_Implicit($"{statePrefix}_{threshold}"));
    }
  }

  public void ChangeDamageGroupColor(
    Entity<SpriteComponent> spriteEnt,
    DamageVisualsComponent damageVisuals,
    string group,
    string color)
  {
    if (damageVisuals.TargetLayers == null || damageVisuals.DamageOverlayGroups == null)
      return;
    foreach (Enum targetLayerMapKey in damageVisuals.TargetLayerMapKeys)
    {
      int num;
      if (this.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), $"{targetLayerMapKey}{group}", ref num, false))
        this.SpriteSystem.LayerSetColor(spriteEnt.AsNullable(), num, Color.FromHex((ReadOnlySpan<char>) color, new Color?()));
    }
    damageVisuals.DamageOverlayGroups[group].Color = color;
  }
}
