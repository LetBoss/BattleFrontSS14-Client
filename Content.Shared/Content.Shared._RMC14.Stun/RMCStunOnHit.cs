using System;
using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Stun;

[Serializable]
[DataDefinition]
[NetSerializable]
public struct RMCStunOnHit : ISerializationGenerated<RMCStunOnHit>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist = null;

	[DataField(null, false, 1, false, false, null)]
	public float MaxRange = 2.5f;

	[DataField(null, false, 1, false, false, null)]
	public float KnockBackPowerMin = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float KnockBackPowerMax = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float KnockBackSpeed = 5f;

	[DataField(null, false, 1, false, false, null)]
	public bool ForceKnockBack = false;

	[DataField(null, false, 1, false, false, null)]
	public bool LosesEffectWithRange = false;

	[DataField(null, false, 1, false, false, null)]
	public bool SlowsEffectBigXenos = false;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan StunTime = TimeSpan.FromSeconds(1.4);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan SuperSlowTime = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan SlowTime = TimeSpan.FromSeconds(2L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DazeTime = default(TimeSpan);

	[DataField(null, false, 1, false, false, null)]
	public float StunArea = 0.5f;

	public RMCStunOnHit()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCStunOnHit target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (serialization.TryCustomCopy<RMCStunOnHit>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		float MaxRangeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MaxRange, ref MaxRangeTemp, hookCtx, false, context))
		{
			MaxRangeTemp = MaxRange;
		}
		float KnockBackPowerMinTemp = 0f;
		if (!serialization.TryCustomCopy<float>(KnockBackPowerMin, ref KnockBackPowerMinTemp, hookCtx, false, context))
		{
			KnockBackPowerMinTemp = KnockBackPowerMin;
		}
		float KnockBackPowerMaxTemp = 0f;
		if (!serialization.TryCustomCopy<float>(KnockBackPowerMax, ref KnockBackPowerMaxTemp, hookCtx, false, context))
		{
			KnockBackPowerMaxTemp = KnockBackPowerMax;
		}
		float KnockBackSpeedTemp = 0f;
		if (!serialization.TryCustomCopy<float>(KnockBackSpeed, ref KnockBackSpeedTemp, hookCtx, false, context))
		{
			KnockBackSpeedTemp = KnockBackSpeed;
		}
		bool ForceKnockBackTemp = false;
		if (!serialization.TryCustomCopy<bool>(ForceKnockBack, ref ForceKnockBackTemp, hookCtx, false, context))
		{
			ForceKnockBackTemp = ForceKnockBack;
		}
		bool LosesEffectWithRangeTemp = false;
		if (!serialization.TryCustomCopy<bool>(LosesEffectWithRange, ref LosesEffectWithRangeTemp, hookCtx, false, context))
		{
			LosesEffectWithRangeTemp = LosesEffectWithRange;
		}
		bool SlowsEffectBigXenosTemp = false;
		if (!serialization.TryCustomCopy<bool>(SlowsEffectBigXenos, ref SlowsEffectBigXenosTemp, hookCtx, false, context))
		{
			SlowsEffectBigXenosTemp = SlowsEffectBigXenos;
		}
		TimeSpan StunTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(StunTime, ref StunTimeTemp, hookCtx, false, context))
		{
			StunTimeTemp = serialization.CreateCopy<TimeSpan>(StunTime, hookCtx, context, false);
		}
		TimeSpan SuperSlowTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(SuperSlowTime, ref SuperSlowTimeTemp, hookCtx, false, context))
		{
			SuperSlowTimeTemp = serialization.CreateCopy<TimeSpan>(SuperSlowTime, hookCtx, context, false);
		}
		TimeSpan SlowTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(SlowTime, ref SlowTimeTemp, hookCtx, false, context))
		{
			SlowTimeTemp = serialization.CreateCopy<TimeSpan>(SlowTime, hookCtx, context, false);
		}
		TimeSpan DazeTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(DazeTime, ref DazeTimeTemp, hookCtx, false, context))
		{
			DazeTimeTemp = serialization.CreateCopy<TimeSpan>(DazeTime, hookCtx, context, false);
		}
		float StunAreaTemp = 0f;
		if (!serialization.TryCustomCopy<float>(StunArea, ref StunAreaTemp, hookCtx, false, context))
		{
			StunAreaTemp = StunArea;
		}
		RMCStunOnHit rMCStunOnHit = target;
		rMCStunOnHit.Whitelist = WhitelistTemp;
		rMCStunOnHit.MaxRange = MaxRangeTemp;
		rMCStunOnHit.KnockBackPowerMin = KnockBackPowerMinTemp;
		rMCStunOnHit.KnockBackPowerMax = KnockBackPowerMaxTemp;
		rMCStunOnHit.KnockBackSpeed = KnockBackSpeedTemp;
		rMCStunOnHit.ForceKnockBack = ForceKnockBackTemp;
		rMCStunOnHit.LosesEffectWithRange = LosesEffectWithRangeTemp;
		rMCStunOnHit.SlowsEffectBigXenos = SlowsEffectBigXenosTemp;
		rMCStunOnHit.StunTime = StunTimeTemp;
		rMCStunOnHit.SuperSlowTime = SuperSlowTimeTemp;
		rMCStunOnHit.SlowTime = SlowTimeTemp;
		rMCStunOnHit.DazeTime = DazeTimeTemp;
		rMCStunOnHit.StunArea = StunAreaTemp;
		target = rMCStunOnHit;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCStunOnHit target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCStunOnHit cast = (RMCStunOnHit)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCStunOnHit Instantiate()
	{
		return new RMCStunOnHit();
	}
}
