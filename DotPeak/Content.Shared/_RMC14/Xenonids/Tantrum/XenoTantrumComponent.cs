// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tantrum.XenoTantrumComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Xenonids.Tantrum;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoTantrumComponent : 
  Component,
  ISerializationGenerated<XenoTantrumComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier BuffSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoRoar");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FuryCost = 75;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = FixedPoint2.New(100);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SelfArmorBoost = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SelfArmorDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OtherArmorDuration = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OtherSpeedDuration = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId EnrageEffect = (EntProtoId) "RMCEffectEmpowerTantrum";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color EnrageColor = Color.FromHex((ReadOnlySpan<char>) "#A31010", new Color?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 8f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTantrumComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTantrumComponent) target1;
    if (serialization.TryCustomCopy<XenoTantrumComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.BuffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BuffSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.BuffSound, hookCtx, context);
    target.BuffSound = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.FuryCost, ref target3, hookCtx, false, context))
      target3 = this.FuryCost;
    target.FuryCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.SelfArmorBoost, ref target5, hookCtx, false, context))
      target5 = this.SelfArmorBoost;
    target.SelfArmorBoost = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SelfArmorDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SelfArmorDuration, hookCtx, context);
    target.SelfArmorDuration = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OtherArmorDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.OtherArmorDuration, hookCtx, context);
    target.OtherArmorDuration = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OtherSpeedDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.OtherSpeedDuration, hookCtx, context);
    target.OtherSpeedDuration = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EnrageEffect, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.EnrageEffect, hookCtx, context);
    target.EnrageEffect = target9;
    Color target10 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EnrageColor, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Color>(this.EnrageColor, hookCtx, context);
    target.EnrageColor = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target11, hookCtx, false, context))
      target11 = this.Range;
    target.Range = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTantrumComponent target,
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
    XenoTantrumComponent target1 = (XenoTantrumComponent) target;
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
    XenoTantrumComponent target1 = (XenoTantrumComponent) target;
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
    XenoTantrumComponent target1 = (XenoTantrumComponent) target;
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
  virtual XenoTantrumComponent Component.Instantiate() => new XenoTantrumComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTantrumComponent_AutoState : IComponentState
  {
    public SoundSpecifier BuffSound;
    public int FuryCost;
    public FixedPoint2 PlasmaCost;
    public int SelfArmorBoost;
    public TimeSpan SelfArmorDuration;
    public TimeSpan OtherArmorDuration;
    public TimeSpan OtherSpeedDuration;
    public EntProtoId EnrageEffect;
    public Color EnrageColor;
    public float Range;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTantrumComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTantrumComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTantrumComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTantrumComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTantrumComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTantrumComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTantrumComponent.XenoTantrumComponent_AutoState()
      {
        BuffSound = component.BuffSound,
        FuryCost = component.FuryCost,
        PlasmaCost = component.PlasmaCost,
        SelfArmorBoost = component.SelfArmorBoost,
        SelfArmorDuration = component.SelfArmorDuration,
        OtherArmorDuration = component.OtherArmorDuration,
        OtherSpeedDuration = component.OtherSpeedDuration,
        EnrageEffect = component.EnrageEffect,
        EnrageColor = component.EnrageColor,
        Range = component.Range
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTantrumComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTantrumComponent.XenoTantrumComponent_AutoState current))
        return;
      component.BuffSound = current.BuffSound;
      component.FuryCost = current.FuryCost;
      component.PlasmaCost = current.PlasmaCost;
      component.SelfArmorBoost = current.SelfArmorBoost;
      component.SelfArmorDuration = current.SelfArmorDuration;
      component.OtherArmorDuration = current.OtherArmorDuration;
      component.OtherSpeedDuration = current.OtherSpeedDuration;
      component.EnrageEffect = current.EnrageEffect;
      component.EnrageColor = current.EnrageColor;
      component.Range = current.Range;
    }
  }
}
