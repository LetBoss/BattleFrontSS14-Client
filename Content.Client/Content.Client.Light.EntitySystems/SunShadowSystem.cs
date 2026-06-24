using System;
using System.Numerics;
using Content.Client.GameTicking.Managers;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Light.EntitySystems;

public sealed class SunShadowSystem : SharedSunShadowSystem
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		AllEntityQueryEnumerator<SunShadowCycleComponent, SunShadowComponent> val = ((EntitySystem)this).AllEntityQuery<SunShadowCycleComponent, SunShadowComponent>();
		EntityUid val2 = default(EntityUid);
		SunShadowCycleComponent sunShadowCycleComponent = default(SunShadowCycleComponent);
		SunShadowComponent sunShadowComponent = default(SunShadowComponent);
		while (val.MoveNext(ref val2, ref sunShadowCycleComponent, ref sunShadowComponent))
		{
			if (((Component)sunShadowCycleComponent).Running && sunShadowCycleComponent.Directions.Count != 0)
			{
				TimeSpan pauseTime = _metadata.GetPauseTime(val2, (MetaDataComponent)null);
				float time = (float)(_timing.CurTime.Add(sunShadowCycleComponent.Offset).Subtract(_ticker.RoundStartTimeSpan).Subtract(pauseTime)
					.TotalSeconds % sunShadowCycleComponent.Duration.TotalSeconds);
				(Vector2 Direction, float Alpha) shadow = GetShadow(Entity<SunShadowCycleComponent>.op_Implicit((val2, sunShadowCycleComponent)), time);
				Vector2 item = shadow.Direction;
				float item2 = shadow.Alpha;
				sunShadowComponent.Direction = item;
				sunShadowComponent.Alpha = item2;
			}
		}
	}

	public (Vector2 Direction, float Alpha) GetShadow(Entity<SunShadowCycleComponent> entity, float time)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)((double)time / entity.Comp.Duration.TotalSeconds);
		for (int num2 = entity.Comp.Directions.Count - 1; num2 >= 0; num2--)
		{
			SunShadowCycleDirection sunShadowCycleDirection = entity.Comp.Directions[num2];
			if (num > sunShadowCycleDirection.Ratio)
			{
				SunShadowCycleDirection sunShadowCycleDirection2 = entity.Comp.Directions[(num2 + 1) % entity.Comp.Directions.Count];
				float num3 = ((num2 != entity.Comp.Directions.Count - 1) ? sunShadowCycleDirection2.Ratio : (sunShadowCycleDirection2.Ratio + 1f));
				float num4 = num3 - sunShadowCycleDirection.Ratio;
				float num5 = (num - sunShadowCycleDirection.Ratio) / num4;
				Angle val = DirectionExtensions.ToAngle(sunShadowCycleDirection.Direction);
				Angle val2 = DirectionExtensions.ToAngle(sunShadowCycleDirection2.Direction);
				Angle val3 = Angle.Lerp(ref val, ref val2, num5);
				float amount = MathF.Pow(num5, 0.5f);
				float num6 = float.Lerp(sunShadowCycleDirection.Direction.Length(), sunShadowCycleDirection2.Direction.Length(), amount);
				Vector2 item = ((Angle)(ref val3)).ToVec() * num6;
				float item2 = float.Lerp(sunShadowCycleDirection.Alpha, sunShadowCycleDirection2.Alpha, num5);
				return (Direction: item, Alpha: item2);
			}
		}
		throw new InvalidOperationException();
	}
}
