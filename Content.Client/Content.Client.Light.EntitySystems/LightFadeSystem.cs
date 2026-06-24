using System;
using Content.Client.Light.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Light.EntitySystems;

public sealed class LightFadeSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _player;

	private const string FadeTrack = "light-fade";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LightFadeComponent, ComponentStartup>((ComponentEventHandler<LightFadeComponent, ComponentStartup>)OnFadeStartup, (Type[])null, (Type[])null);
	}

	private void OnFadeStartup(EntityUid uid, LightFadeComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0098: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		PointLightComponent val = default(PointLightComponent);
		if (((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val))
		{
			Animation val2 = new Animation
			{
				Length = TimeSpan.FromSeconds(component.Duration),
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					Property = "Energy",
					ComponentType = typeof(PointLightComponent),
					InterpolationMode = (AnimationInterpolationMode)1,
					KeyFrames = 
					{
						new KeyFrame((object)((SharedPointLightComponent)val).Energy, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)0f, component.Duration, (Func<float, float>)null)
					}
				} }
			};
			_player.Play(uid, val2, "light-fade");
		}
	}
}
