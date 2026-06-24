// Decompiled with JetBrains decompiler
// Type: Content.Shared.Beeper.Components.BeeperComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Beeper.Systems;
using Content.Shared.FixedPoint;
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
namespace Content.Shared.Beeper.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (BeeperSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class BeeperComponent : 
  Component,
  ISerializationGenerated<BeeperComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public FixedPoint2 IntervalScaling = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan MaxBeepInterval = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan MinBeepInterval = TimeSpan.FromSeconds(0.25);
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan Interval;
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan LastBeepTime;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool IsMuted;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public SoundSpecifier? BeepSound;

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextBeep
  {
    get
    {
      return !(this.LastBeepTime == TimeSpan.MaxValue) ? this.LastBeepTime + this.Interval : TimeSpan.MaxValue;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BeeperComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BeeperComponent) component;
    if (serialization.TryCustomCopy<BeeperComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.IntervalScaling, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.IntervalScaling, hookCtx, context, false);
    target.IntervalScaling = fixedPoint2;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxBeepInterval, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.MaxBeepInterval, hookCtx, context, false);
    target.MaxBeepInterval = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinBeepInterval, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.MinBeepInterval, hookCtx, context, false);
    target.MinBeepInterval = timeSpan2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IsMuted, ref flag, hookCtx, false, context))
      flag = this.IsMuted;
    target.IsMuted = flag;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BeepSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.BeepSound, hookCtx, context, false);
    target.BeepSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BeeperComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BeeperComponent target1 = (BeeperComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BeeperComponent target1 = (BeeperComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BeeperComponent target1 = (BeeperComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BeeperComponent Component.Instantiate() => new BeeperComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BeeperComponent_AutoState : IComponentState
  {
    public FixedPoint2 IntervalScaling;
    public TimeSpan MaxBeepInterval;
    public TimeSpan MinBeepInterval;
    public bool IsMuted;
    public SoundSpecifier? BeepSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BeeperComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BeeperComponent, ComponentGetState>(new ComponentEventRefHandler<BeeperComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BeeperComponent, ComponentHandleState>(new ComponentEventRefHandler<BeeperComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, BeeperComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new BeeperComponent.BeeperComponent_AutoState()
      {
        IntervalScaling = component.IntervalScaling,
        MaxBeepInterval = component.MaxBeepInterval,
        MinBeepInterval = component.MinBeepInterval,
        IsMuted = component.IsMuted,
        BeepSound = component.BeepSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BeeperComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is BeeperComponent.BeeperComponent_AutoState current))
        return;
      component.IntervalScaling = current.IntervalScaling;
      component.MaxBeepInterval = current.MaxBeepInterval;
      component.MinBeepInterval = current.MinBeepInterval;
      component.IsMuted = current.IsMuted;
      component.BeepSound = current.BeepSound;
    }
  }
}
