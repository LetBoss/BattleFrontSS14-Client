// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Energy.XenoEnergyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Energy;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoEnergySystem)})]
public sealed class XenoEnergyComponent : 
  Component,
  ISerializationGenerated<XenoEnergyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Current;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Max = 350;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Gain = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan GainEvery = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextGain;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int GainAttack = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int GainAttackDowned = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreLateInfected;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool GainOnProjectiles = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string PopupGain = "rmc-xeno-energy-increase-user";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string PopupNotEnough = "rmc-xeno-not-enough-energy";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AlertPrototype> Alert = (ProtoId<AlertPrototype>) "XenoEnergyBase";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? GenerationCap;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoEnergyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoEnergyComponent) target1;
    if (serialization.TryCustomCopy<XenoEnergyComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Current, ref target2, hookCtx, false, context))
      target2 = this.Current;
    target.Current = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Gain, ref target4, hookCtx, false, context))
      target4 = this.Gain;
    target.Gain = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GainEvery, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.GainEvery, hookCtx, context);
    target.GainEvery = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextGain, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextGain, hookCtx, context);
    target.NextGain = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.GainAttack, ref target7, hookCtx, false, context))
      target7 = this.GainAttack;
    target.GainAttack = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.GainAttackDowned, ref target8, hookCtx, false, context))
      target8 = this.GainAttackDowned;
    target.GainAttackDowned = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreLateInfected, ref target9, hookCtx, false, context))
      target9 = this.IgnoreLateInfected;
    target.IgnoreLateInfected = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.GainOnProjectiles, ref target10, hookCtx, false, context))
      target10 = this.GainOnProjectiles;
    target.GainOnProjectiles = target10;
    string target11 = (string) null;
    if (this.PopupGain == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PopupGain, ref target11, hookCtx, false, context))
      target11 = this.PopupGain;
    target.PopupGain = target11;
    string target12 = (string) null;
    if (this.PopupNotEnough == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PopupNotEnough, ref target12, hookCtx, false, context))
      target12 = this.PopupNotEnough;
    target.PopupNotEnough = target12;
    ProtoId<AlertPrototype> target13 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.Alert, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.Alert, hookCtx, context);
    target.Alert = target13;
    int? target14 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.GenerationCap, ref target14, hookCtx, false, context))
      target14 = this.GenerationCap;
    target.GenerationCap = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoEnergyComponent target,
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
    XenoEnergyComponent target1 = (XenoEnergyComponent) target;
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
    XenoEnergyComponent target1 = (XenoEnergyComponent) target;
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
    XenoEnergyComponent target1 = (XenoEnergyComponent) target;
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
  virtual XenoEnergyComponent Component.Instantiate() => new XenoEnergyComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEnergyComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEnergyComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoEnergyComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoEnergyComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextGain += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoEnergyComponent_AutoState : IComponentState
  {
    public int Current;
    public int Max;
    public int Gain;
    public TimeSpan GainEvery;
    public TimeSpan NextGain;
    public int GainAttack;
    public int GainAttackDowned;
    public bool IgnoreLateInfected;
    public bool GainOnProjectiles;
    public 
    #nullable enable
    string PopupGain;
    public string PopupNotEnough;
    public ProtoId<AlertPrototype> Alert;
    public int? GenerationCap;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEnergyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEnergyComponent, ComponentGetState>(new ComponentEventRefHandler<XenoEnergyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoEnergyComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoEnergyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoEnergyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoEnergyComponent.XenoEnergyComponent_AutoState()
      {
        Current = component.Current,
        Max = component.Max,
        Gain = component.Gain,
        GainEvery = component.GainEvery,
        NextGain = component.NextGain,
        GainAttack = component.GainAttack,
        GainAttackDowned = component.GainAttackDowned,
        IgnoreLateInfected = component.IgnoreLateInfected,
        GainOnProjectiles = component.GainOnProjectiles,
        PopupGain = component.PopupGain,
        PopupNotEnough = component.PopupNotEnough,
        Alert = component.Alert,
        GenerationCap = component.GenerationCap
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoEnergyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoEnergyComponent.XenoEnergyComponent_AutoState current))
        return;
      component.Current = current.Current;
      component.Max = current.Max;
      component.Gain = current.Gain;
      component.GainEvery = current.GainEvery;
      component.NextGain = current.NextGain;
      component.GainAttack = current.GainAttack;
      component.GainAttackDowned = current.GainAttackDowned;
      component.IgnoreLateInfected = current.IgnoreLateInfected;
      component.GainOnProjectiles = current.GainOnProjectiles;
      component.PopupGain = current.PopupGain;
      component.PopupNotEnough = current.PopupNotEnough;
      component.Alert = current.Alert;
      component.GenerationCap = current.GenerationCap;
    }
  }
}
