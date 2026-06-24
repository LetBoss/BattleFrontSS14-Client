// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.UniformAccessories.UniformAccessoryHolderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.UniformAccessories;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedUniformAccessorySystem)})]
public sealed class UniformAccessoryHolderComponent : 
  Component,
  ISerializationGenerated<UniformAccessoryHolderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "rmc_uniform_accessories";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> AllowedCategories;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId>? StartingAccessories;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HideAccessories;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UniformAccessoryHolderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UniformAccessoryHolderComponent) target1;
    if (serialization.TryCustomCopy<UniformAccessoryHolderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    List<string> target3 = (List<string>) null;
    if (this.AllowedCategories == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.AllowedCategories, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<string>>(this.AllowedCategories, hookCtx, context);
    target.AllowedCategories = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.StartingAccessories, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.StartingAccessories, hookCtx, context);
    target.StartingAccessories = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.HideAccessories, ref target5, hookCtx, false, context))
      target5 = this.HideAccessories;
    target.HideAccessories = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UniformAccessoryHolderComponent target,
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
    UniformAccessoryHolderComponent target1 = (UniformAccessoryHolderComponent) target;
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
    UniformAccessoryHolderComponent target1 = (UniformAccessoryHolderComponent) target;
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
    UniformAccessoryHolderComponent target1 = (UniformAccessoryHolderComponent) target;
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
  virtual UniformAccessoryHolderComponent Component.Instantiate()
  {
    return new UniformAccessoryHolderComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UniformAccessoryHolderComponent_AutoState : IComponentState
  {
    public string ContainerId;
    public List<string> AllowedCategories;
    public List<EntProtoId>? StartingAccessories;
    public bool HideAccessories;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UniformAccessoryHolderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UniformAccessoryHolderComponent, ComponentGetState>(new ComponentEventRefHandler<UniformAccessoryHolderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UniformAccessoryHolderComponent, ComponentHandleState>(new ComponentEventRefHandler<UniformAccessoryHolderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UniformAccessoryHolderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UniformAccessoryHolderComponent.UniformAccessoryHolderComponent_AutoState()
      {
        ContainerId = component.ContainerId,
        AllowedCategories = component.AllowedCategories,
        StartingAccessories = component.StartingAccessories,
        HideAccessories = component.HideAccessories
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UniformAccessoryHolderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UniformAccessoryHolderComponent.UniformAccessoryHolderComponent_AutoState current))
        return;
      component.ContainerId = current.ContainerId;
      component.AllowedCategories = current.AllowedCategories == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.AllowedCategories);
      component.StartingAccessories = current.StartingAccessories == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.StartingAccessories);
      component.HideAccessories = current.HideAccessories;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, UniformAccessoryHolderComponent>(uid, component, ref args1);
    }
  }
}
