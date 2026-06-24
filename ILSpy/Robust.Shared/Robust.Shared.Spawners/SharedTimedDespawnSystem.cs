using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Robust.Shared.Spawners;

public abstract class SharedTimedDespawnSystem : EntitySystem
{
	[Dependency]
	private readonly IGameTiming _timing;

	private readonly HashSet<EntityUid> _queuedDespawnEntities = new HashSet<EntityUid>();

	public override void Initialize()
	{
		base.Initialize();
		base.UpdatesOutsidePrediction = true;
	}

	public override void Update(float frameTime)
	{
		base.Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		_queuedDespawnEntities.Clear();
		EntityQueryEnumerator<TimedDespawnComponent> entityQueryEnumerator = EntityQueryEnumerator<TimedDespawnComponent>();
		EntityUid uid;
		TimedDespawnComponent comp;
		while (entityQueryEnumerator.MoveNext(out uid, out comp))
		{
			comp.Lifetime -= frameTime;
			if (CanDelete(uid) && comp.Lifetime <= 0f)
			{
				_queuedDespawnEntities.Add(uid);
			}
		}
		foreach (EntityUid queuedDespawnEntity in _queuedDespawnEntities)
		{
			TimedDespawnEvent args = default(TimedDespawnEvent);
			RaiseLocalEvent(queuedDespawnEntity, ref args);
			QueueDel(queuedDespawnEntity);
		}
	}

	protected abstract bool CanDelete(EntityUid uid);
}
