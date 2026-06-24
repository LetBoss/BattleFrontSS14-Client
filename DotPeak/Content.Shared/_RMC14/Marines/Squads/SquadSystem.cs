// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Squads.SquadSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Cryostorage;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared._RMC14.Pointing;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Tracker;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Chat;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Dataset;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Marines.Squads;

public sealed class SquadSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private EncryptionKeySystem _encryptionKey;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedIdCardSystem _id;
  [Dependency]
  private SharedCMInventorySystem _cmInventory;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedJobSystem _job;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedMarineSystem _marine;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private SharedMarineOrdersSystem _marineOrders;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedRMCBanSystem _rmcBan;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  private static readonly ProtoId<JobPrototype> SquadLeaderJob = (ProtoId<JobPrototype>) "CMSquadLeader";
  private static readonly ProtoId<JobPrototype> IntelOfficerJob = (ProtoId<JobPrototype>) "CMIntelOfficer";
  public static readonly EntProtoId<SquadTeamComponent> EchoSquadId = (EntProtoId<SquadTeamComponent>) "SquadEcho";
  private readonly HashSet<EntityUid> _membersToUpdate = new HashSet<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<RMCMapToSquadComponent> _mapToSquadQuery;
  private Robust.Shared.GameObjects.EntityQuery<OriginalRoleComponent> _originalRoleQuery;
  private Robust.Shared.GameObjects.EntityQuery<SquadArmorWearerComponent> _squadArmorWearerQuery;
  private Robust.Shared.GameObjects.EntityQuery<SquadMemberComponent> _squadMemberQuery;
  private Robust.Shared.GameObjects.EntityQuery<SquadTeamComponent> _squadTeamQuery;

  public ImmutableArray<Robust.Shared.Prototypes.EntityPrototype> SquadPrototypes { get; private set; }

  public ImmutableArray<JobPrototype> SquadRolePrototypes { get; private set; }

  public override void Initialize()
  {
    this._mapToSquadQuery = this.GetEntityQuery<RMCMapToSquadComponent>();
    this._originalRoleQuery = this.GetEntityQuery<OriginalRoleComponent>();
    this._squadArmorWearerQuery = this.GetEntityQuery<SquadArmorWearerComponent>();
    this._squadMemberQuery = this.GetEntityQuery<SquadMemberComponent>();
    this._squadTeamQuery = this.GetEntityQuery<SquadTeamComponent>();
    this.SubscribeLocalEvent<SquadArmorComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<SquadArmorComponent, GetEquipmentVisualsEvent>(this.OnSquadArmorGetVisuals), after: new Type[1]
    {
      typeof (ClothingSystem)
    });
    this.SubscribeLocalEvent<SquadMemberComponent, MapInitEvent>(new EntityEventRefHandler<SquadMemberComponent, MapInitEvent>(this.OnSquadMemberMapInit));
    this.SubscribeLocalEvent<SquadMemberComponent, ComponentRemove>(new EntityEventRefHandler<SquadMemberComponent, ComponentRemove>(this.OnSquadMemberRemove));
    this.SubscribeLocalEvent<SquadMemberComponent, EntityTerminatingEvent>(new EntityEventRefHandler<SquadMemberComponent, EntityTerminatingEvent>(this.OnSquadMemberTerminating));
    this.SubscribeLocalEvent<SquadMemberComponent, MobStateChangedEvent>(new EntityEventRefHandler<SquadMemberComponent, MobStateChangedEvent>(this.OnSquadMemberMobStateChanged));
    this.SubscribeLocalEvent<SquadMemberComponent, PlayerAttachedEvent>(new EntityEventRefHandler<SquadMemberComponent, PlayerAttachedEvent>(this.OnSquadMemberPlayerAttached));
    this.SubscribeLocalEvent<SquadMemberComponent, PlayerDetachedEvent>(new EntityEventRefHandler<SquadMemberComponent, PlayerDetachedEvent>(this.OnSquadMemberPlayerDetached));
    this.SubscribeLocalEvent<SquadMemberComponent, GetMarineIconEvent>(new EntityEventRefHandler<SquadMemberComponent, GetMarineIconEvent>(this.OnSquadRoleGetIcon), after: new Type[1]
    {
      typeof (SharedMarineSystem)
    });
    this.SubscribeLocalEvent<SquadMemberComponent, EnteredCryostorageEvent>(new EntityEventRefHandler<SquadMemberComponent, EnteredCryostorageEvent>(this.OnSquadMemberEnteredCryo));
    this.SubscribeLocalEvent<SquadMemberComponent, LeftCryostorageEvent>(new EntityEventRefHandler<SquadMemberComponent, LeftCryostorageEvent>(this.OnSquadMemberLeftCryo));
    this.SubscribeLocalEvent<SquadMemberComponent, GetMarineSquadNameEvent>(new EntityEventRefHandler<SquadMemberComponent, GetMarineSquadNameEvent>(this.OnSquadRoleGetName));
    this.SubscribeLocalEvent<SquadLeaderComponent, EntityTerminatingEvent>(new EntityEventRefHandler<SquadLeaderComponent, EntityTerminatingEvent>(this.OnSquadLeaderTerminating));
    this.SubscribeLocalEvent<SquadLeaderComponent, GetMarineIconEvent>(new EntityEventRefHandler<SquadLeaderComponent, GetMarineIconEvent>(this.OnSquadLeaderGetMarineIcon), after: new Type[1]
    {
      typeof (SharedMarineSystem)
    });
    this.SubscribeLocalEvent<SquadLeaderHeadsetComponent, EncryptionChannelsChangedEvent>(new EntityEventRefHandler<SquadLeaderHeadsetComponent, EncryptionChannelsChangedEvent>(this.OnSquadLeaderHeadsetChannelsChanged), new Type[1]
    {
      typeof (SharedHeadsetSystem)
    });
    this.SubscribeLocalEvent<SquadLeaderHeadsetComponent, EntityTerminatingEvent>(new EntityEventRefHandler<SquadLeaderHeadsetComponent, EntityTerminatingEvent>(this.OnSquadLeaderHeadsetTerminating));
    this.SubscribeLocalEvent<AssignSquadComponent, PlayerSpawnCompleteEvent>(new EntityEventRefHandler<AssignSquadComponent, PlayerSpawnCompleteEvent>(this.OnAssignSquadPlayerSpawnComplete));
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.RefreshSquadPrototypes();
  }

  private void OnSquadArmorGetVisuals(
    Entity<SquadArmorComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    SlotDefinition slotDefinition;
    SquadMemberComponent component1;
    SquadArmorWearerComponent component2;
    if (this._inventory.TryGetSlot(args.Equipee, args.Slot, out slotDefinition) && (slotDefinition.SlotFlags & ent.Comp.Slot) == SlotFlags.NONE || !this._squadMemberQuery.TryComp(args.Equipee, out component1) || !this._squadArmorWearerQuery.TryComp(args.Equipee, out component2) || component1.BlacklistedSquadArmor.Contains(ent.Comp.Layer))
      return;
    SpriteSpecifier.Rsi rsi = component2.Leader ? ent.Comp.LeaderRsi : ent.Comp.Rsi;
    string layer = $"enum.{"SquadArmorLayers"}.{ent.Comp.Layer}";
    if (args.Layers.Any<(string, PrototypeLayerData)>((Func<(string, PrototypeLayerData), bool>) (l => l.Item1 == layer)))
      return;
    args.Layers.Add((layer, new PrototypeLayerData()
    {
      RsiPath = rsi.RsiPath.ToString(),
      State = rsi.RsiState,
      Color = new Color?(component1.BackgroundColor),
      Visible = new bool?(true)
    }));
  }

  private void OnSquadMemberMapInit(Entity<SquadMemberComponent> ent, ref MapInitEvent args)
  {
    this._membersToUpdate.Add((EntityUid) ent);
  }

  private void OnSquadMemberRemove(Entity<SquadMemberComponent> ent, ref ComponentRemove args)
  {
    SquadTeamComponent component;
    if (!this._squadTeamQuery.TryComp(ent.Comp.Squad, out component))
      return;
    component.Members.Remove((EntityUid) ent);
  }

  private void OnSquadMemberTerminating(
    Entity<SquadMemberComponent> ent,
    ref EntityTerminatingEvent args)
  {
    SquadTeamComponent component;
    if (!this._squadTeamQuery.TryComp(ent.Comp.Squad, out component))
      return;
    component.Members.Remove((EntityUid) ent);
  }

  private void OnSquadMemberMobStateChanged(
    Entity<SquadMemberComponent> ent,
    ref MobStateChangedEvent args)
  {
    EntityUid? squad = ent.Comp.Squad;
    if (!squad.HasValue)
      return;
    SquadMemberUpdatedEvent args1 = new SquadMemberUpdatedEvent(squad.GetValueOrDefault());
    this.RaiseLocalEvent<SquadMemberUpdatedEvent>((EntityUid) ent, ref args1);
  }

  private void OnSquadMemberPlayerAttached(
    Entity<SquadMemberComponent> ent,
    ref PlayerAttachedEvent args)
  {
    EntityUid? squad = ent.Comp.Squad;
    if (!squad.HasValue)
      return;
    SquadMemberUpdatedEvent args1 = new SquadMemberUpdatedEvent(squad.GetValueOrDefault());
    this.RaiseLocalEvent<SquadMemberUpdatedEvent>((EntityUid) ent, ref args1);
  }

  private void OnSquadMemberPlayerDetached(
    Entity<SquadMemberComponent> ent,
    ref PlayerDetachedEvent args)
  {
    EntityUid? squad = ent.Comp.Squad;
    if (!squad.HasValue)
      return;
    SquadMemberUpdatedEvent args1 = new SquadMemberUpdatedEvent(squad.GetValueOrDefault());
    this.RaiseLocalEvent<SquadMemberUpdatedEvent>((EntityUid) ent, ref args1);
  }

  private void OnSquadRoleGetIcon(Entity<SquadMemberComponent> member, ref GetMarineIconEvent args)
  {
    args.Background = member.Comp.Background;
    args.BackgroundColor = new Color?(member.Comp.BackgroundColor);
  }

  private void OnSquadRoleGetName(
    Entity<SquadMemberComponent> member,
    ref GetMarineSquadNameEvent args)
  {
    Entity<SquadTeamComponent> squad;
    if (this.TryGetMemberSquad((Entity<SquadMemberComponent>) member.Owner, out squad))
      args.SquadName = this.Name((EntityUid) squad);
    JobPrototype prototype;
    if (this._prototypes.TryIndex<JobPrototype>((ProtoId<JobPrototype>?) this._originalRoleQuery.CompOrNull((EntityUid) member)?.Job, out prototype))
    {
      args.RoleName = prototype.LocalizedName;
    }
    else
    {
      EntityUid mindId;
      string name;
      if (!this._mind.TryGetMind((EntityUid) member, out mindId, out MindComponent _) || !this._job.MindTryGetJobName(new EntityUid?(mindId), out name))
        return;
      args.RoleName = name;
    }
  }

  private void OnSquadMemberEnteredCryo(
    Entity<SquadMemberComponent> ent,
    ref EnteredCryostorageEvent args)
  {
    OriginalRoleComponent component1;
    if (!this._originalRoleQuery.TryComp((EntityUid) ent, out component1))
      return;
    ProtoId<JobPrototype>? job = component1.Job;
    if (!job.HasValue)
      return;
    ProtoId<JobPrototype> valueOrDefault = job.GetValueOrDefault();
    SquadTeamComponent component2;
    int num;
    if (!this._squadTeamQuery.TryComp(ent.Comp.Squad, out component2) || !component2.Roles.TryGetValue(valueOrDefault, out num) || num <= 0)
      return;
    component2.Roles[valueOrDefault] = num - 1;
  }

  private void OnSquadMemberLeftCryo(
    Entity<SquadMemberComponent> ent,
    ref LeftCryostorageEvent args)
  {
    OriginalRoleComponent component1;
    if (!this._originalRoleQuery.TryComp((EntityUid) ent, out component1))
      return;
    ProtoId<JobPrototype>? job = component1.Job;
    if (!job.HasValue)
      return;
    ProtoId<JobPrototype> valueOrDefault = job.GetValueOrDefault();
    SquadTeamComponent component2;
    int num;
    if (!this._squadTeamQuery.TryComp(ent.Comp.Squad, out component2) || !component2.Roles.TryGetValue(valueOrDefault, out num))
      return;
    component2.Roles[valueOrDefault] = num + 1;
  }

  private void OnSquadLeaderTerminating(
    Entity<SquadLeaderComponent> ent,
    ref EntityTerminatingEvent args)
  {
    EntityUid? headset = ent.Comp.Headset;
    if (!headset.HasValue)
      return;
    this.RemCompDeferred<SquadLeaderHeadsetComponent>(headset.GetValueOrDefault());
  }

  private void OnSquadLeaderGetMarineIcon(
    Entity<SquadLeaderComponent> ent,
    ref GetMarineIconEvent args)
  {
    args.Icon = (SpriteSpecifier) ent.Comp.Icon;
  }

  private void OnSquadLeaderHeadsetChannelsChanged(
    Entity<SquadLeaderHeadsetComponent> ent,
    ref EncryptionChannelsChangedEvent args)
  {
    foreach (ProtoId<RadioChannelPrototype> channel in ent.Comp.Channels)
      args.Component.Channels.Add((string) channel);
  }

  private void OnSquadLeaderHeadsetTerminating(
    Entity<SquadLeaderHeadsetComponent> ent,
    ref EntityTerminatingEvent args)
  {
    SquadLeaderComponent comp;
    if (!this.TryComp<SquadLeaderComponent>(ent.Comp.Leader, out comp))
      return;
    comp.Headset = new EntityUid?();
    this.Dirty(ent.Comp.Leader, (IComponent) comp);
  }

  private void OnAssignSquadPlayerSpawnComplete(
    Entity<AssignSquadComponent> ent,
    ref PlayerSpawnCompleteEvent args)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadTeamComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadTeamComponent>();
    EntityUid uid;
    SquadTeamComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (this._entityWhitelist.IsWhitelistPass(ent.Comp.Whitelist, uid))
        this.AssignSquad((EntityUid) ent, (Entity<SquadTeamComponent>) (uid, comp1), (ProtoId<JobPrototype>?) args.JobId);
    }
  }

  private void SearchForMappedItems(Entity<SquadMemberComponent> user, EntityUid squad)
  {
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (!this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) user.Owner, out containerSlotEnumerator))
      return;
    ContainerSlot container;
    while (containerSlotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity1 = container.ContainedEntity;
      if (containedEntity1.HasValue)
      {
        EntityUid valueOrDefault = containedEntity1.GetValueOrDefault();
        RMCMapToSquadComponent component1;
        if (this._mapToSquadQuery.TryComp(valueOrDefault, out component1))
        {
          this.MapToSquad((Entity<RMCMapToSquadComponent>) (valueOrDefault, component1), (EntityUid) user, squad);
        }
        else
        {
          StorageComponent comp1;
          if (this.TryComp<StorageComponent>(valueOrDefault, out comp1))
          {
            foreach (EntityUid containedEntity2 in (IEnumerable<EntityUid>) comp1.Container.ContainedEntities)
            {
              RMCMapToSquadComponent component2;
              if (this._mapToSquadQuery.TryComp(containedEntity2, out component2))
                this.MapToSquad((Entity<RMCMapToSquadComponent>) (containedEntity2, component2), (EntityUid) user, squad);
            }
          }
          else
          {
            EncryptionKeyHolderComponent comp2;
            if (this.TryComp<EncryptionKeyHolderComponent>(valueOrDefault, out comp2))
            {
              this._encryptionKey.UpdateChannels(valueOrDefault, comp2);
              break;
            }
          }
        }
      }
    }
  }

  private void MapToSquad(Entity<RMCMapToSquadComponent> ent, EntityUid user, EntityUid squad)
  {
    if (this._net.IsClient)
      return;
    EntProtoId? nullable1 = new EntProtoId?();
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = this.CompOrNull<MetaDataComponent>(squad)?.EntityPrototype;
    EntProtoId entProtoId;
    if (entityPrototype != null && ent.Comp.Map.TryGetValue((EntProtoId) entityPrototype.ID, out entProtoId))
      nullable1 = new EntProtoId?(entProtoId);
    if (nullable1.HasValue)
    {
      EntProtoId? nullable2 = nullable1;
      EntityUid orDrop = this.SpawnNextToOrDrop(nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null, user);
      ClothingComponent comp;
      if (this.TryComp<ClothingComponent>(orDrop, out comp) && !this._cmInventory.TryEquipClothing(user, (Entity<ClothingComponent>) (orDrop, comp)))
        this._hands.TryPickupAnyHand(user, orDrop);
    }
    this.QueueDel(new EntityUid?((EntityUid) ent));
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<Robust.Shared.Prototypes.EntityPrototype>() && !ev.WasModified<JobPrototype>())
      return;
    this.RefreshSquadPrototypes();
  }

  private void RefreshSquadPrototypes()
  {
    ImmutableArray<Robust.Shared.Prototypes.EntityPrototype>.Builder builder1 = ImmutableArray.CreateBuilder<Robust.Shared.Prototypes.EntityPrototype>();
    foreach (Robust.Shared.Prototypes.EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<Robust.Shared.Prototypes.EntityPrototype>())
    {
      if (enumeratePrototype.HasComponent<SquadTeamComponent>())
        builder1.Add(enumeratePrototype);
    }
    builder1.Sort((Comparison<Robust.Shared.Prototypes.EntityPrototype>) ((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)));
    this.SquadPrototypes = builder1.ToImmutable();
    ImmutableArray<JobPrototype>.Builder builder2 = ImmutableArray.CreateBuilder<JobPrototype>();
    foreach (JobPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<JobPrototype>())
    {
      if (enumeratePrototype.HasSquad)
        builder2.Add(enumeratePrototype);
    }
    JobPrototype prototype;
    if (this._prototypes.TryIndex<JobPrototype>(SquadSystem.IntelOfficerJob, out prototype))
      builder2.Add(prototype);
    this.SquadRolePrototypes = builder2.ToImmutable();
  }

  public bool TryGetSquad(EntProtoId prototype, out Entity<SquadTeamComponent> squad)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadTeamComponent, MetaDataComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadTeamComponent, MetaDataComponent>();
    EntityUid uid;
    SquadTeamComponent comp1;
    MetaDataComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp2.EntityPrototype?.ID != prototype.Id))
      {
        squad = (Entity<SquadTeamComponent>) (uid, comp1);
        return true;
      }
    }
    squad = new Entity<SquadTeamComponent>();
    return false;
  }

  public bool TryGetMemberSquad(
    Entity<SquadMemberComponent?> member,
    out Entity<SquadTeamComponent> squad)
  {
    squad = new Entity<SquadTeamComponent>();
    SquadTeamComponent comp;
    if (!this.Resolve<SquadMemberComponent>((EntityUid) member, ref member.Comp, false) || !this.TryComp<SquadTeamComponent>(member.Comp.Squad, out comp))
      return false;
    squad = (Entity<SquadTeamComponent>) (member.Comp.Squad.Value, comp);
    return true;
  }

  public bool HasSquad(EntProtoId id) => this.TryGetSquad(id, out Entity<SquadTeamComponent> _);

  public bool TryEnsureSquad(EntProtoId id, out Entity<SquadTeamComponent> squad)
  {
    Robust.Shared.Prototypes.EntityPrototype prototype;
    if (!this._prototypes.TryIndex(id, out prototype) || !prototype.HasComponent<SquadTeamComponent>(this._compFactory))
    {
      squad = new Entity<SquadTeamComponent>();
      return false;
    }
    if (this.TryGetSquad(id, out squad))
      return true;
    EntityUid uid = this.Spawn((string) id);
    SquadTeamComponent comp;
    if (!this.TryComp<SquadTeamComponent>(uid, out comp))
    {
      this.Log.Error($"Squad entity prototype {id} had {"SquadTeamComponent"}, but none found on entity {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
      return false;
    }
    squad = (Entity<SquadTeamComponent>) (uid, comp);
    return true;
  }

  public int GetSquadMembersAlive(Entity<SquadTeamComponent> team)
  {
    int squadMembersAlive = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadMemberComponent>();
    EntityUid uid;
    SquadMemberComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? squad = comp1.Squad;
      EntityUid entityUid = (EntityUid) team;
      if ((squad.HasValue ? (squad.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && !this._mobState.IsDead(uid))
        ++squadMembersAlive;
    }
    return squadMembersAlive;
  }

  public void AssignSquad(
    EntityUid marine,
    Entity<SquadTeamComponent?> team,
    ProtoId<JobPrototype>? job)
  {
    if (!this.Resolve<SquadTeamComponent>((EntityUid) team, ref team.Comp))
      return;
    SquadMemberComponent squadMemberComponent = this.EnsureComp<SquadMemberComponent>(marine);
    EntityUid? squad = squadMemberComponent.Squad;
    ProtoId<JobPrototype>? nullable = (ProtoId<JobPrototype>?) (job ?? this._originalRoleQuery.CompOrNull(marine)?.Job);
    SquadTeamComponent component;
    if (this._squadTeamQuery.TryComp(squad, out component))
    {
      component.Members.Remove(marine);
      int num;
      if (nullable.HasValue && component.Roles.TryGetValue(nullable.Value, out num) && num > 0)
        component.Roles[nullable.Value] = num - 1;
    }
    squadMemberComponent.Squad = new EntityUid?((EntityUid) team);
    squadMemberComponent.Background = team.Comp.Background;
    squadMemberComponent.BackgroundColor = team.Comp.Color;
    squadMemberComponent.AccessibleBackgroundColor = team.Comp.AccessibleColor;
    squadMemberComponent.BlacklistedSquadArmor = team.Comp.BlacklistedSquadArmor;
    this.Dirty(marine, (IComponent) squadMemberComponent);
    SquadGrantAccessComponent grantAccessComponent = this.EnsureComp<SquadGrantAccessComponent>(marine);
    grantAccessComponent.AccessLevels = team.Comp.AccessLevels;
    JobPrototype prototype;
    if (this._prototypes.TryIndex<JobPrototype>(job, out prototype))
    {
      grantAccessComponent.RoleName = $"{this.Name((EntityUid) team)} {prototype.LocalizedName}";
    }
    else
    {
      EntityUid mindId;
      string name;
      if (this._mind.TryGetMind(marine, out mindId, out MindComponent _) && this._job.MindTryGetJobName(new EntityUid?(mindId), out name))
        this.MarineSetTitle(marine, $"{this.Name((EntityUid) team)} {name}");
    }
    this.Dirty(marine, (IComponent) grantAccessComponent);
    team.Comp.Members.Add(marine);
    if (nullable.HasValue)
    {
      int num;
      team.Comp.Roles.TryGetValue(nullable.Value, out num);
      team.Comp.Roles[nullable.Value] = num + 1;
    }
    SquadMemberUpdatedEvent args1 = new SquadMemberUpdatedEvent((EntityUid) team);
    this.RaiseLocalEvent<SquadMemberUpdatedEvent>(marine, ref args1);
    if (squad.HasValue && component != null)
    {
      SquadMemberRemovedEvent args2 = new SquadMemberRemovedEvent((Entity<SquadTeamComponent>) (squad.Value, component), marine);
      this.RaiseLocalEvent<SquadMemberRemovedEvent>(marine, ref args2, true);
    }
    SquadMemberAddedEvent args3 = new SquadMemberAddedEvent((Entity<SquadTeamComponent>) ((EntityUid) team, team.Comp), marine);
    this.RaiseLocalEvent<SquadMemberAddedEvent>(marine, ref args3, true);
    string id = this.Prototype((EntityUid) team)?.ID;
    if (id != null)
      this._appearance.SetData(marine, (Enum) SquadVisuals.Squad, (object) id);
    this.UpdateSquadTitle(marine);
    this.SearchForMappedItems((Entity<SquadMemberComponent>) (marine, squadMemberComponent), squadMemberComponent.Squad.Value);
  }

  public void RemoveSquad(EntityUid marine, ProtoId<JobPrototype>? job)
  {
    this.RemComp<SquadLeaderComponent>(marine);
    SquadMemberComponent comp;
    if (!this.TryComp<SquadMemberComponent>(marine, out comp))
      return;
    EntityUid? squad = comp.Squad;
    ProtoId<JobPrototype>? nullable = (ProtoId<JobPrototype>?) (job ?? this._originalRoleQuery.CompOrNull(marine)?.Job);
    SquadTeamComponent component;
    if (this._squadTeamQuery.TryComp(squad, out component))
    {
      component.Members.Remove(marine);
      int num;
      if (nullable.HasValue && component.Roles.TryGetValue(nullable.Value, out num) && num > 0)
        component.Roles[nullable.Value] = num - 1;
    }
    this.RemComp<SquadMemberComponent>(marine);
    if (!squad.HasValue || component == null)
      return;
    SquadMemberRemovedEvent args = new SquadMemberRemovedEvent((Entity<SquadTeamComponent>) (squad.Value, component), marine);
    this.RaiseLocalEvent<SquadMemberRemovedEvent>(marine, ref args, true);
  }

  public void UpdateSquadTitle(EntityUid marine)
  {
    SquadNameOverrideComponent comp;
    if (this.TryComp<SquadNameOverrideComponent>(marine, out comp))
    {
      this.MarineSetTitle(marine, this.Loc.GetString((string) comp.Name));
    }
    else
    {
      GetMarineSquadNameEvent args = new GetMarineSquadNameEvent();
      this.RaiseLocalEvent<GetMarineSquadNameEvent>(marine, ref args);
      this.MarineSetTitle(marine, $"{args.SquadName} {args.RoleName}");
    }
  }

  public void MarineSetTitle(EntityUid marine, string title)
  {
    foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities((Entity<HandsComponent, InventoryComponent>) marine))
    {
      IdCardComponent comp;
      if (this.TryComp<IdCardComponent>(orInventoryEntity, out comp))
        this._id.TryChangeJobTitle(orInventoryEntity, title, comp);
    }
  }

  public void RefreshSquad(Entity<SquadTeamComponent?> squad)
  {
    if (!this._squadTeamQuery.Resolve((EntityUid) squad, ref squad.Comp, false))
      return;
    List<EntityUid> other = new List<EntityUid>();
    foreach (EntityUid member in squad.Comp.Members)
    {
      SquadMemberComponent component;
      if (!this.TerminatingOrDeleted(member) && this._squadMemberQuery.TryComp(member, out component))
      {
        EntityUid? squad1 = component.Squad;
        EntityUid entityUid = (EntityUid) squad;
        if ((squad1.HasValue ? (squad1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
          continue;
      }
      other.Add(member);
    }
    squad.Comp.Members.ExceptWith((IEnumerable<EntityUid>) other);
    this.Dirty<SquadTeamComponent>(squad);
    foreach (EntityUid entityUid in other)
    {
      SquadMemberUpdatedEvent args1 = new SquadMemberUpdatedEvent((EntityUid) squad);
      this.RaiseLocalEvent<SquadMemberUpdatedEvent>(entityUid, ref args1);
      SquadMemberRemovedEvent args2 = new SquadMemberRemovedEvent((Entity<SquadTeamComponent>) ((EntityUid) squad, squad.Comp), entityUid);
      this.RaiseLocalEvent<SquadMemberRemovedEvent>(entityUid, ref args2, true);
    }
  }

  public bool IsInSquad(Entity<SquadMemberComponent?> member, EntProtoId<SquadTeamComponent> squad)
  {
    if (!this.Resolve<SquadMemberComponent>((EntityUid) member, ref member.Comp, false))
      return false;
    EntityUid? squad1 = member.Comp.Squad;
    return squad1.HasValue && this.Prototype(squad1.GetValueOrDefault())?.ID == squad.Id;
  }

  public bool IsInSquad(Entity<SquadMemberComponent?> member, EntityUid squad)
  {
    if (!this.Resolve<SquadMemberComponent>((EntityUid) member, ref member.Comp, false))
      return false;
    EntityUid? squad1 = member.Comp.Squad;
    return squad1.HasValue && squad1.GetValueOrDefault() == squad;
  }

  public void PromoteSquadLeader(
    Entity<SquadMemberComponent?> toPromote,
    EntityUid user,
    SpriteSpecifier.Rsi icon)
  {
    if (this.HasComp<SquadLeaderComponent>((EntityUid) toPromote))
      return;
    if (this._rmcBan.IsJobBanned((Entity<ActorComponent>) toPromote.Owner, SquadSystem.SquadLeaderJob))
      this._popup.PopupCursor(this.Name((EntityUid) toPromote) + " is unfit to lead!", user, PopupType.MediumCaution);
    else if (this._mobState.IsDead((EntityUid) toPromote))
    {
      this._popup.PopupCursor(this.Name((EntityUid) toPromote) + " is KIA!", user, PopupType.MediumCaution);
    }
    else
    {
      EntityUid? nullable1;
      if (this.Resolve<SquadMemberComponent>((EntityUid) toPromote, ref toPromote.Comp, false))
      {
        Robust.Shared.GameObjects.EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent>();
        EntityUid uid;
        SquadLeaderComponent comp1;
        SquadMemberComponent comp2;
        while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
        {
          EntityUid? squad = comp2.Squad;
          nullable1 = toPromote.Comp.Squad;
          if ((squad.HasValue == nullable1.HasValue ? (squad.HasValue ? (squad.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          {
            nullable1 = comp1.Headset;
            if (nullable1.HasValue)
            {
              EntityUid valueOrDefault = nullable1.GetValueOrDefault();
              this.RemComp<SquadLeaderHeadsetComponent>(valueOrDefault);
              EncryptionKeyHolderComponent comp;
              if (this.TryComp<EncryptionKeyHolderComponent>(valueOrDefault, out comp))
                this._encryptionKey.UpdateChannels(valueOrDefault, comp);
            }
            MarineComponent comp3;
            if (this.TryComp<MarineComponent>(uid, out comp3) && object.Equals((object) comp3.Icon, (object) comp1.Icon))
              this._marine.ClearIcon((Entity<MarineComponent>) (uid, comp3));
            MarineOrdersComponent comp4;
            if (this.TryComp<MarineOrdersComponent>(uid, out comp4) && !comp4.Intrinsic)
              this.RemCompDeferred<MarineOrdersComponent>(uid);
            this.RemComp<SquadLeaderComponent>(uid);
            this.RemComp<RMCTrackableComponent>(uid);
            this.RemCompDeferred<RMCPointingComponent>(uid);
          }
        }
      }
      SquadLeaderComponent squadLeaderComponent = this.EnsureComp<SquadLeaderComponent>((EntityUid) toPromote);
      squadLeaderComponent.Icon = icon;
      MarineOrdersComponent comp5;
      if (!this.EnsureComp<MarineOrdersComponent>((EntityUid) toPromote, out comp5))
      {
        comp5.Intrinsic = false;
        this.Dirty((EntityUid) toPromote, (IComponent) comp5);
        this._marineOrders.StartActionUseDelay((Entity<MarineOrdersComponent>) ((EntityUid) toPromote, comp5));
      }
      this.EnsureComp<RMCTrackableComponent>((EntityUid) toPromote);
      this.EnsureComp<RMCPointingComponent>((EntityUid) toPromote);
      InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) toPromote.Owner, SlotFlags.EARS);
      ContainerSlot container;
      while (slotEnumerator.MoveNext(out container))
      {
        nullable1 = container.ContainedEntity;
        if (nullable1.HasValue)
        {
          EntityUid valueOrDefault = nullable1.GetValueOrDefault();
          EncryptionKeyHolderComponent comp6;
          if (this.TryComp<EncryptionKeyHolderComponent>(valueOrDefault, out comp6))
          {
            squadLeaderComponent.Headset = new EntityUid?(valueOrDefault);
            this.Dirty((EntityUid) toPromote, (IComponent) squadLeaderComponent);
            this.EnsureComp<SquadLeaderHeadsetComponent>(valueOrDefault);
            this._encryptionKey.UpdateChannels(valueOrDefault, comp6);
            break;
          }
        }
      }
      SquadMemberComponent comp7 = toPromote.Comp;
      EntityUid? nullable2;
      if (comp7 == null)
      {
        nullable1 = new EntityUid?();
        nullable2 = nullable1;
      }
      else
        nullable2 = comp7.Squad;
      EntityUid? uid1 = nullable2;
      ActorComponent comp8;
      if (this.TryComp<ActorComponent>((EntityUid) toPromote, out comp8))
      {
        string str = $"Overwatch: You've been promoted to 'ACTING SQUAD LEADER'{(this.Exists(uid1) ? " for " + this.Name(uid1.Value) : string.Empty)}. Your headset has access to the command channel (:v).";
        this._rmcChat.ChatMessageToOne(ChatChannel.Local, str, str, new EntityUid(), false, comp8.PlayerSession.Channel, new Color?(Color.FromHex((ReadOnlySpan<char>) "#0084FF", new Color?())), true);
      }
      if (!this.Exists(uid1))
        return;
      Robust.Shared.Prototypes.EntityPrototype entityPrototype = this.Prototype(uid1.Value);
      if (entityPrototype == null)
        return;
      this._marineAnnounce.AnnounceSquad("Attention: A new Squad Leader has been set: " + this.Name((EntityUid) toPromote), (EntProtoId<SquadTeamComponent>) entityPrototype.ID);
      this._popup.PopupCursor($"{this.Name((EntityUid) toPromote)} is {this.Name(uid1.Value)}'s new leader!", user, PopupType.Medium);
    }
  }

  public bool AreInSameSquad(Entity<SquadMemberComponent?> one, Entity<SquadMemberComponent?> two)
  {
    if (!this.Resolve<SquadMemberComponent>((EntityUid) one, ref one.Comp, false) || !this.Resolve<SquadMemberComponent>((EntityUid) two, ref two.Comp, false) || !one.Comp.Squad.HasValue)
      return false;
    EntityUid? squad1 = one.Comp.Squad;
    EntityUid? squad2 = two.Comp.Squad;
    if (squad1.HasValue != squad2.HasValue)
      return false;
    return !squad1.HasValue || squad1.GetValueOrDefault() == squad2.GetValueOrDefault();
  }

  public bool TryGetSquadLeader(
    Entity<SquadTeamComponent> squad,
    out Entity<SquadLeaderComponent> leader)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent>();
    EntityUid uid;
    SquadLeaderComponent comp1;
    SquadMemberComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      EntityUid? squad1 = comp2.Squad;
      EntityUid entityUid = (EntityUid) squad;
      if ((squad1.HasValue ? (squad1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
      {
        leader = (Entity<SquadLeaderComponent>) (uid, comp1);
        return true;
      }
    }
    leader = new Entity<SquadLeaderComponent>();
    return false;
  }

  public bool IsSquadLeader(ProtoId<JobPrototype> job) => job == SquadSystem.SquadLeaderJob;

  public bool HasSpaceForRole(Entity<SquadTeamComponent> squad, ProtoId<JobPrototype> job)
  {
    int num1;
    if (!squad.Comp.MaxRoles.TryGetValue(job, out num1))
      return true;
    int num2;
    squad.Comp.Roles.TryGetValue(job, out num2);
    return num2 < num1;
  }

  public List<EntityUid>? GetEntitiesWithHighestSquad(
    List<EntityUid> entities,
    ProtoId<DatasetPrototype> squadHierarchyId)
  {
    List<EntityUid> entityUidList = new List<EntityUid>();
    DatasetPrototype prototype;
    if (!this._prototypes.TryIndex<DatasetPrototype>(squadHierarchyId, out prototype))
      return (List<EntityUid>) null;
    List<string> list = prototype.Values.ToList<string>();
    if (list.Count == 0)
    {
      this.Log.Error($"The squad hierarchy dataset '{squadHierarchyId}' has an invalid value: empty. The highest squad cannot be determined.");
      return (List<EntityUid>) null;
    }
    Dictionary<EntityUid, int> source = new Dictionary<EntityUid, int>();
    int highestSquadIndex = -1;
    foreach (EntityUid entity in entities)
    {
      SquadMemberComponent comp;
      if (this.TryComp<SquadMemberComponent>(entity, out comp) && comp.Squad.HasValue)
      {
        int num = !comp.Squad.HasValue || comp.Squad.ToString() == null ? -1 : list.IndexOf(comp.Squad.ToString());
        if (num != -1)
        {
          source[entity] = num;
          if (num > highestSquadIndex)
            highestSquadIndex = num;
        }
      }
    }
    return highestSquadIndex == -1 ? (List<EntityUid>) null : source.Where<KeyValuePair<EntityUid, int>>((Func<KeyValuePair<EntityUid, int>, bool>) (pair => pair.Value == highestSquadIndex)).Select<KeyValuePair<EntityUid, int>, EntityUid>((Func<KeyValuePair<EntityUid, int>, EntityUid>) (pair => pair.Key)).ToList<EntityUid>();
  }

  public bool TryGetSquadMemberColor(EntityUid entity, out Color color, bool accessible = false)
  {
    color = new Color();
    SquadMemberComponent comp;
    if (!this.TryComp<SquadMemberComponent>(entity, out comp))
      return false;
    color = !accessible || !comp.AccessibleBackgroundColor.HasValue ? comp.BackgroundColor : comp.AccessibleBackgroundColor.Value;
    return true;
  }

  public void SetSquadMaxRole(
    Entity<SquadTeamComponent?> squad,
    ProtoId<JobPrototype> job,
    int amount)
  {
    if (!this.Resolve<SquadTeamComponent>((EntityUid) squad, ref squad.Comp, false))
      return;
    squad.Comp.MaxRoles[job] = amount;
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadGrantAccessComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadGrantAccessComponent>();
    EntityUid uid1;
    SquadGrantAccessComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
    {
      if (comp1.RoleName != null)
        this.UpdateSquadTitle(uid1);
      foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities((Entity<HandsComponent, InventoryComponent>) uid1))
      {
        AccessComponent comp2;
        if (comp1.AccessLevels.Length != 0 && this.TryComp<AccessComponent>(orInventoryEntity, out comp2))
        {
          foreach (ProtoId<AccessLevelPrototype> accessLevel in comp1.AccessLevels)
            comp2.Tags.Add(accessLevel);
          this.Dirty(orInventoryEntity, (IComponent) comp2);
        }
        IdCardOwnerComponent comp3;
        if (this.HasComp<IdCardComponent>(orInventoryEntity) && !this.EnsureComp<IdCardOwnerComponent>(orInventoryEntity, out comp3))
          comp3.Id = uid1;
      }
      this.RemCompDeferred<SquadGrantAccessComponent>(uid1);
    }
    foreach (EntityUid uid2 in this._membersToUpdate)
    {
      SquadMemberComponent component1;
      SquadTeamComponent component2;
      if (!this.TerminatingOrDeleted(uid2) && this._squadMemberQuery.TryComp(uid2, out component1) && this._squadTeamQuery.TryComp(component1.Squad, out component2))
        component2.Members.Add(uid2);
    }
    this._membersToUpdate.Clear();
  }
}
