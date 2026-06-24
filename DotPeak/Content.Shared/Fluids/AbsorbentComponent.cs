// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.AbsorbentComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Fluids;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class AbsorbentComponent : 
  Component,
  ISerializationGenerated<AbsorbentComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Color, float> Progress = new Dictionary<Color, float>();
  [DataField(null, false, 1, false, false, null)]
  public string SolutionName = "absorbed";
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 PickupAmount = FixedPoint2.New(100);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MoppedEffect = (EntProtoId) "PuddleSparkle";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier PickupSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/watersplash.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier TransferSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/slosh.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f)).WithVolume(-3f)));
  public static readonly SoundSpecifier DefaultTransferSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/slosh.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f)).WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  public bool UseAbsorberSolution = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AbsorbentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AbsorbentComponent) target1;
    if (serialization.TryCustomCopy<AbsorbentComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Color, float> target2 = (Dictionary<Color, float>) null;
    if (this.Progress == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Color, float>>(this.Progress, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Color, float>>(this.Progress, hookCtx, context);
    target.Progress = target2;
    string target3 = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref target3, hookCtx, false, context))
      target3 = this.SolutionName;
    target.SolutionName = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PickupAmount, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.PickupAmount, hookCtx, context);
    target.PickupAmount = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MoppedEffect, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.MoppedEffect, hookCtx, context);
    target.MoppedEffect = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.PickupSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PickupSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.PickupSound, hookCtx, context);
    target.PickupSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.TransferSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TransferSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.TransferSound, hookCtx, context);
    target.TransferSound = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseAbsorberSolution, ref target8, hookCtx, false, context))
      target8 = this.UseAbsorberSolution;
    target.UseAbsorberSolution = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AbsorbentComponent target,
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
    AbsorbentComponent target1 = (AbsorbentComponent) target;
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
    AbsorbentComponent target1 = (AbsorbentComponent) target;
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
    AbsorbentComponent target1 = (AbsorbentComponent) target;
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
  virtual AbsorbentComponent Component.Instantiate() => new AbsorbentComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AbsorbentComponent_AutoState : IComponentState
  {
    public Dictionary<Color, float> Progress;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AbsorbentComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AbsorbentComponent, ComponentGetState>(new ComponentEventRefHandler<AbsorbentComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AbsorbentComponent, ComponentHandleState>(new ComponentEventRefHandler<AbsorbentComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AbsorbentComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AbsorbentComponent.AbsorbentComponent_AutoState()
      {
        Progress = component.Progress
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AbsorbentComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AbsorbentComponent.AbsorbentComponent_AutoState current))
        return;
      component.Progress = current.Progress == null ? (Dictionary<Color, float>) null : new Dictionary<Color, float>((IDictionary<Color, float>) current.Progress);
    }
  }
}
