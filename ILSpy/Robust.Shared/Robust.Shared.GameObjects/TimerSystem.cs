using Robust.Shared.Collections;

namespace Robust.Shared.GameObjects;

public sealed class TimerSystem : EntitySystem
{
	public override void Update(float frameTime)
	{
		base.Update(frameTime);
		EntityQueryEnumerator<TimerComponent> entityQueryEnumerator = EntityQueryEnumerator<TimerComponent>();
		ValueList<(EntityUid, TimerComponent)> valueList = default(ValueList<(EntityUid, TimerComponent)>);
		EntityUid uid;
		TimerComponent comp;
		while (entityQueryEnumerator.MoveNext(out uid, out comp))
		{
			valueList.Add((uid, comp));
		}
		foreach (var item in valueList)
		{
			item.Item2.Update(frameTime);
		}
		foreach (var (uid2, timerComponent) in valueList)
		{
			if (!timerComponent.Deleted && !EntityManager.Deleted(uid2) && timerComponent.RemoveOnEmpty && timerComponent.TimerCount == 0)
			{
				RemComp<TimerComponent>(uid2);
			}
		}
	}
}
