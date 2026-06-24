// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.Boons.HiveBoonEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Hive;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[DataDefinition]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
[Virtual]
[Serializable]
public class HiveBoonEvent : ISerializationGenerated<HiveBoonEvent>, ISerializationGenerated
{
  [NonSerialized]
  public EntityUid Boon;
  [NonSerialized]
  public Entity<HiveComponent> Hive;
  [NonSerialized]
  public EntityUid? Core;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref HiveBoonEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<HiveBoonEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref HiveBoonEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HiveBoonEvent target1 = (HiveBoonEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual HiveBoonEvent Instantiate() => new HiveBoonEvent();
}
