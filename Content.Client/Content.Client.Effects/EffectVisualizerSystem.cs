using System;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Effects;

public sealed class EffectVisualizerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EffectVisualsComponent, AnimationCompletedEvent>((ComponentEventHandler<EffectVisualsComponent, AnimationCompletedEvent>)OnEffectAnimComplete, (Type[])null, (Type[])null);
	}

	private void OnEffectAnimComplete(EntityUid uid, EffectVisualsComponent component, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).QueueDel((EntityUid?)uid);
	}
}
