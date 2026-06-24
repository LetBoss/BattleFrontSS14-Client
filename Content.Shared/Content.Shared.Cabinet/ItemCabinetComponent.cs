using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Cabinet;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ItemCabinetSystem) })]
public sealed class ItemCabinetComponent : Component, ISerializationGenerated<ItemCabinetComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Slot = "ItemCabinet";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemCabinetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemCabinetComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemCabinetComponent>(this, ref target, hookCtx, false, context))
		{
			string SlotTemp = null;
			if (Slot == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Slot, ref SlotTemp, hookCtx, false, context))
			{
				SlotTemp = Slot;
			}
			target.Slot = SlotTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemCabinetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemCabinetComponent cast = (ItemCabinetComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemCabinetComponent cast = (ItemCabinetComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemCabinetComponent def = (ItemCabinetComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemCabinetComponent Instantiate()
	{
		return new ItemCabinetComponent();
	}
}
