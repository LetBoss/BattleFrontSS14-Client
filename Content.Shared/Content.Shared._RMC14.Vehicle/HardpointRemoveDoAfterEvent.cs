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
public sealed class HardpointRemoveDoAfterEvent : DoAfterEvent, ISerializationGenerated<HardpointRemoveDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string SlotId = string.Empty;

	public HardpointRemoveDoAfterEvent()
	{
	}

	public HardpointRemoveDoAfterEvent(string slotId)
	{
		SlotId = slotId;
	}

	public override DoAfterEvent Clone()
	{
		return new HardpointRemoveDoAfterEvent(SlotId);
	}

	public override bool IsDuplicate(DoAfterEvent other)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (other is HardpointRemoveDoAfterEvent remove && remove.SlotId == SlotId && other.User == base.User)
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
	public void InternalCopy(ref HardpointRemoveDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HardpointRemoveDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<HardpointRemoveDoAfterEvent>(this, ref target, hookCtx, false, context))
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
	public void Copy(ref HardpointRemoveDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointRemoveDoAfterEvent cast = (HardpointRemoveDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointRemoveDoAfterEvent cast = (HardpointRemoveDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HardpointRemoveDoAfterEvent Instantiate()
	{
		return new HardpointRemoveDoAfterEvent();
	}
}
