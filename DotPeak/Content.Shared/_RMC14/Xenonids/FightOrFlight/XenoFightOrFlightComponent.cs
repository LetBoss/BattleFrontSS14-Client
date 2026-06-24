// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.FightOrFlight.XenoFightOrFlightComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Xenonids.FightOrFlight;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoFightOrFlightComponent : 
  Component,
  ISerializationGenerated<XenoFightOrFlightComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LowRange = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HighRange = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FuryThreshold = 75;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier RoarSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/xenos_roaring.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId RoarEffect = (EntProtoId) "RMCEffectScreechValkyrie";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId WeakRoarEffect = (EntProtoId) "RMCEffectScreechValkyrieWeak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HealEffect = (EntProtoId) "RMCEffectHealAilments";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Jitter = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<StatusEffectPrototype>[] AilmentsRemove = new ProtoId<StatusEffectPrototype>[4]
  {
    (ProtoId<StatusEffectPrototype>) "KnockedDown",
    (ProtoId<StatusEffectPrototype>) "Stun",
    (ProtoId<StatusEffectPrototype>) "Dazed",
    (ProtoId<StatusEffectPrototype>) "Unconscious"
  };
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry ComponentsRemove;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFightOrFlightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFightOrFlightComponent) target1;
    if (serialization.TryCustomCopy<XenoFightOrFlightComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.LowRange, ref target2, hookCtx, false, context))
      target2 = this.LowRange;
    target.LowRange = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.HighRange, ref target3, hookCtx, false, context))
      target3 = this.HighRange;
    target.HighRange = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.FuryThreshold, ref target4, hookCtx, false, context))
      target4 = this.FuryThreshold;
    target.FuryThreshold = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.RoarSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RoarSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.RoarSound, hookCtx, context);
    target.RoarSound = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RoarEffect, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.RoarEffect, hookCtx, context);
    target.RoarEffect = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.WeakRoarEffect, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.WeakRoarEffect, hookCtx, context);
    target.WeakRoarEffect = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HealEffect, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.HealEffect, hookCtx, context);
    target.HealEffect = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Jitter, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.Jitter, hookCtx, context);
    target.Jitter = target9;
    ProtoId<StatusEffectPrototype>[] target10 = (ProtoId<StatusEffectPrototype>[]) null;
    if (this.AilmentsRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>[]>(this.AilmentsRemove, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<ProtoId<StatusEffectPrototype>[]>(this.AilmentsRemove, hookCtx, context);
    target.AilmentsRemove = target10;
    ComponentRegistry target11 = (ComponentRegistry) null;
    if (this.ComponentsRemove == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.ComponentsRemove, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<ComponentRegistry>(this.ComponentsRemove, hookCtx, context);
    target.ComponentsRemove = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFightOrFlightComponent target,
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
    XenoFightOrFlightComponent target1 = (XenoFightOrFlightComponent) target;
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
    XenoFightOrFlightComponent target1 = (XenoFightOrFlightComponent) target;
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
    XenoFightOrFlightComponent target1 = (XenoFightOrFlightComponent) target;
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
  virtual XenoFightOrFlightComponent Component.Instantiate() => new XenoFightOrFlightComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFightOrFlightComponent_AutoState : IComponentState
  {
    public int LowRange;
    public int HighRange;
    public int FuryThreshold;
    public SoundSpecifier RoarSound;
    public EntProtoId RoarEffect;
    public EntProtoId WeakRoarEffect;
    public EntProtoId HealEffect;
    public TimeSpan Jitter;
    public ProtoId<StatusEffectPrototype>[] AilmentsRemove;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFightOrFlightComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFightOrFlightComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFightOrFlightComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFightOrFlightComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFightOrFlightComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFightOrFlightComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFightOrFlightComponent.XenoFightOrFlightComponent_AutoState()
      {
        LowRange = component.LowRange,
        HighRange = component.HighRange,
        FuryThreshold = component.FuryThreshold,
        RoarSound = component.RoarSound,
        RoarEffect = component.RoarEffect,
        WeakRoarEffect = component.WeakRoarEffect,
        HealEffect = component.HealEffect,
        Jitter = component.Jitter,
        AilmentsRemove = component.AilmentsRemove
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFightOrFlightComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFightOrFlightComponent.XenoFightOrFlightComponent_AutoState current))
        return;
      component.LowRange = current.LowRange;
      component.HighRange = current.HighRange;
      component.FuryThreshold = current.FuryThreshold;
      component.RoarSound = current.RoarSound;
      component.RoarEffect = current.RoarEffect;
      component.WeakRoarEffect = current.WeakRoarEffect;
      component.HealEffect = current.HealEffect;
      component.Jitter = current.Jitter;
      component.AilmentsRemove = current.AilmentsRemove;
    }
  }
}
