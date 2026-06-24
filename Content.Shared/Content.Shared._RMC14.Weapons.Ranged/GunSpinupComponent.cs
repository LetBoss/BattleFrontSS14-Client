using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[Access(new Type[] { typeof(GunSpinupSystem) })]
public sealed class GunSpinupComponent : Component, ISerializationGenerated<GunSpinupComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float BaseShotDelay = 0.7f;

	[DataField(null, false, 1, false, false, null)]
	public float BaseScatter = 18f;

	[DataField(null, false, 1, false, false, null)]
	public float SpinUpTime = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float GraceAfterStop = 2f;

	[DataField(null, false, 1, false, false, null)]
	public float SpinDownTime = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float MinSpinLevel = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxSpinLevel = 11f;

	[DataField(null, false, 1, false, false, null)]
	public int[] RateTiers = new int[11]
	{
		1, 1, 2, 2, 3, 3, 3, 4, 4, 4,
		5
	};

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? StartSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_RMC14/Vehicle/weapons/minigun_start.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? LoopSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/minigun.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? StopSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_RMC14/Vehicle/weapons/minigun_stop.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SelectSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_RMC14/Vehicle/weapons/minigun_select.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public float LoopSoundCooldown = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float FireWindowPadding = 0.12f;

	[DataField(null, false, 1, false, false, null)]
	public float InitialWindupDelay;

	[DataField(null, false, 1, false, false, null)]
	public float InitialWindupResetGap = 0.2f;

	public TimeSpan LastUpdate;

	public TimeSpan? LastShotAt;

	public TimeSpan? LastAttemptAt;

	public TimeSpan? PendingWindupUntil;

	public TimeSpan? LastLoopSoundAt;

	public float CurrentSpinLevel = 1f;

	public float LastAppliedRate = -1f;

	public float LastAppliedScatter = -1f;

	public bool WasFiring;

	public bool StartSoundPlayed;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GunSpinupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GunSpinupComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GunSpinupComponent>(this, ref target, hookCtx, false, context))
		{
			float BaseShotDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BaseShotDelay, ref BaseShotDelayTemp, hookCtx, false, context))
			{
				BaseShotDelayTemp = BaseShotDelay;
			}
			target.BaseShotDelay = BaseShotDelayTemp;
			float BaseScatterTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BaseScatter, ref BaseScatterTemp, hookCtx, false, context))
			{
				BaseScatterTemp = BaseScatter;
			}
			target.BaseScatter = BaseScatterTemp;
			float SpinUpTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpinUpTime, ref SpinUpTimeTemp, hookCtx, false, context))
			{
				SpinUpTimeTemp = SpinUpTime;
			}
			target.SpinUpTime = SpinUpTimeTemp;
			float GraceAfterStopTemp = 0f;
			if (!serialization.TryCustomCopy<float>(GraceAfterStop, ref GraceAfterStopTemp, hookCtx, false, context))
			{
				GraceAfterStopTemp = GraceAfterStop;
			}
			target.GraceAfterStop = GraceAfterStopTemp;
			float SpinDownTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpinDownTime, ref SpinDownTimeTemp, hookCtx, false, context))
			{
				SpinDownTimeTemp = SpinDownTime;
			}
			target.SpinDownTime = SpinDownTimeTemp;
			float MinSpinLevelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinSpinLevel, ref MinSpinLevelTemp, hookCtx, false, context))
			{
				MinSpinLevelTemp = MinSpinLevel;
			}
			target.MinSpinLevel = MinSpinLevelTemp;
			float MaxSpinLevelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxSpinLevel, ref MaxSpinLevelTemp, hookCtx, false, context))
			{
				MaxSpinLevelTemp = MaxSpinLevel;
			}
			target.MaxSpinLevel = MaxSpinLevelTemp;
			int[] RateTiersTemp = null;
			if (RateTiers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<int[]>(RateTiers, ref RateTiersTemp, hookCtx, true, context))
			{
				RateTiersTemp = serialization.CreateCopy<int[]>(RateTiers, hookCtx, context, false);
			}
			target.RateTiers = RateTiersTemp;
			SoundSpecifier StartSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(StartSound, ref StartSoundTemp, hookCtx, true, context))
			{
				StartSoundTemp = serialization.CreateCopy<SoundSpecifier>(StartSound, hookCtx, context, false);
			}
			target.StartSound = StartSoundTemp;
			SoundSpecifier LoopSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(LoopSound, ref LoopSoundTemp, hookCtx, true, context))
			{
				LoopSoundTemp = serialization.CreateCopy<SoundSpecifier>(LoopSound, hookCtx, context, false);
			}
			target.LoopSound = LoopSoundTemp;
			SoundSpecifier StopSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(StopSound, ref StopSoundTemp, hookCtx, true, context))
			{
				StopSoundTemp = serialization.CreateCopy<SoundSpecifier>(StopSound, hookCtx, context, false);
			}
			target.StopSound = StopSoundTemp;
			SoundSpecifier SelectSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(SelectSound, ref SelectSoundTemp, hookCtx, true, context))
			{
				SelectSoundTemp = serialization.CreateCopy<SoundSpecifier>(SelectSound, hookCtx, context, false);
			}
			target.SelectSound = SelectSoundTemp;
			float LoopSoundCooldownTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LoopSoundCooldown, ref LoopSoundCooldownTemp, hookCtx, false, context))
			{
				LoopSoundCooldownTemp = LoopSoundCooldown;
			}
			target.LoopSoundCooldown = LoopSoundCooldownTemp;
			float FireWindowPaddingTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FireWindowPadding, ref FireWindowPaddingTemp, hookCtx, false, context))
			{
				FireWindowPaddingTemp = FireWindowPadding;
			}
			target.FireWindowPadding = FireWindowPaddingTemp;
			float InitialWindupDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InitialWindupDelay, ref InitialWindupDelayTemp, hookCtx, false, context))
			{
				InitialWindupDelayTemp = InitialWindupDelay;
			}
			target.InitialWindupDelay = InitialWindupDelayTemp;
			float InitialWindupResetGapTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InitialWindupResetGap, ref InitialWindupResetGapTemp, hookCtx, false, context))
			{
				InitialWindupResetGapTemp = InitialWindupResetGap;
			}
			target.InitialWindupResetGap = InitialWindupResetGapTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GunSpinupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunSpinupComponent cast = (GunSpinupComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunSpinupComponent cast = (GunSpinupComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunSpinupComponent def = (GunSpinupComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GunSpinupComponent Instantiate()
	{
		return new GunSpinupComponent();
	}
}
