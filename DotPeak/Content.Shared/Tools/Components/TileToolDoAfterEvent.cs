// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.TileToolDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[NetSerializable]
[Serializable]
public sealed class TileToolDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<TileToolDoAfterEvent>,
  ISerializationGenerated
{
  public NetEntity Grid;
  public Vector2i GridTile;

  public TileToolDoAfterEvent(NetEntity grid, Vector2i gridTile)
  {
    this.Grid = grid;
    this.GridTile = gridTile;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  public override bool IsDuplicate(DoAfterEvent other)
  {
    return other is TileToolDoAfterEvent toolDoAfterEvent && this.Grid == toolDoAfterEvent.Grid && Vector2i.op_Equality(this.GridTile, toolDoAfterEvent.GridTile);
  }

  public TileToolDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TileToolDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TileToolDoAfterEvent) target1;
    serialization.TryCustomCopy<TileToolDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TileToolDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TileToolDoAfterEvent target1 = (TileToolDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TileToolDoAfterEvent target1 = (TileToolDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TileToolDoAfterEvent DoAfterEvent.Instantiate() => new TileToolDoAfterEvent();
}
