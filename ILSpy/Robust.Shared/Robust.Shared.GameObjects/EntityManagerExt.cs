using Robust.Shared.Collections;
using Robust.Shared.Random;

namespace Robust.Shared.GameObjects;

public static class EntityManagerExt
{
	public static T? GetComponentOrNull<T>(this IEntityManager entityManager, EntityUid entityUid) where T : IComponent
	{
		if (entityManager.TryGetComponent<T>(entityUid, out T component))
		{
			return component;
		}
		return default(T);
	}

	public static T? GetComponentOrNull<T>(this IEntityManager entityManager, EntityUid? entityUid) where T : IComponent
	{
		if (entityUid.HasValue && entityManager.TryGetComponent<T>(entityUid.Value, out T component))
		{
			return component;
		}
		return default(T);
	}

	public static bool TryGetRandom<TComp1>(this IEntityManager entManager, IRobustRandom random, out EntityUid entity, bool includePaused = false) where TComp1 : IComponent
	{
		ValueList<EntityUid> list = default(ValueList<EntityUid>);
		TComp1 comp;
		if (includePaused)
		{
			AllEntityQueryEnumerator<TComp1> allEntityQueryEnumerator = entManager.AllEntityQueryEnumerator<TComp1>();
			EntityUid uid;
			while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
			{
				list.Add(uid);
			}
		}
		else
		{
			EntityQueryEnumerator<TComp1> entityQueryEnumerator = entManager.EntityQueryEnumerator<TComp1>();
			EntityUid uid2;
			while (entityQueryEnumerator.MoveNext(out uid2, out comp))
			{
				list.Add(uid2);
			}
		}
		if (list.Count == 0)
		{
			entity = EntityUid.Invalid;
			return false;
		}
		entity = random.Pick(list);
		return true;
	}

	public static bool TryGetRandom<TComp1, TComp2>(this IEntityManager entManager, IRobustRandom random, out EntityUid entity, bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent
	{
		ValueList<EntityUid> list = default(ValueList<EntityUid>);
		TComp1 comp;
		TComp2 comp2;
		if (includePaused)
		{
			AllEntityQueryEnumerator<TComp1, TComp2> allEntityQueryEnumerator = entManager.AllEntityQueryEnumerator<TComp1, TComp2>();
			EntityUid uid;
			while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2))
			{
				list.Add(uid);
			}
		}
		else
		{
			EntityQueryEnumerator<TComp1, TComp2> entityQueryEnumerator = entManager.EntityQueryEnumerator<TComp1, TComp2>();
			EntityUid uid2;
			while (entityQueryEnumerator.MoveNext(out uid2, out comp, out comp2))
			{
				list.Add(uid2);
			}
		}
		if (list.Count == 0)
		{
			entity = EntityUid.Invalid;
			return false;
		}
		entity = random.Pick(list);
		return true;
	}
}
