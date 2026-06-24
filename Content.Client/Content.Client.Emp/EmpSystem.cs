using System;
using Content.Shared.Emp;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Client.Emp;

public sealed class EmpSystem : SharedEmpSystem
{
	[Dependency]
	private IRobustRandom _random;

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<EmpDisabledComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<EmpDisabledComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		EmpDisabledComponent empDisabledComponent = default(EmpDisabledComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref empDisabledComponent, ref val3))
		{
			if (Timing.CurTime > empDisabledComponent.TargetTime)
			{
				empDisabledComponent.TargetTime = Timing.CurTime + _random.NextFloat(0.8f, 1.2f) * TimeSpan.FromSeconds(empDisabledComponent.EffectCooldown);
				((EntitySystem)this).Spawn("EffectEmpDisabled", val3.Coordinates);
			}
		}
	}
}
