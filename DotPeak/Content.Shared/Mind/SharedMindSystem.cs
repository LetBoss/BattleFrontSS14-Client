// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mind.SharedMindSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Objectives.Systems;
using Content.Shared.Players;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Mind;

public abstract class SharedMindSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedObjectivesSystem _objectives;
  [Dependency]
  private SharedPlayerSystem _player;
  [Dependency]
  private ISharedPlayerManager _playerManager;
  [Dependency]
  private MetaDataSystem _metadata;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Robust.Shared.ViewVariables.ViewVariables]
  protected readonly Dictionary<NetUserId, EntityUid> UserMinds = new Dictionary<NetUserId, EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MindContainerComponent, ExaminedEvent>(new ComponentEventHandler<MindContainerComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<MindContainerComponent, SuicideEvent>(new ComponentEventHandler<MindContainerComponent, SuicideEvent>(this.OnSuicide));
    this.SubscribeLocalEvent<VisitingMindComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<VisitingMindComponent, EntityTerminatingEvent>(this.OnVisitingTerminating));
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnReset));
    this.SubscribeLocalEvent<MindComponent, ComponentStartup>(new ComponentEventHandler<MindComponent, ComponentStartup>(this.OnMindStartup));
    this.SubscribeLocalEvent<MindComponent, EntityRenamedEvent>(new EntityEventRefHandler<MindComponent, EntityRenamedEvent>(this.OnRenamed));
    this.InitializeRelay();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    this.WipeAllMinds();
  }

  private void OnMindStartup(EntityUid uid, MindComponent component, ComponentStartup args)
  {
    if (!component.UserId.HasValue)
      return;
    Dictionary<NetUserId, EntityUid> userMinds1 = this.UserMinds;
    NetUserId? nullable1 = component.UserId;
    NetUserId key1 = nullable1.Value;
    EntityUid entityUid1 = uid;
    if (userMinds1.TryAdd(key1, entityUid1))
      return;
    Dictionary<NetUserId, EntityUid> userMinds2 = this.UserMinds;
    nullable1 = component.UserId;
    NetUserId key2 = nullable1.Value;
    EntityUid uid1 = userMinds2[key2];
    if (uid1 == uid)
      return;
    if (!this.Exists(uid1))
    {
      this.Log.Error($"Found deleted entity in mind dictionary while initializing mind {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
      Dictionary<NetUserId, EntityUid> userMinds3 = this.UserMinds;
      nullable1 = component.UserId;
      NetUserId key3 = nullable1.Value;
      EntityUid entityUid2 = uid;
      userMinds3[key3] = entityUid2;
    }
    else
    {
      this.Log.Error($"Encountered a user {component.UserId} that is already assigned to a mind while initializing mind {this.ToPrettyString((Entity<MetaDataComponent>) uid)}. Ignoring user field.");
      MindComponent mindComponent = component;
      nullable1 = new NetUserId?();
      NetUserId? nullable2 = nullable1;
      mindComponent.UserId = nullable2;
    }
  }

  private void OnReset(RoundRestartCleanupEvent ev) => this.WipeAllMinds();

  public virtual void WipeAllMinds()
  {
    this.Log.Info("Wiping all minds");
    foreach (EntityUid entityUid in this.UserMinds.Values.ToArray<EntityUid>())
      this.WipeMind(new EntityUid?(entityUid));
    if (this.UserMinds.Count == 0)
      return;
    foreach (EntityUid uid in this.UserMinds.Values)
    {
      if (this.Exists(uid))
        this.Log.Error($"Failed to wipe mind: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
    this.UserMinds.Clear();
  }

  public EntityUid? GetMind(NetUserId user)
  {
    EntityUid? mindId;
    this.TryGetMind(user, out mindId, out MindComponent _);
    return mindId;
  }

  public virtual bool TryGetMind(NetUserId user, [NotNullWhen(true)] out EntityUid? mindId, [NotNullWhen(true)] out MindComponent? mind)
  {
    EntityUid uid;
    if (this.UserMinds.TryGetValue(user, out uid) && this.TryComp<MindComponent>(uid, out mind))
    {
      mindId = new EntityUid?(uid);
      return true;
    }
    mindId = new EntityUid?();
    mind = (MindComponent) null;
    return false;
  }

  public bool TryGetMind(NetUserId user, [NotNullWhen(true)] out Entity<MindComponent>? mind)
  {
    EntityUid? mindId;
    MindComponent mind1;
    if (!this.TryGetMind(user, out mindId, out mind1))
    {
      mind = new Entity<MindComponent>?();
      return false;
    }
    mind = new Entity<MindComponent>?((Entity<MindComponent>) (mindId.Value, mind1));
    return true;
  }

  public Entity<MindComponent> GetOrCreateMind(NetUserId user)
  {
    Entity<MindComponent>? mind;
    if (!this.TryGetMind(user, out mind))
      mind = new Entity<MindComponent>?(this.CreateMind(new NetUserId?(user)));
    return mind.Value;
  }

  private void OnVisitingTerminating(
    EntityUid uid,
    VisitingMindComponent component,
    ref EntityTerminatingEvent args)
  {
    if (!component.MindId.HasValue)
      return;
    this.UnVisit(component.MindId.Value);
  }

  private void OnExamined(EntityUid uid, MindContainerComponent mindContainer, ExaminedEvent args)
  {
    if (!mindContainer.ShowExamineInfo || !args.IsInDetailsRange || this._net.IsClient)
      return;
    bool flag1 = this._mobState.IsDead(uid);
    NetUserId? userId = (NetUserId?) this.CompOrNull<MindComponent>(mindContainer.Mind)?.UserId;
    bool flag2 = userId.HasValue && this._playerManager.ValidSessionId(userId.Value);
    if (flag1 && !userId.HasValue)
      args.PushMarkup($"[color=mediumpurple]{this.Loc.GetString("comp-mind-examined-dead-and-irrecoverable", ("ent", (object) uid))}[/color]");
    else if (flag1 && !flag2)
      args.PushMarkup($"[color=yellow]{this.Loc.GetString("comp-mind-examined-dead-and-ssd", ("ent", (object) uid))}[/color]");
    else if (!userId.HasValue)
    {
      args.PushMarkup($"[color=mediumpurple]{this.Loc.GetString("comp-mind-examined-catatonic", ("ent", (object) uid))}[/color]");
    }
    else
    {
      if (flag2)
        return;
      args.PushMarkup($"[color=yellow]{this.Loc.GetString("comp-mind-examined-ssd", ("ent", (object) uid))}[/color]");
    }
  }

  private void OnSuicide(EntityUid uid, MindContainerComponent component, SuicideEvent args)
  {
    MindComponent comp;
    if (args.Handled || !this.TryComp<MindComponent>(component.Mind, out comp) || !comp.PreventSuicide)
      return;
    args.Handled = true;
  }

  private void OnRenamed(Entity<MindComponent> ent, ref EntityRenamedEvent args)
  {
    ent.Comp.CharacterName = args.NewName;
    this.Dirty<MindComponent>(ent);
  }

  public EntityUid? GetMind(EntityUid uid, MindContainerComponent? mind = null)
  {
    if (!this.Resolve<MindContainerComponent>(uid, ref mind))
      return new EntityUid?();
    return mind.HasMind ? mind.Mind : new EntityUid?();
  }

  public Entity<MindComponent> CreateMind(NetUserId? userId, string? name = null)
  {
    EntityUid entityUid = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
    this._metadata.SetEntityName(entityUid, name == null ? "mind" : $"mind ({name})");
    MindComponent mind = this.EnsureComp<MindComponent>(entityUid);
    mind.CharacterName = name;
    this.SetUserId(entityUid, userId, mind);
    return (Entity<MindComponent>) (entityUid, mind);
  }

  public bool IsCharacterDeadPhysically(MindComponent mind)
  {
    if (!mind.OwnedEntity.HasValue)
      return true;
    MobStateComponent componentOrNull = this.EntityManager.GetComponentOrNull<MobStateComponent>(mind.OwnedEntity);
    return componentOrNull == null || this._mobState.IsDead(mind.OwnedEntity.Value, componentOrNull);
  }

  public bool IsCharacterUnrevivablePhysically(MindComponent mind)
  {
    return !mind.OwnedEntity.HasValue || !this.HasComp<MobStateComponent>(mind.OwnedEntity);
  }

  public virtual void Visit(EntityUid mindId, EntityUid entity, MindComponent? mind = null)
  {
  }

  public virtual void UnVisit(EntityUid mindId, MindComponent? mind = null)
  {
  }

  public void UnVisit(ICommonSession? player)
  {
    EntityUid mindId;
    MindComponent mind;
    if (player == null || !this.TryGetMind(player, out mindId, out mind))
      return;
    this.UnVisit(mindId, mind);
  }

  protected void RemoveVisitingEntity(EntityUid mindId, MindComponent mind)
  {
    if (!mind.VisitingEntity.HasValue)
      return;
    EntityUid uid = mind.VisitingEntity.Value;
    mind.VisitingEntity = new EntityUid?();
    VisitingMindComponent comp;
    if (this.TryComp<VisitingMindComponent>(uid, out comp))
    {
      comp.MindId = new EntityUid?();
      this.RemCompDeferred(uid, (IComponent) comp);
    }
    this.Dirty(mindId, (IComponent) mind);
    this.RaiseLocalEvent<MindUnvisitedMessage>(uid, new MindUnvisitedMessage(), true);
  }

  public void WipeMind(ICommonSession player)
  {
    this.WipeMind((EntityUid?) this._player.ContentData(player)?.Mind);
  }

  public void WipeMind(EntityUid? mindId, MindComponent? mind = null)
  {
    if (!mindId.HasValue || !this.Resolve<MindComponent>(mindId.Value, ref mind, false))
      return;
    this.TransferTo(mindId.Value, new EntityUid?(), createGhost: false, mind: mind);
    this.SetUserId(mindId.Value, new NetUserId?(), mind);
  }

  public virtual void TransferTo(
    EntityUid mindId,
    EntityUid? entity,
    bool ghostCheckOverride = false,
    bool createGhost = true,
    MindComponent? mind = null)
  {
  }

  public virtual void ControlMob(EntityUid user, EntityUid target)
  {
  }

  public virtual void ControlMob(NetUserId user, EntityUid target)
  {
  }

  public bool TryAddObjective(EntityUid mindId, MindComponent mind, string proto)
  {
    EntityUid? objective = this._objectives.TryCreateObjective(mindId, mind, proto);
    if (!objective.HasValue)
      return false;
    this.AddObjective(mindId, mind, objective.Value);
    return true;
  }

  public void AddObjective(EntityUid mindId, MindComponent mind, EntityUid objective)
  {
    string str = this.Name(objective);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(31 /*0x1F*/, 3);
    logStringHandler.AppendLiteral("Objective ");
    logStringHandler.AppendFormatted<EntityUid>(objective, nameof (objective));
    logStringHandler.AppendLiteral(" (");
    logStringHandler.AppendFormatted(str);
    logStringHandler.AppendLiteral(") added to mind of ");
    logStringHandler.AppendFormatted(this.MindOwnerLoggingString(mind));
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Mind, LogImpact.Low, ref local);
    mind.Objectives.Add(objective);
  }

  public bool TryRemoveObjective(EntityUid mindId, MindComponent mind, int index)
  {
    if (index < 0 || index >= mind.Objectives.Count)
      return false;
    EntityUid objective = mind.Objectives[index];
    string str = this.Name(objective);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(39, 3);
    logStringHandler.AppendLiteral("Objective ");
    logStringHandler.AppendFormatted<EntityUid>(objective, "objective");
    logStringHandler.AppendLiteral(" (");
    logStringHandler.AppendFormatted(str);
    logStringHandler.AppendLiteral(") removed from the mind of ");
    logStringHandler.AppendFormatted(this.MindOwnerLoggingString(mind));
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Mind, LogImpact.Low, ref local);
    mind.Objectives.Remove(objective);
    AllEntityQueryEnumerator<MindComponent> entityQueryEnumerator = this.AllEntityQuery<MindComponent>();
    MindComponent comp1;
    while (entityQueryEnumerator.MoveNext(out EntityUid _, out comp1))
    {
      if (comp1.Objectives.Contains(objective))
        return true;
    }
    this.Del(new EntityUid?(objective));
    return true;
  }

  public bool TryGetObjectiveComp<T>(EntityUid uid, [NotNullWhen(true)] out T? objective) where T : IComponent
  {
    EntityUid mindId;
    MindComponent mind;
    if (this.TryGetMind(uid, out mindId, out mind) && this.TryGetObjectiveComp<T>(mindId, out objective, mind))
      return true;
    objective = default (T);
    return false;
  }

  public bool TryGetObjectiveComp<T>(EntityUid mindId, [NotNullWhen(true)] out T? objective, MindComponent? mind = null) where T : IComponent
  {
    if (this.Resolve<MindComponent>(mindId, ref mind))
    {
      Robust.Shared.GameObjects.EntityQuery<T> entityQuery = this.GetEntityQuery<T>();
      foreach (EntityUid objective1 in mind.Objectives)
      {
        if (entityQuery.TryGetComponent(objective1, out objective))
          return true;
      }
    }
    objective = default (T);
    return false;
  }

  public void CopyObjectives(
    Entity<MindComponent?> source,
    Entity<MindComponent?> target,
    EntityWhitelist? whitelist = null,
    EntityWhitelist? blacklist = null)
  {
    if (!this.Resolve<MindComponent>((EntityUid) source, ref source.Comp) || !this.Resolve<MindComponent>((EntityUid) target, ref target.Comp))
      return;
    foreach (EntityUid objective in source.Comp.Objectives)
    {
      if (!target.Comp.Objectives.Contains(objective) && this._whitelist.CheckBoth(new EntityUid?(objective), blacklist, whitelist))
        this.AddObjective((EntityUid) target, target.Comp, objective);
    }
  }

  public bool TryFindObjective(
    Entity<MindComponent?> mind,
    string prototype,
    [NotNullWhen(true)] out EntityUid? objective)
  {
    objective = new EntityUid?();
    if (!this.Resolve<MindComponent>((EntityUid) mind, ref mind.Comp))
      return false;
    foreach (EntityUid objective1 in mind.Comp.Objectives)
    {
      if (this.MetaData(objective1).EntityPrototype?.ID == prototype)
      {
        objective = new EntityUid?(objective1);
        return true;
      }
    }
    return false;
  }

  public bool TryGetMind(
    EntityUid uid,
    out EntityUid mindId,
    [NotNullWhen(true)] out MindComponent? mind,
    MindContainerComponent? container = null,
    VisitingMindComponent? visitingmind = null)
  {
    mindId = new EntityUid();
    mind = (MindComponent) null;
    if (!this.Resolve<MindContainerComponent>(uid, ref container, false))
      return false;
    if (!container.HasMind)
    {
      if (!this.Resolve<VisitingMindComponent>(uid, ref visitingmind, false))
        return false;
      mindId = visitingmind.MindId.GetValueOrDefault();
      return this.TryComp<MindComponent>(mindId, out mind);
    }
    mindId = container.Mind.GetValueOrDefault();
    return this.TryComp<MindComponent>(mindId, out mind);
  }

  public bool TryGetMind(ICommonSession? player, out EntityUid mindId, [NotNullWhen(true)] out MindComponent? mind)
  {
    if (player == null)
    {
      mindId = new EntityUid();
      mind = (MindComponent) null;
      return false;
    }
    EntityUid? mindId1;
    if (this.TryGetMind(player.UserId, out mindId1, out mind))
    {
      mindId = mindId1.Value;
      return true;
    }
    mindId = new EntityUid();
    return false;
  }

  public virtual void SetUserId(EntityUid mindId, NetUserId? userId, MindComponent? mind = null)
  {
  }

  public bool IsCharacterDeadIc(MindComponent mind)
  {
    EntityUid? ownedEntity = mind.OwnedEntity;
    if (ownedEntity.HasValue)
    {
      EntityUid valueOrDefault = ownedEntity.GetValueOrDefault();
      GetCharactedDeadIcEvent args = new GetCharactedDeadIcEvent(new bool?());
      this.RaiseLocalEvent<GetCharactedDeadIcEvent>(valueOrDefault, ref args);
      if (args.Dead.HasValue)
        return args.Dead.Value;
    }
    return this.IsCharacterDeadPhysically(mind);
  }

  public bool IsCharacterUnrevivableIc(MindComponent mind)
  {
    EntityUid? ownedEntity = mind.OwnedEntity;
    if (ownedEntity.HasValue)
    {
      EntityUid valueOrDefault = ownedEntity.GetValueOrDefault();
      GetCharacterUnrevivableIcEvent args = new GetCharacterUnrevivableIcEvent(new bool?());
      this.RaiseLocalEvent<GetCharacterUnrevivableIcEvent>(valueOrDefault, ref args);
      if (args.Unrevivable.HasValue)
        return args.Unrevivable.Value;
    }
    return this.IsCharacterUnrevivablePhysically(mind);
  }

  public string MindOwnerLoggingString(MindComponent mind)
  {
    if (mind.OwnedEntity.HasValue)
      return (string) this.ToPrettyString((Entity<MetaDataComponent>) mind.OwnedEntity.Value);
    return mind.UserId.HasValue ? mind.UserId.Value.ToString() : $"(originally {mind.OriginalOwnerUserId.ToString()})";
  }

  public string? GetCharacterName(NetUserId userId)
  {
    MindComponent mind;
    return !this.TryGetMind(userId, out EntityUid? _, out mind) ? (string) null : mind.CharacterName;
  }

  public HashSet<Entity<MindComponent>> GetAliveHumans(EntityUid? exclude = null)
  {
    HashSet<Entity<MindComponent>> aliveHumans = new HashSet<Entity<MindComponent>>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<MobStateComponent, HumanoidAppearanceComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MobStateComponent, HumanoidAppearanceComponent>();
    EntityUid uid;
    MobStateComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out HumanoidAppearanceComponent _))
    {
      EntityUid mindId;
      MindComponent mind;
      if (this.TryGetMind(uid, out mindId, out mind))
      {
        EntityUid entityUid = mindId;
        EntityUid? nullable = exclude;
        if ((nullable.HasValue ? (entityUid == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && this._mobState.IsAlive(uid, comp1))
          aliveHumans.Add(new Entity<MindComponent>(mindId, mind));
      }
    }
    return aliveHumans;
  }

  public void InitializeRelay()
  {
    this.SubscribeLocalEvent<MindContainerComponent, RefreshNameModifiersEvent>(new ComponentEventRefHandler<MindContainerComponent, RefreshNameModifiersEvent>(this.RelayRefToMind<RefreshNameModifiersEvent>));
  }

  protected void RelayToMind<T>(EntityUid uid, MindContainerComponent component, T args) where T : class
  {
    MindRelayedEvent<T> args1 = new MindRelayedEvent<T>(args);
    EntityUid mindId;
    MindComponent mind;
    if (!this.TryGetMind(uid, out mindId, out mind, component))
      return;
    this.RaiseLocalEvent<MindRelayedEvent<T>>(mindId, ref args1);
    foreach (EntityUid mindRole in mind.MindRoles)
      this.RaiseLocalEvent<MindRelayedEvent<T>>(mindRole, ref args1);
  }

  protected void RelayRefToMind<T>(EntityUid uid, MindContainerComponent component, ref T args) where T : class
  {
    MindRelayedEvent<T> args1 = new MindRelayedEvent<T>(args);
    EntityUid mindId;
    MindComponent mind;
    if (this.TryGetMind(uid, out mindId, out mind, component))
    {
      this.RaiseLocalEvent<MindRelayedEvent<T>>(mindId, ref args1);
      foreach (EntityUid mindRole in mind.MindRoles)
        this.RaiseLocalEvent<MindRelayedEvent<T>>(mindRole, ref args1);
    }
    args = args1.Args;
  }
}
