// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.Container
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Containers;

[SerializedType("Container")]
public sealed class Container : 
  BaseContainer,
  ISerializationGenerated<Container>,
  ISerializationGenerated
{
  [DataField("ents", false, 1, false, false, null)]
  [NonSerialized]
  private List<EntityUid> _containerList = new List<EntityUid>();

  public override int Count => this._containerList.Count;

  public override IReadOnlyList<EntityUid> ContainedEntities
  {
    get => (IReadOnlyList<EntityUid>) this._containerList;
  }

  protected internal override void InternalInsert(EntityUid toInsert, IEntityManager entMan)
  {
    this._containerList.Add(toInsert);
  }

  protected internal override void InternalRemove(EntityUid toRemove, IEntityManager entMan)
  {
    this._containerList.Remove(toRemove);
  }

  public override bool Contains(EntityUid contained) => this._containerList.Contains(contained);

  protected internal override void InternalShutdown(
    IEntityManager entMan,
    SharedContainerSystem system,
    bool isClient)
  {
    foreach (EntityUid entityUid in this._containerList.ToArray())
    {
      if (!isClient)
        entMan.DeleteEntity(new EntityUid?(entityUid));
      else if (entMan.EntityExists(entityUid))
        system.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, (BaseContainer) this, false, true);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Container target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseContainer target1 = (BaseContainer) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Container) target1;
    if (serialization.TryCustomCopy<Container>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this._containerList == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this._containerList, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this._containerList, hookCtx, context);
    target._containerList = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Container target,
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
    Container target1 = (Container) target;
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
    Container target1 = (Container) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Container BaseContainer.Instantiate() => new Container();
}
