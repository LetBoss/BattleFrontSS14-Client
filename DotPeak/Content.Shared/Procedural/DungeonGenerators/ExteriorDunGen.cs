// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.ExteriorDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class ExteriorDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<ExteriorDunGen>
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<DungeonConfigPrototype> Proto;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExteriorDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ExteriorDunGen>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<DungeonConfigPrototype> target1 = new ProtoId<DungeonConfigPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DungeonConfigPrototype>>(this.Proto, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<DungeonConfigPrototype>>(this.Proto, hookCtx, context);
    target.Proto = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExteriorDunGen target,
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
    ExteriorDunGen target1 = (ExteriorDunGen) target;
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
    ExteriorDunGen target1 = (ExteriorDunGen) target;
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
  public ExteriorDunGen Instantiate() => new ExteriorDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
