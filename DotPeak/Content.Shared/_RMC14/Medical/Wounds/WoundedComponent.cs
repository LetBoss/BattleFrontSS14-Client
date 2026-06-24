// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Wounds.WoundedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Wounds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedWoundsSystem)})]
public sealed class WoundedComponent : 
  Component,
  ISerializationGenerated<WoundedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> BruteWoundGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> BurnWoundGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<Wound> Wounds = new List<Wound>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PassiveHealing = FixedPoint2.New(-0.05f);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan UpdateAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateCooldown = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WoundedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WoundedComponent) target1;
    if (serialization.TryCustomCopy<WoundedComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<DamageGroupPrototype> target2 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.BruteWoundGroup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.BruteWoundGroup, hookCtx, context);
    target.BruteWoundGroup = target2;
    ProtoId<DamageGroupPrototype> target3 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.BurnWoundGroup, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.BurnWoundGroup, hookCtx, context);
    target.BurnWoundGroup = target3;
    List<Wound> target4 = (List<Wound>) null;
    if (this.Wounds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Wound>>(this.Wounds, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<Wound>>(this.Wounds, hookCtx, context);
    target.Wounds = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PassiveHealing, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.PassiveHealing, hookCtx, context);
    target.PassiveHealing = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.UpdateAt, hookCtx, context);
    target.UpdateAt = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.UpdateCooldown, hookCtx, context);
    target.UpdateCooldown = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WoundedComponent target,
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
    WoundedComponent target1 = (WoundedComponent) target;
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
    WoundedComponent target1 = (WoundedComponent) target;
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
    WoundedComponent target1 = (WoundedComponent) target;
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
  virtual WoundedComponent Component.Instantiate() => new WoundedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WoundedComponent_AutoState : IComponentState
  {
    public ProtoId<DamageGroupPrototype> BruteWoundGroup;
    public ProtoId<DamageGroupPrototype> BurnWoundGroup;
    public List<Wound> Wounds;
    public FixedPoint2 PassiveHealing;
    public TimeSpan UpdateAt;
    public TimeSpan UpdateCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WoundedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WoundedComponent, ComponentGetState>(new ComponentEventRefHandler<WoundedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WoundedComponent, ComponentHandleState>(new ComponentEventRefHandler<WoundedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, WoundedComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new WoundedComponent.WoundedComponent_AutoState()
      {
        BruteWoundGroup = component.BruteWoundGroup,
        BurnWoundGroup = component.BurnWoundGroup,
        Wounds = component.Wounds,
        PassiveHealing = component.PassiveHealing,
        UpdateAt = component.UpdateAt,
        UpdateCooldown = component.UpdateCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WoundedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WoundedComponent.WoundedComponent_AutoState current))
        return;
      component.BruteWoundGroup = current.BruteWoundGroup;
      component.BurnWoundGroup = current.BurnWoundGroup;
      component.Wounds = current.Wounds == null ? (List<Wound>) null : new List<Wound>((IEnumerable<Wound>) current.Wounds);
      component.PassiveHealing = current.PassiveHealing;
      component.UpdateAt = current.UpdateAt;
      component.UpdateCooldown = current.UpdateCooldown;
    }
  }
}
