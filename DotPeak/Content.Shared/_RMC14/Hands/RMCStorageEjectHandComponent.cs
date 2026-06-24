// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Hands.RMCStorageEjectHandComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Hands;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCHandsSystem)})]
public sealed class RMCStorageEjectHandComponent : 
  Component,
  ISerializationGenerated<RMCStorageEjectHandComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCStorageEjectState State = RMCStorageEjectState.Unequip;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanToggleStorage = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EjectWhenEmpty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? NestedWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCStorageEjectHandComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCStorageEjectHandComponent) target1;
    if (serialization.TryCustomCopy<RMCStorageEjectHandComponent>(this, ref target, hookCtx, false, context))
      return;
    RMCStorageEjectState target2 = RMCStorageEjectState.Last;
    if (!serialization.TryCustomCopy<RMCStorageEjectState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanToggleStorage, ref target3, hookCtx, false, context))
      target3 = this.CanToggleStorage;
    target.CanToggleStorage = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, context);
    }
    target.Whitelist = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.EjectWhenEmpty, ref target5, hookCtx, false, context))
      target5 = this.EjectWhenEmpty;
    target.EjectWhenEmpty = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.NestedWhitelist, ref target6, hookCtx, false, context))
    {
      if (this.NestedWhitelist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.NestedWhitelist, ref target6, hookCtx, context);
    }
    target.NestedWhitelist = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCStorageEjectHandComponent target,
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
    RMCStorageEjectHandComponent target1 = (RMCStorageEjectHandComponent) target;
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
    RMCStorageEjectHandComponent target1 = (RMCStorageEjectHandComponent) target;
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
    RMCStorageEjectHandComponent target1 = (RMCStorageEjectHandComponent) target;
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
  virtual RMCStorageEjectHandComponent Component.Instantiate()
  {
    return new RMCStorageEjectHandComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCStorageEjectHandComponent_AutoState : IComponentState
  {
    public RMCStorageEjectState State;
    public bool CanToggleStorage;
    public EntityWhitelist? Whitelist;
    public bool EjectWhenEmpty;
    public EntityWhitelist? NestedWhitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCStorageEjectHandComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCStorageEjectHandComponent, ComponentGetState>(new ComponentEventRefHandler<RMCStorageEjectHandComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCStorageEjectHandComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCStorageEjectHandComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCStorageEjectHandComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCStorageEjectHandComponent.RMCStorageEjectHandComponent_AutoState()
      {
        State = component.State,
        CanToggleStorage = component.CanToggleStorage,
        Whitelist = component.Whitelist,
        EjectWhenEmpty = component.EjectWhenEmpty,
        NestedWhitelist = component.NestedWhitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCStorageEjectHandComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCStorageEjectHandComponent.RMCStorageEjectHandComponent_AutoState current))
        return;
      component.State = current.State;
      component.CanToggleStorage = current.CanToggleStorage;
      component.Whitelist = current.Whitelist;
      component.EjectWhenEmpty = current.EjectWhenEmpty;
      component.NestedWhitelist = current.NestedWhitelist;
    }
  }
}
