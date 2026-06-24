// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.SunShadowCycleComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SunShadowCycleComponent : 
  Component,
  ISerializationGenerated<SunShadowCycleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromMinutes(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Offset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<SunShadowCycleDirection> Directions = new List<SunShadowCycleDirection>()
  {
    new SunShadowCycleDirection(0.0f, new Vector2(0.0f, 3f), 0.0f),
    new SunShadowCycleDirection(0.25f, new Vector2(-3f, -0.1f), 0.5f),
    new SunShadowCycleDirection(0.5f, new Vector2(0.0f, -3f), 0.8f),
    new SunShadowCycleDirection(0.75f, new Vector2(3f, -0.1f), 0.5f)
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SunShadowCycleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SunShadowCycleComponent) target1;
    if (serialization.TryCustomCopy<SunShadowCycleComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Offset, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Offset, hookCtx, context);
    target.Offset = target3;
    List<SunShadowCycleDirection> target4 = (List<SunShadowCycleDirection>) null;
    if (this.Directions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SunShadowCycleDirection>>(this.Directions, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<SunShadowCycleDirection>>(this.Directions, hookCtx, context);
    target.Directions = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SunShadowCycleComponent target,
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
    SunShadowCycleComponent target1 = (SunShadowCycleComponent) target;
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
    SunShadowCycleComponent target1 = (SunShadowCycleComponent) target;
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
    SunShadowCycleComponent target1 = (SunShadowCycleComponent) target;
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
  virtual SunShadowCycleComponent Component.Instantiate() => new SunShadowCycleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SunShadowCycleComponent_AutoState : IComponentState
  {
    public TimeSpan Duration;
    public TimeSpan Offset;
    public List<SunShadowCycleDirection> Directions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SunShadowCycleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SunShadowCycleComponent, ComponentGetState>(new ComponentEventRefHandler<SunShadowCycleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SunShadowCycleComponent, ComponentHandleState>(new ComponentEventRefHandler<SunShadowCycleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SunShadowCycleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SunShadowCycleComponent.SunShadowCycleComponent_AutoState()
      {
        Duration = component.Duration,
        Offset = component.Offset,
        Directions = component.Directions
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SunShadowCycleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SunShadowCycleComponent.SunShadowCycleComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.Offset = current.Offset;
      component.Directions = current.Directions == null ? (List<SunShadowCycleDirection>) null : new List<SunShadowCycleDirection>((IEnumerable<SunShadowCycleDirection>) current.Directions);
    }
  }
}
