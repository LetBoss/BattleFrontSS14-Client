// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Wounds.WoundableComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Wounds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedWoundsSystem)})]
public sealed class WoundableComponent : 
  Component,
  ISerializationGenerated<WoundableComponent>,
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
  public FixedPoint2 BleedMinDamage = (FixedPoint2) 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BloodLossMultiplier = 0.0375f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DurationMultiplier = TimeSpan.FromSeconds(2.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WoundableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WoundableComponent) target1;
    if (serialization.TryCustomCopy<WoundableComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<DamageGroupPrototype> target2 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.BruteWoundGroup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.BruteWoundGroup, hookCtx, context);
    target.BruteWoundGroup = target2;
    ProtoId<DamageGroupPrototype> target3 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.BurnWoundGroup, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.BurnWoundGroup, hookCtx, context);
    target.BurnWoundGroup = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BleedMinDamage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.BleedMinDamage, hookCtx, context);
    target.BleedMinDamage = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BloodLossMultiplier, ref target5, hookCtx, false, context))
      target5 = this.BloodLossMultiplier;
    target.BloodLossMultiplier = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DurationMultiplier, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DurationMultiplier, hookCtx, context);
    target.DurationMultiplier = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WoundableComponent target,
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
    WoundableComponent target1 = (WoundableComponent) target;
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
    WoundableComponent target1 = (WoundableComponent) target;
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
    WoundableComponent target1 = (WoundableComponent) target;
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
  virtual WoundableComponent Component.Instantiate() => new WoundableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WoundableComponent_AutoState : IComponentState
  {
    public ProtoId<DamageGroupPrototype> BruteWoundGroup;
    public ProtoId<DamageGroupPrototype> BurnWoundGroup;
    public FixedPoint2 BleedMinDamage;
    public float BloodLossMultiplier;
    public TimeSpan DurationMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WoundableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WoundableComponent, ComponentGetState>(new ComponentEventRefHandler<WoundableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WoundableComponent, ComponentHandleState>(new ComponentEventRefHandler<WoundableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WoundableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WoundableComponent.WoundableComponent_AutoState()
      {
        BruteWoundGroup = component.BruteWoundGroup,
        BurnWoundGroup = component.BurnWoundGroup,
        BleedMinDamage = component.BleedMinDamage,
        BloodLossMultiplier = component.BloodLossMultiplier,
        DurationMultiplier = component.DurationMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WoundableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WoundableComponent.WoundableComponent_AutoState current))
        return;
      component.BruteWoundGroup = current.BruteWoundGroup;
      component.BurnWoundGroup = current.BurnWoundGroup;
      component.BleedMinDamage = current.BleedMinDamage;
      component.BloodLossMultiplier = current.BloodLossMultiplier;
      component.DurationMultiplier = current.DurationMultiplier;
    }
  }
}
