// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.BaseContainer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Containers;

[ImplicitDataDefinitionForInheritors]
public abstract class BaseContainer : ISerializationGenerated<BaseContainer>, ISerializationGenerated
{
  protected SharedContainerSystem? System;
  [Robust.Shared.ViewVariables.ViewVariables]
  [NonSerialized]
  public List<NetEntity> ExpectedEntities = new List<NetEntity>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedContainerSystem), typeof (ContainerManagerComponent)})]
  [NonSerialized]
  public string ID;
  [NonSerialized]
  protected internal ContainerManagerComponent Manager;

  [Access(new Type[] {typeof (SharedContainerSystem), typeof (ContainerManagerComponent)})]
  internal void Init(
    SharedContainerSystem system,
    string id,
    Entity<ContainerManagerComponent> owner)
  {
    this.ID = id;
    this.Owner = (EntityUid) owner;
    this.Manager = (ContainerManagerComponent) owner;
    this.System = system;
  }

  public abstract IReadOnlyList<EntityUid> ContainedEntities { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  private IReadOnlyList<NetEntity> NetContainedEntities
  {
    get
    {
      return (IReadOnlyList<NetEntity>) this.ContainedEntities.Select<EntityUid, NetEntity>((Func<EntityUid, NetEntity>) (o => IoCManager.Resolve<IEntityManager>().GetNetEntity(o))).ToList<NetEntity>();
    }
  }

  public abstract int Count { get; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("occludes", false, 1, false, false, null)]
  public bool OccludesLight { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid Owner { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("showEnts", false, 1, false, false, null)]
  public bool ShowContents { get; set; }

  public abstract bool Contains(EntityUid contained);

  protected internal virtual bool CanInsert(
    EntityUid toInsert,
    bool assumeEmpty,
    IEntityManager entMan)
  {
    return true;
  }

  [Access(new Type[] {typeof (SharedContainerSystem)})]
  protected internal abstract void InternalInsert(EntityUid toInsert, IEntityManager entMan);

  [Access(new Type[] {typeof (SharedContainerSystem)})]
  protected internal abstract void InternalRemove(EntityUid toRemove, IEntityManager entMan);

  [Access(new Type[] {typeof (SharedContainerSystem)})]
  protected internal abstract void InternalShutdown(
    IEntityManager entMan,
    SharedContainerSystem system,
    bool isClient);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref BaseContainer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BaseContainer>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.OccludesLight, ref target1, hookCtx, false, context))
      target1 = this.OccludesLight;
    target.OccludesLight = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowContents, ref target2, hookCtx, false, context))
      target2 = this.ShowContents;
    target.ShowContents = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref BaseContainer target,
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
    BaseContainer target1 = (BaseContainer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual BaseContainer Instantiate() => throw new NotImplementedException();
}
