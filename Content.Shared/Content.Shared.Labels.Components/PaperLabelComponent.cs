using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Labels.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Labels.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(LabelSystem) })]
public sealed class PaperLabelComponent : Component, ISerializationGenerated<PaperLabelComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ItemSlot LabelSlot = new ItemSlot();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PaperLabelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PaperLabelComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<PaperLabelComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ItemSlot LabelSlotTemp = null;
		if (LabelSlot == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(LabelSlot, ref LabelSlotTemp, hookCtx, false, context))
		{
			if (LabelSlot == null)
			{
				LabelSlotTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(LabelSlot, ref LabelSlotTemp, hookCtx, context, true);
			}
		}
		target.LabelSlot = LabelSlotTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PaperLabelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperLabelComponent cast = (PaperLabelComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperLabelComponent cast = (PaperLabelComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperLabelComponent def = (PaperLabelComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PaperLabelComponent Instantiate()
	{
		return new PaperLabelComponent();
	}
}
