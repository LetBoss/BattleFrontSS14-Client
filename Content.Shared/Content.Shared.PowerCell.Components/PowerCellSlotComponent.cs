using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.PowerCell.Components;

[RegisterComponent]
public sealed class PowerCellSlotComponent : Component, ISerializationGenerated<PowerCellSlotComponent>, ISerializationGenerated
{
	[DataField("cellSlotId", false, 1, true, false, null)]
	public string CellSlotId = string.Empty;

	[DataField("fitsInCharger", false, 1, false, false, null)]
	public bool FitsInCharger = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PowerCellSlotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PowerCellSlotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PowerCellSlotComponent>(this, ref target, hookCtx, false, context))
		{
			string CellSlotIdTemp = null;
			if (CellSlotId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CellSlotId, ref CellSlotIdTemp, hookCtx, false, context))
			{
				CellSlotIdTemp = CellSlotId;
			}
			target.CellSlotId = CellSlotIdTemp;
			bool FitsInChargerTemp = false;
			if (!serialization.TryCustomCopy<bool>(FitsInCharger, ref FitsInChargerTemp, hookCtx, false, context))
			{
				FitsInChargerTemp = FitsInCharger;
			}
			target.FitsInCharger = FitsInChargerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PowerCellSlotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerCellSlotComponent cast = (PowerCellSlotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerCellSlotComponent cast = (PowerCellSlotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerCellSlotComponent def = (PowerCellSlotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PowerCellSlotComponent Instantiate()
	{
		return new PowerCellSlotComponent();
	}
}
