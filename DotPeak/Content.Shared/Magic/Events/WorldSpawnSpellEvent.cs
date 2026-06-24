// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.WorldSpawnSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Storage;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class WorldSpawnSpellEvent : 
  WorldTargetActionEvent,
  ISerializationGenerated<WorldSpawnSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntitySpawnEntry> Prototypes = new List<EntitySpawnEntry>();
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Offset;
  [DataField(null, false, 1, false, false, null)]
  public float? Lifetime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WorldSpawnSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldTargetActionEvent target1 = (WorldTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WorldSpawnSpellEvent) target1;
    if (serialization.TryCustomCopy<WorldSpawnSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    List<EntitySpawnEntry> target2 = (List<EntitySpawnEntry>) null;
    if (this.Prototypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Prototypes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Prototypes, hookCtx, context);
    target.Prototypes = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target3;
    float? target4 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Lifetime, ref target4, hookCtx, false, context))
      target4 = this.Lifetime;
    target.Lifetime = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WorldSpawnSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref WorldTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldSpawnSpellEvent target1 = (WorldSpawnSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (WorldTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldSpawnSpellEvent target1 = (WorldSpawnSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WorldSpawnSpellEvent WorldTargetActionEvent.Instantiate() => new WorldSpawnSpellEvent();
}
