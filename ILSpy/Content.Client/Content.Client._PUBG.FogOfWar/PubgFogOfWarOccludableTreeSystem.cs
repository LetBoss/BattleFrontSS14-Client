using System.Numerics;
using Robust.Client.GameObjects;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;

namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarOccludableTreeSystem : ComponentTreeSystem<PubgFogOfWarOccludableTreeComponent, PubgFogOfWarOccludableComponent>
{
	[Dependency]
	private SpriteSystem _sprite;

	protected override bool DoFrameUpdate => true;

	protected override bool DoTickUpdate => false;

	protected override bool Recursive => false;

	protected override Box2 ExtractAabb(in ComponentTreeEntry<PubgFogOfWarOccludableComponent> entry, Vector2 pos, Angle rot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(entry.Uid, ref item))
		{
			return default(Box2);
		}
		Box2Rotated val = _sprite.CalculateBounds(Entity<SpriteComponent>.op_Implicit((entry.Uid, item)), pos, rot, default(Angle));
		return ((Box2Rotated)(ref val)).CalcBoundingBox();
	}
}
