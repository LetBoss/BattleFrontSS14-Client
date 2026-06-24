using System;
using Content.Shared._RMC14.Deafness;
using Content.Shared.StatusEffect;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Deafness;

public sealed class DeafnessSystem : SharedDeafnessSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IAudioManager _audio;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private IGameTiming _timing;

	private float _originalVolume = 0.5f;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeafComponent, ComponentShutdown>((ComponentEventHandler<DeafComponent, ComponentShutdown>)OnDeafShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeafComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<DeafComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _cfg, CVars.AudioMasterVolume, (Action<float>)delegate(float value)
		{
			_originalVolume = value;
		}, true);
	}

	private void OnDeafShutdown(EntityUid uid, DeafComponent component, ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			_audio.SetMasterGain(_originalVolume);
		}
	}

	private void OnPlayerDetached(EntityUid uid, DeafComponent component, LocalPlayerDetachedEvent args)
	{
		_audio.SetMasterGain(_originalVolume);
	}

	public override void Update(float frameTime)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		TimeSpan curTime = _timing.CurTime;
		EntityQueryEnumerator<DeafComponent> val = ((EntitySystem)this).EntityQueryEnumerator<DeafComponent>();
		EntityUid val2 = default(EntityUid);
		DeafComponent deafComponent = default(DeafComponent);
		while (val.MoveNext(ref val2, ref deafComponent))
		{
			if (valueOrDefault != val2)
			{
				continue;
			}
			(TimeSpan, TimeSpan)? time = null;
			(TimeSpan, TimeSpan)? time2 = null;
			if (!_statusEffects.TryGetTime(valueOrDefault, ProtoId<StatusEffectPrototype>.op_Implicit(DeafKey), out time) && !_statusEffects.TryGetTime(valueOrDefault, "Unconscious", out time2))
			{
				continue;
			}
			if (time2.HasValue && (!time.HasValue || time.Value.Item2 < time2.Value.Item2))
			{
				time = time2.Value;
			}
			if (!time.HasValue)
			{
				continue;
			}
			(TimeSpan, TimeSpan) value = time.Value;
			float num = (float)(value.Item2 - value.Item1).TotalSeconds;
			float num2 = (float)(value.Item2 - curTime).TotalSeconds;
			float num3 = (float)(curTime - value.Item1).TotalSeconds;
			float num4 = 0f;
			float num5 = Math.Clamp(num * 0.35f, 0.2f, 2f);
			float num6 = Math.Clamp(num * 0.15f, 0.1f, 1f);
			if (num3 <= 2f && !deafComponent.DidFadeOut)
			{
				num4 = (1f - num3 / num5) * _originalVolume;
				if (num4 <= 0.1f)
				{
					num4 = 0f;
					deafComponent.DidFadeOut = true;
				}
			}
			else if (num2 <= 1f)
			{
				num4 = (1f - num2 / num6) * _originalVolume;
			}
			num4 = Math.Max(0f, num4);
			_audio.SetMasterGain(num4);
		}
	}
}
