using System;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.PDA;

[RegisterComponent]
[NetworkedComponent]
public sealed class PdaComponent : Component, ISerializationGenerated<PdaComponent>, ISerializationGenerated
{
	public const string PdaIdSlotId = "PDA-id";

	public const string PdaPenSlotId = "PDA-pen";

	public const string PdaPaiSlotId = "PDA-pai";

	[DataField("idSlot", false, 1, false, false, null)]
	public ItemSlot IdSlot = new ItemSlot();

	[DataField("penSlot", false, 1, false, false, null)]
	public ItemSlot PenSlot = new ItemSlot();

	[DataField("paiSlot", false, 1, false, false, null)]
	public ItemSlot PaiSlot = new ItemSlot();

	[DataField("id", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string? IdCard;

	[ViewVariables]
	public EntityUid? ContainedId;

	[ViewVariables]
	public bool FlashlightOn;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? OwnerName;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntityUid? PdaOwner;

	[ViewVariables]
	public string? StationName;

	[ViewVariables]
	public string? StationAlertLevel;

	[ViewVariables]
	public Color StationAlertColor = Color.White;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PdaComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PdaComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<PdaComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ItemSlot IdSlotTemp = null;
		if (IdSlot == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(IdSlot, ref IdSlotTemp, hookCtx, false, context))
		{
			if (IdSlot == null)
			{
				IdSlotTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(IdSlot, ref IdSlotTemp, hookCtx, context, true);
			}
		}
		target.IdSlot = IdSlotTemp;
		ItemSlot PenSlotTemp = null;
		if (PenSlot == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(PenSlot, ref PenSlotTemp, hookCtx, false, context))
		{
			if (PenSlot == null)
			{
				PenSlotTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(PenSlot, ref PenSlotTemp, hookCtx, context, true);
			}
		}
		target.PenSlot = PenSlotTemp;
		ItemSlot PaiSlotTemp = null;
		if (PaiSlot == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(PaiSlot, ref PaiSlotTemp, hookCtx, false, context))
		{
			if (PaiSlot == null)
			{
				PaiSlotTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(PaiSlot, ref PaiSlotTemp, hookCtx, context, true);
			}
		}
		target.PaiSlot = PaiSlotTemp;
		string IdCardTemp = null;
		if (!serialization.TryCustomCopy<string>(IdCard, ref IdCardTemp, hookCtx, false, context))
		{
			IdCardTemp = IdCard;
		}
		target.IdCard = IdCardTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PdaComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PdaComponent cast = (PdaComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PdaComponent cast = (PdaComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PdaComponent def = (PdaComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PdaComponent Instantiate()
	{
		return new PdaComponent();
	}
}
