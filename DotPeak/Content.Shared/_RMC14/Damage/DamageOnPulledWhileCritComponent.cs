// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.DamageOnPulledWhileCritComponent
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
public sealed class DamageOnPulledWhileCritComponent : 
  Component,
  ISerializationGenerated<DamageOnPulledWhileCritComponent>,
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
  public double Every = 0.1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOnPulledWhileCritComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageOnPulledWhileCritComponent) target1;
    if (serialization.TryCustomCopy<DamageOnPulledWhileCritComponent>(this, ref target, hookCtx, false, context))
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
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOnPulledWhileCritComponent target,
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
    DamageOnPulledWhileCritComponent target1 = (DamageOnPulledWhileCritComponent) target;
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
    DamageOnPulledWhileCritComponent target1 = (DamageOnPulledWhileCritComponent) target;
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
    DamageOnPulledWhileCritComponent target1 = (DamageOnPulledWhileCritComponent) target;
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
  virtual DamageOnPulledWhileCritComponent Component.Instantiate()
  {
    return new DamageOnPulledWhileCritComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageOnPulledWhileCritComponent_AutoState : IComponentState
  {
    public DamageSpecifier? Damage;
    public EntityWhitelist? PullerWhitelist;
    public double Every;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageOnPulledWhileCritComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageOnPulledWhileCritComponent, ComponentGetState>(new ComponentEventRefHandler<DamageOnPulledWhileCritComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageOnPulledWhileCritComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageOnPulledWhileCritComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageOnPulledWhileCritComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageOnPulledWhileCritComponent.DamageOnPulledWhileCritComponent_AutoState()
      {
        Damage = component.Damage,
        PullerWhitelist = component.PullerWhitelist,
        Every = component.Every
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageOnPulledWhileCritComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageOnPulledWhileCritComponent.DamageOnPulledWhileCritComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.PullerWhitelist = current.PullerWhitelist;
      component.Every = current.Every;
    }
  }
}
