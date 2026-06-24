using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Robust.Shared.Player;

public sealed class Filter
{
	private HashSet<ICommonSession> _recipients = new HashSet<ICommonSession>();

	public bool CheckPrediction { get; private set; } = true;

	public bool SendReliable { get; private set; }

	public int Count => _recipients.Count;

	public IEnumerable<ICommonSession> Recipients => _recipients;

	private Filter()
	{
	}

	public Filter AddPlayer(ICommonSession player)
	{
		_recipients.Add(player);
		return this;
	}

	public Filter AddPlayersByPvs(EntityUid origin, float rangeMultiplier = 2f, IEntityManager? entityManager = null, ISharedPlayerManager? playerMan = null, IConfigurationManager? cfgMan = null)
	{
		IoCManager.Resolve(ref entityManager, ref playerMan, ref cfgMan);
		TransformComponent component = entityManager.GetComponent<TransformComponent>(origin);
		SharedTransformSystem sharedTransformSystem = entityManager.System<SharedTransformSystem>();
		return AddPlayersByPvs(sharedTransformSystem.GetMapCoordinates(component), rangeMultiplier, entityManager, playerMan, cfgMan);
	}

	public Filter AddPlayersByPvs(EntityCoordinates origin, float rangeMultiplier = 2f, IEntityManager? entityMan = null, ISharedPlayerManager? playerMan = null)
	{
		IoCManager.Resolve(ref entityMan, ref playerMan);
		SharedTransformSystem sharedTransformSystem = entityMan.System<SharedTransformSystem>();
		return AddPlayersByPvs(sharedTransformSystem.ToMapCoordinates(origin), rangeMultiplier, entityMan, playerMan);
	}

	public Filter AddPlayersByPvs(MapCoordinates origin, float rangeMultiplier = 2f, IEntityManager? entManager = null, ISharedPlayerManager? playerMan = null, IConfigurationManager? cfgMan = null)
	{
		IoCManager.Resolve(ref playerMan, ref cfgMan);
		if (!cfgMan.GetCVar(CVars.NetPVS))
		{
			return AddAllPlayers();
		}
		float range = cfgMan.GetCVar(CVars.NetMaxUpdateRange) * rangeMultiplier;
		return AddInRange(origin, range, playerMan, entManager);
	}

	public Filter AddPlayers(IEnumerable<ICommonSession> players)
	{
		foreach (ICommonSession player in players)
		{
			AddPlayer(player);
		}
		return this;
	}

	public static IEnumerable<ICommonSession> GetAllPlayers(ISharedPlayerManager? playerManager = null)
	{
		IoCManager.Resolve(ref playerManager);
		return playerManager.NetworkedSessions;
	}

	public Filter AddAllPlayers(ISharedPlayerManager? playerMan = null)
	{
		IoCManager.Resolve(ref playerMan);
		_recipients = new HashSet<ICommonSession>(playerMan.NetworkedSessions);
		return this;
	}

	public Filter AddWhere(Predicate<ICommonSession> predicate, ISharedPlayerManager? playerMan = null)
	{
		IoCManager.Resolve(ref playerMan);
		ICommonSession[] networkedSessions = playerMan.NetworkedSessions;
		foreach (ICommonSession commonSession in networkedSessions)
		{
			if (predicate(commonSession))
			{
				AddPlayer(commonSession);
			}
		}
		return this;
	}

