// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.XenoShieldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Shields;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoShieldComponent : 
  Component,
  ISerializationGenerated<XenoShieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoShieldSystem.ShieldType Shield;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ShieldAmount = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? Duration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShieldDecayAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double DecayPerSecond;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ShieldBreak = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Bullets/shield_break_c1.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ShieldImpact = (SoundSpecifier) new SoundCollectionSpecifier("RMCShieldImpact", new AudioParams?(AudioParams.Default.WithVolume(-4f)));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoShieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoShieldComponent) target1;
    if (serialization.TryCustomCopy<XenoShieldComponent>(this, ref target, hookCtx, false, context))
      return;
    XenoShieldSystem.ShieldType target2 = XenoShieldSystem.ShieldType.Generic;
    if (!serialization.TryCustomCopy<XenoShieldSystem.ShieldType>(this.Shield, ref target2, hookCtx, false, context))
      target2 = this.Shield;
    target.Shield = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ShieldAmount, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ShieldAmount, hookCtx, context);
    target.ShieldAmount = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Duration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.Duration, hookCtx, context);
    target.Duration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShieldDecayAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ShieldDecayAt, hookCtx, context);
    target.ShieldDecayAt = target5;
    double target6 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.DecayPerSecond, ref target6, hookCtx, false, context))
      target6 = this.DecayPerSecond;
    target.DecayPerSecond = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target7, hookCtx, false, context))
      target7 = this.Active;
    target.Active = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.ShieldBreak == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShieldBreak, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.ShieldBreak, hookCtx, context);
    target.ShieldBreak = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.ShieldImpact == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShieldImpact, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.ShieldImpact, hookCtx, context);
    target.ShieldImpact = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoShieldComponent target,
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
    XenoShieldComponent target1 = (XenoShieldComponent) target;
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
    XenoShieldComponent target1 = (XenoShieldComponent) target;
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
    XenoShieldComponent target1 = (XenoShieldComponent) target;
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
  virtual XenoShieldComponent Component.Instantiate() => new XenoShieldComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoShieldComponent_AutoState : IComponentState
  {
    public XenoShieldSystem.ShieldType Shield;
    public FixedPoint2 ShieldAmount;
    public TimeSpan? Duration;
    public TimeSpan ShieldDecayAt;
    public double DecayPerSecond;
    public bool Active;
    public SoundSpecifier ShieldBreak;
    public SoundSpecifier ShieldImpact;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoShieldComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoShieldComponent, ComponentGetState>(new ComponentEventRefHandler<XenoShieldComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoShieldComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoShieldComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoShieldComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoShieldComponent.XenoShieldComponent_AutoState()
      {
        Shield = component.Shield,
        ShieldAmount = component.ShieldAmount,
        Duration = component.Duration,
        ShieldDecayAt = component.ShieldDecayAt,
        DecayPerSecond = component.DecayPerSecond,
        Active = component.Active,
        ShieldBreak = component.ShieldBreak,
        ShieldImpact = component.ShieldImpact
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoShieldComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoShieldComponent.XenoShieldComponent_AutoState current))
        return;
      component.Shield = current.Shield;
      component.ShieldAmount = current.ShieldAmount;
      component.Duration = current.Duration;
      component.ShieldDecayAt = current.ShieldDecayAt;
      component.DecayPerSecond = current.DecayPerSecond;
      component.Active = current.Active;
      component.ShieldBreak = current.ShieldBreak;
      component.ShieldImpact = current.ShieldImpact;
    }
  }
}
