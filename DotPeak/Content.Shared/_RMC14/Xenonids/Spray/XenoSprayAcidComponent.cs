// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Spray.XenoSprayAcidComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Xenonids.Spray;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSprayAcidSystem)})]
public sealed class XenoSprayAcidComponent : 
  Component,
  ISerializationGenerated<XenoSprayAcidComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Acid = (EntProtoId) "XenoAcidSprayWeak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(0.2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier BarricadeDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BarricadeDuration = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/refill.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 6f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSprayAcidComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSprayAcidComponent) target1;
    if (serialization.TryCustomCopy<XenoSprayAcidComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Acid, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Acid, hookCtx, context);
    target.Acid = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoAfter, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DoAfter, hookCtx, context);
    target.DoAfter = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
    DamageSpecifier target6 = (DamageSpecifier) null;
    if (this.BarricadeDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BarricadeDamage, ref target6, hookCtx, false, context))
    {
      if (this.BarricadeDamage == null)
        target6 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BarricadeDamage, ref target6, hookCtx, context, true);
    }
    target.BarricadeDamage = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BarricadeDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.BarricadeDuration, hookCtx, context);
    target.BarricadeDuration = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target9, hookCtx, false, context))
      target9 = this.Range;
    target.Range = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSprayAcidComponent target,
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
    XenoSprayAcidComponent target1 = (XenoSprayAcidComponent) target;
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
    XenoSprayAcidComponent target1 = (XenoSprayAcidComponent) target;
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
    XenoSprayAcidComponent target1 = (XenoSprayAcidComponent) target;
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
  virtual XenoSprayAcidComponent Component.Instantiate() => new XenoSprayAcidComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSprayAcidComponent_AutoState : IComponentState
  {
    public EntProtoId Acid;
    public FixedPoint2 PlasmaCost;
    public TimeSpan DoAfter;
    public TimeSpan Delay;
    public DamageSpecifier BarricadeDamage;
    public TimeSpan BarricadeDuration;
    public SoundSpecifier Sound;
    public float Range;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSprayAcidComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSprayAcidComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSprayAcidComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSprayAcidComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSprayAcidComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSprayAcidComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSprayAcidComponent.XenoSprayAcidComponent_AutoState()
      {
        Acid = component.Acid,
        PlasmaCost = component.PlasmaCost,
        DoAfter = component.DoAfter,
        Delay = component.Delay,
        BarricadeDamage = component.BarricadeDamage,
        BarricadeDuration = component.BarricadeDuration,
        Sound = component.Sound,
        Range = component.Range
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSprayAcidComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSprayAcidComponent.XenoSprayAcidComponent_AutoState current))
        return;
      component.Acid = current.Acid;
      component.PlasmaCost = current.PlasmaCost;
      component.DoAfter = current.DoAfter;
      component.Delay = current.Delay;
      component.BarricadeDamage = current.BarricadeDamage;
      component.BarricadeDuration = current.BarricadeDuration;
      component.Sound = current.Sound;
      component.Range = current.Range;
    }
  }
}
