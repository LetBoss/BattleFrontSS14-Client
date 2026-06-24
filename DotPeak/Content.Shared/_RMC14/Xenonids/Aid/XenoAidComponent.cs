// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Aid.XenoAidComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Aid;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoAidSystem)})]
public sealed class XenoAidComponent : 
  Component,
  ISerializationGenerated<XenoAidComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Heal = (FixedPoint2) 150;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int EnergyCost = 100;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? HealEffect = (EntProtoId?) "RMCEffectHeal";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AilmentsRange = 8f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? AilmentsEffects = (EntProtoId?) "RMCEffectHealAilments";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AilmentsJitterDuration = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<StatusEffectPrototype>[] AilmentsRemove = new ProtoId<StatusEffectPrototype>[2]
  {
    (ProtoId<StatusEffectPrototype>) "KnockedDown",
    (ProtoId<StatusEffectPrototype>) "Stun"
  };
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry ComponentsRemove;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoAidComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoAidComponent) target1;
    if (serialization.TryCustomCopy<XenoAidComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Heal, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Heal, hookCtx, context);
    target.Heal = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.EnergyCost, ref target3, hookCtx, false, context))
      target3 = this.EnergyCost;
    target.EnergyCost = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HealEffect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.HealEffect, hookCtx, context);
    target.HealEffect = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AilmentsRange, ref target5, hookCtx, false, context))
      target5 = this.AilmentsRange;
    target.AilmentsRange = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.AilmentsEffects, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.AilmentsEffects, hookCtx, context);
    target.AilmentsEffects = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AilmentsJitterDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.AilmentsJitterDuration, hookCtx, context);
    target.AilmentsJitterDuration = target7;
    ProtoId<StatusEffectPrototype>[] target8 = (ProtoId<StatusEffectPrototype>[]) null;
    if (this.AilmentsRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>[]>(this.AilmentsRemove, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<ProtoId<StatusEffectPrototype>[]>(this.AilmentsRemove, hookCtx, context);
    target.AilmentsRemove = target8;
    ComponentRegistry target9 = (ComponentRegistry) null;
    if (this.ComponentsRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.ComponentsRemove, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ComponentRegistry>(this.ComponentsRemove, hookCtx, context);
    target.ComponentsRemove = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoAidComponent target,
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
    XenoAidComponent target1 = (XenoAidComponent) target;
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
    XenoAidComponent target1 = (XenoAidComponent) target;
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
    XenoAidComponent target1 = (XenoAidComponent) target;
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
  virtual XenoAidComponent Component.Instantiate() => new XenoAidComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoAidComponent_AutoState : IComponentState
  {
    public FixedPoint2 Heal;
    public int EnergyCost;
    public EntProtoId? HealEffect;
    public float AilmentsRange;
    public EntProtoId? AilmentsEffects;
    public TimeSpan AilmentsJitterDuration;
    public ProtoId<StatusEffectPrototype>[] AilmentsRemove;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoAidComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoAidComponent, ComponentGetState>(new ComponentEventRefHandler<XenoAidComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoAidComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoAidComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoAidComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoAidComponent.XenoAidComponent_AutoState()
      {
        Heal = component.Heal,
        EnergyCost = component.EnergyCost,
        HealEffect = component.HealEffect,
        AilmentsRange = component.AilmentsRange,
        AilmentsEffects = component.AilmentsEffects,
        AilmentsJitterDuration = component.AilmentsJitterDuration,
        AilmentsRemove = component.AilmentsRemove
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoAidComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoAidComponent.XenoAidComponent_AutoState current))
        return;
      component.Heal = current.Heal;
      component.EnergyCost = current.EnergyCost;
      component.HealEffect = current.HealEffect;
      component.AilmentsRange = current.AilmentsRange;
      component.AilmentsEffects = current.AilmentsEffects;
      component.AilmentsJitterDuration = current.AilmentsJitterDuration;
      component.AilmentsRemove = current.AilmentsRemove;
    }
  }
}
