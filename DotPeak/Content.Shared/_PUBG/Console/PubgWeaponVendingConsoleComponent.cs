// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Console.PubgWeaponVendingConsoleComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Console;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class PubgWeaponVendingConsoleComponent : 
  Component,
  ISerializationGenerated<PubgWeaponVendingConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public ProtoId<PubgWeaponVendingInventoryPrototype> InventoryPrototype;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DispenseDelay = TimeSpan.FromSeconds(1.5);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan NextDispenseTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgWeaponVendingConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgWeaponVendingConsoleComponent) target1;
    if (serialization.TryCustomCopy<PubgWeaponVendingConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<PubgWeaponVendingInventoryPrototype> target2 = new ProtoId<PubgWeaponVendingInventoryPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<PubgWeaponVendingInventoryPrototype>>(this.InventoryPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<PubgWeaponVendingInventoryPrototype>>(this.InventoryPrototype, hookCtx, context);
    target.InventoryPrototype = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DispenseDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DispenseDelay, hookCtx, context);
    target.DispenseDelay = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgWeaponVendingConsoleComponent target,
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
    PubgWeaponVendingConsoleComponent target1 = (PubgWeaponVendingConsoleComponent) target;
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
    PubgWeaponVendingConsoleComponent target1 = (PubgWeaponVendingConsoleComponent) target;
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
    PubgWeaponVendingConsoleComponent target1 = (PubgWeaponVendingConsoleComponent) target;
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
  virtual PubgWeaponVendingConsoleComponent Component.Instantiate()
  {
    return new PubgWeaponVendingConsoleComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgWeaponVendingConsoleComponent_AutoState : IComponentState
  {
    public ProtoId<PubgWeaponVendingInventoryPrototype> InventoryPrototype;
    public TimeSpan NextDispenseTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgWeaponVendingConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgWeaponVendingConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<PubgWeaponVendingConsoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgWeaponVendingConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgWeaponVendingConsoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgWeaponVendingConsoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgWeaponVendingConsoleComponent.PubgWeaponVendingConsoleComponent_AutoState()
      {
        InventoryPrototype = component.InventoryPrototype,
        NextDispenseTime = component.NextDispenseTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgWeaponVendingConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgWeaponVendingConsoleComponent.PubgWeaponVendingConsoleComponent_AutoState current))
        return;
      component.InventoryPrototype = current.InventoryPrototype;
      component.NextDispenseTime = current.NextDispenseTime;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, PubgWeaponVendingConsoleComponent>(uid, component, ref args1);
    }
  }
}
