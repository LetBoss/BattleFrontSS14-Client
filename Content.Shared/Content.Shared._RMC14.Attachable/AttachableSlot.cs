using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[DataDefinition]
[NetSerializable]
public struct AttachableSlot : ISerializationGenerated<AttachableSlot>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Locked = false;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist = null;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<AttachableComponent>? StartingAttachable = null;

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId<AttachableComponent>>? Random = null;

	[DataField(null, false, 1, false, false, null)]
	public float RandomChance = 1f;

	public AttachableSlot()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AttachableSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (serialization.TryCustomCopy<AttachableSlot>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		bool LockedTemp = false;
		if (!serialization.TryCustomCopy<bool>(Locked, ref LockedTemp, hookCtx, false, context))
		{
			LockedTemp = Locked;
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
		EntProtoId<AttachableComponent>? StartingAttachableTemp = null;
		if (!serialization.TryCustomCopy<EntProtoId<AttachableComponent>?>(StartingAttachable, ref StartingAttachableTemp, hookCtx, false, context))
		{
			StartingAttachableTemp = serialization.CreateCopy<EntProtoId<AttachableComponent>?>(StartingAttachable, hookCtx, context, false);
		}
		List<EntProtoId<AttachableComponent>> RandomTemp = null;
		if (!serialization.TryCustomCopy<List<EntProtoId<AttachableComponent>>>(Random, ref RandomTemp, hookCtx, true, context))
		{
			RandomTemp = serialization.CreateCopy<List<EntProtoId<AttachableComponent>>>(Random, hookCtx, context, false);
		}
		float RandomChanceTemp = 0f;
		if (!serialization.TryCustomCopy<float>(RandomChance, ref RandomChanceTemp, hookCtx, false, context))
		{
			RandomChanceTemp = RandomChance;
		}
		AttachableSlot attachableSlot = target;
		attachableSlot.Locked = LockedTemp;
		attachableSlot.Whitelist = WhitelistTemp;
		attachableSlot.StartingAttachable = StartingAttachableTemp;
		attachableSlot.Random = RandomTemp;
		attachableSlot.RandomChance = RandomChanceTemp;
		target = attachableSlot;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AttachableSlot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableSlot cast = (AttachableSlot)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AttachableSlot Instantiate()
	{
		return new AttachableSlot();
	}
}
