// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Pulling.ParalyzeOnPullAttemptComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Pulling;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ParalyzeOnPullAttemptComponent : 
  Component,
  ISerializationGenerated<ParalyzeOnPullAttemptComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/pierce.ogg", new AudioParams?(AudioParams.Default.WithVolume(-10f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinPitch = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxPitch = 4f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParalyzeOnPullAttemptComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParalyzeOnPullAttemptComponent) target1;
    if (serialization.TryCustomCopy<ParalyzeOnPullAttemptComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinPitch, ref target4, hookCtx, false, context))
      target4 = this.MinPitch;
    target.MinPitch = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxPitch, ref target5, hookCtx, false, context))
      target5 = this.MaxPitch;
    target.MaxPitch = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParalyzeOnPullAttemptComponent target,
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
    ParalyzeOnPullAttemptComponent target1 = (ParalyzeOnPullAttemptComponent) target;
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
    ParalyzeOnPullAttemptComponent target1 = (ParalyzeOnPullAttemptComponent) target;
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
    ParalyzeOnPullAttemptComponent target1 = (ParalyzeOnPullAttemptComponent) target;
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
  virtual ParalyzeOnPullAttemptComponent Component.Instantiate()
  {
    return new ParalyzeOnPullAttemptComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ParalyzeOnPullAttemptComponent_AutoState : IComponentState
  {
    public TimeSpan Duration;
    public SoundSpecifier? Sound;
    public float MinPitch;
    public float MaxPitch;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ParalyzeOnPullAttemptComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ParalyzeOnPullAttemptComponent, ComponentGetState>(new ComponentEventRefHandler<ParalyzeOnPullAttemptComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ParalyzeOnPullAttemptComponent, ComponentHandleState>(new ComponentEventRefHandler<ParalyzeOnPullAttemptComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ParalyzeOnPullAttemptComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ParalyzeOnPullAttemptComponent.ParalyzeOnPullAttemptComponent_AutoState()
      {
        Duration = component.Duration,
        Sound = component.Sound,
        MinPitch = component.MinPitch,
        MaxPitch = component.MaxPitch
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ParalyzeOnPullAttemptComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ParalyzeOnPullAttemptComponent.ParalyzeOnPullAttemptComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.Sound = current.Sound;
      component.MinPitch = current.MinPitch;
      component.MaxPitch = current.MaxPitch;
    }
  }
}
