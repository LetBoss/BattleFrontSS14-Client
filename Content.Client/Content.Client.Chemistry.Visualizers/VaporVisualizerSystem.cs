using System;
using Content.Shared.Vapor;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chemistry.Visualizers;

public sealed class VaporVisualizerSystem : VisualizerSystem<VaporVisualsComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VaporVisualsComponent, ComponentInit>((ComponentEventHandler<VaporVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, VaporVisualsComponent comp, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0059: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		comp.VaporFlick = new Animation
		{
			Length = TimeSpan.FromSeconds(comp.AnimationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = VaporVisualLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.AnimationState), 0f)
				}
			} }
		};
		bool flag = default(bool);
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)VaporVisuals.State, ref flag, (AppearanceComponent)null) && flag && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val) && !base.AnimationSystem.HasRunningAnimation(uid, val, "flick_animation"))
		{
			base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), comp.VaporFlick, "flick_animation");
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, VaporVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)VaporVisuals.Color, ref val, args.Component) && args.Sprite != null)
		{
			base.SpriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), val);
		}
	}
}
