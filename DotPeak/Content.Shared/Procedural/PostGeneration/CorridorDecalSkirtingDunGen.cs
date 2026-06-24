// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.CorridorDecalSkirtingDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.PostGeneration;

public sealed class CorridorDecalSkirtingDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<CorridorDecalSkirtingDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<DirectionFlag, string> CardinalDecals = new Dictionary<DirectionFlag, string>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<Direction, string> PocketDecals = new Dictionary<Direction, string>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<DirectionFlag, string> CornerDecals = new Dictionary<DirectionFlag, string>();
  [DataField(null, false, 1, false, false, null)]
  public Robust.Shared.Maths.Color? Color;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CorridorDecalSkirtingDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CorridorDecalSkirtingDunGen>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<DirectionFlag, string> target1 = (Dictionary<DirectionFlag, string>) null;
    if (this.CardinalDecals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<DirectionFlag, string>>(this.CardinalDecals, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<Dictionary<DirectionFlag, string>>(this.CardinalDecals, hookCtx, context);
    target.CardinalDecals = target1;
    Dictionary<Direction, string> target2 = (Dictionary<Direction, string>) null;
    if (this.PocketDecals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Direction, string>>(this.PocketDecals, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Direction, string>>(this.PocketDecals, hookCtx, context);
    target.PocketDecals = target2;
    Dictionary<DirectionFlag, string> target3 = (Dictionary<DirectionFlag, string>) null;
    if (this.CornerDecals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<DirectionFlag, string>>(this.CornerDecals, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<DirectionFlag, string>>(this.CornerDecals, hookCtx, context);
    target.CornerDecals = target3;
    Robust.Shared.Maths.Color? target4 = new Robust.Shared.Maths.Color?();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color?>(this.Color, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Robust.Shared.Maths.Color?>(this.Color, hookCtx, context);
    target.Color = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CorridorDecalSkirtingDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CorridorDecalSkirtingDunGen target1 = (CorridorDecalSkirtingDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CorridorDecalSkirtingDunGen target1 = (CorridorDecalSkirtingDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDunGenLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CorridorDecalSkirtingDunGen Instantiate() => new CorridorDecalSkirtingDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
