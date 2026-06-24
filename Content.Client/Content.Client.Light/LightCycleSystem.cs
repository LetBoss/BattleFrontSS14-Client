using System;
using Content.Client.GameTicking.Managers;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Light;

public sealed class LightCycleSystem : SharedLightCycleSystem
{
	[Dependency]
	private ClientGameTicker _ticker;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MetaDataSystem _metadata;

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		AllEntityQueryEnumerator<LightCycleComponent, MapLightComponent> val = ((EntitySystem)this).AllEntityQuery<LightCycleComponent, MapLightComponent>();
		EntityUid val2 = default(EntityUid);
		LightCycleComponent lightCycleComponent = default(LightCycleComponent);
		MapLightComponent val3 = default(MapLightComponent);
		while (val.MoveNext(ref val2, ref lightCycleComponent, ref val3))
		{
			if (((Component)lightCycleComponent).Running)
			{
				TimeSpan pauseTime = _metadata.GetPauseTime(val2, (MetaDataComponent)null);
				float time = (float)_timing.CurTime.Add(lightCycleComponent.Offset).Subtract(_ticker.RoundStartTimeSpan).Subtract(pauseTime)
					.TotalSeconds;
				Color color = SharedLightCycleSystem.GetColor(Entity<LightCycleComponent>.op_Implicit((val2, lightCycleComponent)), lightCycleComponent.OriginalColor, time);
				val3.AmbientLightColor = color;
			}
		}
	}
}
