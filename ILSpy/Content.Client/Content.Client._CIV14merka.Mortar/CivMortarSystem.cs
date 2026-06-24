using System;
using Content.Shared._CIV14merka.Mortar;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Mortar;

public sealed class CivMortarSystem : SharedCivMortarSystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	private const string AnimationKey = "civ_mortar_fire";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeAllEvent<CivMortarFiredEvent>((EntityEventHandler<CivMortarFiredEvent>)OnMortarFiredEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CivMortarComponent, AfterAutoHandleStateEvent>)OnMortarHandleState, (Type[])null, (Type[])null);
	}

	private void OnMortarFiredEvent(CivMortarFiredEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00ba: Expected O, but got Unknown
		EntityUid? val = default(EntityUid?);
		CivMortarComponent civMortarComponent = default(CivMortarComponent);
		if (((EntitySystem)this).TryGetEntity(ev.Mortar, ref val) && ((EntitySystem)this).TryComp<CivMortarComponent>(val, ref civMortarComponent) && !_animation.HasRunningAnimation(val.Value, "civ_mortar_fire"))
		{
			_animation.Play(val.Value, new Animation
			{
				Length = civMortarComponent.AnimationTime,
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = civMortarComponent.AnimationLayer,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(civMortarComponent.AnimationState), 0f)
					},
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(civMortarComponent.DeployedState), 0.3f)
					}
				} }
			}, "civ_mortar_fire");
		}
	}

	private void OnMortarHandleState(Entity<CivMortarComponent> mortar, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<CivMortarComponent>.op_Implicit(mortar), ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is CivMortarBui civMortarBui)
			{
				civMortarBui.Refresh();
			}
		}
	}
}
