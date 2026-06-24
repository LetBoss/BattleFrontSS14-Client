using System;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

public abstract class MetaDataSystem : EntitySystem
{
	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly IPrototypeManager _proto;

	private EntityPausedEvent _pausedEvent;

	private EntityQuery<MetaDataComponent> _metaQuery;

	public override void Initialize()
	{
		_metaQuery = GetEntityQuery<MetaDataComponent>();
		SubscribeLocalEvent<MetaDataComponent, ComponentHandleState>(OnMetaDataHandle);
		SubscribeLocalEvent<MetaDataComponent, ComponentGetState>(OnMetaDataGetState);
	}

	private void OnMetaDataGetState(EntityUid uid, MetaDataComponent component, ref ComponentGetState args)
	{
		args.State = new MetaDataComponentState(component._entityName, component._entityDescription, component._entityPrototype?.ID, component.PauseTime);
	}

	private void OnMetaDataHandle(EntityUid uid, MetaDataComponent component, ref ComponentHandleState args)
	{
		if (args.Current is MetaDataComponentState metaDataComponentState)
		{
			component._entityName = metaDataComponentState.Name;
			component._entityDescription = metaDataComponentState.Description;
			if (metaDataComponentState.PrototypeId != null && metaDataComponentState.PrototypeId != component._entityPrototype?.ID)
			{
				component._entityPrototype = _proto.Index<EntityPrototype>(metaDataComponentState.PrototypeId);
			}
			component.PauseTime = metaDataComponentState.PauseTime;
		}
	}

	public void SetEntityName(EntityUid uid, string value, MetaDataComponent? metadata = null, bool raiseEvents = true)
	{
		if (_metaQuery.Resolve(uid, ref metadata) && !value.Equals(metadata.EntityName))
		{
			string entityName = metadata.EntityName;
			metadata._entityName = value;
			if (raiseEvents)
			{
				EntityRenamedEvent args = new EntityRenamedEvent(uid, entityName, value);
				RaiseLocalEvent(uid, ref args, broadcast: true);
			}
			Dirty(uid, metadata, metadata);
		}
	}

	public void SetEntityDescription(EntityUid uid, string value, MetaDataComponent? metadata = null)
	{
		if (_metaQuery.Resolve(uid, ref metadata) && !value.Equals(metadata.EntityDescription))
		{
			metadata._entityDescription = value;
			Dirty(uid, metadata, metadata);
		}
	}

	internal void SetEntityPrototype(EntityUid uid, EntityPrototype? value, MetaDataComponent? metadata = null)
	{
		if (_metaQuery.Resolve(uid, ref metadata) && (value == null || !value.Equals(metadata._entityPrototype)))
		{
			metadata._entityPrototype = value;
		}
	}

	public bool EntityPaused(EntityUid uid, MetaDataComponent? metadata = null)
	{
		if (!_metaQuery.Resolve(uid, ref metadata))
		{
			return true;
		}
		return metadata.EntityPaused;
	}

	public void SetEntityPaused(EntityUid uid, bool value, MetaDataComponent? metadata = null)
	{
		if (_metaQuery.Resolve(uid, ref metadata) && metadata.EntityPaused != value)
		{
			if (value)
			{
				metadata.PauseTime = _timing.CurTime;
				RaiseLocalEvent(uid, ref _pausedEvent);
			}
			else
			{
				EntityUnpausedEvent args = new EntityUnpausedEvent(_timing.CurTime - metadata.PauseTime.Value);
				metadata.PauseTime = null;
				RaiseLocalEvent(uid, ref args);
			}
			Dirty(uid, metadata, metadata);
		}
	}

	public TimeSpan GetPauseTime(EntityUid uid, MetaDataComponent? metadata = null)
	{
		if (!_metaQuery.Resolve(uid, ref metadata))
		{
			return TimeSpan.Zero;
		}
		TimeSpan curTime = _timing.CurTime;
		TimeSpan? pauseTime = metadata.PauseTime;
		return (curTime - pauseTime) ?? TimeSpan.Zero;
	}

	public void PauseOffset(EntityUid uid, ref TimeSpan time, MetaDataComponent? metadata = null)
	{
		TimeSpan pauseTime = GetPauseTime(uid, metadata);
		time += pauseTime;
	}

	public void SetFlag(Entity<MetaDataComponent?> entity, MetaDataFlags flags, bool enabled)
	{
		if (_metaQuery.Resolve(entity, ref entity.Comp))
		{
			if (enabled)
			{
				entity.Comp.Flags |= flags;
			}
			else
			{
				RemoveFlag(entity, flags, entity.Comp);
			}
		}
	}

	public void AddFlag(EntityUid uid, MetaDataFlags flags, MetaDataComponent? comp = null)
	{
		SetFlag((Owner: uid, Comp: comp), flags, enabled: true);
	}

	public void RemoveFlag(EntityUid uid, MetaDataFlags flags, MetaDataComponent? component = null)
	{
		if (_metaQuery.Resolve(uid, ref component))
		{
			MetaDataFlags metaDataFlags = component.Flags & flags;
			if (metaDataFlags != MetaDataFlags.None)
			{
				MetaFlagRemoveAttemptEvent args = new MetaFlagRemoveAttemptEvent(metaDataFlags);
				RaiseLocalEvent(uid, ref args, broadcast: true);
				component.Flags &= (MetaDataFlags)(byte)(~(int)args.ToRemove);
			}
		}
	}
}
