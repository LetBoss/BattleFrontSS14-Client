using System.Numerics;
using Content.Shared._CIV14merka.Aircraft;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Aircraft;

public sealed class AircraftVisualizerSystem : EntitySystem
{
	[Dependency]
	private readonly ISharedPlayerManager _player;

	public override void FrameUpdate(float frameTime)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		EntityUid? localEntity = _player.LocalEntity;
		EntityUid val2 = default(EntityUid);
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			EntityQueryEnumerator<AircraftComponent, VehicleComponent> val = ((EntitySystem)this).EntityQueryEnumerator<AircraftComponent, VehicleComponent>();
			AircraftComponent aircraftComponent = default(AircraftComponent);
			VehicleComponent vehicleComponent = default(VehicleComponent);
			while (val.MoveNext(ref val2, ref aircraftComponent, ref vehicleComponent))
			{
				if (aircraftComponent.Altitude > 0)
				{
					localEntity = vehicleComponent.Operator;
					val2 = valueOrDefault;
					if (localEntity.HasValue && localEntity.GetValueOrDefault() == val2)
					{
						flag = true;
						break;
					}
				}
			}
		}
		EntityQueryEnumerator<AircraftComponent, SpriteComponent> val3 = ((EntitySystem)this).EntityQueryEnumerator<AircraftComponent, SpriteComponent>();
		AircraftComponent aircraftComponent2 = default(AircraftComponent);
		SpriteComponent val4 = default(SpriteComponent);
		while (val3.MoveNext(ref val2, ref aircraftComponent2, ref val4))
		{
			if (aircraftComponent2.Altitude <= 0)
			{
				if (val4.Color != Color.White)
				{
					val4.Color = Color.White;
				}
				if (val4.Scale != Vector2.One)
				{
					val4.Scale = Vector2.One;
				}
				if (val4.DrawDepth != 6)
				{
					val4.DrawDepth = 6;
				}
				continue;
			}
			float num = aircraftComponent2.AirborneScale + (float)(aircraftComponent2.Altitude - 1) * aircraftComponent2.ScalePerLevel;
			Vector2 vector = new Vector2(num, num);
			if (val4.Scale != vector)
			{
				val4.Scale = vector;
			}
			if (val4.DrawDepth != 13)
			{
				val4.DrawDepth = 13;
			}
			Color val5;
			if (!flag)
			{
				Color black = Color.Black;
				val5 = ((Color)(ref black)).WithAlpha(aircraftComponent2.ShadowAlpha);
			}
			else
			{
				val5 = Color.White;
			}
			Color val6 = val5;
			if (val4.Color != val6)
			{
				val4.Color = val6;
			}
		}
		EntityQueryEnumerator<HighAltitudeProjectileComponent, SpriteComponent> val7 = ((EntitySystem)this).EntityQueryEnumerator<HighAltitudeProjectileComponent, SpriteComponent>();
		HighAltitudeProjectileComponent highAltitudeProjectileComponent = default(HighAltitudeProjectileComponent);
		SpriteComponent val8 = default(SpriteComponent);
		while (val7.MoveNext(ref val2, ref highAltitudeProjectileComponent, ref val8))
		{
			if (val8.Visible != flag)
			{
				val8.Visible = flag;
			}
		}
	}
}
