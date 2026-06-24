// Decompiled with JetBrains decompiler
// Type: Content.Shared.IdentityManagement.Identity
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.Ghost;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Shared.IdentityManagement;

public static class Identity
{
  public static IdentityEntity Name(EntityUid uid, IEntityManager ent, EntityUid? viewer = null)
  {
    if (!uid.IsValid())
      return new IdentityEntity(uid, string.Empty);
    MetaDataComponent component1 = ent.GetComponent<MetaDataComponent>(uid);
    if (component1.EntityLifeStage <= EntityLifeStage.Initializing)
      return new IdentityEntity(uid, component1.EntityName);
    string entityName1 = component1.EntityName;
    EntityWhitelistSystem entityWhitelistSystem = ent.System<EntityWhitelistSystem>();
    FixedIdentityComponent component2;
    if (viewer.HasValue && ent.TryGetComponent<FixedIdentityComponent>(uid, out component2))
    {
      LocId? name = component2.Name;
      if (name.HasValue)
      {
        LocId valueOrDefault = name.GetValueOrDefault();
        if (entityWhitelistSystem.IsWhitelistPass(component2.Whitelist, viewer.Value))
        {
          RMCGetFixedIdentityEvent args = new RMCGetFixedIdentityEvent(Loc.GetString((string) valueOrDefault));
          ent.EventBus.RaiseLocalEvent<RMCGetFixedIdentityEvent>(uid, ref args);
          return new IdentityEntity(uid, args.Name);
        }
      }
    }
    IdentityComponent component3;
    if (!ent.TryGetComponent<IdentityComponent>(uid, out component3))
      return new IdentityEntity(uid, entityName1);
    EntityUid? containedEntity = component3.IdentityEntitySlot.ContainedEntity;
    if (!containedEntity.HasValue)
      return new IdentityEntity(uid, entityName1);
    string entityName2 = ent.GetComponent<MetaDataComponent>(containedEntity.Value).EntityName;
    if (!viewer.HasValue || !Identity.CanSeeThroughIdentity(uid, viewer.Value, ent))
      return new IdentityEntity(uid, entityName2);
    return entityName1 == entityName2 ? new IdentityEntity(uid, entityName1) : new IdentityEntity(uid, $"{entityName1} ({entityName2})");
  }

  public static EntityUid Entity(EntityUid uid, IEntityManager ent, EntityUid? viewer = null)
  {
    IdentityComponent component;
    return !ent.TryGetComponent<IdentityComponent>(uid, out component) || viewer.HasValue && Identity.CanSeeThroughIdentity(uid, viewer.Value, ent) ? uid : component.IdentityEntitySlot.ContainedEntity ?? uid;
  }

  public static bool CanSeeThroughIdentity(EntityUid uid, EntityUid viewer, IEntityManager ent)
  {
    return ent.HasComponent<GhostComponent>(viewer);
  }
}
