using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Map;

public struct RMCAnchoredEntitiesEnumerator(SharedTransformSystem transform, AnchoredEntitiesEnumerator enumerator, DirectionFlag facing = (DirectionFlag)0) : IDisposable
{
	private AnchoredEntitiesEnumerator _enumerator = enumerator;

	public static readonly RMCAnchoredEntitiesEnumerator Empty = new RMCAnchoredEntitiesEnumerator(null, AnchoredEntitiesEnumerator.Empty, (DirectionFlag)0);

	public bool MoveNext(out EntityUid uid)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? uidNullable = default(EntityUid?);
		while (((AnchoredEntitiesEnumerator)(ref _enumerator)).MoveNext(ref uidNullable))
		{
			if ((int)facing == 0)
			{
				uid = uidNullable.Value;
				return true;
			}
			Angle worldRotation = transform.GetWorldRotation(uidNullable.Value);
			if ((DirectionExtensions.AsFlag(((Angle)(ref worldRotation)).GetDir()) & facing) != 0)
			{
				uid = uidNullable.Value;
				return true;
			}
		}
		uid = default(EntityUid);
		return false;
	}

	public void Dispose()
	{
		((AnchoredEntitiesEnumerator)(ref _enumerator)).Dispose();
	}
}
public struct RMCAnchoredEntitiesEnumerator<T>(IEntityManager entity, SharedTransformSystem transform, AnchoredEntitiesEnumerator enumerator, DirectionFlag facing = (DirectionFlag)0) : IDisposable where T : IComponent
{
	private AnchoredEntitiesEnumerator _enumerator = enumerator;

	public static readonly RMCAnchoredEntitiesEnumerator<T> Empty = new RMCAnchoredEntitiesEnumerator<T>(null, null, AnchoredEntitiesEnumerator.Empty, (DirectionFlag)0);

	public bool MoveNext(out EntityUid uid)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? uidNullable = default(EntityUid?);
		while (((AnchoredEntitiesEnumerator)(ref _enumerator)).MoveNext(ref uidNullable))
		{
			if (entity.HasComponent<T>(uidNullable))
			{
				if ((int)facing == 0)
				{
					uid = uidNullable.Value;
					return true;
				}
				Angle worldRotation = transform.GetWorldRotation(uidNullable.Value);
				if ((DirectionExtensions.AsFlag(((Angle)(ref worldRotation)).GetDir()) & facing) != 0)
				{
					uid = uidNullable.Value;
					return true;
				}
			}
		}
		uid = default(EntityUid);
		return false;
	}

	public void Dispose()
	{
		((AnchoredEntitiesEnumerator)(ref _enumerator)).Dispose();
	}
}
