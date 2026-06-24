// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Neurotoxin.NeurotoxinInjectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Neurotoxin;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedNeurotoxinSystem)})]
public sealed class NeurotoxinInjectorComponent : 
  Component,
  ISerializationGenerated<NeurotoxinInjectorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public float NeuroPerSecond;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeBetweenGasInjects = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectsDead;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectsInfectedNested;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier ToxinDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier OxygenDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier CoughDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InjectInContact = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime = TimeSpan.FromSeconds(6L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NeurotoxinInjectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NeurotoxinInjectorComponent) target1;
    if (serialization.TryCustomCopy<NeurotoxinInjectorComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NeuroPerSecond, ref target2, hookCtx, false, context))
      target2 = this.NeuroPerSecond;
    target.NeuroPerSecond = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenGasInjects, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenGasInjects, hookCtx, context);
    target.TimeBetweenGasInjects = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectsDead, ref target4, hookCtx, false, context))
      target4 = this.AffectsDead;
    target.AffectsDead = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectsInfectedNested, ref target5, hookCtx, false, context))
      target5 = this.AffectsInfectedNested;
    target.AffectsInfectedNested = target5;
    DamageSpecifier target6 = (DamageSpecifier) null;
    if (this.ToxinDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ToxinDamage, ref target6, hookCtx, false, context))
    {
      if (this.ToxinDamage == null)
        target6 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ToxinDamage, ref target6, hookCtx, context, true);
    }
    target.ToxinDamage = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.OxygenDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.OxygenDamage, ref target7, hookCtx, false, context))
    {
      if (this.OxygenDamage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.OxygenDamage, ref target7, hookCtx, context, true);
    }
    target.OxygenDamage = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (this.CoughDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CoughDamage, ref target8, hookCtx, false, context))
    {
      if (this.CoughDamage == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CoughDamage, ref target8, hookCtx, context, true);
    }
    target.CoughDamage = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.InjectInContact, ref target9, hookCtx, false, context))
      target9 = this.InjectInContact;
    target.InjectInContact = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NeurotoxinInjectorComponent target,
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
    NeurotoxinInjectorComponent target1 = (NeurotoxinInjectorComponent) target;
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
    NeurotoxinInjectorComponent target1 = (NeurotoxinInjectorComponent) target;
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
    NeurotoxinInjectorComponent target1 = (NeurotoxinInjectorComponent) target;
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
  virtual NeurotoxinInjectorComponent Component.Instantiate() => new NeurotoxinInjectorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NeurotoxinInjectorComponent_AutoState : IComponentState
  {
    public float NeuroPerSecond;
    public TimeSpan TimeBetweenGasInjects;
    public bool AffectsDead;
    public bool AffectsInfectedNested;
    public DamageSpecifier ToxinDamage;
    public DamageSpecifier OxygenDamage;
    public DamageSpecifier CoughDamage;
    public bool InjectInContact;
    public TimeSpan DazeTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NeurotoxinInjectorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NeurotoxinInjectorComponent, ComponentGetState>(new ComponentEventRefHandler<NeurotoxinInjectorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NeurotoxinInjectorComponent, ComponentHandleState>(new ComponentEventRefHandler<NeurotoxinInjectorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NeurotoxinInjectorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NeurotoxinInjectorComponent.NeurotoxinInjectorComponent_AutoState()
      {
        NeuroPerSecond = component.NeuroPerSecond,
        TimeBetweenGasInjects = component.TimeBetweenGasInjects,
        AffectsDead = component.AffectsDead,
        AffectsInfectedNested = component.AffectsInfectedNested,
        ToxinDamage = component.ToxinDamage,
        OxygenDamage = component.OxygenDamage,
        CoughDamage = component.CoughDamage,
        InjectInContact = component.InjectInContact,
        DazeTime = component.DazeTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NeurotoxinInjectorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NeurotoxinInjectorComponent.NeurotoxinInjectorComponent_AutoState current))
        return;
      component.NeuroPerSecond = current.NeuroPerSecond;
      component.TimeBetweenGasInjects = current.TimeBetweenGasInjects;
      component.AffectsDead = current.AffectsDead;
      component.AffectsInfectedNested = current.AffectsInfectedNested;
      component.ToxinDamage = current.ToxinDamage;
      component.OxygenDamage = current.OxygenDamage;
      component.CoughDamage = current.CoughDamage;
      component.InjectInContact = current.InjectInContact;
      component.DazeTime = current.DazeTime;
    }
  }
}
