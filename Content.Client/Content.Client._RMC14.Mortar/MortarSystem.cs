using System;
using Content.Shared._RMC14.Mortar;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Mortar;

public sealed class MortarSystem : SharedMortarSystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	private const string AnimationKey = "rmc_mortar_fire";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeAllEvent<MortarFiredEvent>((EntityEventHandler<MortarFiredEvent>)OnMortarFiredEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MortarComponent, AfterAutoHandleStateEvent>)OnMortarHandleState, (Type[])null, (Type[])null);
	}

	private void OnMortarFiredEvent(MortarFiredEvent ev)
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
		MortarComponent mortarComponent = default(MortarComponent);
		if (((EntitySystem)this).TryGetEntity(ev.Mortar, ref val) && ((EntitySystem)this).TryComp<MortarComponent>(val, ref mortarComponent) && !_animation.HasRunningAnimation(val.Value, "rmc_mortar_fire"))
		{
			_animation.Play(val.Value, new Animation
			{
				Length = mortarComponent.AnimationTime,
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = mortarComponent.AnimationLayer,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(mortarComponent.AnimationState), 0f)
					},
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(mortarComponent.DeployedState), 0.3f)
					}
				} }
			}, "rmc_mortar_fire");
		}
	}

	private void OnMortarHandleState(Entity<MortarComponent> mortar, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<MortarComponent>.op_Implicit(mortar), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is MortarBui mortarBui)
				{
					mortarBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"MortarBui"}:\n{value}");
		}
	}
}
