// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.PartAssemblySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Components;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction;

public sealed class PartAssemblySystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private TagSystem _tag;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PartAssemblyComponent, ComponentInit>(new ComponentEventHandler<PartAssemblyComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PartAssemblyComponent, InteractUsingEvent>(new ComponentEventHandler<PartAssemblyComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PartAssemblyComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PartAssemblyComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnInit(EntityUid uid, PartAssemblyComponent component, ComponentInit args)
  {
    component.PartsContainer = this._container.EnsureContainer<Container>(uid, component.ContainerId, (ContainerManagerComponent) null);
  }

  private void OnInteractUsing(
    EntityUid uid,
    PartAssemblyComponent component,
    InteractUsingEvent args)
  {
    if (!this.TryInsertPart(args.Used, uid, component))
      return;
    args.Handled = true;
  }

  private void OnEntRemoved(
    EntityUid uid,
    PartAssemblyComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (((ContainerModifiedMessage) args).Container.ID != component.ContainerId || ((BaseContainer) component.PartsContainer).ContainedEntities.Count != 0)
      return;
    component.CurrentAssembly = (string) null;
  }

  public bool TryInsertPart(EntityUid part, EntityUid uid, PartAssemblyComponent? component = null)
  {
    if (!this.Resolve<PartAssemblyComponent>(uid, ref component, true))
      return false;
    string assemblyId = (string) null ?? component.CurrentAssembly;
    if (assemblyId == null)
    {
      foreach ((string key, List<string> stringList) in component.Parts)
      {
        foreach (string str in stringList)
        {
          if (this._tag.HasTag(part, ProtoId<TagPrototype>.op_Implicit(str)))
          {
            assemblyId = key;
            break;
          }
        }
        if (assemblyId != null)
          break;
      }
    }
    if (assemblyId == null || !this.IsPartValid(uid, part, assemblyId, component))
      return false;
    component.CurrentAssembly = assemblyId;
    this._container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(part), (BaseContainer) component.PartsContainer, (TransformComponent) null, false);
    PartAssemblyPartInsertedEvent partInsertedEvent = new PartAssemblyPartInsertedEvent();
    this.RaiseLocalEvent<PartAssemblyPartInsertedEvent>(uid, partInsertedEvent, false);
    return true;
  }

  public bool IsPartValid(
    EntityUid uid,
    EntityUid part,
    string assemblyId,
    PartAssemblyComponent? component = null)
  {
    if (!this.Resolve<PartAssemblyComponent>(uid, ref component, false))
      return true;
    List<string> collection;
    if (!component.Parts.TryGetValue(assemblyId, out collection))
      return false;
    List<string> stringList = new List<string>((IEnumerable<string>) collection);
    List<EntityUid> entityUidList = new List<EntityUid>((IEnumerable<EntityUid>) ((BaseContainer) component.PartsContainer).ContainedEntities);
    foreach (string str in collection)
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ((BaseContainer) component.PartsContainer).ContainedEntities)
      {
        if (entityUidList.Contains(containedEntity) && this._tag.HasTag(containedEntity, ProtoId<TagPrototype>.op_Implicit(str)))
        {
          stringList.Remove(str);
          entityUidList.Remove(containedEntity);
          break;
        }
      }
    }
    foreach (string str in stringList)
    {
      if (this._tag.HasTag(part, ProtoId<TagPrototype>.op_Implicit(str)))
        return true;
    }
    return false;
  }

  public bool IsAssemblyFinished(EntityUid uid, string assemblyId, PartAssemblyComponent? component = null)
  {
    if (!this.Resolve<PartAssemblyComponent>(uid, ref component, false))
      return true;
    List<string> stringList;
    if (!component.Parts.TryGetValue(assemblyId, out stringList))
      return false;
    List<EntityUid> collection = new List<EntityUid>((IEnumerable<EntityUid>) ((BaseContainer) component.PartsContainer).ContainedEntities);
    foreach (string str in stringList)
    {
      bool flag = false;
      foreach (EntityUid entityUid in new List<EntityUid>((IEnumerable<EntityUid>) collection))
      {
        if (this._tag.HasTag(entityUid, ProtoId<TagPrototype>.op_Implicit(str)))
        {
          flag = true;
          collection.Remove(entityUid);
          break;
        }
      }
      if (!flag)
        return false;
    }
    return true;
  }
}
