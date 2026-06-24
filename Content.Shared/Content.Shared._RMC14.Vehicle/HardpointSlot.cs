using System;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class HardpointSlot : ISerializationGenerated<HardpointSlot>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Id { get; set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string HardpointType { get; set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HardpointSlotTypePrototype>? SlotType { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public string? CompatibilityId { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public string VisualLayer { get; set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public bool Required { get; set; } = true;

	[DataField(null, false, 1, false, false, null)]
	public float InsertDelay { get; set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float RemoveDelay { get; set; } = -1f;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HardpointSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<HardpointSlot>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string IdTemp = null;
		if (Id == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Id, ref IdTemp, hookCtx, false, context))
		{
			IdTemp = Id;
		}
		target.Id = IdTemp;
		string HardpointTypeTemp = null;
		if (HardpointType == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(HardpointType, ref HardpointTypeTemp, hookCtx, false, context))
		{
			HardpointTypeTemp = HardpointType;
		}
		target.HardpointType = HardpointTypeTemp;
		ProtoId<HardpointSlotTypePrototype>? SlotTypeTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<HardpointSlotTypePrototype>?>(SlotType, ref SlotTypeTemp, hookCtx, false, context))
		{
			SlotTypeTemp = serialization.CreateCopy<ProtoId<HardpointSlotTypePrototype>?>(SlotType, hookCtx, context, false);
		}
		target.SlotType = SlotTypeTemp;
		string CompatibilityIdTemp = null;
		if (!serialization.TryCustomCopy<string>(CompatibilityId, ref CompatibilityIdTemp, hookCtx, false, context))
		{
			CompatibilityIdTemp = CompatibilityId;
		}
		target.CompatibilityId = CompatibilityIdTemp;
		string VisualLayerTemp = null;
		if (VisualLayer == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(VisualLayer, ref VisualLayerTemp, hookCtx, false, context))
		{
			VisualLayerTemp = VisualLayer;
		}
		target.VisualLayer = VisualLayerTemp;
		bool RequiredTemp = false;
		if (!serialization.TryCustomCopy<bool>(Required, ref RequiredTemp, hookCtx, false, context))
		{
			RequiredTemp = Required;
		}
		target.Required = RequiredTemp;
		float InsertDelayTemp = 0f;
		if (!serialization.TryCustomCopy<float>(InsertDelay, ref InsertDelayTemp, hookCtx, false, context))
		{
			InsertDelayTemp = InsertDelay;
		}
		target.InsertDelay = InsertDelayTemp;
		float RemoveDelayTemp = 0f;
		if (!serialization.TryCustomCopy<float>(RemoveDelay, ref RemoveDelayTemp, hookCtx, false, context))
		{
			RemoveDelayTemp = RemoveDelay;
		}
		target.RemoveDelay = RemoveDelayTemp;
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
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HardpointSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointSlot cast = (HardpointSlot)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public HardpointSlot Instantiate()
	{
		return new HardpointSlot();
	}
}
