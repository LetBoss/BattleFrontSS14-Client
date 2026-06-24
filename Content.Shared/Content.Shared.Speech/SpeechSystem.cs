using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Speech;

public sealed class SpeechSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpeakAttemptEvent>((EntityEventHandler<SpeakAttemptEvent>)OnSpeakAttempt, (Type[])null, (Type[])null);
	}

	public void SetSpeech(EntityUid uid, bool value, SpeechComponent? component = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!value || ((EntitySystem)this).Resolve<SpeechComponent>(uid, ref component, true))
		{
			component = ((EntitySystem)this).EnsureComp<SpeechComponent>(uid);
			if (component.Enabled != value)
			{
				component.Enabled = value;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	private void OnSpeakAttempt(SpeakAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SpeechComponent speech = default(SpeechComponent);
		if (!((EntitySystem)this).TryComp<SpeechComponent>(args.Uid, ref speech) || !speech.Enabled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
