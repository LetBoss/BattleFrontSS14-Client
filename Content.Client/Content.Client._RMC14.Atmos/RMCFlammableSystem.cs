using System;
using Content.Shared._RMC14.Atmos;
using Content.Shared.Mobs;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Atmos;

public sealed class RMCFlammableSystem : SharedRMCFlammableSystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	private const string RollKey = "StopDropRollAnimation";

	private static readonly ProtoId<StatusEffectPrototype> KnockdownedKey = ProtoId<StatusEffectPrototype>.op_Implicit("KnockedDown");

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeAllEvent<RMCStopDropRollVisualsNetworkEvent>((EntityEventHandler<RMCStopDropRollVisualsNetworkEvent>)OnResist, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStopDropRollVisualsComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCStopDropRollVisualsComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStopDropRollVisualsComponent, StatusEffectEndedEvent>((EntityEventRefHandler<RMCStopDropRollVisualsComponent, StatusEffectEndedEvent>)OnStatusEffectEnded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStopDropRollVisualsComponent, StoodEvent>((EntityEventRefHandler<RMCStopDropRollVisualsComponent, StoodEvent>)OnStood, (Type[])null, (Type[])null);
	}

	private void OnResist(RMCStopDropRollVisualsNetworkEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Expected O, but got Unknown
		//IL_043b: Expected O, but got Unknown
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(ev.User, ref val) && ((EntitySystem)this).HasComp<RMCStopDropRollVisualsComponent>(val) && !_animation.HasRunningAnimation(val.Value, "StopDropRollAnimation"))
		{
			Animation val2 = new Animation
			{
				Length = TimeSpan.FromSeconds(5L),
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					ComponentType = typeof(TransformComponent),
					Property = "LocalRotation",
					InterpolationMode = (AnimationInterpolationMode)0,
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(90.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(180.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(270.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(90.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(180.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(270.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(90.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(180.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(270.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(90.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(180.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(270.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(90.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(180.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(270.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(90.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(180.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.FromDegrees(270.0), 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Angle.Zero, 0.25f, (Func<float, float>)null)
					}
				} }
			};
			_animation.Play(val.Value, val2, "StopDropRollAnimation");
		}
	}

	private void OnMobStateChanged(Entity<RMCStopDropRollVisualsComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Alive)
		{
			_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "StopDropRollAnimation");
		}
	}

	private void OnStatusEffectEnded(Entity<RMCStopDropRollVisualsComponent> ent, ref StatusEffectEndedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != KnockdownedKey))
		{
			_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "StopDropRollAnimation");
		}
	}

	private void OnStood(Entity<RMCStopDropRollVisualsComponent> ent, ref StoodEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "StopDropRollAnimation");
	}
}
