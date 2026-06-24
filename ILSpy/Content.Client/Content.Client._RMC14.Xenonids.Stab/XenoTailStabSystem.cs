using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Interactable;
using Content.Shared._RMC14.Xenonids.Stab;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Xenonids.Stab;

public sealed class XenoTailStabSystem : SharedXenoTailStabSystem
{
	private sealed class TailStabOverlay : Overlay
	{
		public Box2Rotated? Last;

		public override OverlaySpace Space => (OverlaySpace)4;

		protected override void Draw(in OverlayDrawArgs args)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (Last.HasValue)
			{
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Box2Rotated value = Last.Value;
				worldHandle.DrawRect(ref value, Color.Red, true);
			}
		}
	}

	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private InteractionSystem _interaction;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private TransformSystem _transform;

	private const string TailAnimationKey = "cm-xeno-tail";

	private const string TailFadeAnimationKey = "cm-xeno-tail-fade";

	private bool _showTailAttack;

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
	}

	protected override void DoLunge(Entity<XenoTailStabComponent, TransformComponent> user, Vector2 localPos, EntProtoId animationId)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0170: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Expected O, but got Unknown
		//IL_0215: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			EntityUid val = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(animationId), user.Comp2.Coordinates);
			((SharedTransformSystem)_transform).SetParent(val, Entity<XenoTailStabComponent, TransformComponent>.op_Implicit(user));
			SpriteComponent val2 = ((EntitySystem)this).EnsureComp<SpriteComponent>(val);
			val2.NoRotation = true;
			_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((val, val2)), DirectionExtensions.ToWorldAngle(localPos));
			float num = localPos.Length() * 0.8f;
			MapCoordinates mapCoordinates = ((SharedTransformSystem)_transform).GetMapCoordinates(Entity<XenoTailStabComponent, TransformComponent>.op_Implicit(user));
			float num2 = _interaction.UnobstructedDistance(mapCoordinates, ((MapCoordinates)(ref mapCoordinates)).Offset(localPos));
			if (num > num2)
			{
				num = num2;
			}
			Angle rotation = val2.Rotation;
			Vector2 vector = new Vector2(0f, (0f - num) / 5f);
			Vector2 vector2 = ((Angle)(ref rotation)).RotateVec(ref vector);
			rotation = val2.Rotation;
			vector = new Vector2(0f, 0f - num);
			Vector2 vector3 = ((Angle)(ref rotation)).RotateVec(ref vector);
			Animation val3 = new Animation
			{
				Length = TimeSpan.FromSeconds(0.10000000149011612),
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					ComponentType = typeof(SpriteComponent),
					Property = "Offset",
					KeyFrames = 
					{
						new KeyFrame((object)vector2, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)vector3, 0.1f, (Func<float, float>)null)
					}
				} }
			};
			_animation.Play(val, val3, "cm-xeno-tail");
			Animation val4 = new Animation
			{
				Length = TimeSpan.FromSeconds(0.15000000596046448)
			};
			List<AnimationTrack> animationTracks = val4.AnimationTracks;
			AnimationTrackComponentProperty val5 = new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Color",
				KeyFrames = 
				{
					new KeyFrame((object)val2.Color, 0.05f, (Func<float, float>)null)
				}
			};
			List<KeyFrame> keyFrames = ((AnimationTrackProperty)val5).KeyFrames;
			Color color = val2.Color;
			keyFrames.Add(new KeyFrame((object)((Color)(ref color)).WithAlpha((byte)0), 0.1f, (Func<float, float>)null));
			animationTracks.Add((AnimationTrack)val5);
			Animation val6 = val4;
			_animation.Play(val, val6, "cm-xeno-tail-fade");
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		TailStabOverlay tailStabOverlay = default(TailStabOverlay);
		if (_overlays.TryGetOverlay<TailStabOverlay>(ref tailStabOverlay))
		{
			tailStabOverlay.Last = LastTailAttack;
		}
	}
}
