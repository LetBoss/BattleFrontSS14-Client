using System;
using Content.Shared.Inventory;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.StepTrigger.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(StepTriggerImmuneSystem) })]
public sealed class ProtectedFromStepTriggersComponent : Component, IClothingSlots, ISerializationGenerated<ProtectedFromStepTriggersComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SlotFlags Slots { get; set; } = SlotFlags.FEET;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ProtectedFromStepTriggersComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ProtectedFromStepTriggersComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ProtectedFromStepTriggersComponent>(this, ref target, hookCtx, false, context))
		{
			SlotFlags SlotsTemp = SlotFlags.NONE;
			if (!serialization.TryCustomCopy<SlotFlags>(Slots, ref SlotsTemp, hookCtx, false, context))
			{
				SlotsTemp = Slots;
			}
			target.Slots = SlotsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ProtectedFromStepTriggersComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProtectedFromStepTriggersComponent cast = (ProtectedFromStepTriggersComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProtectedFromStepTriggersComponent cast = (ProtectedFromStepTriggersComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProtectedFromStepTriggersComponent def = (ProtectedFromStepTriggersComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ProtectedFromStepTriggersComponent Instantiate()
	{
		return new ProtectedFromStepTriggersComponent();
	}
}
