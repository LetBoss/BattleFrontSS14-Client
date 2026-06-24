using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Dataset;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Marines.Roles.Ranks;

public abstract class SharedRankSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IEntityManager _entMan;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RankComponent, ExaminedEvent>((EntityEventRefHandler<RankComponent, ExaminedEvent>)OnRankExamined, (Type[])null, (Type[])null);
	}

	private void OnRankExamined(Entity<RankComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("SharedRankSystem", 1))
		{
			EntityUid user = ent.Owner;
			string rank = GetRankString(user, isShort: false, hasPaygrade: true);
			if (rank != null)
			{
				string finalString = base.Loc.GetString("rmc-rank-component-examine", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("rank", rank));
				args.PushMarkup(finalString);
			}
		}
	}

	public void SetRank(EntityUid uid, RankPrototype from)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetRank(uid, ProtoId<RankPrototype>.op_Implicit(from.ID));
	}

	public void SetRank(EntityUid uid, ProtoId<RankPrototype> from)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		RankComponent comp = ((EntitySystem)this).EnsureComp<RankComponent>(uid);
		comp.Rank = from;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	public RankPrototype? GetRank(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RankComponent component = default(RankComponent);
		if (((EntitySystem)this).TryComp<RankComponent>(uid, ref component))
		{
			return GetRank(component);
		}
		return null;
	}

	public RankPrototype? GetRank(RankComponent component)
	{
		RankPrototype rankProto = default(RankPrototype);
		if (_prototypes.TryIndex<RankPrototype>(component.Rank, ref rankProto) && rankProto != null)
		{
			return rankProto;
		}
		return null;
	}

	public string? GetRankString(EntityUid uid, bool isShort = false, bool hasPaygrade = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		RankPrototype rank = GetRank(uid);
		if (rank == null)
		{
			return null;
		}
		if (isShort)
		{
			if (rank.FemalePrefix == null || rank.MalePrefix == null)
			{
				return rank.Prefix;
			}
			HumanoidAppearanceComponent humanoidAppearance = default(HumanoidAppearanceComponent);
			if (!((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(uid, ref humanoidAppearance))
			{
				return rank.Prefix;
			}
			Gender gender = humanoidAppearance.Gender;
			if ((int)gender != 2)
			{
				if ((int)gender == 3)
				{
					return rank.MalePrefix;
				}
				return rank.Prefix;
			}
			return rank.FemalePrefix;
		}
		if (hasPaygrade && rank.Paygrade != null)
		{
			return "(" + base.Loc.GetString(rank.Paygrade) + ") " + base.Loc.GetString(rank.Name);
		}
		return rank.Name;
	}

	public string? GetSpeakerRankName(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		string rank = GetRankString(uid, isShort: true);
		if (rank == null)
		{
			return null;
		}
		return rank + " " + ((EntitySystem)this).Name(uid, (MetaDataComponent)null);
	}

	public string? GetSpeakerFullRankName(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		string rank = GetRankString(uid);
		if (rank == null)
		{
			return null;
		}
		return rank + " " + ((EntitySystem)this).Name(uid, (MetaDataComponent)null);
	}

	public List<EntityUid>? GetEntitiesWithHighestRank(List<EntityUid> entities, ProtoId<DatasetPrototype> rankHierarchyId)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		new List<EntityUid>();
		DatasetPrototype rankHierarchy = default(DatasetPrototype);
		if (!_prototypes.TryIndex<DatasetPrototype>(rankHierarchyId, ref rankHierarchy))
		{
			return null;
		}
		List<string> rankOrder = rankHierarchy.Values.ToList();
		if (rankOrder.Count == 0)
		{
			((EntitySystem)this).Log.Error($"The rank hierarchy dataset '{rankHierarchyId}' has an invalid value: empty. The highest rank cannot be determined.");
			return null;
		}
		Dictionary<EntityUid, int> rankScores = new Dictionary<EntityUid, int>();
		int highestRank = -1;
		RankComponent rankComp = default(RankComponent);
		RankPrototype rankProto = default(RankPrototype);
		foreach (EntityUid candidate in entities)
		{
			if (!((EntitySystem)this).TryComp<RankComponent>(candidate, ref rankComp) || !rankComp.Rank.HasValue || !_prototypes.TryIndex<RankPrototype>(rankComp.Rank, ref rankProto))
			{
				continue;
			}
			int rankIndex = rankOrder.IndexOf(rankProto.ID);
			if (rankIndex != -1)
			{
				rankScores[candidate] = rankIndex;
				if (rankIndex > highestRank)
				{
					highestRank = rankIndex;
				}
			}
		}
		if (highestRank == -1)
		{
			return null;
		}
		return (from pair in rankScores
			where pair.Value == highestRank
			select pair.Key).ToList();
	}

	public bool HasInvalidRank(EntityUid entity, ProtoId<RankPrototype> invalidRankId = default(ProtoId<RankPrototype>))
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		RankComponent rankComp = default(RankComponent);
		if (!_entMan.TryGetComponent<RankComponent>(entity, ref rankComp))
		{
			return true;
		}
		if (!rankComp.Rank.HasValue)
		{
			return true;
		}
		if (invalidRankId != default(ProtoId<RankPrototype>))
		{
			ProtoId<RankPrototype>? rank = rankComp.Rank;
			ProtoId<RankPrototype> val = invalidRankId;
			if (rank.HasValue && rank.GetValueOrDefault() == val)
			{
				return true;
			}
		}
		return false;
	}
}
