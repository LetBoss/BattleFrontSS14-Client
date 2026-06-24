// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Components.EntityTargetActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedActionsSystem)})]
[EntityCategory(new string[] {"Actions"})]
[AutoGenerateComponentState(false, false)]
public sealed class EntityTargetActionComponent : 
  Component,
  ISerializationGenerated<EntityTargetActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [NonSerialized]
  public EntityTargetActionEvent? Event;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanTargetSelf = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RotateOnUse = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TargetCheckCanInteract = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ToggleOutline = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityTargetActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (EntityTargetActionComponent) component;
    if (serialization.TryCustomCopy<EntityTargetActionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityTargetActionEvent targetActionEvent = (EntityTargetActionEvent) null;
    if (!serialization.TryCustomCopy<EntityTargetActionEvent>(this.Event, ref targetActionEvent, hookCtx, true, context))
      targetActionEvent = serialization.CreateCopy<EntityTargetActionEvent>(this.Event, hookCtx, context, false);
    target.Event = targetActionEvent;
    EntityWhitelist entityWhitelist1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, context, false);
    }
    target.Whitelist = entityWhitelist1;
    EntityWhitelist entityWhitelist2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        entityWhitelist2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, context, false);
    }
    target.Blacklist = entityWhitelist2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanTargetSelf, ref flag1, hookCtx, false, context))
      flag1 = this.CanTargetSelf;
    target.CanTargetSelf = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RotateOnUse, ref flag2, hookCtx, false, context))
      flag2 = this.RotateOnUse;
    target.RotateOnUse = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.TargetCheckCanInteract, ref flag3, hookCtx, false, context))
      flag3 = this.TargetCheckCanInteract;
    target.TargetCheckCanInteract = flag3;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ToggleOutline, ref flag4, hookCtx, false, context))
      flag4 = this.ToggleOutline;
    target.ToggleOutline = flag4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityTargetActionComponent target,
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
    EntityTargetActionComponent target1 = (EntityTargetActionComponent) target;
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
    EntityTargetActionComponent target1 = (EntityTargetActionComponent) target;
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
    EntityTargetActionComponent target1 = (EntityTargetActionComponent) target;
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
  virtual EntityTargetActionComponent Component.Instantiate() => new EntityTargetActionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EntityTargetActionComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
    public EntityWhitelist? Blacklist;
    public bool CanTargetSelf;
    public bool RotateOnUse;
    public bool TargetCheckCanInteract;
    public bool ToggleOutline;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EntityTargetActionComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<EntityTargetActionComponent, ComponentGetState>(new ComponentEventRefHandler<EntityTargetActionComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<EntityTargetActionComponent, ComponentHandleState>(new ComponentEventRefHandler<EntityTargetActionComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      EntityTargetActionComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new EntityTargetActionComponent.EntityTargetActionComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        Blacklist = component.Blacklist,
        CanTargetSelf = component.CanTargetSelf,
        RotateOnUse = component.RotateOnUse,
        TargetCheckCanInteract = component.TargetCheckCanInteract,
        ToggleOutline = component.ToggleOutline
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EntityTargetActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is EntityTargetActionComponent.EntityTargetActionComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.Blacklist = current.Blacklist;
      component.CanTargetSelf = current.CanTargetSelf;
      component.RotateOnUse = current.RotateOnUse;
      component.TargetCheckCanInteract = current.TargetCheckCanInteract;
      component.ToggleOutline = current.ToggleOutline;
    }
  }
}
