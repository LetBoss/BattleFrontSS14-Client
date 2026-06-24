// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Access.IdModificationConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using System.Collections.Frozen;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.Access;

public sealed class IdModificationConsoleSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private GunIFFSystem _iff;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedRankSystem _rank;
  [Dependency]
  private ISerializationManager _serialization;
  [Dependency]
  private SquadSystem _squad;
  private FrozenDictionary<string, AccessGroupPrototype> _accessGroup = FrozenDictionary<string, AccessGroupPrototype>.Empty;
  private FrozenDictionary<string, AccessLevelPrototype> _accessLevel = FrozenDictionary<string, AccessLevelPrototype>.Empty;

  public override void Initialize()
  {
    this.Subs.BuiEvents<IdModificationConsoleComponent>((object) IdModificationConsoleUIKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<IdModificationConsoleComponent>) (subs =>
    {
      subs.Event<IdModificationConsoleAccessChangeBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleAccessChangeBuiMsg>(this.OnAccessChangeMsg));
      subs.Event<IdModificationConsoleMultipleAccessChangeBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleMultipleAccessChangeBuiMsg>(this.OnMultipleAccessChangeMsg));
      subs.Event<IdModificationConsoleSignInBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleSignInBuiMsg>(this.OnSignInMsg));
      subs.Event<IdModificationConsoleSignInTargetBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleSignInTargetBuiMsg>(this.OnSignInTargetMsg));
      subs.Event<IdModificationConsoleIFFChangeBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleIFFChangeBuiMsg>(this.OnIFFChangeMsg));
      subs.Event<IdModificationConsoleJobChangeBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleJobChangeBuiMsg>(this.OnJobChangeMsg));
      subs.Event<IdModificationConsoleTerminateConfirmBuiMsg>(new EntityEventRefHandler<IdModificationConsoleComponent, IdModificationConsoleTerminateConfirmBuiMsg>(this.OnTerminateConfirmMsg));
    }));
    this.SubscribeLocalEvent<IdModificationConsoleComponent, MapInitEvent>(new EntityEventRefHandler<IdModificationConsoleComponent, MapInitEvent>(this.OnComponentInit));
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.SubscribeLocalEvent<IdModificationConsoleComponent, InteractUsingEvent>(new EntityEventRefHandler<IdModificationConsoleComponent, InteractUsingEvent>(this.OnInteractHand));
    this.ReloadJobPrototypes();
    this.ReloadAccessPrototypes();
  }

  private void OnInteractHand(
    Entity<IdModificationConsoleComponent> ent,
    ref InteractUsingEvent args)
  {
    args.Handled = this.ContainerInHandler(ent, args.User);
  }

  private void OnJobChangeMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleJobChangeBuiMsg args)
  {
    EntityUid? contained;
    AccessComponent comp;
    if (!ent.Comp.Authenticated || !this.TryContainerEntity(ent, ent.Comp.TargetIdSlot, out contained) || !this.TryComp<AccessComponent>(contained, out comp))
      return;
    comp.Tags.Clear();
    AccessGroupPrototype prototype;
    if (!this._prototype.TryIndex<AccessGroupPrototype>(args.AccessGroup, out prototype))
      return;
    foreach (ProtoId<AccessLevelPrototype> tag in prototype.Tags)
      comp.Tags.Add(tag);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(33, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" has changed the accesses of ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted(prototype.Name);
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref local);
  }

  private void OnTerminateConfirmMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleTerminateConfirmBuiMsg args)
  {
    EntityUid? contained;
    ItemIFFComponent comp1;
    IdCardComponent comp2;
    AccessComponent comp3;
    if (!ent.Comp.Authenticated || !this.TryContainerEntity(ent, ent.Comp.TargetIdSlot, out contained) || !this.TryComp<ItemIFFComponent>(contained, out comp1) || !this.TryComp<IdCardComponent>(contained, out comp2) || !this.TryComp<AccessComponent>(contained, out comp3))
      return;
    this._iff.SetIdFaction((Entity<ItemIFFComponent>) (contained.Value, comp1), (EntProtoId<IFFFactionComponent>) "FactionSurvivor");
    ent.Comp.HasIFF = false;
    foreach (ProtoId<AccessLevelPrototype> access in ent.Comp.AccessList)
      comp3.Tags.Remove(access);
    foreach (ProtoId<AccessLevelPrototype> hiddenAccess in ent.Comp.HiddenAccessList)
      comp3.Tags.Remove(hiddenAccess);
    comp2._jobTitle = "Civilian";
    this.Dirty(contained.Value, (IComponent) comp2);
    if (comp2.OriginalOwner.HasValue)
    {
      this._rank.SetRank(comp2.OriginalOwner.Value, (ProtoId<RankPrototype>) "RMCRankCivilian");
      this._squad.RemoveSquad(comp2.OriginalOwner.Value, new ProtoId<JobPrototype>?());
      this._metaData.SetEntityName(contained.Value, $"{this.MetaData(comp2.OriginalOwner.Value).EntityName} ({comp2._jobTitle})");
    }
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(19, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" has terminated ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" & ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(comp2.OriginalOwner), "player", "ToPrettyString(idCard.OriginalOwner)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.RMCIdModify, LogImpact.High, ref local);
  }

  private void OnIFFChangeMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleIFFChangeBuiMsg args)
  {
    EntityUid? contained;
    if (!ent.Comp.Authenticated || !this.TryContainerEntity(ent, ent.Comp.TargetIdSlot, out contained) || !contained.HasValue)
      return;
    ItemIFFComponent comp;
    this.EnsureComp<ItemIFFComponent>(contained.Value, out comp);
    if (!comp.Factions.Contains(ent.Comp.Faction) && !args.Revoke)
    {
      this._iff.SetIdFaction((Entity<ItemIFFComponent>) (contained.Value, comp), ent.Comp.Faction);
      ent.Comp.HasIFF = true;
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(26, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has revoked the ");
      logStringHandler.AppendFormatted<EntProtoId<IFFFactionComponent>>(ent.Comp.Faction, "ent.Comp.Faction");
      logStringHandler.AppendLiteral(" IFF for ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.RMCIdModify, LogImpact.Medium, ref local);
    }
    else if (args.Revoke)
    {
      this._iff.SetIdFaction((Entity<ItemIFFComponent>) (contained.Value, comp), (EntProtoId<IFFFactionComponent>) "FactionSurvivor");
      ent.Comp.HasIFF = false;
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(26, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has granted the ");
      logStringHandler.AppendFormatted<EntProtoId<IFFFactionComponent>>(ent.Comp.Faction, "ent.Comp.Faction");
      logStringHandler.AppendLiteral(" IFF for ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref local);
    }
    this.Dirty<IdModificationConsoleComponent>(ent);
  }

  private void OnSignInTargetMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleSignInTargetBuiMsg args)
  {
    if (this.TryContainerEntity(ent, ent.Comp.TargetIdSlot, out EntityUid? _))
    {
      this.ContainerOutHandler(ent, args.Actor, ent.Comp.TargetIdSlot);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(25, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has ejected from ");
      logStringHandler.AppendFormatted(ent.Comp.TargetIdSlot);
      logStringHandler.AppendLiteral(" from: ");
      logStringHandler.AppendFormatted<EntityUid>(ent.Owner, "entity", "ent.Owner");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref local);
    }
    else
      this.ContainerInHandler(ent, args.Actor, ent.Comp.TargetIdSlot);
  }

  private void OnSignInMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleSignInBuiMsg args)
  {
    if (this.TryContainerEntity(ent, ent.Comp.PrivilegedIdSlot, out EntityUid? _))
    {
      this.ContainerOutHandler(ent, args.Actor, ent.Comp.PrivilegedIdSlot);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(25, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has ejected from ");
      logStringHandler.AppendFormatted(ent.Comp.PrivilegedIdSlot);
      logStringHandler.AppendLiteral(" from: ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) ent.Owner), "entity", "ToPrettyString(ent.Owner)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref local);
    }
    else
    {
      this.ContainerInHandler(ent, args.Actor, ent.Comp.PrivilegedIdSlot);
      AccessLevelPrototype prototype;
      if (ent.Comp.Authenticated || !this._prototype.TryIndex<AccessLevelPrototype>(ent.Comp.Access, out prototype) || prototype.Name == null)
        return;
      this._popup.PopupClient("This id is missing the " + this.Loc.GetString(prototype.Name), new EntityUid?(args.Actor), PopupType.MediumCaution);
    }
  }

  private void OnMultipleAccessChangeMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleMultipleAccessChangeBuiMsg args)
  {
    EntityUid? contained;
    AccessComponent comp;
    if (!ent.Comp.Authenticated || !this.TryContainerEntity(ent, ent.Comp.TargetIdSlot, out contained) || !this.TryComp<AccessComponent>(contained, out comp))
      return;
    switch (args.Type)
    {
      case "GrantAll":
        foreach (ProtoId<AccessLevelPrototype> access in ent.Comp.AccessList)
        {
          AccessLevelPrototype prototype;
          if (this._prototype.TryIndex<AccessLevelPrototype>(access, out prototype) && !(prototype.AccessGroup != args.AccessList))
            comp.Tags.Add((ProtoId<AccessLevelPrototype>) prototype);
        }
        ISharedAdminLogManager adminLogger1 = this._adminLogger;
        LogStringHandler logStringHandler1 = new LogStringHandler(34, 3);
        logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
        logStringHandler1.AppendLiteral(" has granted all accesses for ");
        logStringHandler1.AppendFormatted(args.AccessList);
        logStringHandler1.AppendLiteral(" on ");
        logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
        ref LogStringHandler local1 = ref logStringHandler1;
        adminLogger1.Add(LogType.RMCIdModify, LogImpact.Medium, ref local1);
        break;
      case "RevokeAll":
        foreach (ProtoId<AccessLevelPrototype> access in ent.Comp.AccessList)
        {
          AccessLevelPrototype prototype;
          if (this._prototype.TryIndex<AccessLevelPrototype>(access, out prototype) && !(prototype.AccessGroup != args.AccessList))
            comp.Tags.Remove(access);
        }
        ISharedAdminLogManager adminLogger2 = this._adminLogger;
        LogStringHandler logStringHandler2 = new LogStringHandler(34, 3);
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
        logStringHandler2.AppendLiteral(" has revoked all accesses for ");
        logStringHandler2.AppendFormatted(args.AccessList);
        logStringHandler2.AppendLiteral(" on ");
        logStringHandler2.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
        ref LogStringHandler local2 = ref logStringHandler2;
        adminLogger2.Add(LogType.RMCIdModify, LogImpact.Medium, ref local2);
        break;
      case "GrantAllGroup":
        foreach (ProtoId<AccessLevelPrototype> access in ent.Comp.AccessList)
          comp.Tags.Add(access);
        ISharedAdminLogManager adminLogger3 = this._adminLogger;
        LogStringHandler logStringHandler3 = new LogStringHandler(29, 2);
        logStringHandler3.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
        logStringHandler3.AppendLiteral(" has granted all accesses on ");
        logStringHandler3.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
        ref LogStringHandler local3 = ref logStringHandler3;
        adminLogger3.Add(LogType.RMCIdModify, LogImpact.Medium, ref local3);
        break;
      case "RevokeAllGroup":
        foreach (ProtoId<AccessLevelPrototype> access in ent.Comp.AccessList)
          comp.Tags.Remove(access);
        ISharedAdminLogManager adminLogger4 = this._adminLogger;
        LogStringHandler logStringHandler4 = new LogStringHandler(29, 2);
        logStringHandler4.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
        logStringHandler4.AppendLiteral(" has revoked all accesses on ");
        logStringHandler4.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
        ref LogStringHandler local4 = ref logStringHandler4;
        adminLogger4.Add(LogType.RMCIdModify, LogImpact.Medium, ref local4);
        break;
    }
    this.Dirty<IdModificationConsoleComponent>(ent);
  }

  private void OnAccessChangeMsg(
    Entity<IdModificationConsoleComponent> ent,
    ref IdModificationConsoleAccessChangeBuiMsg args)
  {
    EntityUid? contained;
    AccessComponent comp;
    if (!ent.Comp.Authenticated || !this.TryContainerEntity(ent, ent.Comp.TargetIdSlot, out contained) || !this.TryComp<AccessComponent>(contained, out comp))
      return;
    if (args.Add)
    {
      comp.Tags.Add(args.Access);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(17, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has granted ");
      logStringHandler.AppendFormatted<ProtoId<AccessLevelPrototype>>(args.Access, "args.Access");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref local);
    }
    else
    {
      comp.Tags.Remove(args.Access);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(17, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has revoked ");
      logStringHandler.AppendFormatted<ProtoId<AccessLevelPrototype>>(args.Access, "args.Access");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(contained), "entity", "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.RMCIdModify, LogImpact.Low, ref local);
    }
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (ev.WasModified<AccessLevelPrototype>())
      this.ReloadAccessPrototypes();
    if (!ev.WasModified<AccessGroupPrototype>())
      return;
    this.ReloadJobPrototypes();
  }

  private void ReloadAccessPrototypes()
  {
    Dictionary<string, AccessLevelPrototype> source = new Dictionary<string, AccessLevelPrototype>();
    foreach (AccessLevelPrototype enumeratePrototype in this._prototype.EnumeratePrototypes<AccessLevelPrototype>())
    {
      object target = (object) new AccessLevelPrototype();
      this._serialization.CopyTo((object) enumeratePrototype, ref target);
      if (target is AccessLevelPrototype accessLevelPrototype)
        source[enumeratePrototype.ID] = accessLevelPrototype;
    }
    this._accessLevel = source.ToFrozenDictionary<string, AccessLevelPrototype>();
  }

  private void ReloadJobPrototypes()
  {
    Dictionary<string, AccessGroupPrototype> source = new Dictionary<string, AccessGroupPrototype>();
    foreach (AccessGroupPrototype enumeratePrototype in this._prototype.EnumeratePrototypes<AccessGroupPrototype>())
    {
      object target = (object) new AccessGroupPrototype();
      this._serialization.CopyTo((object) enumeratePrototype, ref target);
      if (target is AccessGroupPrototype accessGroupPrototype)
        source[enumeratePrototype.ID] = accessGroupPrototype;
    }
    this._accessGroup = source.ToFrozenDictionary<string, AccessGroupPrototype>();
  }

  private bool ContainerInHandler(Entity<IdModificationConsoleComponent> ent, EntityUid user)
  {
    EntityUid? uid;
    AccessComponent comp;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out uid) || !this.TryComp<IdCardComponent>(uid, out IdCardComponent _) || !this.TryComp<AccessComponent>(uid, out comp))
      return false;
    return comp.Tags.Contains(ent.Comp.Access) ? this.ContainerInHandler(ent, user, ent.Comp.PrivilegedIdSlot) : this.ContainerInHandler(ent, user, ent.Comp.TargetIdSlot);
  }

  private bool ContainerInHandler(
    Entity<IdModificationConsoleComponent> ent,
    EntityUid user,
    string containerType)
  {
    EntityUid? uid;
    AccessComponent comp1;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out uid) || !this.TryComp<IdCardComponent>(uid, out IdCardComponent _) || !this.TryComp<AccessComponent>(uid, out comp1))
      return false;
    if (comp1.Tags.Contains(ent.Comp.Access) && containerType == ent.Comp.PrivilegedIdSlot)
      ent.Comp.Authenticated = true;
    ItemIFFComponent comp2;
    if (this.TryComp<ItemIFFComponent>(uid, out comp2) && containerType == ent.Comp.TargetIdSlot)
      ent.Comp.HasIFF = comp2.Factions.Contains(ent.Comp.Faction);
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, containerType);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid.Value, (BaseContainer) container);
    this.Dirty<IdModificationConsoleComponent>(ent);
    return true;
  }

  private bool ContainerOutHandler(
    Entity<IdModificationConsoleComponent> ent,
    EntityUid user,
    string containerType)
  {
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, containerType);
    EntityUid? containedEntity = container.ContainedEntity;
    if (!containedEntity.HasValue)
      return false;
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) containedEntity.Value, (BaseContainer) container);
    if (containerType == ent.Comp.PrivilegedIdSlot)
      ent.Comp.Authenticated = false;
    if (containerType == ent.Comp.TargetIdSlot)
      ent.Comp.HasIFF = false;
    this._hands.PickupOrDrop(new EntityUid?(user), containedEntity.Value);
    this.Dirty<IdModificationConsoleComponent>(ent);
    return true;
  }

  private bool TryContainerEntity(
    Entity<IdModificationConsoleComponent> ent,
    string containerType,
    out EntityUid? contained)
  {
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, containerType);
    contained = containerSlot.ContainedEntity;
    this.Dirty<IdModificationConsoleComponent>(ent);
    return contained.HasValue;
  }

  private void OnComponentInit(Entity<IdModificationConsoleComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAccessList(ent);
  }

  private void UpdateAccessList(Entity<IdModificationConsoleComponent> ent)
  {
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet1 = new HashSet<ProtoId<AccessLevelPrototype>>();
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet2 = new HashSet<ProtoId<AccessLevelPrototype>>();
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet3 = new HashSet<ProtoId<AccessLevelPrototype>>();
    foreach (AccessLevelPrototype accessLevelPrototype in this._accessLevel.Values)
    {
      EntProtoId<IFFFactionComponent>? faction1 = accessLevelPrototype.Faction;
      EntProtoId<IFFFactionComponent> faction2 = ent.Comp.Faction;
      if ((faction1.HasValue ? (faction1.GetValueOrDefault() == faction2 ? 1 : 0) : 0) != 0 && !accessLevelPrototype.Hidden)
      {
        if (accessLevelPrototype.Name != null && accessLevelPrototype.Name.Contains("protobaseaccess"))
          protoIdSet3.Add((ProtoId<AccessLevelPrototype>) accessLevelPrototype);
        else
          protoIdSet1.Add((ProtoId<AccessLevelPrototype>) accessLevelPrototype);
      }
      else
      {
        EntProtoId<IFFFactionComponent>? faction3 = accessLevelPrototype.Faction;
        EntProtoId<IFFFactionComponent> faction4 = ent.Comp.Faction;
        if ((faction3.HasValue ? (faction3.GetValueOrDefault() == faction4 ? 1 : 0) : 0) != 0 && accessLevelPrototype.Hidden && accessLevelPrototype.Name != null && !accessLevelPrototype.Name.Contains("protobaseaccess"))
          protoIdSet2.Add((ProtoId<AccessLevelPrototype>) accessLevelPrototype);
      }
    }
    ent.Comp.AccessGroups = protoIdSet3;
    ent.Comp.AccessList = protoIdSet1;
    ent.Comp.HiddenAccessList = protoIdSet2;
    HashSet<ProtoId<AccessGroupPrototype>> protoIdSet4 = new HashSet<ProtoId<AccessGroupPrototype>>();
    HashSet<ProtoId<AccessGroupPrototype>> protoIdSet5 = new HashSet<ProtoId<AccessGroupPrototype>>();
    foreach (AccessGroupPrototype accessGroupPrototype in this._accessGroup.Values)
    {
      EntProtoId<IFFFactionComponent>? faction5 = accessGroupPrototype.Faction;
      EntProtoId<IFFFactionComponent> faction6 = ent.Comp.Faction;
      if ((faction5.HasValue ? (faction5.GetValueOrDefault() == faction6 ? 1 : 0) : 0) != 0 && !accessGroupPrototype.Hidden)
      {
        if (accessGroupPrototype.Name != null && accessGroupPrototype.Name.Contains("protobaseaccess"))
          protoIdSet5.Add((ProtoId<AccessGroupPrototype>) accessGroupPrototype);
        else
          protoIdSet4.Add((ProtoId<AccessGroupPrototype>) accessGroupPrototype);
      }
    }
    ent.Comp.JobGroups = protoIdSet5;
    ent.Comp.JobList = protoIdSet4;
  }
}
