using System;
using Content.Shared.Whitelist;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Cargo.Prototypes;

[Serializable]
[DataDefinition]
[NetSerializable]
public readonly record struct CargoBountyItemEntry() : ISerializationGenerated<CargoBountyItemEntry>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist Whitelist { get; init; } = null;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist { get; init; } = null;

	[DataField(null, false, 1, false, false, null)]
	public int Amount { get; init; } = 1;

	[DataField(null, false, 1, false, false, null)]
	public LocId Name { get; init; } = LocId.op_Implicit(string.Empty);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CargoBountyItemEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<CargoBountyItemEntry>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (Whitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, true);
			}
		}
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		int AmountTemp = 0;
		if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
		{
			AmountTemp = Amount;
		}
		LocId NameTemp = default(LocId);
		if (!serialization.TryCustomCopy<LocId>(Name, ref NameTemp, hookCtx, false, context))
		{
			NameTemp = serialization.CreateCopy<LocId>(Name, hookCtx, context, false);
		}
		target = target with
		{
			Whitelist = WhitelistTemp,
			Blacklist = BlacklistTemp,
			Amount = AmountTemp,
			Name = NameTemp
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CargoBountyItemEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoBountyItemEntry cast = (CargoBountyItemEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CargoBountyItemEntry Instantiate()
	{
		return new CargoBountyItemEntry();
	}
}
