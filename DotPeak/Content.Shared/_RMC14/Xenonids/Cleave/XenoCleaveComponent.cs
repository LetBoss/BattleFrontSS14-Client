// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Cleave.XenoCleaveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Xenonids.Cleave;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoCleaveComponent : 
  Component,
  ISerializationGenerated<XenoCleaveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RootTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RootTimeBuffed = TimeSpan.FromSeconds(1.8);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlingDistance = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlingDistanceBuffed = 6f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("Punch");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId RootEffect = (EntProtoId) "CMEffectPunch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId FlingEffect = (EntProtoId) "RMCEffectSlam";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoCleaveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoCleaveComponent) target1;
    if (serialization.TryCustomCopy<XenoCleaveComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RootTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.RootTime, hookCtx, context);
    target.RootTime = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RootTimeBuffed, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.RootTimeBuffed, hookCtx, context);
    target.RootTimeBuffed = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlingDistance, ref target4, hookCtx, false, context))
      target4 = this.FlingDistance;
    target.FlingDistance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlingDistanceBuffed, ref target5, hookCtx, false, context))
      target5 = this.FlingDistanceBuffed;
    target.FlingDistanceBuffed = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RootEffect, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.RootEffect, hookCtx, context);
    target.RootEffect = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FlingEffect, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.FlingEffect, hookCtx, context);
    target.FlingEffect = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoCleaveComponent target,
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
    XenoCleaveComponent target1 = (XenoCleaveComponent) target;
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
    XenoCleaveComponent target1 = (XenoCleaveComponent) target;
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
    XenoCleaveComponent target1 = (XenoCleaveComponent) target;
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
  virtual XenoCleaveComponent Component.Instantiate() => new XenoCleaveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoCleaveComponent_AutoState : IComponentState
  {
    public TimeSpan RootTime;
    public TimeSpan RootTimeBuffed;
    public float FlingDistance;
    public float FlingDistanceBuffed;
    public SoundSpecifier Sound;
    public EntProtoId RootEffect;
    public EntProtoId FlingEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoCleaveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoCleaveComponent, ComponentGetState>(new ComponentEventRefHandler<XenoCleaveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoCleaveComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoCleaveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoCleaveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoCleaveComponent.XenoCleaveComponent_AutoState()
      {
        RootTime = component.RootTime,
        RootTimeBuffed = component.RootTimeBuffed,
        FlingDistance = component.FlingDistance,
        FlingDistanceBuffed = component.FlingDistanceBuffed,
        Sound = component.Sound,
        RootEffect = component.RootEffect,
        FlingEffect = component.FlingEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoCleaveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoCleaveComponent.XenoCleaveComponent_AutoState current))
        return;
      component.RootTime = current.RootTime;
      component.RootTimeBuffed = current.RootTimeBuffed;
      component.FlingDistance = current.FlingDistance;
      component.FlingDistanceBuffed = current.FlingDistanceBuffed;
      component.Sound = current.Sound;
      component.RootEffect = current.RootEffect;
      component.FlingEffect = current.FlingEffect;
    }
  }
}
