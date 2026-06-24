// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacticalMapLabelsComponent
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
namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedTacticalMapSystem)})]
public sealed class TacticalMapLabelsComponent : 
  Component,
  ISerializationGenerated<TacticalMapLabelsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Vector2i, string> MarineLabels = new Dictionary<Vector2i, string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Vector2i, string> XenoLabels = new Dictionary<Vector2i, string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TacticalMapLabelsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TacticalMapLabelsComponent) target1;
    if (serialization.TryCustomCopy<TacticalMapLabelsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Vector2i, string> target2 = (Dictionary<Vector2i, string>) null;
    if (this.MarineLabels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, string>>(this.MarineLabels, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Vector2i, string>>(this.MarineLabels, hookCtx, context);
    target.MarineLabels = target2;
    Dictionary<Vector2i, string> target3 = (Dictionary<Vector2i, string>) null;
    if (this.XenoLabels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, string>>(this.XenoLabels, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<Vector2i, string>>(this.XenoLabels, hookCtx, context);
    target.XenoLabels = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TacticalMapLabelsComponent target,
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
    TacticalMapLabelsComponent target1 = (TacticalMapLabelsComponent) target;
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
    TacticalMapLabelsComponent target1 = (TacticalMapLabelsComponent) target;
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
    TacticalMapLabelsComponent target1 = (TacticalMapLabelsComponent) target;
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
  virtual TacticalMapLabelsComponent Component.Instantiate() => new TacticalMapLabelsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TacticalMapLabelsComponent_AutoState : IComponentState
  {
    public Dictionary<Vector2i, string> MarineLabels;
    public Dictionary<Vector2i, string> XenoLabels;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapLabelsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapLabelsComponent, ComponentGetState>(new ComponentEventRefHandler<TacticalMapLabelsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TacticalMapLabelsComponent, ComponentHandleState>(new ComponentEventRefHandler<TacticalMapLabelsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TacticalMapLabelsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TacticalMapLabelsComponent.TacticalMapLabelsComponent_AutoState()
      {
        MarineLabels = component.MarineLabels,
        XenoLabels = component.XenoLabels
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TacticalMapLabelsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TacticalMapLabelsComponent.TacticalMapLabelsComponent_AutoState current))
        return;
      component.MarineLabels = current.MarineLabels == null ? (Dictionary<Vector2i, string>) null : new Dictionary<Vector2i, string>((IDictionary<Vector2i, string>) current.MarineLabels);
      component.XenoLabels = current.XenoLabels == null ? (Dictionary<Vector2i, string>) null : new Dictionary<Vector2i, string>((IDictionary<Vector2i, string>) current.XenoLabels);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, TacticalMapLabelsComponent>(uid, component, ref args1);
    }
  }
}
