using System;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunSpinupSystem : EntitySystem
{
	private const float ClientWindupSafetyPadding = 0.05f;

	private const float ModifierRefreshEpsilon = 0.01f;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunSpinupComponent, ComponentStartup>((EntityEventRefHandler<GunSpinupComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSpinupComponent, AttemptShootEvent>((EntityEventRefHandler<GunSpinupComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSpinupComponent, GunShotEvent>((EntityEventRefHandler<GunSpinupComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSpinupComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunSpinupComponent, GunRefreshModifiersEvent>)OnRefreshModifiers, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<GunSpinupComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.LastUpdate = _timing.CurTime;
		ent.Comp.CurrentSpinLevel = ent.Comp.MinSpinLevel;
		ent.Comp.LastAppliedRate = -1f;
		ent.Comp.LastAppliedScatter = -1f;
		ent.Comp.WasFiring = false;
		ent.Comp.StartSoundPlayed = false;
		ent.Comp.LastAttemptAt = null;
		ent.Comp.PendingWindupUntil = null;
		ent.Comp.LastLoopSoundAt = null;
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(Entity<GunSpinupComponent>.op_Implicit(ent), ref gun))
		{
			_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit((Entity<GunSpinupComponent>.op_Implicit(ent), gun)));
		}
	}

	private void OnAttemptShoot(Entity<GunSpinupComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		if ((_net.IsClient && !_timing.IsFirstTimePredicted) || args.Cancelled || ent.Comp.InitialWindupDelay <= 0f)
		{
			return;
		}
		TimeSpan now = _timing.CurTime;
		if (IsSpinActive(ent.Comp, now))
		{
			return;
		}
		TimeSpan? lastAttemptAt;
		if (!ent.Comp.PendingWindupUntil.HasValue)
		{
			lastAttemptAt = ent.Comp.LastAttemptAt;
			if (lastAttemptAt.HasValue)
			{
				TimeSpan lastAttempt = lastAttemptAt.GetValueOrDefault();
				if ((now - lastAttempt).TotalSeconds > (double)ent.Comp.InitialWindupResetGap)
				{
					ent.Comp.PendingWindupUntil = null;
					ent.Comp.StartSoundPlayed = false;
				}
			}
		}
		ent.Comp.LastAttemptAt = now;
		lastAttemptAt = ent.Comp.PendingWindupUntil;
		TimeSpan pendingUntil;
		if (lastAttemptAt.HasValue)
		{
			pendingUntil = lastAttemptAt.GetValueOrDefault();
		}
		else
		{
			pendingUntil = now + TimeSpan.FromSeconds(ent.Comp.InitialWindupDelay);
			ent.Comp.PendingWindupUntil = pendingUntil;
			if (!ent.Comp.StartSoundPlayed && ent.Comp.StartSound != null)
			{
				_audio.PlayPredicted(ent.Comp.StartSound, Entity<GunSpinupComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
				ent.Comp.StartSoundPlayed = true;
			}
		}
		TimeSpan readyAt = pendingUntil + (_net.IsClient ? TimeSpan.FromSeconds(0.05000000074505806) : TimeSpan.Zero);
		if (now >= readyAt)
		{
			ent.Comp.PendingWindupUntil = null;
			return;
		}
		args.Cancelled = true;
		args.ResetCooldown = true;
	}

	private void OnGunShot(Entity<GunSpinupComponent> ent, ref GunShotEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan now = _timing.CurTime;
		bool num = IsSpinActive(ent.Comp, now);
		ent.Comp.LastShotAt = now;
		ent.Comp.PendingWindupUntil = null;
		if (!num && ent.Comp.StartSound != null && !ent.Comp.StartSoundPlayed)
		{
			_audio.PlayPredicted(ent.Comp.StartSound, Entity<GunSpinupComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
		if (ent.Comp.LoopSound != null && (!ent.Comp.LastLoopSoundAt.HasValue || (now - ent.Comp.LastLoopSoundAt.Value).TotalSeconds >= (double)MathF.Max(ent.Comp.LoopSoundCooldown, 0f)))
		{
			_audio.PlayPredicted(ent.Comp.LoopSound, Entity<GunSpinupComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
			ent.Comp.LastLoopSoundAt = now;
		}
		ent.Comp.StartSoundPlayed = true;
	}

	private void OnRefreshModifiers(Entity<GunSpinupComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		float rate = GetRateMultiplier(ent.Comp);
		float finalFireRate = rate / MathF.Max(ent.Comp.BaseShotDelay, 0.01f);
		Angle scatterAngle = Angle.FromDegrees((double)MathF.Max(ent.Comp.BaseScatter / MathF.Max(rate, 1f), 0f));
		args.FireRate = finalFireRate;
		args.MinAngle = scatterAngle;
		args.MaxAngle = scatterAngle;
		args.AngleIncrease = Angle.Zero;
		args.AngleDecay = Angle.Zero;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan now = _timing.CurTime;
		EntityQueryEnumerator<GunSpinupComponent, GunComponent> query = ((EntitySystem)this).EntityQueryEnumerator<GunSpinupComponent, GunComponent>();
		EntityUid uid = default(EntityUid);
		GunSpinupComponent spin = default(GunSpinupComponent);
		GunComponent gun = default(GunComponent);
		while (query.MoveNext(ref uid, ref spin, ref gun))
		{
			float dt = (float)(now - spin.LastUpdate).TotalSeconds;
			if (dt <= 0f)
			{
				continue;
			}
			spin.LastUpdate = now;
			bool isFiring = IsFiring(spin, now);
			bool isSpinActive = IsSpinActive(spin, now);
			if (spin.WasFiring && !isSpinActive && spin.StopSound != null)
			{
				_audio.PlayPvs(spin.StopSound, uid, (AudioParams?)null);
			}
			if (spin.WasFiring && !isSpinActive)
			{
				spin.StartSoundPlayed = false;
			}
			spin.WasFiring = isSpinActive;
			float spinRange = MathF.Max(spin.MaxSpinLevel - spin.MinSpinLevel, 0f);
			if (!(spinRange <= 0f))
			{
				float level = Math.Clamp(spin.CurrentSpinLevel, spin.MinSpinLevel, spin.MaxSpinLevel);
				if (isFiring)
				{
					float up = spinRange / MathF.Max(spin.SpinUpTime, 0.01f);
					level += up * dt;
				}
				else if (!isSpinActive)
				{
					float down = spinRange / MathF.Max(spin.SpinDownTime, 0.01f);
					level -= down * dt;
				}
				float newRate = GetRateMultiplier(level: spin.CurrentSpinLevel = Math.Clamp(level, spin.MinSpinLevel, spin.MaxSpinLevel), comp: spin);
				float newScatter = spin.BaseScatter / MathF.Max(newRate, 1f);
				if (!(Math.Abs(newRate - spin.LastAppliedRate) < 0.01f) || !(Math.Abs(newScatter - spin.LastAppliedScatter) < 0.01f))
				{
					spin.LastAppliedRate = newRate;
					spin.LastAppliedScatter = newScatter;
					_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit((uid, gun)));
				}
			}
		}
	}

	private static bool IsFiring(GunSpinupComponent comp, TimeSpan now)
	{
		TimeSpan? lastShotAt = comp.LastShotAt;
		if (lastShotAt.HasValue)
		{
			TimeSpan lastShot = lastShotAt.GetValueOrDefault();
			float currentRate = GetRateMultiplier(comp, comp.CurrentSpinLevel);
			float fireWindow = comp.BaseShotDelay / MathF.Max(currentRate, 1f) + MathF.Max(comp.FireWindowPadding, 0f);
			return (now - lastShot).TotalSeconds <= (double)fireWindow;
		}
		return false;
	}

	private static bool IsSpinActive(GunSpinupComponent comp, TimeSpan now)
	{
		TimeSpan? lastShotAt = comp.LastShotAt;
		if (lastShotAt.HasValue)
		{
			TimeSpan lastShot = lastShotAt.GetValueOrDefault();
			return (now - lastShot).TotalSeconds <= (double)MathF.Max(comp.GraceAfterStop, 0f);
		}
		return false;
	}

	private static float GetRateMultiplier(GunSpinupComponent comp)
	{
		return GetRateMultiplier(comp, comp.CurrentSpinLevel);
	}

	private static float GetRateMultiplier(GunSpinupComponent comp, float level)
	{
		if (comp.RateTiers.Length == 0)
		{
			return 1f;
		}
		if (comp.RateTiers.Length == 1)
		{
			return MathF.Max(comp.RateTiers[0], 1f);
		}
		float num = Math.Clamp(level, 1f, comp.RateTiers.Length);
		int lowerIndex = Math.Clamp((int)MathF.Floor(num) - 1, 0, comp.RateTiers.Length - 1);
		int upperIndex = Math.Min(lowerIndex + 1, comp.RateTiers.Length - 1);
		float localT = num - (float)(lowerIndex + 1);
		float lower = MathF.Max(comp.RateTiers[lowerIndex], 1f);
		float upper = MathF.Max(comp.RateTiers[upperIndex], 1f);
		return lower + (upper - lower) * localT;
	}
}
