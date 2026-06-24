// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.NodeEntities.StaticNodeEntity
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;

#nullable enable
namespace Content.Shared.Construction.NodeEntities;

[DataDefinition]
public sealed class StaticNodeEntity : 
  IGraphNodeEntity,
  ISerializationGenerated<StaticNodeEntity>,
  ISerializationGenerated
{
  [DataField("id", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? Id { get; private set; }

  public StaticNodeEntity()
  {
  }

  public StaticNodeEntity(string id) => this.Id = id;

  public string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args) => this.Id;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StaticNodeEntity target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<StaticNodeEntity>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Id, ref str, hookCtx, false, context))
      str = this.Id;
    target.Id = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StaticNodeEntity target,
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
    StaticNodeEntity target1 = (StaticNodeEntity) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public StaticNodeEntity Instantiate() => new StaticNodeEntity();
}
