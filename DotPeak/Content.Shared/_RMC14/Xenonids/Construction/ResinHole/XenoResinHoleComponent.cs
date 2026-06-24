// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.ResinHole.XenoResinHoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoResinHoleComponent : 
  Component,
  ISerializationGenerated<XenoResinHoleComponent>,
  ISerializationGenerated
{
  public const string ParasitePrototype = "CMXenoParasite";
  public const string BoilerAcid = "XenoBombardAcidProjectile";
  public const string AcidGasPrototype = "RMCSmokeAcid";
  public const string BoilerNeuro = "XenoBombardNeurotoxinProjectile";
  public const string NeuroGasPrototype = "RMCSmokeNeurotoxin";
  public const string AcidPrototype = "XenoAcidSprayTrap";
  public const string WeakAcidPrototype = "XenoAcidSprayTrapWeak";
  public const string StrongAcidPrototype = "XenoAcidSprayTrapStrong";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? TrapPrototype;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StepStunDuration;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AddParasiteDelay;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AddFluidDelay;
  [DataField(null, false, 1, false, false, null)]
  public float ParasiteActivationRange;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? FluidFillSound;
  [DataField(null, false, 1, false, false, null)]
  public Color MessageColor;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BuildSound;

  public XenoResinHoleComponent()
  {
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("RMCResinBuild");
    collectionSpecifier.Params = AudioParams.Default.WithVolume(-5f);
    this.BuildSound = (SoundSpecifier) collectionSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoResinHoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoResinHoleComponent) target1;
    if (serialization.TryCustomCopy<XenoResinHoleComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.TrapPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.TrapPrototype, hookCtx, context);
    target.TrapPrototype = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StepStunDuration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.StepStunDuration, hookCtx, context);
    target.StepStunDuration = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AddParasiteDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.AddParasiteDelay, hookCtx, context);
    target.AddParasiteDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AddFluidDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.AddFluidDelay, hookCtx, context);
    target.AddFluidDelay = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ParasiteActivationRange, ref target6, hookCtx, false, context))
      target6 = this.ParasiteActivationRange;
    target.ParasiteActivationRange = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FluidFillSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.FluidFillSound, hookCtx, context);
    target.FluidFillSound = target7;
    Color target8 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.MessageColor, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Color>(this.MessageColor, hookCtx, context);
    target.MessageColor = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.BuildSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BuildSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.BuildSound, hookCtx, context);
    target.BuildSound = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoResinHoleComponent target,
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
    XenoResinHoleComponent target1 = (XenoResinHoleComponent) target;
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
    XenoResinHoleComponent target1 = (XenoResinHoleComponent) target;
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
    XenoResinHoleComponent target1 = (XenoResinHoleComponent) target;
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
  virtual XenoResinHoleComponent Component.Instantiate() => new XenoResinHoleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoResinHoleComponent_AutoState : IComponentState
  {
    public EntProtoId? TrapPrototype;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoResinHoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoResinHoleComponent, ComponentGetState>(new ComponentEventRefHandler<XenoResinHoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoResinHoleComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoResinHoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoResinHoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoResinHoleComponent.XenoResinHoleComponent_AutoState()
      {
        TrapPrototype = component.TrapPrototype
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoResinHoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoResinHoleComponent.XenoResinHoleComponent_AutoState current))
        return;
      component.TrapPrototype = current.TrapPrototype;
    }
  }
}
