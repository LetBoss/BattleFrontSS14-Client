// Decompiled with JetBrains decompiler
// Type: Content.Client.Damage.DamageVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Damage;

[RegisterComponent]
public sealed class DamageVisualsComponent : 
  Component,
  ISerializationGenerated<DamageVisualsComponent>,
  ISerializationGenerated
{
  [DataField("thresholds", false, 1, true, false, null)]
  public List<FixedPoint2> Thresholds = new List<FixedPoint2>();
  [DataField("targetLayers", false, 1, false, false, null)]
  public List<Enum>? TargetLayers;
  [DataField("damageOverlayGroups", false, 1, false, false, null)]
  public Dictionary<string, DamageVisualizerSprite>? DamageOverlayGroups;
  [DataField("overlay", false, 1, false, false, null)]
  public bool Overlay = true;
  [DataField("damageGroup", false, 1, false, false, null)]
  public string? DamageGroup;
  [DataField("damageDivisor", false, 1, false, false, null)]
  public float Divisor = 1f;
  [DataField("trackAllDamage", false, 1, false, false, null)]
  public bool TrackAllDamage;
  [DataField("damageOverlay", false, 1, false, false, null)]
  public DamageVisualizerSprite? DamageOverlay;
  public readonly List<Enum> TargetLayerMapKeys = new List<Enum>();
  public bool Disabled;
  public bool Valid = true;
  public FixedPoint2 LastDamageThreshold = FixedPoint2.Zero;
  public readonly Dictionary<object, bool> DisabledLayers = new Dictionary<object, bool>();
  public readonly Dictionary<object, string> LayerMapKeyStates = new Dictionary<object, string>();
  public readonly Dictionary<string, FixedPoint2> LastThresholdPerGroup = new Dictionary<string, FixedPoint2>();
  public string TopMostLayerKey;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageVisualsComponent) component;
    if (serialization.TryCustomCopy<DamageVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    List<FixedPoint2> fixedPoint2List = (List<FixedPoint2>) null;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<FixedPoint2>>(this.Thresholds, ref fixedPoint2List, hookCtx, true, context))
      fixedPoint2List = serialization.CreateCopy<List<FixedPoint2>>(this.Thresholds, hookCtx, context, false);
    target.Thresholds = fixedPoint2List;
    List<Enum> enumList = (List<Enum>) null;
    if (!serialization.TryCustomCopy<List<Enum>>(this.TargetLayers, ref enumList, hookCtx, true, context))
      enumList = serialization.CreateCopy<List<Enum>>(this.TargetLayers, hookCtx, context, false);
    target.TargetLayers = enumList;
    Dictionary<string, DamageVisualizerSprite> dictionary = (Dictionary<string, DamageVisualizerSprite>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, DamageVisualizerSprite>>(this.DamageOverlayGroups, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, DamageVisualizerSprite>>(this.DamageOverlayGroups, hookCtx, context, false);
    target.DamageOverlayGroups = dictionary;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Overlay, ref flag1, hookCtx, false, context))
      flag1 = this.Overlay;
    target.Overlay = flag1;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DamageGroup, ref str, hookCtx, false, context))
      str = this.DamageGroup;
    target.DamageGroup = str;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Divisor, ref num, hookCtx, false, context))
      num = this.Divisor;
    target.Divisor = num;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.TrackAllDamage, ref flag2, hookCtx, false, context))
      flag2 = this.TrackAllDamage;
    target.TrackAllDamage = flag2;
    DamageVisualizerSprite visualizerSprite = (DamageVisualizerSprite) null;
    if (!serialization.TryCustomCopy<DamageVisualizerSprite>(this.DamageOverlay, ref visualizerSprite, hookCtx, false, context))
    {
      if (this.DamageOverlay == null)
        visualizerSprite = (DamageVisualizerSprite) null;
      else
        serialization.CopyTo<DamageVisualizerSprite>(this.DamageOverlay, ref visualizerSprite, hookCtx, context, false);
    }
    target.DamageOverlay = visualizerSprite;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageVisualsComponent target1 = (DamageVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageVisualsComponent target1 = (DamageVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageVisualsComponent target1 = (DamageVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DamageVisualsComponent Component.Instantiate() => new DamageVisualsComponent();
}
