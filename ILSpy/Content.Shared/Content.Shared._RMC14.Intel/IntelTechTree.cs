using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Intel.Tech;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Intel;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class IntelTechTree : ISerializationGenerated<IntelTechTree>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Points;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 TotalEarned;

	[DataField(null, false, 1, false, false, null)]
	public IntelObjectiveAmount Documents;

	[DataField(null, false, 1, false, false, null)]
	public IntelObjectiveAmount UploadData;

	[DataField(null, false, 1, false, false, null)]
	public IntelObjectiveAmount RetrieveItems;

	[DataField(null, false, 1, false, false, null)]
	public IntelObjectiveAmount Miscellaneous;

	[DataField(null, false, 1, false, false, null)]
	public int AnalyzeChemicals;

	[DataField(null, false, 1, false, false, null)]
	public int RescueSurvivors;

	[DataField(null, false, 1, false, false, null)]
	public int RecoverCorpses;

	[DataField(null, false, 1, false, false, null)]
	public bool ColonyCommunications;

	[DataField(null, false, 1, false, false, null)]
	public bool ColonyPower;

	[DataField(null, false, 1, false, false, null)]
	public int Tier;

	[DataField(null, false, 1, false, false, null)]
	public List<List<TechOption>> Options = new List<List<TechOption>>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<LocId, Dictionary<NetEntity, string>> Clues = new Dictionary<LocId, Dictionary<NetEntity, string>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IntelTechTree target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<IntelTechTree>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 PointsTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Points, ref PointsTemp, hookCtx, false, context))
			{
				PointsTemp = serialization.CreateCopy<FixedPoint2>(Points, hookCtx, context, false);
			}
			target.Points = PointsTemp;
			FixedPoint2 TotalEarnedTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(TotalEarned, ref TotalEarnedTemp, hookCtx, false, context))
			{
				TotalEarnedTemp = serialization.CreateCopy<FixedPoint2>(TotalEarned, hookCtx, context, false);
			}
			target.TotalEarned = TotalEarnedTemp;
			IntelObjectiveAmount DocumentsTemp = default(IntelObjectiveAmount);
			if (!serialization.TryCustomCopy<IntelObjectiveAmount>(Documents, ref DocumentsTemp, hookCtx, false, context))
			{
				DocumentsTemp = serialization.CreateCopy<IntelObjectiveAmount>(Documents, hookCtx, context, false);
			}
			target.Documents = DocumentsTemp;
			IntelObjectiveAmount UploadDataTemp = default(IntelObjectiveAmount);
			if (!serialization.TryCustomCopy<IntelObjectiveAmount>(UploadData, ref UploadDataTemp, hookCtx, false, context))
			{
				UploadDataTemp = serialization.CreateCopy<IntelObjectiveAmount>(UploadData, hookCtx, context, false);
			}
			target.UploadData = UploadDataTemp;
			IntelObjectiveAmount RetrieveItemsTemp = default(IntelObjectiveAmount);
			if (!serialization.TryCustomCopy<IntelObjectiveAmount>(RetrieveItems, ref RetrieveItemsTemp, hookCtx, false, context))
			{
				RetrieveItemsTemp = serialization.CreateCopy<IntelObjectiveAmount>(RetrieveItems, hookCtx, context, false);
			}
			target.RetrieveItems = RetrieveItemsTemp;
			IntelObjectiveAmount MiscellaneousTemp = default(IntelObjectiveAmount);
			if (!serialization.TryCustomCopy<IntelObjectiveAmount>(Miscellaneous, ref MiscellaneousTemp, hookCtx, false, context))
			{
				MiscellaneousTemp = serialization.CreateCopy<IntelObjectiveAmount>(Miscellaneous, hookCtx, context, false);
			}
			target.Miscellaneous = MiscellaneousTemp;
			int AnalyzeChemicalsTemp = 0;
			if (!serialization.TryCustomCopy<int>(AnalyzeChemicals, ref AnalyzeChemicalsTemp, hookCtx, false, context))
			{
				AnalyzeChemicalsTemp = AnalyzeChemicals;
			}
			target.AnalyzeChemicals = AnalyzeChemicalsTemp;
			int RescueSurvivorsTemp = 0;
			if (!serialization.TryCustomCopy<int>(RescueSurvivors, ref RescueSurvivorsTemp, hookCtx, false, context))
			{
				RescueSurvivorsTemp = RescueSurvivors;
			}
			target.RescueSurvivors = RescueSurvivorsTemp;
			int RecoverCorpsesTemp = 0;
			if (!serialization.TryCustomCopy<int>(RecoverCorpses, ref RecoverCorpsesTemp, hookCtx, false, context))
			{
				RecoverCorpsesTemp = RecoverCorpses;
			}
			target.RecoverCorpses = RecoverCorpsesTemp;
			bool ColonyCommunicationsTemp = false;
			if (!serialization.TryCustomCopy<bool>(ColonyCommunications, ref ColonyCommunicationsTemp, hookCtx, false, context))
			{
				ColonyCommunicationsTemp = ColonyCommunications;
			}
			target.ColonyCommunications = ColonyCommunicationsTemp;
			bool ColonyPowerTemp = false;
			if (!serialization.TryCustomCopy<bool>(ColonyPower, ref ColonyPowerTemp, hookCtx, false, context))
			{
				ColonyPowerTemp = ColonyPower;
			}
			target.ColonyPower = ColonyPowerTemp;
			int TierTemp = 0;
			if (!serialization.TryCustomCopy<int>(Tier, ref TierTemp, hookCtx, false, context))
			{
				TierTemp = Tier;
			}
			target.Tier = TierTemp;
			List<List<TechOption>> OptionsTemp = null;
			if (Options == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<List<TechOption>>>(Options, ref OptionsTemp, hookCtx, true, context))
			{
				OptionsTemp = serialization.CreateCopy<List<List<TechOption>>>(Options, hookCtx, context, false);
			}
			target.Options = OptionsTemp;
			Dictionary<LocId, Dictionary<NetEntity, string>> CluesTemp = null;
			if (Clues == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<LocId, Dictionary<NetEntity, string>>>(Clues, ref CluesTemp, hookCtx, true, context))
			{
				CluesTemp = serialization.CreateCopy<Dictionary<LocId, Dictionary<NetEntity, string>>>(Clues, hookCtx, context, false);
			}
			target.Clues = CluesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IntelTechTree target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntelTechTree cast = (IntelTechTree)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public IntelTechTree Instantiate()
	{
		return new IntelTechTree();
	}
}
