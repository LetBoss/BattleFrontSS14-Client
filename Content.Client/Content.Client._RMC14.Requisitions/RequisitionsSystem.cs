using System;
using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Requisitions.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Requisitions;

public sealed class RequisitionsSystem : SharedRequisitionsSystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private SpriteSystem _sprite;

	private const string AnimationKey = "cm_requisitions_animation";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RequisitionsElevatorComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RequisitionsElevatorComponent, AfterAutoHandleStateEvent>)OnElevatorHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequisitionsGearComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RequisitionsGearComponent, AfterAutoHandleStateEvent>)OnGearHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequisitionsRailingComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RequisitionsRailingComponent, AfterAutoHandleStateEvent>)OnRailingHandleState, (Type[])null, (Type[])null);
	}

	private void OnElevatorHandleState(Entity<RequisitionsElevatorComponent> elevator, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_014f: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01e5: Expected O, but got Unknown
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<RequisitionsElevatorComponent>.op_Implicit(elevator), ref item) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((elevator.Owner, item)), (Enum)RequisitionsElevatorLayers.Base, ref num, false))
		{
			return;
		}
		if (elevator.Comp.Mode != RequisitionsElevatorMode.Preparing)
		{
			_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(elevator.Owner), "cm_requisitions_animation");
		}
		switch (elevator.Comp.Mode)
		{
		case RequisitionsElevatorMode.Lowered:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((elevator.Owner, item)), num, StateId.op_Implicit(elevator.Comp.LoweredState));
			break;
		case RequisitionsElevatorMode.Raised:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((elevator.Owner, item)), num, StateId.op_Implicit(elevator.Comp.RaisedState));
			break;
		case RequisitionsElevatorMode.Lowering:
		{
			RequisitionsElevatorComponent comp = elevator.Comp;
			if (comp.LoweringAnimation == null)
			{
				comp.LoweringAnimation = (object?)new Animation
				{
					Length = TimeSpan.FromSeconds(2.0999999046325684),
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = RequisitionsElevatorLayers.Base,
						KeyFrames = 
						{
							new KeyFrame(StateId.op_Implicit(elevator.Comp.LoweringState), 0f)
						}
					} }
				};
			}
			_animation.Play(Entity<RequisitionsElevatorComponent>.op_Implicit(elevator), (Animation)elevator.Comp.LoweringAnimation, "cm_requisitions_animation");
			break;
		}
		case RequisitionsElevatorMode.Raising:
		{
			RequisitionsElevatorComponent comp = elevator.Comp;
			if (comp.RaisingAnimation == null)
			{
				comp.RaisingAnimation = (object?)new Animation
				{
					Length = TimeSpan.FromSeconds(2.0999999046325684),
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = RequisitionsElevatorLayers.Base,
						KeyFrames = 
						{
							new KeyFrame(StateId.op_Implicit(elevator.Comp.RaisingState), 0f)
						}
					} }
				};
			}
			_animation.Play(Entity<RequisitionsElevatorComponent>.op_Implicit(elevator), (Animation)elevator.Comp.RaisingAnimation, "cm_requisitions_animation");
			break;
		}
		}
	}

	private void OnGearHandleState(Entity<RequisitionsGearComponent> gear, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<RequisitionsGearComponent>.op_Implicit(gear), ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((gear.Owner, item)), (Enum)RequisitionsGearLayers.Base, ref num, false))
		{
			string text = gear.Comp.Mode switch
			{
				RequisitionsGearMode.Static => gear.Comp.StaticState, 
				RequisitionsGearMode.Moving => gear.Comp.MovingState, 
				_ => gear.Comp.StaticState, 
			};
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((gear.Owner, item)), num, StateId.op_Implicit(text));
		}
	}

	private void OnRailingHandleState(Entity<RequisitionsRailingComponent> railing, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_0141: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01d7: Expected O, but got Unknown
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<RequisitionsRailingComponent>.op_Implicit(railing), ref item) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((railing.Owner, item)), (Enum)RequisitionsRailingLayers.Base, ref num, false))
		{
			return;
		}
		_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(railing.Owner), "cm_requisitions_animation");
		switch (railing.Comp.Mode)
		{
		case RequisitionsRailingMode.Lowered:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((railing.Owner, item)), num, StateId.op_Implicit(railing.Comp.LoweredState));
			break;
		case RequisitionsRailingMode.Raised:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((railing.Owner, item)), num, StateId.op_Implicit(railing.Comp.RaisedState));
			break;
		case RequisitionsRailingMode.Lowering:
		{
			RequisitionsRailingComponent comp = railing.Comp;
			if (comp.LowerAnimation == null)
			{
				comp.LowerAnimation = (object?)new Animation
				{
					Length = TimeSpan.FromSeconds(1.2000000476837158),
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = RequisitionsRailingLayers.Base,
						KeyFrames = 
						{
							new KeyFrame(StateId.op_Implicit(railing.Comp.LoweringState), 0f)
						}
					} }
				};
			}
			_animation.Play(Entity<RequisitionsRailingComponent>.op_Implicit(railing), (Animation)railing.Comp.LowerAnimation, "cm_requisitions_animation");
			break;
		}
		case RequisitionsRailingMode.Raising:
		{
			RequisitionsRailingComponent comp = railing.Comp;
			if (comp.RaiseAnimation == null)
			{
				comp.RaiseAnimation = (object?)new Animation
				{
					Length = TimeSpan.FromSeconds(1.2000000476837158),
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = RequisitionsRailingLayers.Base,
						KeyFrames = 
						{
							new KeyFrame(StateId.op_Implicit(railing.Comp.RaisingState), 0f)
						}
					} }
				};
			}
			_animation.Play(Entity<RequisitionsRailingComponent>.op_Implicit(railing), (Animation)railing.Comp.RaiseAnimation, "cm_requisitions_animation");
			break;
		}
		}
	}

	private void Animate(EntityUid uid, SpriteComponent sprite, Enum layerKey)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!_sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layerKey))
		{
			return;
		}
		ISpriteLayer obj = sprite[(object)layerKey];
		Layer val = (Layer)(object)((obj is Layer) ? obj : null);
		if (val == null)
		{
			return;
		}
		State actualState = val.ActualState;
		int? num = ((actualState != null) ? new int?(actualState.DelayCount) : ((int?)null));
		if (num.HasValue)
		{
			int valueOrDefault = num.GetValueOrDefault();
			if (val.AnimationFrame >= valueOrDefault - 1)
			{
				_sprite.LayerSetAutoAnimated(val, false);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityQueryEnumerator<RequisitionsElevatorComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsElevatorComponent, SpriteComponent>();
		EntityUid uid = default(EntityUid);
		RequisitionsElevatorComponent requisitionsElevatorComponent = default(RequisitionsElevatorComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref uid, ref requisitionsElevatorComponent, ref sprite))
		{
			Animate(uid, sprite, requisitionsElevatorComponent.Mode);
		}
		EntityQueryEnumerator<RequisitionsRailingComponent, SpriteComponent> val2 = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsRailingComponent, SpriteComponent>();
		EntityUid uid2 = default(EntityUid);
		RequisitionsRailingComponent requisitionsRailingComponent = default(RequisitionsRailingComponent);
		SpriteComponent sprite2 = default(SpriteComponent);
		while (val2.MoveNext(ref uid2, ref requisitionsRailingComponent, ref sprite2))
		{
			Animate(uid2, sprite2, requisitionsRailingComponent.Mode);
		}
	}
}
