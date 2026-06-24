using System;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.ForTheHive;

[RegisterComponent]
[NetworkedComponent]
public sealed class ActiveForTheHiveComponent : Component, ISerializationGenerated<ActiveForTheHiveComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Duration;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TimeLeft;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier WindingUpSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Xeno/runner_charging_1.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier WindingDownSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Xeno/runner_charging_2.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan NextUpdate;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan UpdateEvery = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public bool UseWindUpSound = true;

	[DataField(null, false, 1, false, false, null)]
	public float InitialVolume = -3f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxVolume = 23f;

	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier BaseDamage = new DamageSpecifier();

	[DataField(null, false, 1, false, false, null)]
	public float AcidRangeRatio = 200f;

	[DataField(null, false, 1, false, false, null)]
	public float BurnRangeRatio = 100f;

	[DataField(null, false, 1, false, false, null)]
	public float BurnDamageRatio = 5f;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Acid = EntProtoId.op_Implicit("XenoAcidNormal");

	[DataField(null, false, 1, false, false, null)]
	public XenoAcidStrength AcidStrength = XenoAcidStrength.Normal;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan AcidTime = TimeSpan.FromSeconds(255L);

	[DataField(null, false, 1, false, false, null)]
	public float AcidDps = 8f;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId AcidSmoke = EntProtoId.op_Implicit("RMCSmokeRunner");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier KaboomSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Xeno/blobattack.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan CoreSpawnTime = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan CorpseSpawnTime = TimeSpan.FromSeconds(0.5);

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 SlowDown = FixedPoint2.New(0.45);

	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry? MobAcid;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActiveForTheHiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActiveForTheHiveComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ActiveForTheHiveComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		TimeSpan DurationTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(Duration, ref DurationTemp, hookCtx, false, context))
		{
			DurationTemp = serialization.CreateCopy<TimeSpan>(Duration, hookCtx, context, false);
		}
		target.Duration = DurationTemp;
		TimeSpan TimeLeftTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(TimeLeft, ref TimeLeftTemp, hookCtx, false, context))
		{
			TimeLeftTemp = serialization.CreateCopy<TimeSpan>(TimeLeft, hookCtx, context, false);
		}
		target.TimeLeft = TimeLeftTemp;
		SoundSpecifier WindingUpSoundTemp = null;
		if (WindingUpSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(WindingUpSound, ref WindingUpSoundTemp, hookCtx, true, context))
		{
			WindingUpSoundTemp = serialization.CreateCopy<SoundSpecifier>(WindingUpSound, hookCtx, context, false);
		}
		target.WindingUpSound = WindingUpSoundTemp;
		SoundSpecifier WindingDownSoundTemp = null;
		if (WindingDownSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(WindingDownSound, ref WindingDownSoundTemp, hookCtx, true, context))
		{
			WindingDownSoundTemp = serialization.CreateCopy<SoundSpecifier>(WindingDownSound, hookCtx, context, false);
		}
		target.WindingDownSound = WindingDownSoundTemp;
		TimeSpan NextUpdateTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(NextUpdate, ref NextUpdateTemp, hookCtx, false, context))
		{
			NextUpdateTemp = serialization.CreateCopy<TimeSpan>(NextUpdate, hookCtx, context, false);
		}
		target.NextUpdate = NextUpdateTemp;
		TimeSpan UpdateEveryTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(UpdateEvery, ref UpdateEveryTemp, hookCtx, false, context))
		{
			UpdateEveryTemp = serialization.CreateCopy<TimeSpan>(UpdateEvery, hookCtx, context, false);
		}
		target.UpdateEvery = UpdateEveryTemp;
		bool UseWindUpSoundTemp = false;
		if (!serialization.TryCustomCopy<bool>(UseWindUpSound, ref UseWindUpSoundTemp, hookCtx, false, context))
		{
			UseWindUpSoundTemp = UseWindUpSound;
		}
		target.UseWindUpSound = UseWindUpSoundTemp;
		float InitialVolumeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(InitialVolume, ref InitialVolumeTemp, hookCtx, false, context))
		{
			InitialVolumeTemp = InitialVolume;
		}
		target.InitialVolume = InitialVolumeTemp;
		float MaxVolumeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MaxVolume, ref MaxVolumeTemp, hookCtx, false, context))
		{
			MaxVolumeTemp = MaxVolume;
		}
		target.MaxVolume = MaxVolumeTemp;
		DamageSpecifier BaseDamageTemp = null;
		if (BaseDamage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(BaseDamage, ref BaseDamageTemp, hookCtx, false, context))
		{
			if (BaseDamage == null)
			{
				BaseDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(BaseDamage, ref BaseDamageTemp, hookCtx, context, true);
			}
		}
		target.BaseDamage = BaseDamageTemp;
		float AcidRangeRatioTemp = 0f;
		if (!serialization.TryCustomCopy<float>(AcidRangeRatio, ref AcidRangeRatioTemp, hookCtx, false, context))
		{
			AcidRangeRatioTemp = AcidRangeRatio;
		}
		target.AcidRangeRatio = AcidRangeRatioTemp;
		float BurnRangeRatioTemp = 0f;
		if (!serialization.TryCustomCopy<float>(BurnRangeRatio, ref BurnRangeRatioTemp, hookCtx, false, context))
		{
			BurnRangeRatioTemp = BurnRangeRatio;
		}
		target.BurnRangeRatio = BurnRangeRatioTemp;
		float BurnDamageRatioTemp = 0f;
		if (!serialization.TryCustomCopy<float>(BurnDamageRatio, ref BurnDamageRatioTemp, hookCtx, false, context))
		{
			BurnDamageRatioTemp = BurnDamageRatio;
		}
		target.BurnDamageRatio = BurnDamageRatioTemp;
		EntProtoId AcidTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(Acid, ref AcidTemp, hookCtx, false, context))
		{
			AcidTemp = serialization.CreateCopy<EntProtoId>(Acid, hookCtx, context, false);
		}
		target.Acid = AcidTemp;
		XenoAcidStrength AcidStrengthTemp = (XenoAcidStrength)0;
		if (!serialization.TryCustomCopy<XenoAcidStrength>(AcidStrength, ref AcidStrengthTemp, hookCtx, false, context))
		{
			AcidStrengthTemp = AcidStrength;
		}
		target.AcidStrength = AcidStrengthTemp;
		TimeSpan AcidTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(AcidTime, ref AcidTimeTemp, hookCtx, false, context))
		{
			AcidTimeTemp = serialization.CreateCopy<TimeSpan>(AcidTime, hookCtx, context, false);
		}
		target.AcidTime = AcidTimeTemp;
		float AcidDpsTemp = 0f;
		if (!serialization.TryCustomCopy<float>(AcidDps, ref AcidDpsTemp, hookCtx, false, context))
		{
			AcidDpsTemp = AcidDps;
		}
		target.AcidDps = AcidDpsTemp;
		EntProtoId AcidSmokeTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(AcidSmoke, ref AcidSmokeTemp, hookCtx, false, context))
		{
			AcidSmokeTemp = serialization.CreateCopy<EntProtoId>(AcidSmoke, hookCtx, context, false);
		}
		target.AcidSmoke = AcidSmokeTemp;
		SoundSpecifier KaboomSoundTemp = null;
		if (KaboomSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(KaboomSound, ref KaboomSoundTemp, hookCtx, true, context))
		{
			KaboomSoundTemp = serialization.CreateCopy<SoundSpecifier>(KaboomSound, hookCtx, context, false);
		}
		target.KaboomSound = KaboomSoundTemp;
		TimeSpan CoreSpawnTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(CoreSpawnTime, ref CoreSpawnTimeTemp, hookCtx, false, context))
		{
			CoreSpawnTimeTemp = serialization.CreateCopy<TimeSpan>(CoreSpawnTime, hookCtx, context, false);
		}
		target.CoreSpawnTime = CoreSpawnTimeTemp;
		TimeSpan CorpseSpawnTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(CorpseSpawnTime, ref CorpseSpawnTimeTemp, hookCtx, false, context))
		{
			CorpseSpawnTimeTemp = serialization.CreateCopy<TimeSpan>(CorpseSpawnTime, hookCtx, context, false);
		}
		target.CorpseSpawnTime = CorpseSpawnTimeTemp;
		FixedPoint2 SlowDownTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(SlowDown, ref SlowDownTemp, hookCtx, false, context))
		{
			SlowDownTemp = serialization.CreateCopy<FixedPoint2>(SlowDown, hookCtx, context, false);
		}
		target.SlowDown = SlowDownTemp;
		ComponentRegistry MobAcidTemp = null;
		if (!serialization.TryCustomCopy<ComponentRegistry>(MobAcid, ref MobAcidTemp, hookCtx, false, context))
		{
			MobAcidTemp = serialization.CreateCopy<ComponentRegistry>(MobAcid, hookCtx, context, false);
		}
		target.MobAcid = MobAcidTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActiveForTheHiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveForTheHiveComponent cast = (ActiveForTheHiveComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveForTheHiveComponent cast = (ActiveForTheHiveComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveForTheHiveComponent def = (ActiveForTheHiveComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActiveForTheHiveComponent Instantiate()
	{
		return new ActiveForTheHiveComponent();
	}
}
