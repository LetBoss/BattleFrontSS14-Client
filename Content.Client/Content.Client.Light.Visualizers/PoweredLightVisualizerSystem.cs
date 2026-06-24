using System;
using Content.Shared.Light;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Light.Visualizers;

public sealed class PoweredLightVisualizerSystem : VisualizerSystem<PoweredLightVisualsComponent>
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PoweredLightVisualsComponent, AnimationCompletedEvent>((ComponentEventHandler<PoweredLightVisualsComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
	}

	protected override void OnAppearanceChange(EntityUid uid, PoweredLightVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		PoweredLightState poweredLightState = default(PoweredLightState);
		if (args.Sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<PoweredLightState>(uid, (Enum)PoweredLightVisuals.BulbState, ref poweredLightState, args.Component))
		{
			return;
		}
		if (comp.SpriteStateMap.TryGetValue(poweredLightState, out string value))
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PoweredLightLayers.Base, StateId.op_Implicit(value));
		}
		if (base.SpriteSystem.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PoweredLightLayers.Glow))
		{
			PointLightComponent val = default(PointLightComponent);
			if (((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val))
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PoweredLightLayers.Glow, ((SharedPointLightComponent)val).Color);
			}
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PoweredLightLayers.Glow, poweredLightState == PoweredLightState.On);
		}
		bool flag = default(bool);
		SetBlinkingAnimation(uid, poweredLightState == PoweredLightState.On && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)PoweredLightVisuals.Blinking, ref flag, args.Component) && flag, comp);
	}

	private void OnAnimationCompleted(EntityUid uid, PoweredLightVisualsComponent comp, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent item = default(AnimationPlayerComponent);
		if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref item) && !(args.Key != "poweredlight_blinking") && comp.IsBlinking)
		{
			base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, item)), BlinkingAnimation(comp), "poweredlight_blinking");
		}
	}

	private void SetBlinkingAnimation(EntityUid uid, bool shouldBeBlinking, PoweredLightVisualsComponent comp)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (shouldBeBlinking != comp.IsBlinking)
		{
			comp.IsBlinking = shouldBeBlinking;
			AnimationPlayerComponent val = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
			if (shouldBeBlinking)
			{
				base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), BlinkingAnimation(comp), "poweredlight_blinking");
			}
			else if (base.AnimationSystem.HasRunningAnimation(uid, val, "poweredlight_blinking"))
			{
				base.AnimationSystem.Stop(uid, val, "poweredlight_blinking");
			}
		}
	}

	private Animation BlinkingAnimation(PoweredLightVisualsComponent comp)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0102: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		float num = MathHelper.Lerp(comp.MinBlinkingAnimationCycleTime, comp.MaxBlinkingAnimationCycleTime, _random.NextFloat());
		Animation val = new Animation
		{
			Length = TimeSpan.FromSeconds(num),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(PointLightComponent),
				InterpolationMode = (AnimationInterpolationMode)2,
				Property = "AnimatedEnable",
				KeyFrames = 
				{
					new KeyFrame((object)false, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)true, 1f, (Func<float, float>)null)
				}
			} },
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = PoweredLightLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.SpriteStateMap[PoweredLightState.Off]), 0f)
				},
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.SpriteStateMap[PoweredLightState.On]), 0.5f)
				}
			} }
		};
		if (comp.BlinkingSound != null)
		{
			ResolvedSoundSpecifier val2 = _audio.ResolveSound(comp.BlinkingSound);
			val.AnimationTracks.Add((AnimationTrack)new AnimationTrackPlaySound
			{
				KeyFrames = 
				{
					new KeyFrame(val2, 0.5f, (Func<AudioParams>)null)
				}
			});
		}
		return val;
	}
}
