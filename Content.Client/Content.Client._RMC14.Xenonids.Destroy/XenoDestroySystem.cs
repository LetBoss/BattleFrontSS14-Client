using System;
using System.Numerics;
using Content.Shared._RMC14.Xenonids.Destroy;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Destroy;

public sealed class XenoDestroySystem : SharedXenoDestroySystem
{
	[Dependency]
	private AnimationPlayerSystem _animPlayer;

	[Dependency]
	private SpriteSystem _sprite;

	private const float JumpHeight = 10f;

	private const string LeapingAnimationKey = "king-leap-animation";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeAllEvent<XenoDestroyLeapStartEvent>((EntityEventHandler<XenoDestroyLeapStartEvent>)OnXenoLeapStart, (Type[])null, (Type[])null);
	}

	public Animation LeapAnimation(XenoDestroyComponent destroy, Vector2 leapOffset)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Expected O, but got Unknown
		Vector2 vector = leapOffset / 2f;
		Vector2 vector2 = -vector;
		vector += new Vector2(0f, 10f);
		vector2 += new Vector2(0f, 10f);
		float num = (float)(destroy.CrashTime.TotalSeconds / 2.0);
		return new Animation
		{
			Length = destroy.CrashTime,
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)vector, num, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)vector2, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, num, (Func<float, float>)null)
				}
			} }
		};
	}

	private void OnXenoLeapStart(XenoDestroyLeapStartEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		XenoDestroyComponent destroy = default(XenoDestroyComponent);
		SpriteComponent val2 = default(SpriteComponent);
		AnimationPlayerComponent val3 = default(AnimationPlayerComponent);
		if (((EntitySystem)this).TryGetEntity(ev.King, ref val) && ((EntitySystem)this).TryComp<XenoDestroyComponent>(val, ref destroy) && ((EntitySystem)this).TryComp<SpriteComponent>(val, ref val2) && !((EntitySystem)this).TerminatingOrDeleted(val, (MetaDataComponent)null) && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(val, ref val3) && !_animPlayer.HasRunningAnimation(val3, "king-leap-animation"))
		{
			_animPlayer.Play(val.Value, LeapAnimation(destroy, ev.LeapOffset), "king-leap-animation");
		}
	}

	protected override void OnLeapingRemove(Entity<XenoDestroyLeapingComponent> xeno, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		base.OnLeapingRemove(xeno, ref args);
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), ref item) && !((EntitySystem)this).TerminatingOrDeleted(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), (MetaDataComponent)null))
		{
			AnimationPlayerComponent item2 = default(AnimationPlayerComponent);
			if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), ref item2))
			{
				_animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), item2)), "king-leap-animation");
			}
			_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), Vector2.Zero);
		}
	}
}
