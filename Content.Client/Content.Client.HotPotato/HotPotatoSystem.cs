using System;
using Content.Shared.HotPotato;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Client.HotPotato;

public sealed class HotPotatoSystem : SharedHotPotatoSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		AllEntityQueryEnumerator<ActiveHotPotatoComponent> val = ((EntitySystem)this).AllEntityQuery<ActiveHotPotatoComponent>();
		EntityUid val2 = default(EntityUid);
		ActiveHotPotatoComponent activeHotPotatoComponent = default(ActiveHotPotatoComponent);
		while (val.MoveNext(ref val2, ref activeHotPotatoComponent))
		{
			if (!(_timing.CurTime < activeHotPotatoComponent.TargetTime))
			{
				activeHotPotatoComponent.TargetTime = _timing.CurTime + TimeSpan.FromSeconds(activeHotPotatoComponent.EffectCooldown);
				MapCoordinates mapCoordinates = _transform.GetMapCoordinates(val2, (TransformComponent)null);
				((EntitySystem)this).Spawn("HotPotatoEffect", ((MapCoordinates)(ref mapCoordinates)).Offset(_random.NextVector2(0.25f)), (ComponentRegistry)null, default(Angle));
			}
		}
	}
}
