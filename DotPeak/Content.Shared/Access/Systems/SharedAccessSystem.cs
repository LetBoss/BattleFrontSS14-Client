// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.SharedAccessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Access.Systems;

public abstract class SharedAccessSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessComponent, MapInitEvent>(new ComponentEventHandler<AccessComponent, MapInitEvent>((object) this, __methodptr(OnAccessInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessComponent, GetAccessTagsEvent>(new ComponentEventRefHandler<AccessComponent, GetAccessTagsEvent>((object) this, __methodptr(OnGetAccessTags)), (Type[]) null, (Type[]) null);
  }

  private void OnAccessInit(EntityUid uid, AccessComponent component, MapInitEvent args)
  {
    foreach (ProtoId<AccessGroupPrototype> group in component.Groups)
    {
      AccessGroupPrototype accessGroupPrototype;
      if (this._prototypeManager.TryIndex<AccessGroupPrototype>(group, ref accessGroupPrototype))
      {
        component.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) accessGroupPrototype.Tags);
        this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      }
    }
  }

  private void OnGetAccessTags(
    EntityUid uid,
    AccessComponent component,
    ref GetAccessTagsEvent args)
  {
    if (!component.Enabled)
      return;
    args.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) component.Tags);
  }

  public void SetAccessEnabled(EntityUid uid, bool val, AccessComponent? component = null)
  {
    if (!this.Resolve<AccessComponent>(uid, ref component, false))
      return;
    component.Enabled = val;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  public bool TrySetTags(
    EntityUid uid,
    IEnumerable<ProtoId<AccessLevelPrototype>> newTags,
    AccessComponent? access = null)
  {
    if (!this.Resolve<AccessComponent>(uid, ref access, true))
      return false;
    access.Tags.Clear();
    access.Tags.UnionWith(newTags);
    this.Dirty(uid, (IComponent) access, (MetaDataComponent) null);
    return true;
  }

  public IEnumerable<ProtoId<AccessLevelPrototype>>? TryGetTags(
    EntityUid uid,
    AccessComponent? access = null)
  {
    return this.Resolve<AccessComponent>(uid, ref access, true) ? (IEnumerable<ProtoId<AccessLevelPrototype>>) access.Tags : (IEnumerable<ProtoId<AccessLevelPrototype>>) null;
  }

  public bool TryAddGroups(
    EntityUid uid,
    IEnumerable<ProtoId<AccessGroupPrototype>> newGroups,
    AccessComponent? access = null)
  {
    if (!this.Resolve<AccessComponent>(uid, ref access, true))
      return false;
    foreach (ProtoId<AccessGroupPrototype> newGroup in newGroups)
    {
      AccessGroupPrototype accessGroupPrototype;
      if (this._prototypeManager.TryIndex<AccessGroupPrototype>(newGroup, ref accessGroupPrototype))
        access.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) accessGroupPrototype.Tags);
    }
    this.Dirty(uid, (IComponent) access, (MetaDataComponent) null);
    return true;
  }

  public void SetAccessToJob(
    EntityUid uid,
    JobPrototype prototype,
    bool extended,
    AccessComponent? access = null)
  {
    if (!this.Resolve<AccessComponent>(uid, ref access, true))
      return;
    access.Tags.Clear();
    access.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) prototype.Access);
    this.Dirty(uid, (IComponent) access, (MetaDataComponent) null);
    this.TryAddGroups(uid, (IEnumerable<ProtoId<AccessGroupPrototype>>) prototype.AccessGroups, access);
    if (!extended)
      return;
    access.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) prototype.ExtendedAccess);
    this.TryAddGroups(uid, (IEnumerable<ProtoId<AccessGroupPrototype>>) prototype.ExtendedAccessGroups, access);
  }
}
