// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.StoreOnCollideComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.EntitySystems;
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
namespace Content.Shared.Storage.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (StoreOnCollideSystem)})]
public sealed class StoreOnCollideComponent : 
  Component,
  ISerializationGenerated<StoreOnCollideComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public bool LockOnCollide;
  [DataField(null, false, 1, false, false, null)]
  public bool DisableWhenFirstOpened;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Disabled;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StoreOnCollideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StoreOnCollideComponent) target1;
    if (serialization.TryCustomCopy<StoreOnCollideComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.LockOnCollide, ref target3, hookCtx, false, context))
      target3 = this.LockOnCollide;
    target.LockOnCollide = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableWhenFirstOpened, ref target4, hookCtx, false, context))
      target4 = this.DisableWhenFirstOpened;
    target.DisableWhenFirstOpened = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Disabled, ref target5, hookCtx, false, context))
      target5 = this.Disabled;
    target.Disabled = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StoreOnCollideComponent target,
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
    StoreOnCollideComponent target1 = (StoreOnCollideComponent) target;
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
    StoreOnCollideComponent target1 = (StoreOnCollideComponent) target;
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
    StoreOnCollideComponent target1 = (StoreOnCollideComponent) target;
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
  virtual StoreOnCollideComponent Component.Instantiate() => new StoreOnCollideComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StoreOnCollideComponent_AutoState : IComponentState
  {
    public bool Disabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StoreOnCollideComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StoreOnCollideComponent, ComponentGetState>(new ComponentEventRefHandler<StoreOnCollideComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StoreOnCollideComponent, ComponentHandleState>(new ComponentEventRefHandler<StoreOnCollideComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StoreOnCollideComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StoreOnCollideComponent.StoreOnCollideComponent_AutoState()
      {
        Disabled = component.Disabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StoreOnCollideComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StoreOnCollideComponent.StoreOnCollideComponent_AutoState current))
        return;
      component.Disabled = current.Disabled;
    }
  }
}
