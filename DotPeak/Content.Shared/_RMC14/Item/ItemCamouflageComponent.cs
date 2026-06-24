// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Item.ItemCamouflageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Item;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ItemCamouflageComponent : 
  Component,
  ISerializationGenerated<ItemCamouflageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<CamouflageType, ResPath>? CamouflageVariations;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<CamouflageType, string>? States;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, Dictionary<CamouflageType, string>>? Layers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<CamouflageType, Color>? Colors;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemCamouflageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemCamouflageComponent) target1;
    if (serialization.TryCustomCopy<ItemCamouflageComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<CamouflageType, ResPath> target2 = (Dictionary<CamouflageType, ResPath>) null;
    if (!serialization.TryCustomCopy<Dictionary<CamouflageType, ResPath>>(this.CamouflageVariations, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<CamouflageType, ResPath>>(this.CamouflageVariations, hookCtx, context);
    target.CamouflageVariations = target2;
    Dictionary<CamouflageType, string> target3 = (Dictionary<CamouflageType, string>) null;
    if (!serialization.TryCustomCopy<Dictionary<CamouflageType, string>>(this.States, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<CamouflageType, string>>(this.States, hookCtx, context);
    target.States = target3;
    Dictionary<string, Dictionary<CamouflageType, string>> target4 = (Dictionary<string, Dictionary<CamouflageType, string>>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<CamouflageType, string>>>(this.Layers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<string, Dictionary<CamouflageType, string>>>(this.Layers, hookCtx, context);
    target.Layers = target4;
    Dictionary<CamouflageType, Color> target5 = (Dictionary<CamouflageType, Color>) null;
    if (!serialization.TryCustomCopy<Dictionary<CamouflageType, Color>>(this.Colors, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<CamouflageType, Color>>(this.Colors, hookCtx, context);
    target.Colors = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemCamouflageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ItemCamouflageComponent target1 = (ItemCamouflageComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ItemCamouflageComponent target1 = (ItemCamouflageComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ItemCamouflageComponent target1 = (ItemCamouflageComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ItemCamouflageComponent Component.Instantiate() => new ItemCamouflageComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemCamouflageComponent_AutoState : IComponentState
  {
    public Dictionary<CamouflageType, ResPath>? CamouflageVariations;
    public Dictionary<CamouflageType, string>? States;
    public Dictionary<string, Dictionary<CamouflageType, string>>? Layers;
    public Dictionary<CamouflageType, Color>? Colors;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemCamouflageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemCamouflageComponent, ComponentGetState>(new ComponentEventRefHandler<ItemCamouflageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemCamouflageComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemCamouflageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemCamouflageComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemCamouflageComponent.ItemCamouflageComponent_AutoState()
      {
        CamouflageVariations = component.CamouflageVariations,
        States = component.States,
        Layers = component.Layers,
        Colors = component.Colors
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemCamouflageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemCamouflageComponent.ItemCamouflageComponent_AutoState current))
        return;
      component.CamouflageVariations = current.CamouflageVariations == null ? (Dictionary<CamouflageType, ResPath>) null : new Dictionary<CamouflageType, ResPath>((IDictionary<CamouflageType, ResPath>) current.CamouflageVariations);
      component.States = current.States == null ? (Dictionary<CamouflageType, string>) null : new Dictionary<CamouflageType, string>((IDictionary<CamouflageType, string>) current.States);
      component.Layers = current.Layers == null ? (Dictionary<string, Dictionary<CamouflageType, string>>) null : new Dictionary<string, Dictionary<CamouflageType, string>>((IDictionary<string, Dictionary<CamouflageType, string>>) current.Layers);
      component.Colors = current.Colors == null ? (Dictionary<CamouflageType, Color>) null : new Dictionary<CamouflageType, Color>((IDictionary<CamouflageType, Color>) current.Colors);
    }
  }
}
