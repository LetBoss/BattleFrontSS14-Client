using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Containers.ItemSlots;

[Serializable]
[DataDefinition]
[Access(new Type[] { typeof(ItemSlotsSystem) })]
[NetSerializable]
public sealed class ItemSlot : ISerializationGenerated<ItemSlot>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? InsertSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? EjectSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg", (AudioParams?)null);

	[DataField(null, true, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public string Name = string.Empty;

	[NonSerialized]
	[DataField(null, true, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	[Access(/*Could not decode attribute arguments.*/)]
	public string? StartingItem;

	[DataField(null, true, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Locked;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool DisableEject;

	[DataField(null, false, 1, false, false, null)]
	public bool InsertOnInteract = true;

	[DataField(null, false, 1, false, false, null)]
	public bool EjectOnInteract;

	[DataField(null, false, 1, false, false, null)]
	public bool EjectOnUse;

	[DataField(null, false, 1, false, false, null)]
	public string? InsertVerbText;

	[DataField(null, false, 1, false, false, null)]
	public string? EjectVerbText;

	[NonSerialized]
	[ViewVariables]
	public ContainerSlot? ContainerSlot;

	[NonSerialized]
	[DataField(null, false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public bool EjectOnDeconstruct = true;

	[NonSerialized]
	[DataField(null, false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public bool EjectOnBreak;

	[DataField(null, false, 1, false, false, null)]
	public LocId? WhitelistFailPopup;

	[DataField(null, false, 1, false, false, null)]
	public LocId? LockedFailPopup;

	[DataField(null, false, 1, false, false, null)]
	public LocId? InsertSuccessPopup;

	[DataField(null, false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public bool Swap = true;

	[DataField(null, false, 1, false, false, null)]
	public int Priority;

	[NonSerialized]
	public bool Local = true;

	public string? ID => ((BaseContainer)(ContainerSlot?)).ID;

	public bool HasItem
	{
		get
		{
			ContainerSlot? containerSlot = ContainerSlot;
			if (containerSlot == null)
			{
				return false;
			}
			return containerSlot.ContainedEntity.HasValue;
		}
	}

	public EntityUid? Item
	{
		get
		{
			ContainerSlot? containerSlot = ContainerSlot;
			if (containerSlot == null)
			{
				return null;
			}
			return containerSlot.ContainedEntity;
		}
	}

	public ItemSlot()
	{
	}//IL_000f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0019: Expected O, but got Unknown
	//IL_0028: Unknown result type (might be due to invalid IL or missing references)
	//IL_0032: Expected O, but got Unknown


	public ItemSlot(ItemSlot other)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		CopyFrom(other);
	}

	public void CopyFrom(ItemSlot other)
	{
		Whitelist = other.Whitelist;
		InsertSound = other.InsertSound;
		EjectSound = other.EjectSound;
		Name = other.Name;
		Locked = other.Locked;
		InsertOnInteract = other.InsertOnInteract;
		EjectOnInteract = other.EjectOnInteract;
		EjectOnUse = other.EjectOnUse;
		InsertVerbText = other.InsertVerbText;
		EjectVerbText = other.EjectVerbText;
		WhitelistFailPopup = other.WhitelistFailPopup;
		LockedFailPopup = other.LockedFailPopup;
		InsertSuccessPopup = other.InsertSuccessPopup;
		Swap = other.Swap;
		Priority = other.Priority;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<ItemSlot>(this, ref target, hookCtx, false, context))
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
		target.Whitelist = WhitelistTemp;
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		target.Blacklist = BlacklistTemp;
		SoundSpecifier InsertSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(InsertSound, ref InsertSoundTemp, hookCtx, true, context))
		{
			InsertSoundTemp = serialization.CreateCopy<SoundSpecifier>(InsertSound, hookCtx, context, false);
		}
		target.InsertSound = InsertSoundTemp;
		SoundSpecifier EjectSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(EjectSound, ref EjectSoundTemp, hookCtx, true, context))
		{
			EjectSoundTemp = serialization.CreateCopy<SoundSpecifier>(EjectSound, hookCtx, context, false);
		}
		target.EjectSound = EjectSoundTemp;
		string NameTemp = null;
		if (Name == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
		{
			NameTemp = Name;
		}
		target.Name = NameTemp;
		string StartingItemTemp = null;
		if (!serialization.TryCustomCopy<string>(StartingItem, ref StartingItemTemp, hookCtx, false, context))
		{
			StartingItemTemp = StartingItem;
		}
		target.StartingItem = StartingItemTemp;
		bool LockedTemp = false;
		if (!serialization.TryCustomCopy<bool>(Locked, ref LockedTemp, hookCtx, false, context))
		{
			LockedTemp = Locked;
		}
		target.Locked = LockedTemp;
		bool DisableEjectTemp = false;
		if (!serialization.TryCustomCopy<bool>(DisableEject, ref DisableEjectTemp, hookCtx, false, context))
		{
			DisableEjectTemp = DisableEject;
		}
		target.DisableEject = DisableEjectTemp;
		bool InsertOnInteractTemp = false;
		if (!serialization.TryCustomCopy<bool>(InsertOnInteract, ref InsertOnInteractTemp, hookCtx, false, context))
		{
			InsertOnInteractTemp = InsertOnInteract;
		}
		target.InsertOnInteract = InsertOnInteractTemp;
		bool EjectOnInteractTemp = false;
		if (!serialization.TryCustomCopy<bool>(EjectOnInteract, ref EjectOnInteractTemp, hookCtx, false, context))
		{
			EjectOnInteractTemp = EjectOnInteract;
		}
		target.EjectOnInteract = EjectOnInteractTemp;
		bool EjectOnUseTemp = false;
		if (!serialization.TryCustomCopy<bool>(EjectOnUse, ref EjectOnUseTemp, hookCtx, false, context))
		{
			EjectOnUseTemp = EjectOnUse;
		}
		target.EjectOnUse = EjectOnUseTemp;
		string InsertVerbTextTemp = null;
		if (!serialization.TryCustomCopy<string>(InsertVerbText, ref InsertVerbTextTemp, hookCtx, false, context))
		{
			InsertVerbTextTemp = InsertVerbText;
		}
		target.InsertVerbText = InsertVerbTextTemp;
		string EjectVerbTextTemp = null;
		if (!serialization.TryCustomCopy<string>(EjectVerbText, ref EjectVerbTextTemp, hookCtx, false, context))
		{
			EjectVerbTextTemp = EjectVerbText;
		}
		target.EjectVerbText = EjectVerbTextTemp;
		bool EjectOnDeconstructTemp = false;
		if (!serialization.TryCustomCopy<bool>(EjectOnDeconstruct, ref EjectOnDeconstructTemp, hookCtx, false, context))
		{
			EjectOnDeconstructTemp = EjectOnDeconstruct;
		}
		target.EjectOnDeconstruct = EjectOnDeconstructTemp;
		bool EjectOnBreakTemp = false;
		if (!serialization.TryCustomCopy<bool>(EjectOnBreak, ref EjectOnBreakTemp, hookCtx, false, context))
		{
			EjectOnBreakTemp = EjectOnBreak;
		}
		target.EjectOnBreak = EjectOnBreakTemp;
		LocId? WhitelistFailPopupTemp = null;
		if (!serialization.TryCustomCopy<LocId?>(WhitelistFailPopup, ref WhitelistFailPopupTemp, hookCtx, false, context))
		{
			WhitelistFailPopupTemp = serialization.CreateCopy<LocId?>(WhitelistFailPopup, hookCtx, context, false);
		}
		target.WhitelistFailPopup = WhitelistFailPopupTemp;
		LocId? LockedFailPopupTemp = null;
		if (!serialization.TryCustomCopy<LocId?>(LockedFailPopup, ref LockedFailPopupTemp, hookCtx, false, context))
		{
			LockedFailPopupTemp = serialization.CreateCopy<LocId?>(LockedFailPopup, hookCtx, context, false);
		}
		target.LockedFailPopup = LockedFailPopupTemp;
		LocId? InsertSuccessPopupTemp = null;
		if (!serialization.TryCustomCopy<LocId?>(InsertSuccessPopup, ref InsertSuccessPopupTemp, hookCtx, false, context))
		{
			InsertSuccessPopupTemp = serialization.CreateCopy<LocId?>(InsertSuccessPopup, hookCtx, context, false);
		}
		target.InsertSuccessPopup = InsertSuccessPopupTemp;
		bool SwapTemp = false;
		if (!serialization.TryCustomCopy<bool>(Swap, ref SwapTemp, hookCtx, false, context))
		{
			SwapTemp = Swap;
		}
		target.Swap = SwapTemp;
		int PriorityTemp = 0;
		if (!serialization.TryCustomCopy<int>(Priority, ref PriorityTemp, hookCtx, false, context))
		{
			PriorityTemp = Priority;
		}
		target.Priority = PriorityTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlot cast = (ItemSlot)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ItemSlot Instantiate()
	{
		return new ItemSlot();
	}
}
