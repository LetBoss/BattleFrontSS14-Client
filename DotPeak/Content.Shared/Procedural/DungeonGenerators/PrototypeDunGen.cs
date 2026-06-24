// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.PrototypeDunGen
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

public sealed class PrototypeDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<PrototypeDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public DungeonInheritance InheritDungeons;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<DungeonConfigPrototype> Proto;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PrototypeDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PrototypeDunGen>(this, ref target, hookCtx, false, context))
      return;
    DungeonInheritance target1 = DungeonInheritance.None;
    if (!serialization.TryCustomCopy<DungeonInheritance>(this.InheritDungeons, ref target1, hookCtx, false, context))
      target1 = this.InheritDungeons;
    target.InheritDungeons = target1;
    ProtoId<DungeonConfigPrototype> target2 = new ProtoId<DungeonConfigPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DungeonConfigPrototype>>(this.Proto, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<DungeonConfigPrototype>>(this.Proto, hookCtx, context);
    target.Proto = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PrototypeDunGen target,
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
    PrototypeDunGen target1 = (PrototypeDunGen) target;
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
    PrototypeDunGen target1 = (PrototypeDunGen) target;
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
  public PrototypeDunGen Instantiate() => new PrototypeDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
