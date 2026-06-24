using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleAmmoLoaderDoAfterEvent : DoAfterEvent, ISerializationGenerated<VehicleAmmoLoaderDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string SlotId = string.Empty;

	public VehicleAmmoLoaderDoAfterEvent()
	{
	}

	public VehicleAmmoLoaderDoAfterEvent(string slotId)
	{
		SlotId = slotId;
	}

	public override DoAfterEvent Clone()
	{
		return new VehicleAmmoLoaderDoAfterEvent(SlotId);
	}

	public override bool IsDuplicate(DoAfterEvent other)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (other is VehicleAmmoLoaderDoAfterEvent loaderEvent && loaderEvent.SlotId == SlotId && other.User == base.User)
		{
			EntityUid? target = other.Target;
			EntityUid? target2 = base.Target;
			if (target.HasValue == target2.HasValue && (!target.HasValue || target.GetValueOrDefault() == target2.GetValueOrDefault()))
			{
				target2 = other.Used;
				target = base.Used;
				if (target2.HasValue != target.HasValue)
				{
					return false;
				}
				if (!target2.HasValue)
				{
					return true;
				}
				return target2.GetValueOrDefault() == target.GetValueOrDefault();
			}
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleAmmoLoaderDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleAmmoLoaderDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<VehicleAmmoLoaderDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			string SlotIdTemp = null;
			if (SlotId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SlotId, ref SlotIdTemp, hookCtx, false, context))
			{
				SlotIdTemp = SlotId;
			}
			target.SlotId = SlotIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleAmmoLoaderDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleAmmoLoaderDoAfterEvent cast = (VehicleAmmoLoaderDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleAmmoLoaderDoAfterEvent cast = (VehicleAmmoLoaderDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleAmmoLoaderDoAfterEvent Instantiate()
	{
		return new VehicleAmmoLoaderDoAfterEvent();
	}
}
