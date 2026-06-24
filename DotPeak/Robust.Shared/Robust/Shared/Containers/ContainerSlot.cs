// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.ContainerSlot
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Containers;

[SerializedType("ContainerSlot")]
public sealed class ContainerSlot : 
  BaseContainer,
  ISerializationGenerated<ContainerSlot>,
  ISerializationGenerated
{
  [NonSerialized]
  private EntityUid? _containedEntity;
  [NonSerialized]
  private EntityUid[]? _containedEntityArray;

  public override int Count => this.ContainedEntity.HasValue ? 1 : 0;

  public override IReadOnlyList<EntityUid> ContainedEntities
  {
    get
    {
      if (!this._containedEntity.HasValue)
        return (IReadOnlyList<EntityUid>) Array.Empty<EntityUid>();
      if (this._containedEntityArray == null)
        this._containedEntityArray = new EntityUid[1]
        {
          this._containedEntity.Value
        };
      return (IReadOnlyList<EntityUid>) this._containedEntityArray;
    }
  }

  [DataField("ent", false, 1, false, false, null)]
  public EntityUid? ContainedEntity
  {
    get => this._containedEntity;
    private set
    {
      this._containedEntity = value;
      if (!value.HasValue)
        return;
      if (this._containedEntityArray == null)
        this._containedEntityArray = new EntityUid[1];
      this._containedEntityArray[0] = value.Value;
    }
  }

  public override bool Contains(EntityUid contained)
  {
    EntityUid entityUid = contained;
    EntityUid? containedEntity = this.ContainedEntity;
    return (containedEntity.HasValue ? (entityUid != containedEntity.GetValueOrDefault() ? 1 : 0) : 1) == 0;
  }

  protected internal override bool CanInsert(
    EntityUid toInsert,
    bool assumeEmpty,
    IEntityManager entMan)
  {
    return !this.ContainedEntity.HasValue | assumeEmpty;
  }

  protected internal override void InternalInsert(EntityUid toInsert, IEntityManager entMan)
  {
    this.ContainedEntity = new EntityUid?(toInsert);
  }

  protected internal override void InternalRemove(EntityUid toRemove, IEntityManager entMan)
  {
    this.ContainedEntity = new EntityUid?();
  }

  protected internal override void InternalShutdown(
    IEntityManager entMan,
    SharedContainerSystem system,
    bool isClient)
  {
    EntityUid? containedEntity = this.ContainedEntity;
    if (!containedEntity.HasValue)
      return;
    EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
    if (!isClient)
    {
      entMan.DeleteEntity(new EntityUid?(valueOrDefault));
    }
    else
    {
      if (!entMan.EntityExists(valueOrDefault))
        return;
      system.Remove((Entity<TransformComponent, MetaDataComponent>) valueOrDefault, (BaseContainer) this, false, true);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContainerSlot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseContainer target1 = (BaseContainer) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ContainerSlot) target1;
    if (serialization.TryCustomCopy<ContainerSlot>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ContainedEntity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.ContainedEntity, hookCtx, context);
    target.ContainedEntity = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContainerSlot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseContainer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ContainerSlot target1 = (ContainerSlot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseContainer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ContainerSlot target1 = (ContainerSlot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ContainerSlot BaseContainer.Instantiate() => new ContainerSlot();
}
