using System.Numerics;
using Content.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Maths;
using Robust.Shared.Physics;

namespace Content.Client.Audio;

public sealed class AmbientSoundTreeSystem : ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent>
{
	protected override bool DoFrameUpdate => false;

	protected override bool DoTickUpdate => true;

	protected override int InitialCapacity => 256;

	protected override bool Recursive => true;

	protected override Box2 ExtractAabb(in ComponentTreeEntry<AmbientSoundComponent> entry, Vector2 pos, Angle rot)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Box2(pos - entry.Component.RangeVector, pos + entry.Component.RangeVector);
	}

	protected override Box2 ExtractAabb(in ComponentTreeEntry<AmbientSoundComponent> entry)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!entry.Component.TreeUid.HasValue)
		{
			return default(Box2);
		}
		Vector2 relativePosition = base.XformSystem.GetRelativePosition(entry.Transform, entry.Component.TreeUid.Value);
		return ((ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent>)(object)this).ExtractAabb(ref entry, relativePosition, default(Angle));
	}
}
