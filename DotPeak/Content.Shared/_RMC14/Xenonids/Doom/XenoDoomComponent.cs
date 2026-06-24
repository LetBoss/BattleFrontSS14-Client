// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Doom.XenoDoomComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Maths;
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
namespace Content.Shared._RMC14.Xenonids.Doom;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoDoomComponent : 
  Component,
  ISerializationGenerated<XenoDoomComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Smoke = (EntProtoId) "RMCSmokeKingDoom";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = RMCMathExtensions.CircleAreaFromSquareAbilityRange(7f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ExtinguishTimePerDistanceMult = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 RemovalPerReagent = (FixedPoint2) 100;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/deep_alien_screech2.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectScreechKing";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CameraShakeStrength = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string TargetSolution = "chemicals";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OverlayTime = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDoomComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDoomComponent) target1;
    if (serialization.TryCustomCopy<XenoDoomComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Smoke, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Smoke, hookCtx, context);
    target.Smoke = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExtinguishTimePerDistanceMult, ref target4, hookCtx, false, context))
      target4 = this.ExtinguishTimePerDistanceMult;
    target.ExtinguishTimePerDistanceMult = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RemovalPerReagent, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.RemovalPerReagent, hookCtx, context);
    target.RemovalPerReagent = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.CameraShakeStrength, ref target10, hookCtx, false, context))
      target10 = this.CameraShakeStrength;
    target.CameraShakeStrength = target10;
    string target11 = (string) null;
    if (this.TargetSolution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetSolution, ref target11, hookCtx, false, context))
      target11 = this.TargetSolution;
    target.TargetSolution = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OverlayTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.OverlayTime, hookCtx, context);
    target.OverlayTime = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDoomComponent target,
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
    XenoDoomComponent target1 = (XenoDoomComponent) target;
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
    XenoDoomComponent target1 = (XenoDoomComponent) target;
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
    XenoDoomComponent target1 = (XenoDoomComponent) target;
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
  virtual XenoDoomComponent Component.Instantiate() => new XenoDoomComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoDoomComponent_AutoState : IComponentState
  {
    public EntProtoId Smoke;
    public float Range;
    public float ExtinguishTimePerDistanceMult;
    public TimeSpan DazeTime;
    public TimeSpan SlowTime;
    public FixedPoint2 RemovalPerReagent;
    public SoundSpecifier Sound;
    public EntProtoId Effect;
    public int CameraShakeStrength;
    public string TargetSolution;
    public TimeSpan OverlayTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoDoomComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoDoomComponent, ComponentGetState>(new ComponentEventRefHandler<XenoDoomComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoDoomComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoDoomComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoDoomComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoDoomComponent.XenoDoomComponent_AutoState()
      {
        Smoke = component.Smoke,
        Range = component.Range,
        ExtinguishTimePerDistanceMult = component.ExtinguishTimePerDistanceMult,
        DazeTime = component.DazeTime,
        SlowTime = component.SlowTime,
        RemovalPerReagent = component.RemovalPerReagent,
        Sound = component.Sound,
        Effect = component.Effect,
        CameraShakeStrength = component.CameraShakeStrength,
        TargetSolution = component.TargetSolution,
        OverlayTime = component.OverlayTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoDoomComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoDoomComponent.XenoDoomComponent_AutoState current))
        return;
      component.Smoke = current.Smoke;
      component.Range = current.Range;
      component.ExtinguishTimePerDistanceMult = current.ExtinguishTimePerDistanceMult;
      component.DazeTime = current.DazeTime;
      component.SlowTime = current.SlowTime;
      component.RemovalPerReagent = current.RemovalPerReagent;
      component.Sound = current.Sound;
      component.Effect = current.Effect;
      component.CameraShakeStrength = current.CameraShakeStrength;
      component.TargetSolution = current.TargetSolution;
      component.OverlayTime = current.OverlayTime;
    }
  }
}
