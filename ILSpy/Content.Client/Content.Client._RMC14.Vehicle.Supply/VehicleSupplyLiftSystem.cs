using System;
using Content.Shared._RMC14.Vehicle.Supply;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Vehicle.Supply;

public sealed class VehicleSupplyLiftSystem : EntitySystem
{
	[Dependency]
	private readonly AnimationPlayerSystem _animation;

	private const string AnimationKey = "rmc_vehicle_supply_lift";

	private const string BaseLayerKey = "rmc-vehicle-supply-lift-base";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VehicleSupplyLiftComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<VehicleSupplyLiftComponent, AfterAutoHandleStateEvent>)OnLiftHandleState, (Type[])null, (Type[])null);
	}

	private void OnLiftHandleState(Entity<VehicleSupplyLiftComponent> lift, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_010b: Expected O, but got Unknown
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_01a0: Expected O, but got Unknown
		SpriteComponent val = default(SpriteComponent);
		int num = default(int);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<VehicleSupplyLiftComponent>.op_Implicit(lift), ref val) || !val.LayerMapTryGet((object)"rmc-vehicle-supply-lift-base", ref num, false))
		{
			return;
		}
		if (lift.Comp.Mode != VehicleSupplyLiftMode.Preparing)
		{
			_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(lift.Owner), "rmc_vehicle_supply_lift");
		}
		switch (lift.Comp.Mode)
		{
		case VehicleSupplyLiftMode.Lowered:
			val.LayerSetState(num, StateId.op_Implicit(lift.Comp.LoweredState));
			break;
		case VehicleSupplyLiftMode.Raised:
			val.LayerSetState(num, StateId.op_Implicit(lift.Comp.RaisedState));
			break;
		case VehicleSupplyLiftMode.Lowering:
		{
			VehicleSupplyLiftComponent comp = lift.Comp;
			if (comp.LoweringAnimation == null)
			{
				comp.LoweringAnimation = (object?)new Animation
				{
					Length = TimeSpan.FromSeconds(2.0999999046325684),
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = "rmc-vehicle-supply-lift-base",
						KeyFrames = 
						{
							new KeyFrame(StateId.op_Implicit(lift.Comp.LoweringState), 0f)
						}
					} }
				};
			}
			_animation.Play(Entity<VehicleSupplyLiftComponent>.op_Implicit(lift), (Animation)lift.Comp.LoweringAnimation, "rmc_vehicle_supply_lift");
			break;
		}
		case VehicleSupplyLiftMode.Raising:
		{
			VehicleSupplyLiftComponent comp = lift.Comp;
			if (comp.RaisingAnimation == null)
			{
				comp.RaisingAnimation = (object?)new Animation
				{
					Length = TimeSpan.FromSeconds(2.0999999046325684),
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = "rmc-vehicle-supply-lift-base",
						KeyFrames = 
						{
							new KeyFrame(StateId.op_Implicit(lift.Comp.RaisingState), 0f)
						}
					} }
				};
			}
			_animation.Play(Entity<VehicleSupplyLiftComponent>.op_Implicit(lift), (Animation)lift.Comp.RaisingAnimation, "rmc_vehicle_supply_lift");
			break;
		}
		}
	}
}
