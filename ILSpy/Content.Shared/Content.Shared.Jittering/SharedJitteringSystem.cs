using System;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Jittering;

public abstract class SharedJitteringSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming GameTiming;

	[Dependency]
	protected StatusEffectsSystem StatusEffects;

	public float MaxAmplitude = 300f;

	public float MinAmplitude = 1f;

	public float MaxFrequency = 10f;

	public float MinFrequency = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<JitteringComponent, RejuvenateEvent>((ComponentEventHandler<JitteringComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
	}

	private void OnRejuvenate(EntityUid uid, JitteringComponent component, RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<JitteringComponent>(uid);
	}

	public void DoJitter(EntityUid uid, TimeSpan time, bool refresh, float amplitude = 10f, float frequency = 4f, bool forceValueChange = false, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return;
		}
		amplitude = Math.Clamp(amplitude, MinAmplitude, MaxAmplitude);
		frequency = Math.Clamp(frequency, MinFrequency, MaxFrequency);
		if (StatusEffects.TryAddStatusEffect<JitteringComponent>(uid, "Jitter", time, refresh, status, false))
		{
			JitteringComponent jittering = ((EntitySystem)this).Comp<JitteringComponent>(uid);
			if (forceValueChange || jittering.Amplitude < amplitude)
			{
				jittering.Amplitude = amplitude;
			}
			if (forceValueChange || jittering.Frequency < frequency)
			{
				jittering.Frequency = frequency;
			}
		}
	}

	public void AddJitter(EntityUid uid, float amplitude = 10f, float frequency = 4f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		JitteringComponent jitter = ((EntitySystem)this).EnsureComp<JitteringComponent>(uid);
		jitter.Amplitude = amplitude;
		jitter.Frequency = frequency;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)jitter, (MetaDataComponent)null);
	}
}
