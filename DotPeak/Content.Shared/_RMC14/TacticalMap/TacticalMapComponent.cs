// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacticalMapComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedTacticalMapSystem)})]
public sealed class TacticalMapComponent : 
  Component,
  ISerializationGenerated<TacticalMapComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextUpdate = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, TacticalMapBlip> MarineBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, TacticalMapBlip> LastUpdateMarineBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, TacticalMapBlip> XenoBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, TacticalMapBlip> XenoStructureBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, TacticalMapBlip> LastUpdateXenoBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, TacticalMapBlip> LastUpdateXenoStructureBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  public List<TacticalMapLine> MarineLines = new List<TacticalMapLine>();
  [DataField(null, false, 1, false, false, null)]
  public List<TacticalMapLine> XenoLines = new List<TacticalMapLine>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<Vector2i, string> MarineLabels = new Dictionary<Vector2i, string>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<Vector2i, string> XenoLabels = new Dictionary<Vector2i, string>();
  [DataField(null, false, 1, false, false, null)]
  public bool MapDirty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TacticalMapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TacticalMapComponent) target1;
    if (serialization.TryCustomCopy<TacticalMapComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context);
    target.NextUpdate = target2;
    Dictionary<int, TacticalMapBlip> target3 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.MarineBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.MarineBlips, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.MarineBlips, hookCtx, context);
    target.MarineBlips = target3;
    Dictionary<int, TacticalMapBlip> target4 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.LastUpdateMarineBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.LastUpdateMarineBlips, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.LastUpdateMarineBlips, hookCtx, context);
    target.LastUpdateMarineBlips = target4;
    Dictionary<int, TacticalMapBlip> target5 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.XenoBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.XenoBlips, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.XenoBlips, hookCtx, context);
    target.XenoBlips = target5;
    Dictionary<int, TacticalMapBlip> target6 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.XenoStructureBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.XenoStructureBlips, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.XenoStructureBlips, hookCtx, context);
    target.XenoStructureBlips = target6;
    Dictionary<int, TacticalMapBlip> target7 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.LastUpdateXenoBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.LastUpdateXenoBlips, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.LastUpdateXenoBlips, hookCtx, context);
    target.LastUpdateXenoBlips = target7;
    Dictionary<int, TacticalMapBlip> target8 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.LastUpdateXenoStructureBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.LastUpdateXenoStructureBlips, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.LastUpdateXenoStructureBlips, hookCtx, context);
    target.LastUpdateXenoStructureBlips = target8;
    List<TacticalMapLine> target9 = (List<TacticalMapLine>) null;
    if (this.MarineLines == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TacticalMapLine>>(this.MarineLines, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<TacticalMapLine>>(this.MarineLines, hookCtx, context);
    target.MarineLines = target9;
    List<TacticalMapLine> target10 = (List<TacticalMapLine>) null;
    if (this.XenoLines == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TacticalMapLine>>(this.XenoLines, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<TacticalMapLine>>(this.XenoLines, hookCtx, context);
    target.XenoLines = target10;
    Dictionary<Vector2i, string> target11 = (Dictionary<Vector2i, string>) null;
    if (this.MarineLabels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, string>>(this.MarineLabels, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Dictionary<Vector2i, string>>(this.MarineLabels, hookCtx, context);
    target.MarineLabels = target11;
    Dictionary<Vector2i, string> target12 = (Dictionary<Vector2i, string>) null;
    if (this.XenoLabels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, string>>(this.XenoLabels, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<Dictionary<Vector2i, string>>(this.XenoLabels, hookCtx, context);
    target.XenoLabels = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.MapDirty, ref target13, hookCtx, false, context))
      target13 = this.MapDirty;
    target.MapDirty = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TacticalMapComponent target,
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
    TacticalMapComponent target1 = (TacticalMapComponent) target;
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
    TacticalMapComponent target1 = (TacticalMapComponent) target;
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
    TacticalMapComponent target1 = (TacticalMapComponent) target;
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
  virtual TacticalMapComponent Component.Instantiate() => new TacticalMapComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TacticalMapComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TacticalMapComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdate += args.PausedTime;
    }
  }
}
