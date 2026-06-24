// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clock.ClockComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clock;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedClockSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class ClockComponent : 
  Component,
  ISerializationGenerated<ClockComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? StuckTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ClockType ClockType;
  [DataField(null, false, 1, false, false, null)]
  public string HoursBase = "hours_";
  [DataField(null, false, 1, false, false, null)]
  public string MinutesBase = "minutes_";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ClockComponent) component;
    if (serialization.TryCustomCopy<ClockComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan? nullable = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.StuckTime, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<TimeSpan?>(this.StuckTime, hookCtx, context, false);
    target.StuckTime = nullable;
    ClockType clockType = ClockType.TwelveHour;
    if (!serialization.TryCustomCopy<ClockType>(this.ClockType, ref clockType, hookCtx, false, context))
      clockType = this.ClockType;
    target.ClockType = clockType;
    string str1 = (string) null;
    if (this.HoursBase == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HoursBase, ref str1, hookCtx, false, context))
      str1 = this.HoursBase;
    target.HoursBase = str1;
    string str2 = (string) null;
    if (this.MinutesBase == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MinutesBase, ref str2, hookCtx, false, context))
      str2 = this.MinutesBase;
    target.MinutesBase = str2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClockComponent target,
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
    ClockComponent target1 = (ClockComponent) target;
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
    ClockComponent target1 = (ClockComponent) target;
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
    ClockComponent target1 = (ClockComponent) target;
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
  virtual ClockComponent Component.Instantiate() => new ClockComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ClockComponent_AutoState : IComponentState
  {
    public TimeSpan? StuckTime;
    public ClockType ClockType;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ClockComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClockComponent, ComponentGetState>(new ComponentEventRefHandler<ClockComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClockComponent, ComponentHandleState>(new ComponentEventRefHandler<ClockComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ClockComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ClockComponent.ClockComponent_AutoState()
      {
        StuckTime = component.StuckTime,
        ClockType = component.ClockType
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ClockComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ClockComponent.ClockComponent_AutoState current))
        return;
      component.StuckTime = current.StuckTime;
      component.ClockType = current.ClockType;
    }
  }
}