	public Filter AddWhereAttachedEntity(Predicate<EntityUid> predicate)
	{
		return AddWhere(delegate(ICommonSession session)
		{
			EntityUid? attachedEntity = session.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
				return predicate(valueOrDefault);
			}
			return false;
		});
	}

	public Filter AddInGrid(EntityUid uid, IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref entMan);
		EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
		return AddWhereAttachedEntity((EntityUid entity) => xformQuery.GetComponent(entity).GridUid == uid);
	}

	public Filter AddInMap(MapId mapId, IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref entMan);
		EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
		return AddWhereAttachedEntity((EntityUid entity) => xformQuery.GetComponent(entity).MapID == mapId);
	}

	public Filter AddInRange(MapCoordinates position, float range, ISharedPlayerManager? playerMan = null, IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref playerMan, ref entMan);
		EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
		SharedTransformSystem xformSystem = entMan.System<SharedTransformSystem>();
		return AddWhere((ICommonSession session) => session.AttachedEntity.HasValue && xformQuery.TryGetComponent(session.AttachedEntity.Value, out TransformComponent component) && component.MapID == position.MapId && (xformSystem.GetWorldPosition(component) - position.Position).Length() < range, playerMan);
	}

	public Filter RemoveByVisibility(uint flag, IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref entMan);
		return RemoveWhere((ICommonSession session) => !session.AttachedEntity.HasValue || !entMan.TryGetComponent<EyeComponent>(session.AttachedEntity, out EyeComponent component) || (component.VisibilityMask & flag) == 0);
	}

	public Filter RemovePlayer(ICommonSession player)
	{
		_recipients.Remove(player);
		return this;
	}

	public Filter RemovePlayers(IEnumerable<ICommonSession> players)
	{
		foreach (ICommonSession player in players)
		{
			_recipients.Remove(player);
		}
		return this;
	}

	public Filter RemovePlayers(params ICommonSession[] players)
	{
		return RemovePlayers((IEnumerable<ICommonSession>)players);
	}

	public Filter RemovePlayerByAttachedEntity(EntityUid uid)
	{
		return RemoveWhereAttachedEntity((EntityUid e) => e == uid);
	}

	public Filter RemovePlayersByAttachedEntity(IEnumerable<EntityUid> uids)
	{
		return RemoveWhereAttachedEntity((EntityUid e) => uids.Contains(e));
	}

	public Filter RemovePlayersByAttachedEntity(params EntityUid[] uids)
	{
		return RemovePlayersByAttachedEntity((IEnumerable<EntityUid>)uids);
	}

	public Filter RemoveWhere(Predicate<ICommonSession> predicate)
	{
		_recipients.RemoveWhere(predicate);
		return this;
	}

	public Filter RemoveWhereAttachedEntity(Predicate<EntityUid> predicate)
	{
		_recipients.RemoveWhere(delegate(ICommonSession session)
		{
			EntityUid? attachedEntity = session.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
				return predicate(valueOrDefault);
			}
			return false;
		});
		return this;
	}

	public Filter RemoveInRange(MapCoordinates position, float range, IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref entMan);
		EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
		SharedTransformSystem xformSystem = entMan.System<SharedTransformSystem>();
		return RemoveWhere((ICommonSession session) => session.AttachedEntity.HasValue && xformQuery.TryGetComponent(session.AttachedEntity.Value, out TransformComponent component) && component.MapID == position.MapId && (xformSystem.GetWorldPosition(component) - position.Position).Length() < range);
	}

	public Filter Merge(Filter other)
	{
		return AddPlayers(other._recipients);
	}

	public Filter FromEntities(params EntityUid[] entities)
	{
		if (!EntitySystem.TryGet<SharedFilterSystem>(out SharedFilterSystem entitySystem))
		{
			return this;
		}
		return entitySystem.FromEntities(this, entities);
	}

	public Filter Clone()
	{
		return new Filter
		{
			_recipients = new HashSet<ICommonSession>(_recipients),
			SendReliable = SendReliable,
			CheckPrediction = CheckPrediction
		};
	}

	public Filter Unpredicted()
	{
		CheckPrediction = false;
		return this;
	}

	public Filter SendReliably()
	{
		SendReliable = true;
		return this;
	}

	public static Filter Empty()
	{
		return new Filter();
	}

	public static Filter SinglePlayer(ICommonSession player)
	{
		return Empty().AddPlayer(player);
	}

	public static Filter Broadcast()
	{
		return Empty().AddAllPlayers();
	}

	public static Filter BroadcastGrid(EntityUid grid)
	{
		return Empty().AddInGrid(grid);
	}

	public static Filter BroadcastMap(MapId map)
	{
		return Empty().AddInMap(map);
	}

	public static Filter Pvs(EntityUid origin, float rangeMultiplier = 2f, IEntityManager? entityManager = null, ISharedPlayerManager? playerManager = null, IConfigurationManager? cfgManager = null)
	{
		return Empty().AddPlayersByPvs(origin, rangeMultiplier, entityManager, playerManager, cfgManager);
	}

	public static Filter Pvs(EntityCoordinates origin, float rangeMultiplier = 2f, IEntityManager? entityMan = null, ISharedPlayerManager? playerMan = null)
	{
		return Empty().AddPlayersByPvs(origin, rangeMultiplier, entityMan, playerMan);
	}

	public static Filter Pvs(MapCoordinates origin, float rangeMultiplier = 2f)
	{
		return Empty().AddPlayersByPvs(origin, rangeMultiplier);
	}

	public static Filter PvsExcept(EntityUid origin, float rangeMultiplier = 2f, IEntityManager? entityManager = null)
	{
		return Pvs(origin, rangeMultiplier, entityManager).RemoveWhereAttachedEntity((EntityUid e) => e == origin);
	}

	public static Filter Entities(params EntityUid[] entities)
	{
		return Empty().FromEntities(entities);
	}

	public static Filter Local()
	{
		return Empty();
	}
}
