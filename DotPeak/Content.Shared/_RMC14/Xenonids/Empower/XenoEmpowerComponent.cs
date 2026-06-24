// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Empower.XenoEmpowerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Empower;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoEmpowerComponent : 
  Component,
  ISerializationGenerated<XenoEmpowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxTargets = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SuperThreshold = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Cost = (FixedPoint2) 50;
  [DataField(null, false, 1, false, false, null)]
  public float Range = 5.078f;
  [DataField(null, false, 1, false, false, null)]
  public int InitialShield = 50;
  [DataField(null, false, 1, false, false, null)]
  public int ShieldPerTarget = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ActivatedOnce;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TimeoutDuration = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? TimeoutAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FirstActivationAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CooldownDuration = TimeSpan.FromSeconds(18L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier DamageIncreasePer = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier DamageTailIncreasePer = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ShieldDecayTime = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? ShieldDecayAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SuperEmpowerPartialDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public Color SuperEmpowerColor = Color.FromHex((ReadOnlySpan<char>) "#FF000046", new Color?());
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId TargetEffect = (EntProtoId) "RMCEffectXenoTelegraphRedEmpower";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EmpowerEffect = (EntProtoId) "RMCEffectEmpower";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> RoarEmote = (ProtoId<EmotePrototype>) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> TailEmote = (ProtoId<EmotePrototype>) "XenoTailSwipe";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier LeapDamage = new DamageSpecifier();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoEmpowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoEmpowerComponent) target1;
    if (serialization.TryCustomCopy<XenoEmpowerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTargets, ref target2, hookCtx, false, context))
      target2 = this.MaxTargets;
    target.MaxTargets = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.SuperThreshold, ref target3, hookCtx, false, context))
      target3 = this.SuperThreshold;
    target.SuperThreshold = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Cost, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Cost, hookCtx, context);
    target.Cost = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.InitialShield, ref target6, hookCtx, false, context))
      target6 = this.InitialShield;
    target.InitialShield = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShieldPerTarget, ref target7, hookCtx, false, context))
      target7 = this.ShieldPerTarget;
    target.ShieldPerTarget = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.ActivatedOnce, ref target8, hookCtx, false, context))
      target8 = this.ActivatedOnce;
    target.ActivatedOnce = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeoutDuration, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.TimeoutDuration, hookCtx, context);
    target.TimeoutDuration = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.TimeoutAt, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.TimeoutAt, hookCtx, context);
    target.TimeoutAt = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FirstActivationAt, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.FirstActivationAt, hookCtx, context);
    target.FirstActivationAt = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CooldownDuration, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.CooldownDuration, hookCtx, context);
    target.CooldownDuration = target12;
    DamageSpecifier target13 = (DamageSpecifier) null;
    if (this.DamageIncreasePer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageIncreasePer, ref target13, hookCtx, false, context))
    {
      if (this.DamageIncreasePer == null)
        target13 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageIncreasePer, ref target13, hookCtx, context, true);
    }
    target.DamageIncreasePer = target13;
    DamageSpecifier target14 = (DamageSpecifier) null;
    if (this.DamageTailIncreasePer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageTailIncreasePer, ref target14, hookCtx, false, context))
    {
      if (this.DamageTailIncreasePer == null)
        target14 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageTailIncreasePer, ref target14, hookCtx, context, true);
    }
    target.DamageTailIncreasePer = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShieldDecayTime, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.ShieldDecayTime, hookCtx, context);
    target.ShieldDecayTime = target15;
    TimeSpan? target16 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ShieldDecayAt, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan?>(this.ShieldDecayAt, hookCtx, context);
    target.ShieldDecayAt = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SuperEmpowerPartialDuration, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.SuperEmpowerPartialDuration, hookCtx, context);
    target.SuperEmpowerPartialDuration = target17;
    Color target18 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SuperEmpowerColor, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<Color>(this.SuperEmpowerColor, hookCtx, context);
    target.SuperEmpowerColor = target18;
    EntProtoId target19 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TargetEffect, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<EntProtoId>(this.TargetEffect, hookCtx, context);
    target.TargetEffect = target19;
    EntProtoId target20 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EmpowerEffect, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<EntProtoId>(this.EmpowerEffect, hookCtx, context);
    target.EmpowerEffect = target20;
    ProtoId<EmotePrototype> target21 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.RoarEmote, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.RoarEmote, hookCtx, context);
    target.RoarEmote = target21;
    ProtoId<EmotePrototype> target22 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.TailEmote, ref target22, hookCtx, false, context))
      target22 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.TailEmote, hookCtx, context);
    target.TailEmote = target22;
    DamageSpecifier target23 = (DamageSpecifier) null;
    if (this.LeapDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.LeapDamage, ref target23, hookCtx, false, context))
    {
      if (this.LeapDamage == null)
        target23 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.LeapDamage, ref target23, hookCtx, context, true);
    }
    target.LeapDamage = target23;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoEmpowerComponent target,
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
    XenoEmpowerComponent target1 = (XenoEmpowerComponent) target;
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
    XenoEmpowerComponent target1 = (XenoEmpowerComponent) target;
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
    XenoEmpowerComponent target1 = (XenoEmpowerComponent) target;
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
  virtual XenoEmpowerComponent Component.Instantiate() => new XenoEmpowerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoEmpowerComponent_AutoState : IComponentState
  {
    public int MaxTargets;
    public int SuperThreshold;
    public FixedPoint2 Cost;
    public bool ActivatedOnce;
    public DamageSpecifier DamageIncreasePer;
    public DamageSpecifier DamageTailIncreasePer;
    public DamageSpecifier LeapDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEmpowerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEmpowerComponent, ComponentGetState>(new ComponentEventRefHandler<XenoEmpowerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoEmpowerComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoEmpowerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoEmpowerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoEmpowerComponent.XenoEmpowerComponent_AutoState()
      {
        MaxTargets = component.MaxTargets,
        SuperThreshold = component.SuperThreshold,
        Cost = component.Cost,
        ActivatedOnce = component.ActivatedOnce,
        DamageIncreasePer = component.DamageIncreasePer,
        DamageTailIncreasePer = component.DamageTailIncreasePer,
        LeapDamage = component.LeapDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoEmpowerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoEmpowerComponent.XenoEmpowerComponent_AutoState current))
        return;
      component.MaxTargets = current.MaxTargets;
      component.SuperThreshold = current.SuperThreshold;
      component.Cost = current.Cost;
      component.ActivatedOnce = current.ActivatedOnce;
      component.DamageIncreasePer = current.DamageIncreasePer;
      component.DamageTailIncreasePer = current.DamageTailIncreasePer;
      component.LeapDamage = current.LeapDamage;
    }
  }
}
