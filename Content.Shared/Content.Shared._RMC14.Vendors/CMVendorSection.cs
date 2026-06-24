using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vendors;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class CMVendorSection : ISerializationGenerated<CMVendorSection>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public (string Id, int Amount)? Choices;

	[DataField(null, false, 1, false, false, null)]
	public string? TakeAll;

	[DataField(null, false, 1, false, false, null)]
	public string? TakeOne;

	[DataField(null, false, 1, true, false, null)]
	public List<CMVendorEntry> Entries = new List<CMVendorEntry>();

	[DataField(null, false, 1, false, false, null)]
	public int? SharedSpecLimit;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<JobPrototype>> Jobs = new List<ProtoId<JobPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<RankPrototype>> Ranks = new List<ProtoId<RankPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public List<string> Holidays = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public bool HasBoxes;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CMVendorSection target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CMVendorSection>(this, ref target, hookCtx, false, context))
		{
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			(string, int)? ChoicesTemp = null;
			if (!serialization.TryCustomCopy<(string, int)?>(Choices, ref ChoicesTemp, hookCtx, false, context))
			{
				ChoicesTemp = serialization.CreateCopy<(string, int)?>(Choices, hookCtx, context, false);
			}
			target.Choices = ChoicesTemp;
			string TakeAllTemp = null;
			if (!serialization.TryCustomCopy<string>(TakeAll, ref TakeAllTemp, hookCtx, false, context))
			{
				TakeAllTemp = TakeAll;
			}
			target.TakeAll = TakeAllTemp;
			string TakeOneTemp = null;
			if (!serialization.TryCustomCopy<string>(TakeOne, ref TakeOneTemp, hookCtx, false, context))
			{
				TakeOneTemp = TakeOne;
			}
			target.TakeOne = TakeOneTemp;
			List<CMVendorEntry> EntriesTemp = null;
			if (Entries == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<CMVendorEntry>>(Entries, ref EntriesTemp, hookCtx, true, context))
			{
				EntriesTemp = serialization.CreateCopy<List<CMVendorEntry>>(Entries, hookCtx, context, false);
			}
			target.Entries = EntriesTemp;
			int? SharedSpecLimitTemp = null;
			if (!serialization.TryCustomCopy<int?>(SharedSpecLimit, ref SharedSpecLimitTemp, hookCtx, false, context))
			{
				SharedSpecLimitTemp = SharedSpecLimit;
			}
			target.SharedSpecLimit = SharedSpecLimitTemp;
			List<ProtoId<JobPrototype>> JobsTemp = null;
			if (Jobs == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<JobPrototype>>>(Jobs, ref JobsTemp, hookCtx, true, context))
			{
				JobsTemp = serialization.CreateCopy<List<ProtoId<JobPrototype>>>(Jobs, hookCtx, context, false);
			}
			target.Jobs = JobsTemp;
			List<ProtoId<RankPrototype>> RanksTemp = null;
			if (Ranks == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<RankPrototype>>>(Ranks, ref RanksTemp, hookCtx, true, context))
			{
				RanksTemp = serialization.CreateCopy<List<ProtoId<RankPrototype>>>(Ranks, hookCtx, context, false);
			}
			target.Ranks = RanksTemp;
			List<string> HolidaysTemp = null;
			if (Holidays == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Holidays, ref HolidaysTemp, hookCtx, true, context))
			{
				HolidaysTemp = serialization.CreateCopy<List<string>>(Holidays, hookCtx, context, false);
			}
			target.Holidays = HolidaysTemp;
			bool HasBoxesTemp = false;
			if (!serialization.TryCustomCopy<bool>(HasBoxes, ref HasBoxesTemp, hookCtx, false, context))
			{
				HasBoxesTemp = HasBoxes;
			}
			target.HasBoxes = HasBoxesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CMVendorSection target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMVendorSection cast = (CMVendorSection)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CMVendorSection Instantiate()
	{
		return new CMVendorSection();
	}
}
