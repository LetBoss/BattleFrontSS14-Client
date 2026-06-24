// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.ActiveDamageOnPulledWhileCritComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
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
namespace Content.Shared._RMC14.Damage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCDamageableSystem)})]
public sealed class ActiveDamageOnPulledWhileCritComponent : 
  Component,
  ISerializationGenerated<ActiveDamageOnPulledWhileCritComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? PullerWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double Every;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Pulled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double Accumulator;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveDamageOnPulledWhileCritComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveDamageOnPulledWhileCritComponent) target1;
    if (serialization.TryCustomCopy<ActiveDamageOnPulledWhileCritComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context);
    }
    target.Damage = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.PullerWhitelist, ref target3, hookCtx, false, context))
    {
      if (this.PullerWhitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.PullerWhitelist, ref target3, hookCtx, context);
    }
    target.PullerWhitelist = target3;
    double target4 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Every, ref target4, hookCtx, false, context))
      target4 = this.Every;
    target.Every = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Pulled, ref target5, hookCtx, false, context))
      target5 = this.Pulled;
    target.Pulled = target5;
    double target6 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Accumulator, ref target6, hookCtx, false, context))
      target6 = this.Accumulator;
    target.Accumulator = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveDamageOnPulledWhileCritComponent target,
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
    ActiveDamageOnPulledWhileCritComponent target1 = (ActiveDamageOnPulledWhileCritComponent) target;
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
    ActiveDamageOnPulledWhileCritComponent target1 = (ActiveDamageOnPulledWhileCritComponent) target;
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
    ActiveDamageOnPulledWhileCritComponent target1 = (ActiveDamageOnPulledWhileCritComponent) target;
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
  virtual ActiveDamageOnPulledWhileCritComponent Component.Instantiate()
  {
    return new ActiveDamageOnPulledWhileCritComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ActiveDamageOnPulledWhileCritComponent_AutoState : IComponentState
  {
    public DamageSpecifier? Damage;
    public EntityWhitelist? PullerWhitelist;
    public double Every;
    public bool Pulled;
    public double Accumulator;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActiveDamageOnPulledWhileCritComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, ComponentGetState>(new ComponentEventRefHandler<ActiveDamageOnPulledWhileCritComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, ComponentHandleState>(new ComponentEventRefHandler<ActiveDamageOnPulledWhileCritComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ActiveDamageOnPulledWhileCritComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ActiveDamageOnPulledWhileCritComponent.ActiveDamageOnPulledWhileCritComponent_AutoState()
      {
        Damage = component.Damage,
        PullerWhitelist = component.PullerWhitelist,
        Every = component.Every,
        Pulled = component.Pulled,
        Accumulator = component.Accumulator
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActiveDamageOnPulledWhileCritComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ActiveDamageOnPulledWhileCritComponent.ActiveDamageOnPulledWhileCritComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.PullerWhitelist = current.PullerWhitelist;
      component.Every = current.Every;
      component.Pulled = current.Pulled;
      component.Accumulator = current.Accumulator;
    }
  }
}
