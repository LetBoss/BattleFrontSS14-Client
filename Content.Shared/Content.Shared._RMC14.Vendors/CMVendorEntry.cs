using System;
using System.Collections.Generic;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Vendors;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed record CMVendorEntry : ISerializationGenerated<CMVendorEntry>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Id;

	[DataField(null, false, 1, false, false, null)]
	public string? Name;

	[DataField(null, false, 1, false, false, null)]
	public int? Amount;

	[DataField(null, false, 1, false, false, null)]
	public int? OnlinePerStock;

	[DataField(null, false, 1, false, false, null)]
	public int? PerPlayerQuota;

	[DataField(null, false, 1, false, false, null)]
	public int? Points;

	[DataField(null, false, 1, false, false, null)]
	public int Spawn = 1;

	[DataField(null, false, 1, false, false, null)]
	public bool Recommended;

	[DataField(null, false, 1, false, false, null)]
	public int? Multiplier;

	[DataField(null, false, 1, false, false, null)]
	public int? Max;

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> LinkedEntries = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public EntProtoId? Box;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public int? BoxAmount;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public int? BoxSlots;

	[DataField(null, false, 1, false, false, null)]
	public LocId? GiveSquadRoleName;

	[DataField(null, false, 1, false, false, null)]
	public bool IsAppendSquadRoleName;

	[DataField(null, false, 1, false, false, null)]
	public LocId? GivePrefix;

	[DataField(null, false, 1, false, false, null)]
	public bool IsAppendPrefix;

	[DataField(null, false, 1, false, false, null)]
	public Rsi? GiveIcon;

	[DataField(null, false, 1, false, false, null)]
	public Rsi? GiveMapBlip;

	[DataField(null, false, 1, false, false, null)]
	public SlotFlags? ReplaceSlot;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CMVendorEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<CMVendorEntry>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntProtoId IdTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(Id, ref IdTemp, hookCtx, false, context))
		{
			IdTemp = serialization.CreateCopy<EntProtoId>(Id, hookCtx, context, false);
		}
		target.Id = IdTemp;
		string NameTemp = null;
		if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
		{
			NameTemp = Name;
		}
		target.Name = NameTemp;
		int? AmountTemp = null;
		if (!serialization.TryCustomCopy<int?>(Amount, ref AmountTemp, hookCtx, false, context))
		{
			AmountTemp = Amount;
		}
		target.Amount = AmountTemp;
		int? OnlinePerStockTemp = null;
		if (!serialization.TryCustomCopy<int?>(OnlinePerStock, ref OnlinePerStockTemp, hookCtx, false, context))
		{
			OnlinePerStockTemp = OnlinePerStock;
		}
		target.OnlinePerStock = OnlinePerStockTemp;
		int? PerPlayerQuotaTemp = null;
		if (!serialization.TryCustomCopy<int?>(PerPlayerQuota, ref PerPlayerQuotaTemp, hookCtx, false, context))
		{
			PerPlayerQuotaTemp = PerPlayerQuota;
		}
		target.PerPlayerQuota = PerPlayerQuotaTemp;
		int? PointsTemp = null;
		if (!serialization.TryCustomCopy<int?>(Points, ref PointsTemp, hookCtx, false, context))
		{
			PointsTemp = Points;
		}
		target.Points = PointsTemp;
		int SpawnTemp = 0;
		if (!serialization.TryCustomCopy<int>(Spawn, ref SpawnTemp, hookCtx, false, context))
		{
			SpawnTemp = Spawn;
		}
		target.Spawn = SpawnTemp;
		bool RecommendedTemp = false;
		if (!serialization.TryCustomCopy<bool>(Recommended, ref RecommendedTemp, hookCtx, false, context))
		{
			RecommendedTemp = Recommended;
		}
		target.Recommended = RecommendedTemp;
		int? MultiplierTemp = null;
		if (!serialization.TryCustomCopy<int?>(Multiplier, ref MultiplierTemp, hookCtx, false, context))
		{
			MultiplierTemp = Multiplier;
		}
		target.Multiplier = MultiplierTemp;
		int? MaxTemp = null;
		if (!serialization.TryCustomCopy<int?>(Max, ref MaxTemp, hookCtx, false, context))
		{
			MaxTemp = Max;
		}
		target.Max = MaxTemp;
		List<EntProtoId> LinkedEntriesTemp = null;
		if (LinkedEntries == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<EntProtoId>>(LinkedEntries, ref LinkedEntriesTemp, hookCtx, true, context))
		{
			LinkedEntriesTemp = serialization.CreateCopy<List<EntProtoId>>(LinkedEntries, hookCtx, context, false);
		}
		target.LinkedEntries = LinkedEntriesTemp;
		EntProtoId? BoxTemp = null;
		if (!serialization.TryCustomCopy<EntProtoId?>(Box, ref BoxTemp, hookCtx, false, context))
		{
			BoxTemp = serialization.CreateCopy<EntProtoId?>(Box, hookCtx, context, false);
		}
		target.Box = BoxTemp;
		int? BoxAmountTemp = null;
		if (!serialization.TryCustomCopy<int?>(BoxAmount, ref BoxAmountTemp, hookCtx, false, context))
		{
			BoxAmountTemp = BoxAmount;
		}
		target.BoxAmount = BoxAmountTemp;
		int? BoxSlotsTemp = null;
		if (!serialization.TryCustomCopy<int?>(BoxSlots, ref BoxSlotsTemp, hookCtx, false, context))
		{
			BoxSlotsTemp = BoxSlots;
		}
		target.BoxSlots = BoxSlotsTemp;
		LocId? GiveSquadRoleNameTemp = null;
		if (!serialization.TryCustomCopy<LocId?>(GiveSquadRoleName, ref GiveSquadRoleNameTemp, hookCtx, false, context))
		{
			GiveSquadRoleNameTemp = serialization.CreateCopy<LocId?>(GiveSquadRoleName, hookCtx, context, false);
		}
		target.GiveSquadRoleName = GiveSquadRoleNameTemp;
		bool IsAppendSquadRoleNameTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsAppendSquadRoleName, ref IsAppendSquadRoleNameTemp, hookCtx, false, context))
		{
			IsAppendSquadRoleNameTemp = IsAppendSquadRoleName;
		}
		target.IsAppendSquadRoleName = IsAppendSquadRoleNameTemp;
		LocId? GivePrefixTemp = null;
		if (!serialization.TryCustomCopy<LocId?>(GivePrefix, ref GivePrefixTemp, hookCtx, false, context))
		{
			GivePrefixTemp = serialization.CreateCopy<LocId?>(GivePrefix, hookCtx, context, false);
		}
		target.GivePrefix = GivePrefixTemp;
		bool IsAppendPrefixTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsAppendPrefix, ref IsAppendPrefixTemp, hookCtx, false, context))
		{
			IsAppendPrefixTemp = IsAppendPrefix;
		}
		target.IsAppendPrefix = IsAppendPrefixTemp;
		Rsi GiveIconTemp = null;
		if (!serialization.TryCustomCopy<Rsi>(GiveIcon, ref GiveIconTemp, hookCtx, false, context))
		{
			if (GiveIcon == null)
			{
				GiveIconTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(GiveIcon, ref GiveIconTemp, hookCtx, context, false);
			}
		}
		target.GiveIcon = GiveIconTemp;
		Rsi GiveMapBlipTemp = null;
		if (!serialization.TryCustomCopy<Rsi>(GiveMapBlip, ref GiveMapBlipTemp, hookCtx, false, context))
		{
			if (GiveMapBlip == null)
			{
				GiveMapBlipTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(GiveMapBlip, ref GiveMapBlipTemp, hookCtx, context, false);
			}
		}
		target.GiveMapBlip = GiveMapBlipTemp;
		SlotFlags? ReplaceSlotTemp = null;
		if (!serialization.TryCustomCopy<SlotFlags?>(ReplaceSlot, ref ReplaceSlotTemp, hookCtx, false, context))
		{
			ReplaceSlotTemp = ReplaceSlot;
		}
		target.ReplaceSlot = ReplaceSlotTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CMVendorEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMVendorEntry cast = (CMVendorEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CMVendorEntry Instantiate()
	{
		return new CMVendorEntry();
	}
}
