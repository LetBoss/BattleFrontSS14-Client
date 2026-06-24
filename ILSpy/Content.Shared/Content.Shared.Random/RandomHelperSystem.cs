using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Random;

public sealed class RandomHelperSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IRobustRandom _random;

	public void RandomOffset(EntityUid entity, float minX, float maxX, float minY, float maxY)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		float randomX = _random.NextFloat() * (maxX - minX) + minX;
		float randomY = _random.NextFloat() * (maxY - minY) + minY;
		Vector2 offset = new Vector2(randomX, randomY);
		TransformComponent xform = ((EntitySystem)this).Transform(entity);
		_transform.SetLocalPosition(entity, xform.LocalPosition + offset, xform);
	}

	public void RandomOffset(EntityUid entity, float min, float max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RandomOffset(entity, min, max, min, max);
	}

	public void RandomOffset(EntityUid entity, float value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RandomOffset(entity, 0f - value, value);
	}
}
