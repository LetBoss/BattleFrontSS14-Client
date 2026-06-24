// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.XenoBulwarkOfTheHiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Maths;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Shields;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoBulwarkOfTheHiveComponent : 
  Component,
  ISerializationGenerated<XenoBulwarkOfTheHiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DecayTime = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public int DecayAmount = 100;
  [DataField(null, false, 1, false, false, null)]
  public float Range = RMCMathExtensions.CircleAreaFromSquareAbilityRange(6f);
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 ShieldAmount = FixedPoint2.New(200);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/deep_alien_screech.ogg");
  [DataField(null, false, 1, false, false, null)]
  public string VisualState = "king-shield";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LightningDuration = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> Supporting = new List<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoBulwarkOfTheHiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoBulwarkOfTheHiveComponent) target1;
    if (serialization.TryCustomCopy<XenoBulwarkOfTheHiveComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DecayTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DecayTime, hookCtx, context);
    target.DecayTime = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.DecayAmount, ref target3, hookCtx, false, context))
      target3 = this.DecayAmount;
    target.DecayAmount = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target4, hookCtx, false, context))
      target4 = this.Range;
    target.Range = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ShieldAmount, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.ShieldAmount, hookCtx, context);
    target.ShieldAmount = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    string target7 = (string) null;
    if (this.VisualState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VisualState, ref target7, hookCtx, false, context))
      target7 = this.VisualState;
    target.VisualState = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LightningDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.LightningDuration, hookCtx, context);
    target.LightningDuration = target8;
    List<EntityUid> target9 = (List<EntityUid>) null;
    if (this.Supporting == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Supporting, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<EntityUid>>(this.Supporting, hookCtx, context);
    target.Supporting = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoBulwarkOfTheHiveComponent target,
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
    XenoBulwarkOfTheHiveComponent target1 = (XenoBulwarkOfTheHiveComponent) target;
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
    XenoBulwarkOfTheHiveComponent target1 = (XenoBulwarkOfTheHiveComponent) target;
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
    XenoBulwarkOfTheHiveComponent target1 = (XenoBulwarkOfTheHiveComponent) target;
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
  virtual XenoBulwarkOfTheHiveComponent Component.Instantiate()
  {
    return new XenoBulwarkOfTheHiveComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoBulwarkOfTheHiveComponent_AutoState : IComponentState
  {
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoBulwarkOfTheHiveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, ComponentGetState>(new ComponentEventRefHandler<XenoBulwarkOfTheHiveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoBulwarkOfTheHiveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoBulwarkOfTheHiveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoBulwarkOfTheHiveComponent.XenoBulwarkOfTheHiveComponent_AutoState()
      {
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoBulwarkOfTheHiveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoBulwarkOfTheHiveComponent.XenoBulwarkOfTheHiveComponent_AutoState current))
        return;
      component.Sound = current.Sound;
    }
  }
}
