using System;
using Content.Shared.Inventory;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Armor;

[RegisterComponent]
public sealed class PubgArmorRepairKitComponent : Component, ISerializationGenerated<PubgArmorRepairKitComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float RepairAmount = 30f;

	[DataField(null, false, 1, false, false, null)]
	public float Delay = 2f;

	[DataField(null, false, 1, false, false, null)]
	public bool DeleteOnEmpty = true;

	[DataField(null, false, 1, false, false, null)]
	public SlotFlags BlockedSlots = SlotFlags.HEAD | SlotFlags.OUTERCLOTHING;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? RepairSound;

	[DataField(null, false, 1, false, false, null)]
	public float RepairSoundVolume;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgArmorRepairKitComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgArmorRepairKitComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgArmorRepairKitComponent>(this, ref target, hookCtx, false, context))
		{
			float RepairAmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairAmount, ref RepairAmountTemp, hookCtx, false, context))
			{
				RepairAmountTemp = RepairAmount;
			}
			target.RepairAmount = RepairAmountTemp;
			float DelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = Delay;
			}
			target.Delay = DelayTemp;
			bool DeleteOnEmptyTemp = false;
			if (!serialization.TryCustomCopy<bool>(DeleteOnEmpty, ref DeleteOnEmptyTemp, hookCtx, false, context))
			{
				DeleteOnEmptyTemp = DeleteOnEmpty;
			}
			target.DeleteOnEmpty = DeleteOnEmptyTemp;
			SlotFlags BlockedSlotsTemp = SlotFlags.NONE;
			if (!serialization.TryCustomCopy<SlotFlags>(BlockedSlots, ref BlockedSlotsTemp, hookCtx, false, context))
			{
				BlockedSlotsTemp = BlockedSlots;
			}
			target.BlockedSlots = BlockedSlotsTemp;
			SoundSpecifier RepairSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(RepairSound, ref RepairSoundTemp, hookCtx, true, context))
			{
				RepairSoundTemp = serialization.CreateCopy<SoundSpecifier>(RepairSound, hookCtx, context, false);
			}
			target.RepairSound = RepairSoundTemp;
			float RepairSoundVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairSoundVolume, ref RepairSoundVolumeTemp, hookCtx, false, context))
			{
				RepairSoundVolumeTemp = RepairSoundVolume;
			}
			target.RepairSoundVolume = RepairSoundVolumeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgArmorRepairKitComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgArmorRepairKitComponent cast = (PubgArmorRepairKitComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgArmorRepairKitComponent cast = (PubgArmorRepairKitComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgArmorRepairKitComponent def = (PubgArmorRepairKitComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgArmorRepairKitComponent Instantiate()
	{
		return new PubgArmorRepairKitComponent();
	}
}
