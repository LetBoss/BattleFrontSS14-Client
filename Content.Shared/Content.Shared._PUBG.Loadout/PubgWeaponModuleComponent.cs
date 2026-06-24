using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Loadout;

[RegisterComponent]
public sealed class PubgWeaponModuleComponent : Component, ISerializationGenerated<PubgWeaponModuleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public PubgModuleSlotType Slot;

	[DataField(null, false, 1, false, false, null)]
	public float SpreadMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float FocusBonusTiles;

	[DataField(null, false, 1, false, false, null)]
	public float RecoilMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float HipfireSpreadMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float ReloadTimeMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public int MagazineCapacityBonus;

	[DataField(null, false, 1, false, false, null)]
	public float RangeMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SoundGunshotOverride;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SpatialFarSoundOverride;

	[DataField(null, false, 1, false, false, null)]
	public bool DisableSpatialFarSound;

	[DataField(null, false, 1, false, false, null)]
	public float SpatialAudioRangeMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float SpatialNearRangeMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float SpatialConeAngleMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float SpatialNearVolumeMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public bool SuppressMuzzleFlash;

	[DataField(null, false, 1, false, false, null)]
	public string UiCategoryLocKey = "pubg-loadout-module-category-generic";

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? AttachSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_add.ogg", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-6.5f));

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? DetachSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_remove.ogg", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-5.5f));

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgWeaponModuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgWeaponModuleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgWeaponModuleComponent>(this, ref target, hookCtx, false, context))
		{
			PubgModuleSlotType SlotTemp = PubgModuleSlotType.Optic;
			if (!serialization.TryCustomCopy<PubgModuleSlotType>(Slot, ref SlotTemp, hookCtx, false, context))
			{
				SlotTemp = Slot;
			}
			target.Slot = SlotTemp;
			float SpreadMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpreadMultiplier, ref SpreadMultiplierTemp, hookCtx, false, context))
			{
				SpreadMultiplierTemp = SpreadMultiplier;
			}
			target.SpreadMultiplier = SpreadMultiplierTemp;
			float FocusBonusTilesTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FocusBonusTiles, ref FocusBonusTilesTemp, hookCtx, false, context))
			{
				FocusBonusTilesTemp = FocusBonusTiles;
			}
			target.FocusBonusTiles = FocusBonusTilesTemp;
			float RecoilMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RecoilMultiplier, ref RecoilMultiplierTemp, hookCtx, false, context))
			{
				RecoilMultiplierTemp = RecoilMultiplier;
			}
			target.RecoilMultiplier = RecoilMultiplierTemp;
			float HipfireSpreadMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HipfireSpreadMultiplier, ref HipfireSpreadMultiplierTemp, hookCtx, false, context))
			{
				HipfireSpreadMultiplierTemp = HipfireSpreadMultiplier;
			}
			target.HipfireSpreadMultiplier = HipfireSpreadMultiplierTemp;
			float ReloadTimeMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ReloadTimeMultiplier, ref ReloadTimeMultiplierTemp, hookCtx, false, context))
			{
				ReloadTimeMultiplierTemp = ReloadTimeMultiplier;
			}
			target.ReloadTimeMultiplier = ReloadTimeMultiplierTemp;
			int MagazineCapacityBonusTemp = 0;
			if (!serialization.TryCustomCopy<int>(MagazineCapacityBonus, ref MagazineCapacityBonusTemp, hookCtx, false, context))
			{
				MagazineCapacityBonusTemp = MagazineCapacityBonus;
			}
			target.MagazineCapacityBonus = MagazineCapacityBonusTemp;
			float RangeMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RangeMultiplier, ref RangeMultiplierTemp, hookCtx, false, context))
			{
				RangeMultiplierTemp = RangeMultiplier;
			}
			target.RangeMultiplier = RangeMultiplierTemp;
			SoundSpecifier SoundGunshotOverrideTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(SoundGunshotOverride, ref SoundGunshotOverrideTemp, hookCtx, true, context))
			{
				SoundGunshotOverrideTemp = serialization.CreateCopy<SoundSpecifier>(SoundGunshotOverride, hookCtx, context, false);
			}
			target.SoundGunshotOverride = SoundGunshotOverrideTemp;
			SoundSpecifier SpatialFarSoundOverrideTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(SpatialFarSoundOverride, ref SpatialFarSoundOverrideTemp, hookCtx, true, context))
			{
				SpatialFarSoundOverrideTemp = serialization.CreateCopy<SoundSpecifier>(SpatialFarSoundOverride, hookCtx, context, false);
			}
			target.SpatialFarSoundOverride = SpatialFarSoundOverrideTemp;
			bool DisableSpatialFarSoundTemp = false;
			if (!serialization.TryCustomCopy<bool>(DisableSpatialFarSound, ref DisableSpatialFarSoundTemp, hookCtx, false, context))
			{
				DisableSpatialFarSoundTemp = DisableSpatialFarSound;
			}
			target.DisableSpatialFarSound = DisableSpatialFarSoundTemp;
			float SpatialAudioRangeMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpatialAudioRangeMultiplier, ref SpatialAudioRangeMultiplierTemp, hookCtx, false, context))
			{
				SpatialAudioRangeMultiplierTemp = SpatialAudioRangeMultiplier;
			}
			target.SpatialAudioRangeMultiplier = SpatialAudioRangeMultiplierTemp;
			float SpatialNearRangeMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpatialNearRangeMultiplier, ref SpatialNearRangeMultiplierTemp, hookCtx, false, context))
			{
				SpatialNearRangeMultiplierTemp = SpatialNearRangeMultiplier;
			}
			target.SpatialNearRangeMultiplier = SpatialNearRangeMultiplierTemp;
			float SpatialConeAngleMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpatialConeAngleMultiplier, ref SpatialConeAngleMultiplierTemp, hookCtx, false, context))
			{
				SpatialConeAngleMultiplierTemp = SpatialConeAngleMultiplier;
			}
			target.SpatialConeAngleMultiplier = SpatialConeAngleMultiplierTemp;
			float SpatialNearVolumeMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpatialNearVolumeMultiplier, ref SpatialNearVolumeMultiplierTemp, hookCtx, false, context))
			{
				SpatialNearVolumeMultiplierTemp = SpatialNearVolumeMultiplier;
			}
			target.SpatialNearVolumeMultiplier = SpatialNearVolumeMultiplierTemp;
			bool SuppressMuzzleFlashTemp = false;
			if (!serialization.TryCustomCopy<bool>(SuppressMuzzleFlash, ref SuppressMuzzleFlashTemp, hookCtx, false, context))
			{
				SuppressMuzzleFlashTemp = SuppressMuzzleFlash;
			}
			target.SuppressMuzzleFlash = SuppressMuzzleFlashTemp;
			string UiCategoryLocKeyTemp = null;
			if (UiCategoryLocKey == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(UiCategoryLocKey, ref UiCategoryLocKeyTemp, hookCtx, false, context))
			{
				UiCategoryLocKeyTemp = UiCategoryLocKey;
			}
			target.UiCategoryLocKey = UiCategoryLocKeyTemp;
			SoundSpecifier AttachSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(AttachSound, ref AttachSoundTemp, hookCtx, true, context))
			{
				AttachSoundTemp = serialization.CreateCopy<SoundSpecifier>(AttachSound, hookCtx, context, false);
			}
			target.AttachSound = AttachSoundTemp;
			SoundSpecifier DetachSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(DetachSound, ref DetachSoundTemp, hookCtx, true, context))
			{
				DetachSoundTemp = serialization.CreateCopy<SoundSpecifier>(DetachSound, hookCtx, context, false);
			}
			target.DetachSound = DetachSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgWeaponModuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModuleComponent cast = (PubgWeaponModuleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModuleComponent cast = (PubgWeaponModuleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModuleComponent def = (PubgWeaponModuleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgWeaponModuleComponent Instantiate()
	{
		return new PubgWeaponModuleComponent();
	}
}
