// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCSelectiveFireComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCSelectiveFireSystem)})]
public sealed class RMCSelectiveFireComponent : 
  Component,
  ISerializationGenerated<RMCSelectiveFireComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SelectiveFire BaseFireModes = SelectiveFire.SemiAuto;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RecoilWielded = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RecoilUnwielded = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle ScatterIncrease = Angle.FromDegrees(0.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle ScatterDecay = Angle.FromDegrees(0.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle ScatterWielded = Angle.FromDegrees(10.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle ScatterUnwielded = Angle.FromDegrees(10.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseFireRate = 1.429f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BurstFireRateMultiplier = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double BurstScatterMult = 4.0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double BurstScatterMultModified = 4.0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<SelectiveFire, SelectiveFireModifierSet> Modifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>()
  {
    {
      SelectiveFire.Burst,
      new SelectiveFireModifierSet(0.1f, 10.0, true, 2.0, new int?(6))
    },
    {
      SelectiveFire.FullAuto,
      new SelectiveFireModifierSet(0.0f, 26.0, true, 2.0, new int?(4))
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCSelectiveFireComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCSelectiveFireComponent) target1;
    if (serialization.TryCustomCopy<RMCSelectiveFireComponent>(this, ref target, hookCtx, false, context))
      return;
    SelectiveFire target2 = SelectiveFire.Invalid;
    if (!serialization.TryCustomCopy<SelectiveFire>(this.BaseFireModes, ref target2, hookCtx, false, context))
      target2 = this.BaseFireModes;
    target.BaseFireModes = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RecoilWielded, ref target3, hookCtx, false, context))
      target3 = this.RecoilWielded;
    target.RecoilWielded = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RecoilUnwielded, ref target4, hookCtx, false, context))
      target4 = this.RecoilUnwielded;
    target.RecoilUnwielded = target4;
    Angle target5 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.ScatterIncrease, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Angle>(this.ScatterIncrease, hookCtx, context);
    target.ScatterIncrease = target5;
    Angle target6 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.ScatterDecay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Angle>(this.ScatterDecay, hookCtx, context);
    target.ScatterDecay = target6;
    Angle target7 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.ScatterWielded, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Angle>(this.ScatterWielded, hookCtx, context);
    target.ScatterWielded = target7;
    Angle target8 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.ScatterUnwielded, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Angle>(this.ScatterUnwielded, hookCtx, context);
    target.ScatterUnwielded = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseFireRate, ref target9, hookCtx, false, context))
      target9 = this.BaseFireRate;
    target.BaseFireRate = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BurstFireRateMultiplier, ref target10, hookCtx, false, context))
      target10 = this.BurstFireRateMultiplier;
    target.BurstFireRateMultiplier = target10;
    double target11 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.BurstScatterMult, ref target11, hookCtx, false, context))
      target11 = this.BurstScatterMult;
    target.BurstScatterMult = target11;
    double target12 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.BurstScatterMultModified, ref target12, hookCtx, false, context))
      target12 = this.BurstScatterMultModified;
    target.BurstScatterMultModified = target12;
    Dictionary<SelectiveFire, SelectiveFireModifierSet> target13 = (Dictionary<SelectiveFire, SelectiveFireModifierSet>) null;
    if (this.Modifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<SelectiveFire, SelectiveFireModifierSet>>(this.Modifiers, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<Dictionary<SelectiveFire, SelectiveFireModifierSet>>(this.Modifiers, hookCtx, context);
    target.Modifiers = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCSelectiveFireComponent target,
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
    RMCSelectiveFireComponent target1 = (RMCSelectiveFireComponent) target;
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
    RMCSelectiveFireComponent target1 = (RMCSelectiveFireComponent) target;
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
    RMCSelectiveFireComponent target1 = (RMCSelectiveFireComponent) target;
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
  virtual RMCSelectiveFireComponent Component.Instantiate() => new RMCSelectiveFireComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCSelectiveFireComponent_AutoState : IComponentState
  {
    public SelectiveFire BaseFireModes;
    public float RecoilWielded;
    public float RecoilUnwielded;
    public Angle ScatterIncrease;
    public Angle ScatterDecay;
    public Angle ScatterWielded;
    public Angle ScatterUnwielded;
    public float BaseFireRate;
    public float BurstFireRateMultiplier;
    public double BurstScatterMult;
    public double BurstScatterMultModified;
    public Dictionary<SelectiveFire, SelectiveFireModifierSet> Modifiers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCSelectiveFireComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCSelectiveFireComponent, ComponentGetState>(new ComponentEventRefHandler<RMCSelectiveFireComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCSelectiveFireComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCSelectiveFireComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCSelectiveFireComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCSelectiveFireComponent.RMCSelectiveFireComponent_AutoState()
      {
        BaseFireModes = component.BaseFireModes,
        RecoilWielded = component.RecoilWielded,
        RecoilUnwielded = component.RecoilUnwielded,
        ScatterIncrease = component.ScatterIncrease,
        ScatterDecay = component.ScatterDecay,
        ScatterWielded = component.ScatterWielded,
        ScatterUnwielded = component.ScatterUnwielded,
        BaseFireRate = component.BaseFireRate,
        BurstFireRateMultiplier = component.BurstFireRateMultiplier,
        BurstScatterMult = component.BurstScatterMult,
        BurstScatterMultModified = component.BurstScatterMultModified,
        Modifiers = component.Modifiers
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCSelectiveFireComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCSelectiveFireComponent.RMCSelectiveFireComponent_AutoState current))
        return;
      component.BaseFireModes = current.BaseFireModes;
      component.RecoilWielded = current.RecoilWielded;
      component.RecoilUnwielded = current.RecoilUnwielded;
      component.ScatterIncrease = current.ScatterIncrease;
      component.ScatterDecay = current.ScatterDecay;
      component.ScatterWielded = current.ScatterWielded;
      component.ScatterUnwielded = current.ScatterUnwielded;
      component.BaseFireRate = current.BaseFireRate;
      component.BurstFireRateMultiplier = current.BurstFireRateMultiplier;
      component.BurstScatterMult = current.BurstScatterMult;
      component.BurstScatterMultModified = current.BurstScatterMultModified;
      component.Modifiers = current.Modifiers == null ? (Dictionary<SelectiveFire, SelectiveFireModifierSet>) null : new Dictionary<SelectiveFire, SelectiveFireModifierSet>((IDictionary<SelectiveFire, SelectiveFireModifierSet>) current.Modifiers);
    }
  }
}
