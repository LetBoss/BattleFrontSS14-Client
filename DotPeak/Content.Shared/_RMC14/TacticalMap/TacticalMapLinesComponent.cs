// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacticalMapLinesComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedTacticalMapSystem)})]
public sealed class TacticalMapLinesComponent : 
  Component,
  ISerializationGenerated<TacticalMapLinesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<TacticalMapLine> MarineLines = new List<TacticalMapLine>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<TacticalMapLine> XenoLines = new List<TacticalMapLine>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TacticalMapLinesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TacticalMapLinesComponent) target1;
    if (serialization.TryCustomCopy<TacticalMapLinesComponent>(this, ref target, hookCtx, false, context))
      return;
    List<TacticalMapLine> target2 = (List<TacticalMapLine>) null;
    if (this.MarineLines == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TacticalMapLine>>(this.MarineLines, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<TacticalMapLine>>(this.MarineLines, hookCtx, context);
    target.MarineLines = target2;
    List<TacticalMapLine> target3 = (List<TacticalMapLine>) null;
    if (this.XenoLines == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TacticalMapLine>>(this.XenoLines, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<TacticalMapLine>>(this.XenoLines, hookCtx, context);
    target.XenoLines = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TacticalMapLinesComponent target,
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
    TacticalMapLinesComponent target1 = (TacticalMapLinesComponent) target;
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
    TacticalMapLinesComponent target1 = (TacticalMapLinesComponent) target;
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
    TacticalMapLinesComponent target1 = (TacticalMapLinesComponent) target;
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
  virtual TacticalMapLinesComponent Component.Instantiate() => new TacticalMapLinesComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TacticalMapLinesComponent_AutoState : IComponentState
  {
    public List<TacticalMapLine> MarineLines;
    public List<TacticalMapLine> XenoLines;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapLinesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapLinesComponent, ComponentGetState>(new ComponentEventRefHandler<TacticalMapLinesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TacticalMapLinesComponent, ComponentHandleState>(new ComponentEventRefHandler<TacticalMapLinesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TacticalMapLinesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TacticalMapLinesComponent.TacticalMapLinesComponent_AutoState()
      {
        MarineLines = component.MarineLines,
        XenoLines = component.XenoLines
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TacticalMapLinesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TacticalMapLinesComponent.TacticalMapLinesComponent_AutoState current))
        return;
      component.MarineLines = current.MarineLines == null ? (List<TacticalMapLine>) null : new List<TacticalMapLine>((IEnumerable<TacticalMapLine>) current.MarineLines);
      component.XenoLines = current.XenoLines == null ? (List<TacticalMapLine>) null : new List<TacticalMapLine>((IEnumerable<TacticalMapLine>) current.XenoLines);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, TacticalMapLinesComponent>(uid, component, ref args1);
    }
  }
}
