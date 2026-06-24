using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Emoting;

public sealed class EmoteSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EmoteAttemptEvent>((EntityEventHandler<EmoteAttemptEvent>)OnEmoteAttempt, (Type[])null, (Type[])null);
	}

	public void SetEmoting(EntityUid uid, bool value, EmotingComponent? component = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!value || ((EntitySystem)this).Resolve<EmotingComponent>(uid, ref component, true))
		{
			component = ((EntitySystem)this).EnsureComp<EmotingComponent>(uid);
			if (component.Enabled != value)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	private void OnEmoteAttempt(EmoteAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EmotingComponent emote = default(EmotingComponent);
		if (!((EntitySystem)this).TryComp<EmotingComponent>(args.Uid, ref emote) || !emote.Enabled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
