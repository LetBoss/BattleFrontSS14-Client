using System;
using System.Collections.Generic;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.CartridgeLoader;

[RegisterComponent]
[NetworkedComponent]
public sealed class CartridgeLoaderComponent : Component, ISerializationGenerated<CartridgeLoaderComponent>, ISerializationGenerated
{
	public const string CartridgeSlotId = "Cartridge-Slot";

	[DataField(null, false, 1, false, false, null)]
	public ItemSlot CartridgeSlot = new ItemSlot();

	[DataField("preinstalled", false, 1, false, false, null)]
	public List<string> PreinstalledPrograms = new List<string>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntityUid? ActiveProgram;

	[ViewVariables]
	public readonly List<EntityUid> BackgroundPrograms = new List<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	public int DiskSpace = 8;

	[DataField(null, false, 1, false, false, null)]
	public bool NotificationsEnabled = true;

	[DataField(null, false, 1, true, false, null)]
	public Enum UiKey;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CartridgeLoaderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CartridgeLoaderComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<CartridgeLoaderComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ItemSlot CartridgeSlotTemp = null;
		if (CartridgeSlot == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(CartridgeSlot, ref CartridgeSlotTemp, hookCtx, false, context))
		{
			if (CartridgeSlot == null)
			{
				CartridgeSlotTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(CartridgeSlot, ref CartridgeSlotTemp, hookCtx, context, true);
			}
		}
		target.CartridgeSlot = CartridgeSlotTemp;
		List<string> PreinstalledProgramsTemp = null;
		if (PreinstalledPrograms == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(PreinstalledPrograms, ref PreinstalledProgramsTemp, hookCtx, true, context))
		{
			PreinstalledProgramsTemp = serialization.CreateCopy<List<string>>(PreinstalledPrograms, hookCtx, context, false);
		}
		target.PreinstalledPrograms = PreinstalledProgramsTemp;
		int DiskSpaceTemp = 0;
		if (!serialization.TryCustomCopy<int>(DiskSpace, ref DiskSpaceTemp, hookCtx, false, context))
		{
			DiskSpaceTemp = DiskSpace;
		}
		target.DiskSpace = DiskSpaceTemp;
		bool NotificationsEnabledTemp = false;
		if (!serialization.TryCustomCopy<bool>(NotificationsEnabled, ref NotificationsEnabledTemp, hookCtx, false, context))
		{
			NotificationsEnabledTemp = NotificationsEnabled;
		}
		target.NotificationsEnabled = NotificationsEnabledTemp;
		Enum UiKeyTemp = null;
		if (UiKey == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Enum>(UiKey, ref UiKeyTemp, hookCtx, true, context))
		{
			UiKeyTemp = UiKey;
		}
		target.UiKey = UiKeyTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CartridgeLoaderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CartridgeLoaderComponent cast = (CartridgeLoaderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CartridgeLoaderComponent cast = (CartridgeLoaderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CartridgeLoaderComponent def = (CartridgeLoaderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CartridgeLoaderComponent Instantiate()
	{
		return new CartridgeLoaderComponent();
	}
}
