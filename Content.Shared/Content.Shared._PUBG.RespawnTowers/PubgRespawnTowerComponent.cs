using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.RespawnTowers;

[RegisterComponent]
public sealed class PubgRespawnTowerComponent : Component, ISerializationGenerated<PubgRespawnTowerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float ActivationDelay = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float ActivationRadius = 3f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 ReviveHp = FixedPoint2.New(5);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? ActivationSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgRespawnTowerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgRespawnTowerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgRespawnTowerComponent>(this, ref target, hookCtx, false, context))
		{
			float ActivationDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ActivationDelay, ref ActivationDelayTemp, hookCtx, false, context))
			{
				ActivationDelayTemp = ActivationDelay;
			}
			target.ActivationDelay = ActivationDelayTemp;
			float ActivationRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ActivationRadius, ref ActivationRadiusTemp, hookCtx, false, context))
			{
				ActivationRadiusTemp = ActivationRadius;
			}
			target.ActivationRadius = ActivationRadiusTemp;
			FixedPoint2 ReviveHpTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(ReviveHp, ref ReviveHpTemp, hookCtx, false, context))
			{
				ReviveHpTemp = serialization.CreateCopy<FixedPoint2>(ReviveHp, hookCtx, context, false);
			}
			target.ReviveHp = ReviveHpTemp;
			SoundSpecifier ActivationSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(ActivationSound, ref ActivationSoundTemp, hookCtx, true, context))
			{
				ActivationSoundTemp = serialization.CreateCopy<SoundSpecifier>(ActivationSound, hookCtx, context, false);
			}
			target.ActivationSound = ActivationSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgRespawnTowerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgRespawnTowerComponent cast = (PubgRespawnTowerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgRespawnTowerComponent cast = (PubgRespawnTowerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgRespawnTowerComponent def = (PubgRespawnTowerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgRespawnTowerComponent Instantiate()
	{
		return new PubgRespawnTowerComponent();
	}
}
