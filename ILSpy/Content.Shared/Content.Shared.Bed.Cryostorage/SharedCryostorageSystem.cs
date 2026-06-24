using System;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.DragDrop;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Bed.Cryostorage;

public abstract class SharedCryostorageSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _configuration;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected ISharedAdminLogManager AdminLog;

	[Dependency]
	protected SharedMindSystem Mind;

	protected bool CryoSleepRejoiningEnabled;

	protected EntityUid? PausedMap { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CryostorageComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<CryostorageComponent, EntInsertedIntoContainerMessage>)OnInsertedContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryostorageComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<CryostorageComponent, EntRemovedFromContainerMessage>)OnRemovedContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryostorageComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<CryostorageComponent, ContainerIsInsertingAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryostorageComponent, ComponentShutdown>((EntityEventRefHandler<CryostorageComponent, ComponentShutdown>)OnShutdownContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryostorageComponent, CanDropTargetEvent>((EntityEventRefHandler<CryostorageComponent, CanDropTargetEvent>)OnCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryostorageContainedComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<CryostorageContainedComponent, EntGotRemovedFromContainerMessage>)OnRemovedContained, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CryostorageContainedComponent, ComponentShutdown>((EntityEventRefHandler<CryostorageContainedComponent, ComponentShutdown>)OnShutdownContained, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, CCVars.GameCryoSleepRejoining, (Action<bool>)OnCvarChanged, true);
	}

	private void OnCvarChanged(bool value)
	{
		CryoSleepRejoiningEnabled = value;
	}

	protected virtual void OnInsertedContainer(Entity<CryostorageComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Entity<CryostorageComponent> val = ent;
		EntityUid mindId = default(EntityUid);
		CryostorageComponent cryostorageComponent = default(CryostorageComponent);
		val.Deconstruct(ref mindId, ref cryostorageComponent);
		CryostorageComponent comp = cryostorageComponent;
		if (!(((ContainerModifiedMessage)args).Container.ID != comp.ContainerId))
		{
			_appearance.SetData(Entity<CryostorageComponent>.op_Implicit(ent), (Enum)CryostorageVisuals.Full, (object)true, (AppearanceComponent)null);
			if (Timing.IsFirstTimePredicted)
			{
				CryostorageContainedComponent containedComp = ((EntitySystem)this).EnsureComp<CryostorageContainedComponent>(((ContainerModifiedMessage)args).Entity);
				MindComponent mind;
				TimeSpan delay = (Mind.TryGetMind(((ContainerModifiedMessage)args).Entity, out mindId, out mind) ? comp.GracePeriod : comp.NoMindGracePeriod);
				containedComp.GracePeriodEndTime = Timing.CurTime + delay;
				containedComp.Cryostorage = Entity<CryostorageComponent>.op_Implicit(ent);
				((EntitySystem)this).Dirty(((ContainerModifiedMessage)args).Entity, (IComponent)(object)containedComp, (MetaDataComponent)null);
			}
		}
	}

	private void OnRemovedContainer(Entity<CryostorageComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Entity<CryostorageComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		CryostorageComponent cryostorageComponent = default(CryostorageComponent);
		val.Deconstruct(ref val2, ref cryostorageComponent);
		CryostorageComponent comp = cryostorageComponent;
		if (!(((ContainerModifiedMessage)args).Container.ID != comp.ContainerId))
		{
			_appearance.SetData(Entity<CryostorageComponent>.op_Implicit(ent), (Enum)CryostorageVisuals.Full, (object)(((ContainerModifiedMessage)args).Container.ContainedEntities.Count > 0), (AppearanceComponent)null);
		}
	}

	private void OnInsertAttempt(Entity<CryostorageComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Entity<CryostorageComponent> val = ent;
		EntityUid mindId = default(EntityUid);
		CryostorageComponent cryostorageComponent = default(CryostorageComponent);
		val.Deconstruct(ref mindId, ref cryostorageComponent);
		CryostorageComponent comp = cryostorageComponent;
		if (!(((ContainerAttemptEventBase)args).Container.ID != comp.ContainerId))
		{
			MindContainerComponent mindContainer = default(MindContainerComponent);
			MindComponent mindComp;
			if (_mobState.IsIncapacitated(((ContainerAttemptEventBase)args).EntityUid))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
			else if (!((EntitySystem)this).HasComp<CanEnterCryostorageComponent>(((ContainerAttemptEventBase)args).EntityUid) || !((EntitySystem)this).TryComp<MindContainerComponent>(((ContainerAttemptEventBase)args).EntityUid, ref mindContainer))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
			else if (Mind.TryGetMind(((ContainerAttemptEventBase)args).EntityUid, out mindId, out mindComp, mindContainer) && (mindComp.PreventSuicide || mindComp.PreventGhosting))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnShutdownContainer(Entity<CryostorageComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		CryostorageComponent comp = ent.Comp;
		CryostorageContainedComponent containedComponent = default(CryostorageContainedComponent);
		foreach (EntityUid stored in comp.StoredPlayers)
		{
			if (((EntitySystem)this).TryComp<CryostorageContainedComponent>(stored, ref containedComponent))
			{
				containedComponent.Cryostorage = null;
				((EntitySystem)this).Dirty(stored, (IComponent)(object)containedComponent, (MetaDataComponent)null);
			}
		}
		comp.StoredPlayers.Clear();
		((EntitySystem)this).Dirty(Entity<CryostorageComponent>.op_Implicit(ent), (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void OnCanDropTarget(Entity<CryostorageComponent> ent, ref CanDropTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession session = default(ICommonSession);
		if (!(args.Dragged == args.User) && _player.TryGetSessionByEntity(args.Dragged, ref session))
		{
			EntityUid? attachedEntity = session.AttachedEntity;
			EntityUid dragged = args.Dragged;
			if (attachedEntity.HasValue && !(attachedEntity.GetValueOrDefault() != dragged))
			{
				args.CanDrop = false;
				args.Handled = true;
			}
		}
	}

	private void OnRemovedContained(Entity<CryostorageContainedComponent> ent, ref EntGotRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Entity<CryostorageContainedComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		CryostorageContainedComponent cryostorageContainedComponent = default(CryostorageContainedComponent);
		val.Deconstruct(ref val2, ref cryostorageContainedComponent);
		EntityUid uid = val2;
		CryostorageContainedComponent comp = cryostorageContainedComponent;
		if (!IsInPausedMap(Entity<TransformComponent>.op_Implicit(uid)))
		{
			((EntitySystem)this).RemCompDeferred(Entity<CryostorageContainedComponent>.op_Implicit(ent), (IComponent)(object)comp);
		}
	}

	private void OnShutdownContained(Entity<CryostorageContainedComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		CryostorageContainedComponent comp = ent.Comp;
		((EntitySystem)this).CompOrNull<CryostorageComponent>(comp.Cryostorage)?.StoredPlayers.Remove(Entity<CryostorageContainedComponent>.op_Implicit(ent));
		ent.Comp.Cryostorage = null;
		((EntitySystem)this).Dirty(Entity<CryostorageContainedComponent>.op_Implicit(ent), (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void OnRoundRestart(RoundRestartCleanupEvent _)
	{
		DeletePausedMap();
	}

	private void DeletePausedMap()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (PausedMap.HasValue && ((EntitySystem)this).Exists(PausedMap))
		{
			((EntitySystem)this).Del((EntityUid?)PausedMap.Value);
			PausedMap = null;
		}
	}

	protected void EnsurePausedMap()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!PausedMap.HasValue || !((EntitySystem)this).Exists(PausedMap))
		{
			PausedMap = _map.CreateMap(true);
			_map.SetPaused(Entity<MapComponent>.op_Implicit(PausedMap.Value), true);
		}
	}

	public bool IsInPausedMap(Entity<TransformComponent?> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Entity<TransformComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		TransformComponent val3 = default(TransformComponent);
		val.Deconstruct(ref val2, ref val3);
		TransformComponent comp = val3;
		if (comp == null)
		{
			comp = ((EntitySystem)this).Transform(Entity<TransformComponent>.op_Implicit(entity));
		}
		if (comp.MapUid.HasValue)
		{
			EntityUid? mapUid = comp.MapUid;
			EntityUid? pausedMap = PausedMap;
			if (mapUid.HasValue != pausedMap.HasValue)
			{
				return false;
			}
			if (!mapUid.HasValue)
			{
				return true;
			}
			return mapUid.GetValueOrDefault() == pausedMap.GetValueOrDefault();
		}
		return false;
	}
}
