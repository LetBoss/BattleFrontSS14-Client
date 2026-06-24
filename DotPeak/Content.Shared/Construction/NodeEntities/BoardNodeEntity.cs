// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.NodeEntities.BoardNodeEntity
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Construction.NodeEntities;

[DataDefinition]
public sealed class BoardNodeEntity : 
  IGraphNodeEntity,
  ISerializationGenerated<BoardNodeEntity>,
  ISerializationGenerated
{
  [DataField("container", false, 1, false, false, null)]
  public string Container { get; private set; } = string.Empty;

  public string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args)
  {
    if (!uid.HasValue)
      return (string) null;
    BaseContainer baseContainer;
    if (!args.EntityManager.EntitySysManager.GetEntitySystem<SharedContainerSystem>().TryGetContainer(uid.Value, this.Container, ref baseContainer, (ContainerManagerComponent) null) || baseContainer.ContainedEntities.Count == 0)
      return (string) null;
    EntityUid containedEntity = baseContainer.ContainedEntities[0];
    MachineBoardComponent machineBoardComponent;
    if (args.EntityManager.TryGetComponent<MachineBoardComponent>(containedEntity, ref machineBoardComponent))
      return EntProtoId.op_Implicit(machineBoardComponent.Prototype);
    ComputerBoardComponent computerBoardComponent;
    return args.EntityManager.TryGetComponent<ComputerBoardComponent>(containedEntity, ref computerBoardComponent) ? computerBoardComponent.Prototype : (string) null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BoardNodeEntity target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BoardNodeEntity>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Container == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Container, ref str, hookCtx, false, context))
      str = this.Container;
    target.Container = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BoardNodeEntity target,
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
    BoardNodeEntity target1 = (BoardNodeEntity) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public BoardNodeEntity Instantiate() => new BoardNodeEntity();
}
