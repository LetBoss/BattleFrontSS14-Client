using System;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleWheelVisualizerSystem : VisualizerSystem<VehicleWheelSlotsComponent>
{
	[Dependency]
	private readonly SpriteSystem _sprite;

	[Dependency]
	private readonly IGameTiming _timing;

	public override void Initialize()
	{
		base.Initialize();
	}

	protected override void OnAppearanceChange(EntityUid uid, VehicleWheelSlotsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateWheelVisuals(uid);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<VehicleWheelSlotsComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<VehicleWheelSlotsComponent, SpriteComponent>();
		EntityUid uid = default(EntityUid);
		VehicleWheelSlotsComponent vehicleWheelSlotsComponent = default(VehicleWheelSlotsComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref uid, ref vehicleWheelSlotsComponent, ref sprite))
		{
			UpdateWheelVisuals(uid, sprite);
		}
	}

	private void UpdateWheelVisuals(EntityUid uid, SpriteComponent? sprite = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if ((sprite == null && !((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite)) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "rmc-wheels", ref num, false))
		{
			return;
		}
		int num2 = 0;
		int num3 = default(int);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)VehicleWheelVisuals.WheelCount, ref num3, (AppearanceComponent)null))
		{
			num2 = num3;
		}
		bool flag = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)VehicleWheelVisuals.HasAllWheels, ref flag, (AppearanceComponent)null) && num2 == 0)
		{
			num2 = (flag ? 1 : 0);
		}
		if (num2 <= 0)
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
			return;
		}
		bool flag2 = false;
		GridVehicleMoverComponent gridVehicleMoverComponent = default(GridVehicleMoverComponent);
		if (((EntitySystem)this).TryComp<GridVehicleMoverComponent>(uid, ref gridVehicleMoverComponent))
		{
			flag2 = Math.Abs(gridVehicleMoverComponent.CurrentSpeed) > 0.01f || (gridVehicleMoverComponent.TurnInPlace && gridVehicleMoverComponent.InPlaceTurnBlockUntil > _timing.CurTime);
		}
		bool flag3 = false;
		float num4 = 1f;
		int num5 = num2;
		float num6 = 1f;
		int num7 = default(int);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)VehicleWheelVisuals.WheelFunctionalCount, ref num7, (AppearanceComponent)null))
		{
			num5 = num7;
		}
		float num8 = default(float);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(uid, (Enum)VehicleWheelVisuals.WheelIntegrityFraction, ref num8, (AppearanceComponent)null))
		{
			num6 = num8;
		}
		float num9 = ((num2 > 0) ? ((float)num5 / (float)num2) : 1f);
		if (num9 < 0f)
		{
			num9 = 0f;
		}
		else if (num9 > 1f)
		{
			num9 = 1f;
		}
		float x = 0.3f + 0.7f * num6;
		float y = 0.3f + 0.7f * num9;
		num4 = MathF.Min(x, y);
		flag3 = num2 > 0 && num5 <= 0;
		if (flag3)
		{
			num4 = 1f;
		}
		string text = (flag3 ? "wheels_1" : "wheels_0");
		if (sprite.LayerGetState(num) != StateId.op_Implicit(text))
		{
			sprite.LayerSetState(num, StateId.op_Implicit(text));
			if (flag2 && !flag3)
			{
				_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, 0f);
			}
		}
		_sprite.LayerSetAutoAnimated(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, flag2 && !flag3);
		_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, new Color(num4, num4, num4, sprite.Color.A));
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
	}
}
