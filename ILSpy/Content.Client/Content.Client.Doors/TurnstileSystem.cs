using System;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client.Doors;

public sealed class TurnstileSystem : SharedTurnstileSystem
{
	[Dependency]
	private AnimationPlayerSystem _animationPlayer;

	[Dependency]
	private SpriteSystem _sprite;

	private static readonly EntProtoId ExamineArrow = EntProtoId.op_Implicit("TurnstileArrow");

	private const string AnimationKey = "Turnstile";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TurnstileComponent, AnimationCompletedEvent>((EntityEventRefHandler<TurnstileComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TurnstileComponent, ExaminedEvent>((EntityEventRefHandler<TurnstileComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnAnimationCompleted(Entity<TurnstileComponent> ent, ref AnimationCompletedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!(args.Key != "Turnstile") && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<TurnstileComponent>.op_Implicit(ent), ref item))
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), (Enum)TurnstileVisualLayers.Base, new StateId(ent.Comp.DefaultState));
		}
	}

	private void OnExamined(Entity<TurnstileComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ExamineArrow), new EntityCoordinates(Entity<TurnstileComponent>.op_Implicit(ent), 0f, 0f));
	}

	protected override void PlayAnimation(EntityUid uid, string stateId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		SpriteComponent val2 = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val) && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val2))
		{
			(EntityUid, AnimationPlayerComponent) tuple = (uid, val);
			if (_animationPlayer.HasRunningAnimation(val, "Turnstile"))
			{
				_animationPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit(tuple), "Turnstile");
			}
			State val3 = default(State);
			if (val2.BaseRSI != null && val2.BaseRSI.TryGetState(StateId.op_Implicit(stateId), ref val3))
			{
				float animationLength = val3.AnimationLength;
				Animation val4 = new Animation
				{
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = TurnstileVisualLayers.Base,
						KeyFrames = 
						{
							new KeyFrame(val3.StateId, 0f)
						}
					} },
					Length = TimeSpan.FromSeconds(animationLength)
				};
				_animationPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit(tuple), val4, "Turnstile");
			}
		}
	}
}
