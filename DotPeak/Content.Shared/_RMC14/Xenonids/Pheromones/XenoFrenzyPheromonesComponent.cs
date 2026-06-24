// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Pheromones.XenoFrenzyPheromonesComponent
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
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Pheromones;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoPheromonesSystem)})]
public sealed class XenoFrenzyPheromonesComponent : 
  Component,
  ISerializationGenerated<XenoFrenzyPheromonesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), "frenzy");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Multiplier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AttackDamageAddPerMult = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MovementSpeedModifier = (FixedPoint2) 0.06;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PullMovementSpeedModifier = (FixedPoint2) 0.03;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> DamageGroup = (ProtoId<DamageGroupPrototype>) "Brute";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFrenzyPheromonesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFrenzyPheromonesComponent) target1;
    if (serialization.TryCustomCopy<XenoFrenzyPheromonesComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target2 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Multiplier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Multiplier, hookCtx, context);
    target.Multiplier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AttackDamageAddPerMult, ref target4, hookCtx, false, context))
      target4 = this.AttackDamageAddPerMult;
    target.AttackDamageAddPerMult = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MovementSpeedModifier, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.MovementSpeedModifier, hookCtx, context);
    target.MovementSpeedModifier = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PullMovementSpeedModifier, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.PullMovementSpeedModifier, hookCtx, context);
    target.PullMovementSpeedModifier = target6;
    ProtoId<DamageGroupPrototype> target7 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.DamageGroup, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.DamageGroup, hookCtx, context);
    target.DamageGroup = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFrenzyPheromonesComponent target,
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
    XenoFrenzyPheromonesComponent target1 = (XenoFrenzyPheromonesComponent) target;
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
    XenoFrenzyPheromonesComponent target1 = (XenoFrenzyPheromonesComponent) target;
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
    XenoFrenzyPheromonesComponent target1 = (XenoFrenzyPheromonesComponent) target;
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
  virtual XenoFrenzyPheromonesComponent Component.Instantiate()
  {
    return new XenoFrenzyPheromonesComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFrenzyPheromonesComponent_AutoState : IComponentState
  {
    public SpriteSpecifier Icon;
    public FixedPoint2 Multiplier;
    public float AttackDamageAddPerMult;
    public FixedPoint2 MovementSpeedModifier;
    public FixedPoint2 PullMovementSpeedModifier;
    public ProtoId<DamageGroupPrototype> DamageGroup;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFrenzyPheromonesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFrenzyPheromonesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFrenzyPheromonesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFrenzyPheromonesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFrenzyPheromonesComponent.XenoFrenzyPheromonesComponent_AutoState()
      {
        Icon = component.Icon,
        Multiplier = component.Multiplier,
        AttackDamageAddPerMult = component.AttackDamageAddPerMult,
        MovementSpeedModifier = component.MovementSpeedModifier,
        PullMovementSpeedModifier = component.PullMovementSpeedModifier,
        DamageGroup = component.DamageGroup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFrenzyPheromonesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFrenzyPheromonesComponent.XenoFrenzyPheromonesComponent_AutoState current))
        return;
      component.Icon = current.Icon;
      component.Multiplier = current.Multiplier;
      component.AttackDamageAddPerMult = current.AttackDamageAddPerMult;
      component.MovementSpeedModifier = current.MovementSpeedModifier;
      component.PullMovementSpeedModifier = current.PullMovementSpeedModifier;
      component.DamageGroup = current.DamageGroup;
    }
  }
}
