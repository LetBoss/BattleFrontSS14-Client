// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Light.RMCAmbientLightComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Light;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCAmbientLightComponent : 
  Component,
  ISerializationGenerated<RMCAmbientLightComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsAnimating;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StartTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<Color> Colors = new List<Color>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan EndTime => this.StartTime + this.Duration;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCAmbientLightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCAmbientLightComponent) target1;
    if (serialization.TryCustomCopy<RMCAmbientLightComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAnimating, ref target2, hookCtx, false, context))
      target2 = this.IsAnimating;
    target.IsAnimating = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StartTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.StartTime, hookCtx, context);
    target.StartTime = target4;
    List<Color> target5 = (List<Color>) null;
    if (this.Colors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Color>>(this.Colors, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<Color>>(this.Colors, hookCtx, context);
    target.Colors = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCAmbientLightComponent target,
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
    RMCAmbientLightComponent target1 = (RMCAmbientLightComponent) target;
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
    RMCAmbientLightComponent target1 = (RMCAmbientLightComponent) target;
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
    RMCAmbientLightComponent target1 = (RMCAmbientLightComponent) target;
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
  virtual RMCAmbientLightComponent Component.Instantiate() => new RMCAmbientLightComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCAmbientLightComponent_AutoState : IComponentState
  {
    public bool IsAnimating;
    public TimeSpan Duration;
    public TimeSpan StartTime;
    public List<Color> Colors;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCAmbientLightComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCAmbientLightComponent, ComponentGetState>(new ComponentEventRefHandler<RMCAmbientLightComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCAmbientLightComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCAmbientLightComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCAmbientLightComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCAmbientLightComponent.RMCAmbientLightComponent_AutoState()
      {
        IsAnimating = component.IsAnimating,
        Duration = component.Duration,
        StartTime = component.StartTime,
        Colors = component.Colors
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCAmbientLightComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCAmbientLightComponent.RMCAmbientLightComponent_AutoState current))
        return;
      component.IsAnimating = current.IsAnimating;
      component.Duration = current.Duration;
      component.StartTime = current.StartTime;
      component.Colors = current.Colors == null ? (List<Color>) null : new List<Color>((IEnumerable<Color>) current.Colors);
    }
  }
}
