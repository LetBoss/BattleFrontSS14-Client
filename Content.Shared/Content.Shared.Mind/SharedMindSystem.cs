using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

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

	[ViewVariables]
	protected readonly Dictionary<NetUserId, EntityUid> UserMinds = new Dictionary<NetUserId, EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MindContainerComponent, ExaminedEvent>((ComponentEventHandler<MindContainerComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MindContainerComponent, SuicideEvent>((ComponentEventHandler<MindContainerComponent, SuicideEvent>)OnSuicide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisitingMindComponent, EntityTerminatingEvent>((ComponentEventRefHandler<VisitingMindComponent, EntityTerminatingEvent>)OnVisitingTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnReset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MindComponent, ComponentStartup>((ComponentEventHandler<MindComponent, ComponentStartup>)OnMindStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MindComponent, EntityRenamedEvent>((EntityEventRefHandler<MindComponent, EntityRenamedEvent>)OnRenamed, (Type[])null, (Type[])null);
		InitializeRelay();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		WipeAllMinds();
	}

	private void OnMindStartup(EntityUid uid, MindComponent component, ComponentStartup args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (!component.UserId.HasValue || UserMinds.TryAdd(component.UserId.Value, uid))
		{
			return;
		}
		EntityUid existing = UserMinds[component.UserId.Value];
		if (!(existing == uid))
		{
			if (!((EntitySystem)this).Exists(existing))
			{
				((EntitySystem)this).Log.Error($"Found deleted entity in mind dictionary while initializing mind {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
				UserMinds[component.UserId.Value] = uid;
			}
			else
			{
				((EntitySystem)this).Log.Error($"Encountered a user {component.UserId} that is already assigned to a mind while initializing mind {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}. Ignoring user field.");
				component.UserId = null;
			}
		}
	}

	private void OnReset(RoundRestartCleanupEvent ev)
	{
		WipeAllMinds();
	}

	public virtual void WipeAllMinds()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Log.Info("Wiping all minds");
		EntityUid[] array = UserMinds.Values.ToArray();
		foreach (EntityUid mind in array)
		{
			WipeMind(mind, null);
		}
		if (UserMinds.Count == 0)
		{
			return;
		}
		foreach (EntityUid mind2 in UserMinds.Values)
		{
			if (((EntitySystem)this).Exists(mind2))
			{
				((EntitySystem)this).Log.Error($"Failed to wipe mind: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mind2))}");
			}
		}
		UserMinds.Clear();
	}

	public EntityUid? GetMind(NetUserId user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryGetMind(user, out EntityUid? mind, out MindComponent _);
		return mind;
	}

	public virtual bool TryGetMind(NetUserId user, [NotNullWhen(true)] out EntityUid? mindId, [NotNullWhen(true)] out MindComponent? mind)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (UserMinds.TryGetValue(user, out var mindIdValue) && ((EntitySystem)this).TryComp<MindComponent>(mindIdValue, ref mind))
		{
			mindId = mindIdValue;
			return true;
		}
		mindId = null;
		mind = null;
		return false;
	}

	public bool TryGetMind(NetUserId user, [NotNullWhen(true)] out Entity<MindComponent>? mind)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetMind(user, out EntityUid? mindId, out MindComponent mindComp))
		{
			mind = null;
			return false;
		}
		mind = Entity<MindComponent>.op_Implicit((mindId.Value, mindComp));
		return true;
	}

	public Entity<MindComponent> GetOrCreateMind(NetUserId user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetMind(user, out Entity<MindComponent>? mind))
		{
			mind = CreateMind(user);
		}
		return mind.Value;
	}

	private void OnVisitingTerminating(EntityUid uid, VisitingMindComponent component, ref EntityTerminatingEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (component.MindId.HasValue)
		{
			UnVisit(component.MindId.Value);
		}
	}

	private void OnExamined(EntityUid uid, MindContainerComponent mindContainer, ExaminedEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (mindContainer.ShowExamineInfo && args.IsInDetailsRange && !_net.IsClient)
		{
			bool dead = _mobState.IsDead(uid);
			NetUserId? hasUserId = ((EntitySystem)this).CompOrNull<MindComponent>(mindContainer.Mind)?.UserId;
			bool hasActiveSession = hasUserId.HasValue && _playerManager.ValidSessionId(hasUserId.Value);
			if (dead && !hasUserId.HasValue)
			{
				args.PushMarkup("[color=mediumpurple]" + base.Loc.GetString("comp-mind-examined-dead-and-irrecoverable", (ValueTuple<string, object>)("ent", uid)) + "[/color]");
			}
			else if (dead && !hasActiveSession)
			{
				args.PushMarkup("[color=yellow]" + base.Loc.GetString("comp-mind-examined-dead-and-ssd", (ValueTuple<string, object>)("ent", uid)) + "[/color]");
			}
			else if (!hasUserId.HasValue)
			{
				args.PushMarkup("[color=mediumpurple]" + base.Loc.GetString("comp-mind-examined-catatonic", (ValueTuple<string, object>)("ent", uid)) + "[/color]");
			}
			else if (!hasActiveSession)
			{
				args.PushMarkup("[color=yellow]" + base.Loc.GetString("comp-mind-examined-ssd", (ValueTuple<string, object>)("ent", uid)) + "[/color]");
			}
		}
	}

	private void OnSuicide(EntityUid uid, MindContainerComponent component, SuicideEvent args)
	{
		MindComponent mind = default(MindComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<MindComponent>(component.Mind, ref mind) && mind.PreventSuicide)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnRenamed(Entity<MindComponent> ent, ref EntityRenamedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.CharacterName = ((EntityRenamedEvent)(ref args)).NewName;
		((EntitySystem)this).Dirty<MindComponent>(ent, (MetaDataComponent)null);
	}

	public EntityUid? GetMind(EntityUid uid, MindContainerComponent? mind = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindContainerComponent>(uid, ref mind, true))
		{
			return null;
		}
		if (mind.HasMind)
		{
			return mind.Mind;
		}
		return null;
	}

	public Entity<MindComponent> CreateMind(NetUserId? userId, string? name = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid mindId = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		_metadata.SetEntityName(mindId, (name == null) ? "mind" : ("mind (" + name + ")"), (MetaDataComponent)null, true);
		MindComponent mind = ((EntitySystem)this).EnsureComp<MindComponent>(mindId);
		mind.CharacterName = name;
		SetUserId(mindId, userId, mind);
		return Entity<MindComponent>.op_Implicit((mindId, mind));
	}

	public bool IsCharacterDeadPhysically(MindComponent mind)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!mind.OwnedEntity.HasValue)
		{
			return true;
		}
		MobStateComponent targetMobState = EntityManagerExt.GetComponentOrNull<MobStateComponent>((IEntityManager)(object)base.EntityManager, mind.OwnedEntity);
		if (targetMobState == null)
		{
			return true;
		}
		return _mobState.IsDead(mind.OwnedEntity.Value, targetMobState);
	}

	public bool IsCharacterUnrevivablePhysically(MindComponent mind)
	{
		if (!mind.OwnedEntity.HasValue)
		{
			return true;
		}
		if (!((EntitySystem)this).HasComp<MobStateComponent>(mind.OwnedEntity))
		{
			return true;
		}
		return false;
	}

	public virtual void Visit(EntityUid mindId, EntityUid entity, MindComponent? mind = null)
	{
	}

	public virtual void UnVisit(EntityUid mindId, MindComponent? mind = null)
	{
	}

	public void UnVisit(ICommonSession? player)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (player != null && TryGetMind(player, out EntityUid mindId, out MindComponent mind))
		{
			UnVisit(mindId, mind);
		}
	}

	protected void RemoveVisitingEntity(EntityUid mindId, MindComponent mind)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (mind.VisitingEntity.HasValue)
		{
			EntityUid oldVisitingEnt = mind.VisitingEntity.Value;
			mind.VisitingEntity = null;
			VisitingMindComponent visitComp = default(VisitingMindComponent);
			if (((EntitySystem)this).TryComp<VisitingMindComponent>(oldVisitingEnt, ref visitComp))
			{
				visitComp.MindId = null;
				((EntitySystem)this).RemCompDeferred(oldVisitingEnt, (IComponent)(object)visitComp);
			}
			((EntitySystem)this).Dirty(mindId, (IComponent)(object)mind, (MetaDataComponent)null);
			((EntitySystem)this).RaiseLocalEvent<MindUnvisitedMessage>(oldVisitingEnt, new MindUnvisitedMessage(), true);
		}
	}

	public void WipeMind(ICommonSession player)
	{
		EntityUid? mind = _player.ContentData(player)?.Mind;
		WipeMind(mind);
	}

	public void WipeMind(EntityUid? mindId, MindComponent? mind = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (mindId.HasValue && ((EntitySystem)this).Resolve<MindComponent>(mindId.Value, ref mind, false))
		{
			TransferTo(mindId.Value, null, ghostCheckOverride: false, createGhost: false, mind);
			SetUserId(mindId.Value, null, mind);
		}
	}

	public virtual void TransferTo(EntityUid mindId, EntityUid? entity, bool ghostCheckOverride = false, bool createGhost = true, MindComponent? mind = null)
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? objective = _objectives.TryCreateObjective(mindId, mind, proto);
		if (!objective.HasValue)
		{
			return false;
		}
		AddObjective(mindId, mind, objective.Value);
		return true;
	}

	public void AddObjective(EntityUid mindId, MindComponent mind, EntityUid objective)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		string title = ((EntitySystem)this).Name(objective, (MetaDataComponent)null);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(31, 3);
		handler.AppendLiteral("Objective ");
		handler.AppendFormatted<EntityUid>(objective, "objective");
		handler.AppendLiteral(" (");
		handler.AppendFormatted(title);
		handler.AppendLiteral(") added to mind of ");
		handler.AppendFormatted(MindOwnerLoggingString(mind));
		adminLogger.Add(LogType.Mind, LogImpact.Low, ref handler);
		mind.Objectives.Add(objective);
	}

	public bool TryRemoveObjective(EntityUid mindId, MindComponent mind, int index)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index >= mind.Objectives.Count)
		{
			return false;
		}
		EntityUid objective = mind.Objectives[index];
		string title = ((EntitySystem)this).Name(objective, (MetaDataComponent)null);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(39, 3);
		handler.AppendLiteral("Objective ");
		handler.AppendFormatted<EntityUid>(objective, "objective");
		handler.AppendLiteral(" (");
		handler.AppendFormatted(title);
		handler.AppendLiteral(") removed from the mind of ");
		handler.AppendFormatted(MindOwnerLoggingString(mind));
		adminLogger.Add(LogType.Mind, LogImpact.Low, ref handler);
		mind.Objectives.Remove(objective);
		AllEntityQueryEnumerator<MindComponent> mindQuery = ((EntitySystem)this).AllEntityQuery<MindComponent>();
		EntityUid val = default(EntityUid);
		MindComponent queryComp = default(MindComponent);
		while (mindQuery.MoveNext(ref val, ref queryComp))
		{
			if (queryComp.Objectives.Contains(objective))
			{
				return true;
			}
		}
		((EntitySystem)this).Del((EntityUid?)objective);
		return true;
	}

	public bool TryGetObjectiveComp<T>(EntityUid uid, [NotNullWhen(true)] out T? objective) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetMind(uid, out EntityUid mindId, out MindComponent mind) && TryGetObjectiveComp<T>(mindId, out objective, mind))
		{
			return true;
		}
		objective = default(T);
		return false;
	}

	public bool TryGetObjectiveComp<T>(EntityUid mindId, [NotNullWhen(true)] out T? objective, MindComponent? mind = null) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MindComponent>(mindId, ref mind, true))
		{
			EntityQuery<T> query = ((EntitySystem)this).GetEntityQuery<T>();
			foreach (EntityUid uid in mind.Objectives)
			{
				if (query.TryGetComponent(uid, ref objective))
				{
					return true;
				}
			}
		}
		objective = default(T);
		return false;
	}

	public void CopyObjectives(Entity<MindComponent?> source, Entity<MindComponent?> target, EntityWhitelist? whitelist = null, EntityWhitelist? blacklist = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(Entity<MindComponent>.op_Implicit(source), ref source.Comp, true) || !((EntitySystem)this).Resolve<MindComponent>(Entity<MindComponent>.op_Implicit(target), ref target.Comp, true))
		{
			return;
		}
		foreach (EntityUid objective in source.Comp.Objectives)
		{
			if (!target.Comp.Objectives.Contains(objective) && _whitelist.CheckBoth(objective, blacklist, whitelist))
			{
				AddObjective(Entity<MindComponent>.op_Implicit(target), target.Comp, objective);
			}
		}
	}

	public bool TryFindObjective(Entity<MindComponent?> mind, string prototype, [NotNullWhen(true)] out EntityUid? objective)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		objective = null;
		if (!((EntitySystem)this).Resolve<MindComponent>(Entity<MindComponent>.op_Implicit(mind), ref mind.Comp, true))
		{
			return false;
		}
		foreach (EntityUid uid in mind.Comp.Objectives)
		{
			EntityPrototype entityPrototype = ((EntitySystem)this).MetaData(uid).EntityPrototype;
			if (((entityPrototype != null) ? entityPrototype.ID : null) == prototype)
			{
				objective = uid;
				return true;
			}
		}
		return false;
	}

	public bool TryGetMind(EntityUid uid, out EntityUid mindId, [NotNullWhen(true)] out MindComponent? mind, MindContainerComponent? container = null, VisitingMindComponent? visitingmind = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		mindId = default(EntityUid);
		mind = null;
		if (!((EntitySystem)this).Resolve<MindContainerComponent>(uid, ref container, false))
		{
			return false;
		}
		if (!container.HasMind)
		{
			if (!((EntitySystem)this).Resolve<VisitingMindComponent>(uid, ref visitingmind, false))
			{
				return false;
			}
			mindId = visitingmind.MindId.GetValueOrDefault();
			return ((EntitySystem)this).TryComp<MindComponent>(mindId, ref mind);
		}
		mindId = container.Mind.GetValueOrDefault();
		return ((EntitySystem)this).TryComp<MindComponent>(mindId, ref mind);
	}

	public bool TryGetMind(ICommonSession? player, out EntityUid mindId, [NotNullWhen(true)] out MindComponent? mind)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (player == null)
		{
			mindId = default(EntityUid);
			mind = null;
			return false;
		}
		if (TryGetMind(player.UserId, out var mindUid, out mind))
		{
			mindId = mindUid.Value;
			return true;
		}
		mindId = default(EntityUid);
		return false;
	}

	public virtual void SetUserId(EntityUid mindId, NetUserId? userId, MindComponent? mind = null)
	{
	}

	public bool IsCharacterDeadIc(MindComponent mind)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? ownedEntity = mind.OwnedEntity;
		if (ownedEntity.HasValue)
		{
			EntityUid owned = ownedEntity.GetValueOrDefault();
			GetCharactedDeadIcEvent ev = new GetCharactedDeadIcEvent(null);
			((EntitySystem)this).RaiseLocalEvent<GetCharactedDeadIcEvent>(owned, ref ev, false);
			if (ev.Dead.HasValue)
			{
				return ev.Dead.Value;
			}
		}
		return IsCharacterDeadPhysically(mind);
	}

	public bool IsCharacterUnrevivableIc(MindComponent mind)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? ownedEntity = mind.OwnedEntity;
		if (ownedEntity.HasValue)
		{
			EntityUid owned = ownedEntity.GetValueOrDefault();
			GetCharacterUnrevivableIcEvent ev = new GetCharacterUnrevivableIcEvent(null);
			((EntitySystem)this).RaiseLocalEvent<GetCharacterUnrevivableIcEvent>(owned, ref ev, false);
			if (ev.Unrevivable.HasValue)
			{
				return ev.Unrevivable.Value;
			}
		}
		return IsCharacterUnrevivablePhysically(mind);
	}

	public string MindOwnerLoggingString(MindComponent mind)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (mind.OwnedEntity.HasValue)
		{
			return EntityStringRepresentation.op_Implicit(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mind.OwnedEntity.Value)));
		}
		if (mind.UserId.HasValue)
		{
			return ((object)mind.UserId.Value/*cast due to constrained. prefix*/).ToString();
		}
		return "(originally " + mind.OriginalOwnerUserId.ToString() + ")";
	}

	public string? GetCharacterName(NetUserId userId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetMind(userId, out EntityUid? _, out MindComponent mind))
		{
			return null;
		}
		return mind.CharacterName;
	}

	public HashSet<Entity<MindComponent>> GetAliveHumans(EntityUid? exclude = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Entity<MindComponent>> allHumans = new HashSet<Entity<MindComponent>>();
		EntityQueryEnumerator<MobStateComponent, HumanoidAppearanceComponent> query = ((EntitySystem)this).EntityQueryEnumerator<MobStateComponent, HumanoidAppearanceComponent>();
		EntityUid uid = default(EntityUid);
		MobStateComponent mobState = default(MobStateComponent);
		HumanoidAppearanceComponent humanoidAppearanceComponent = default(HumanoidAppearanceComponent);
		while (query.MoveNext(ref uid, ref mobState, ref humanoidAppearanceComponent))
		{
			if (TryGetMind(uid, out EntityUid mind, out MindComponent mindComp))
			{
				EntityUid val = mind;
				EntityUid? val2 = exclude;
				if ((!val2.HasValue || !(val == val2.GetValueOrDefault())) && _mobState.IsAlive(uid, mobState))
				{
					allHumans.Add(new Entity<MindComponent>(mind, mindComp));
				}
			}
		}
		return allHumans;
	}

	public void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<MindContainerComponent, RefreshNameModifiersEvent>((ComponentEventRefHandler<MindContainerComponent, RefreshNameModifiersEvent>)RelayRefToMind, (Type[])null, (Type[])null);
	}

	protected void RelayToMind<T>(EntityUid uid, MindContainerComponent component, T args) where T : class
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		MindRelayedEvent<T> ev = new MindRelayedEvent<T>(args);
		if (!TryGetMind(uid, out EntityUid mindId, out MindComponent mindComp, component))
		{
			return;
		}
		((EntitySystem)this).RaiseLocalEvent<MindRelayedEvent<T>>(mindId, ref ev, false);
		foreach (EntityUid role in mindComp.MindRoles)
		{
			((EntitySystem)this).RaiseLocalEvent<MindRelayedEvent<T>>(role, ref ev, false);
		}
	}

	protected void RelayRefToMind<T>(EntityUid uid, MindContainerComponent component, ref T args) where T : class
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		MindRelayedEvent<T> ev = new MindRelayedEvent<T>(args);
		if (TryGetMind(uid, out EntityUid mindId, out MindComponent mindComp, component))
		{
			((EntitySystem)this).RaiseLocalEvent<MindRelayedEvent<T>>(mindId, ref ev, false);
			foreach (EntityUid role in mindComp.MindRoles)
			{
				((EntitySystem)this).RaiseLocalEvent<MindRelayedEvent<T>>(role, ref ev, false);
			}
		}
		args = ev.Args;
	}
}
