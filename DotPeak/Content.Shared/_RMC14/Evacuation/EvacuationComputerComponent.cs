// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Evacuation.EvacuationComputerComponent
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
namespace Content.Shared._RMC14.Evacuation;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedEvacuationSystem)})]
public sealed class EvacuationComputerComponent : 
  Component,
  ISerializationGenerated<EvacuationComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EvacuationComputerMode Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MaxMobs;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? WarmupSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/escape_pod_warmup.ogg", new AudioParams?(AudioParams.Default.WithVolume(-4f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LaunchSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/escape_pod_launch.ogg", new AudioParams?(AudioParams.Default.WithVolume(-4f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DetonateDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EjectDelay = TimeSpan.FromSeconds(5.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EarlyCrashChance = 0.75f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EvacuationComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EvacuationComputerComponent) target1;
    if (serialization.TryCustomCopy<EvacuationComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    EvacuationComputerMode target2 = EvacuationComputerMode.Disabled;
    if (!serialization.TryCustomCopy<EvacuationComputerMode>(this.Mode, ref target2, hookCtx, false, context))
      target2 = this.Mode;
    target.Mode = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxMobs, ref target3, hookCtx, false, context))
      target3 = this.MaxMobs;
    target.MaxMobs = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WarmupSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.WarmupSound, hookCtx, context);
    target.WarmupSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LaunchSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.LaunchSound, hookCtx, context);
    target.LaunchSound = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DetonateDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DetonateDelay, hookCtx, context);
    target.DetonateDelay = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EjectDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.EjectDelay, hookCtx, context);
    target.EjectDelay = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EarlyCrashChance, ref target8, hookCtx, false, context))
      target8 = this.EarlyCrashChance;
    target.EarlyCrashChance = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EvacuationComputerComponent target,
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
    EvacuationComputerComponent target1 = (EvacuationComputerComponent) target;
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
    EvacuationComputerComponent target1 = (EvacuationComputerComponent) target;
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
    EvacuationComputerComponent target1 = (EvacuationComputerComponent) target;
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
  virtual EvacuationComputerComponent Component.Instantiate() => new EvacuationComputerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EvacuationComputerComponent_AutoState : IComponentState
  {
    public EvacuationComputerMode Mode;
    public int? MaxMobs;
    public SoundSpecifier? WarmupSound;
    public SoundSpecifier? LaunchSound;
    public TimeSpan DetonateDelay;
    public TimeSpan EjectDelay;
    public float EarlyCrashChance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvacuationComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EvacuationComputerComponent, ComponentGetState>(new ComponentEventRefHandler<EvacuationComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EvacuationComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<EvacuationComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EvacuationComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EvacuationComputerComponent.EvacuationComputerComponent_AutoState()
      {
        Mode = component.Mode,
        MaxMobs = component.MaxMobs,
        WarmupSound = component.WarmupSound,
        LaunchSound = component.LaunchSound,
        DetonateDelay = component.DetonateDelay,
        EjectDelay = component.EjectDelay,
        EarlyCrashChance = component.EarlyCrashChance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EvacuationComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EvacuationComputerComponent.EvacuationComputerComponent_AutoState current))
        return;
      component.Mode = current.Mode;
      component.MaxMobs = current.MaxMobs;
      component.WarmupSound = current.WarmupSound;
      component.LaunchSound = current.LaunchSound;
      component.DetonateDelay = current.DetonateDelay;
      component.EjectDelay = current.EjectDelay;
      component.EarlyCrashChance = current.EarlyCrashChance;
    }
  }
}
