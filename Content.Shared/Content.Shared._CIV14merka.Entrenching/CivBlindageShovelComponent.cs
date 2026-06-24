using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._CIV14merka.Entrenching;

[RegisterComponent]
public sealed class CivBlindageShovelComponent : Component, ISerializationGenerated<CivBlindageShovelComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float DigDelay = 2f;

	[DataField(null, false, 1, false, false, null)]
	public float RepairDelay = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier Sound = (SoundSpecifier)new SoundCollectionSpecifier("CIVBlindageShovel", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(2f));

	[DataField(null, false, 1, false, false, null)]
	public float SoundInterval = 0.45f;

	[DataField(null, false, 1, false, false, null)]
	public float MinPitch = 0.95f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxPitch = 1.08f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivBlindageShovelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivBlindageShovelComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CivBlindageShovelComponent>(this, ref target, hookCtx, false, context))
		{
			float DigDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DigDelay, ref DigDelayTemp, hookCtx, false, context))
			{
				DigDelayTemp = DigDelay;
			}
			target.DigDelay = DigDelayTemp;
			float RepairDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairDelay, ref RepairDelayTemp, hookCtx, false, context))
			{
				RepairDelayTemp = RepairDelay;
			}
			target.RepairDelay = RepairDelayTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
			float SoundIntervalTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SoundInterval, ref SoundIntervalTemp, hookCtx, false, context))
			{
				SoundIntervalTemp = SoundInterval;
			}
			target.SoundInterval = SoundIntervalTemp;
			float MinPitchTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinPitch, ref MinPitchTemp, hookCtx, false, context))
			{
				MinPitchTemp = MinPitch;
			}
			target.MinPitch = MinPitchTemp;
			float MaxPitchTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxPitch, ref MaxPitchTemp, hookCtx, false, context))
			{
				MaxPitchTemp = MaxPitch;
			}
			target.MaxPitch = MaxPitchTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivBlindageShovelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelComponent cast = (CivBlindageShovelComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelComponent cast = (CivBlindageShovelComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivBlindageShovelComponent def = (CivBlindageShovelComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivBlindageShovelComponent Instantiate()
	{
		return new CivBlindageShovelComponent();
	}
}
