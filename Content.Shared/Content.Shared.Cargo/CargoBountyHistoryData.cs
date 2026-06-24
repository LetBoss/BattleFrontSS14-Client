using System;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Cargo;

[Serializable]
[DataDefinition]
[NetSerializable]
public readonly record struct CargoBountyHistoryData : ISerializationGenerated<CargoBountyHistoryData>, ISerializationGenerated
{
	public enum BountyResult
	{
		Completed,
		Skipped
	}

	[DataField(null, false, 1, false, false, null)]
	public string Id { get; init; }

	[DataField(null, false, 1, false, false, null)]
	public BountyResult Result { get; init; }

	[DataField(null, false, 1, false, false, null)]
	public string? ActorName { get; init; }

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Timestamp { get; init; }

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<CargoBountyPrototype> Bounty { get; init; }

	public CargoBountyHistoryData(CargoBountyData bounty, BountyResult result, TimeSpan timestamp, string? actorName)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Id = string.Empty;
		Result = BountyResult.Completed;
		ActorName = null;
		Timestamp = TimeSpan.MinValue;
		Bounty = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
		Bounty = bounty.Bounty;
		Result = result;
		Id = bounty.Id;
		ActorName = actorName;
		Timestamp = timestamp;
	}

	public CargoBountyHistoryData()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Id = string.Empty;
		Result = BountyResult.Completed;
		ActorName = null;
		Timestamp = TimeSpan.MinValue;
		Bounty = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CargoBountyHistoryData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CargoBountyHistoryData>(this, ref target, hookCtx, false, context))
		{
			string IdTemp = null;
			if (Id == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = Id;
			}
			BountyResult ResultTemp = BountyResult.Completed;
			if (!serialization.TryCustomCopy<BountyResult>(Result, ref ResultTemp, hookCtx, false, context))
			{
				ResultTemp = Result;
			}
			string ActorNameTemp = null;
			if (!serialization.TryCustomCopy<string>(ActorName, ref ActorNameTemp, hookCtx, false, context))
			{
				ActorNameTemp = ActorName;
			}
			TimeSpan TimestampTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Timestamp, ref TimestampTemp, hookCtx, false, context))
			{
				TimestampTemp = serialization.CreateCopy<TimeSpan>(Timestamp, hookCtx, context, false);
			}
			ProtoId<CargoBountyPrototype> BountyTemp = default(ProtoId<CargoBountyPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<CargoBountyPrototype>>(Bounty, ref BountyTemp, hookCtx, false, context))
			{
				BountyTemp = serialization.CreateCopy<ProtoId<CargoBountyPrototype>>(Bounty, hookCtx, context, false);
			}
			target = target with
			{
				Id = IdTemp,
				Result = ResultTemp,
				ActorName = ActorNameTemp,
				Timestamp = TimestampTemp,
				Bounty = BountyTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CargoBountyHistoryData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoBountyHistoryData cast = (CargoBountyHistoryData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CargoBountyHistoryData Instantiate()
	{
		return new CargoBountyHistoryData();
	}
}
