using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Humanoid.Markings;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class MarkingSet : ISerializationGenerated<MarkingSet>, ISerializationGenerated
{
	[DataField("markings", false, 1, false, false, null)]
	public Dictionary<MarkingCategories, List<Marking>> Markings = new Dictionary<MarkingCategories, List<Marking>>();

	[DataField("points", false, 1, false, false, null)]
	public Dictionary<MarkingCategories, MarkingPoints> Points = new Dictionary<MarkingCategories, MarkingPoints>();

	public MarkingSet()
	{
	}

	public MarkingSet(List<Marking> markings, string pointsPrototype, MarkingManager? markingManager = null, IPrototypeManager? prototypeManager = null)
	{
		IoCManager.Resolve<MarkingManager, IPrototypeManager>(ref markingManager, ref prototypeManager);
		MarkingPointsPrototype points = default(MarkingPointsPrototype);
		if (!prototypeManager.TryIndex<MarkingPointsPrototype>(pointsPrototype, ref points))
		{
			return;
		}
		Points = MarkingPoints.CloneMarkingPointDictionary(points.Points);
		foreach (Marking marking in markings)
		{
			if (markingManager.TryGetMarking(marking, out MarkingPrototype prototype))
			{
				AddBack(prototype.MarkingCategory, marking);
			}
		}
	}

	public MarkingSet(List<Marking> markings, MarkingManager? markingManager = null)
	{
		IoCManager.Resolve<MarkingManager>(ref markingManager);
		foreach (Marking marking in markings)
		{
			if (markingManager.TryGetMarking(marking, out MarkingPrototype prototype))
			{
				AddBack(prototype.MarkingCategory, marking);
			}
		}
	}

	public MarkingSet(string pointsPrototype, MarkingManager? markingManager = null, IPrototypeManager? prototypeManager = null)
	{
		IoCManager.Resolve<MarkingManager, IPrototypeManager>(ref markingManager, ref prototypeManager);
		MarkingPointsPrototype points = default(MarkingPointsPrototype);
		if (prototypeManager.TryIndex<MarkingPointsPrototype>(pointsPrototype, ref points))
		{
			Points = MarkingPoints.CloneMarkingPointDictionary(points.Points);
		}
	}

	public MarkingSet(MarkingSet other)
	{
		foreach (var (key, list2) in other.Markings)
		{
			foreach (Marking marking in list2)
			{
				AddBack(key, new Marking(marking));
			}
		}
		Points = MarkingPoints.CloneMarkingPointDictionary(other.Points);
	}

	public void EnsureSpecies(string species, Color? skinColor, MarkingManager? markingManager = null, IPrototypeManager? prototypeManager = null)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<MarkingManager>(ref markingManager);
		IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
		List<(MarkingCategories, string)> toRemove = new List<(MarkingCategories, string)>();
		SpeciesPrototype speciesProto = prototypeManager.Index<SpeciesPrototype>(species);
		bool onlyWhitelisted = prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints).OnlyWhitelisted;
		MarkingCategories key;
		List<Marking> value;
		foreach (KeyValuePair<MarkingCategories, List<Marking>> marking3 in Markings)
		{
			marking3.Deconstruct(out key, out value);
			MarkingCategories category = key;
			foreach (Marking marking in value)
			{
				if (!markingManager.TryGetMarking(marking, out MarkingPrototype prototype))
				{
					toRemove.Add((category, marking.MarkingId));
					continue;
				}
				if (onlyWhitelisted && prototype.SpeciesRestrictions == null)
				{
					toRemove.Add((category, marking.MarkingId));
				}
				if (prototype.SpeciesRestrictions != null && !prototype.SpeciesRestrictions.Contains(species))
				{
					toRemove.Add((category, marking.MarkingId));
				}
			}
		}
		foreach (var remove in toRemove)
		{
			Remove(remove.Item1, remove.Item2);
		}
		if (!skinColor.HasValue)
		{
			return;
		}
		foreach (KeyValuePair<MarkingCategories, List<Marking>> marking4 in Markings)
		{
			marking4.Deconstruct(out key, out value);
			foreach (Marking marking2 in value)
			{
				if (markingManager.TryGetMarking(marking2, out MarkingPrototype prototype2) && markingManager.MustMatchSkin(species, prototype2.BodyPart, out var alpha, prototypeManager))
				{
					Color value2 = skinColor.Value;
					marking2.SetColor(((Color)(ref value2)).WithAlpha(alpha));
				}
			}
		}
	}

	public void EnsureSexes(Sex sex, MarkingManager? markingManager = null)
	{
		IoCManager.Resolve<MarkingManager>(ref markingManager);
		List<(MarkingCategories, string)> toRemove = new List<(MarkingCategories, string)>();
		foreach (var (category, list2) in Markings)
		{
			foreach (Marking marking in list2)
			{
				if (!markingManager.TryGetMarking(marking, out MarkingPrototype prototype))
				{
					toRemove.Add((category, marking.MarkingId));
				}
				else if (prototype.SexRestriction.HasValue && prototype.SexRestriction != sex)
				{
					toRemove.Add((category, marking.MarkingId));
				}
			}
		}
		foreach (var remove in toRemove)
		{
			Remove(remove.Item1, remove.Item2);
		}
	}

	public void EnsureValid(MarkingManager? markingManager = null)
	{
		IoCManager.Resolve<MarkingManager>(ref markingManager);
		List<int> toRemove = new List<int>();
		foreach (KeyValuePair<MarkingCategories, List<Marking>> marking2 in Markings)
		{
			marking2.Deconstruct(out var key, out var value);
			MarkingCategories category = key;
			List<Marking> list = value;
			for (int i = 0; i < list.Count; i++)
			{
				if (!markingManager.TryGetMarking(list[i], out MarkingPrototype marking))
				{
					toRemove.Add(i);
				}
				else if (marking.Sprites.Count != list[i].MarkingColors.Count)
				{
					list[i] = new Marking(marking.ID, marking.Sprites.Count);
				}
			}
			foreach (int i2 in toRemove)
			{
				Remove(category, i2);
			}
		}
	}

	public void EnsureDefault(Color? skinColor = null, Color? eyeColor = null, MarkingManager? markingManager = null)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<MarkingManager>(ref markingManager);
		foreach (var (category, points) in Points)
		{
			if (points.Points <= 0 || points.DefaultMarkings.Count <= 0)
			{
				continue;
			}
			for (int index = 0; points.Points > 0 || index < points.DefaultMarkings.Count; index++)
			{
				if (markingManager.Markings.TryGetValue(ProtoId<MarkingPrototype>.op_Implicit(points.DefaultMarkings[index]), out MarkingPrototype prototype))
				{
					List<Color> colors = MarkingColoring.GetMarkingLayerColors(prototype, skinColor, eyeColor, this);
					Marking marking = new Marking(ProtoId<MarkingPrototype>.op_Implicit(points.DefaultMarkings[index]), colors);
					AddBack(category, marking);
				}
			}
		}
	}

	public int PointsLeft(MarkingCategories category)
	{
		if (!Points.TryGetValue(category, out MarkingPoints points))
		{
			return -1;
		}
		return points.Points;
	}

	public void AddFront(MarkingCategories category, Marking marking)
	{
		if (!marking.Forced && Points.TryGetValue(category, out MarkingPoints points))
		{
			if (points.Points <= 0)
			{
				return;
			}
			points.Points--;
		}
		if (!Markings.TryGetValue(category, out List<Marking> markings))
		{
			markings = new List<Marking>();
			Markings[category] = markings;
		}
		markings.Insert(0, marking);
	}

	public void AddBack(MarkingCategories category, Marking marking)
	{
		if (!marking.Forced && Points.TryGetValue(category, out MarkingPoints points))
		{
			if (points.Points <= 0)
			{
				return;
			}
			points.Points--;
		}
		if (!Markings.TryGetValue(category, out List<Marking> markings))
		{
			markings = new List<Marking>();
			Markings[category] = markings;
		}
		markings.Add(marking);
	}

	public List<Marking> AddCategory(MarkingCategories category)
	{
		List<Marking> markings = new List<Marking>();
		Markings.Add(category, markings);
		return markings;
	}

	public void Replace(MarkingCategories category, int index, Marking marking)
	{
		if (index >= 0 && Markings.TryGetValue(category, out List<Marking> markings) && index < markings.Count)
		{
			markings[index] = marking;
		}
	}

	public bool Remove(MarkingCategories category, string id)
	{
		if (!Markings.TryGetValue(category, out List<Marking> markings))
		{
			return false;
		}
		for (int i = 0; i < markings.Count; i++)
		{
			if (!(markings[i].MarkingId != id))
			{
				if (!markings[i].Forced && Points.TryGetValue(category, out MarkingPoints points))
				{
					points.Points++;
				}
				markings.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public void Remove(MarkingCategories category, int idx)
	{
		if (Markings.TryGetValue(category, out List<Marking> markings) && idx >= 0 && idx < markings.Count)
		{
			if (!markings[idx].Forced && Points.TryGetValue(category, out MarkingPoints points))
			{
				points.Points++;
			}
			markings.RemoveAt(idx);
		}
	}

	public bool RemoveCategory(MarkingCategories category)
	{
		if (!Markings.TryGetValue(category, out List<Marking> markings))
		{
			return false;
		}
		if (Points.TryGetValue(category, out MarkingPoints points))
		{
			foreach (Marking item in markings)
			{
				if (!item.Forced)
				{
					points.Points++;
				}
			}
		}
		Markings.Remove(category);
		return true;
	}

	public void Clear()
	{
		MarkingCategories[] values = Enum.GetValues<MarkingCategories>();
		foreach (MarkingCategories category in values)
		{
			RemoveCategory(category);
		}
	}

	public int FindIndexOf(MarkingCategories category, string id)
	{
		if (!Markings.TryGetValue(category, out List<Marking> markings))
		{
			return -1;
		}
		return markings.FindIndex((Marking m) => m.MarkingId == id);
	}

	public bool TryGetCategory(MarkingCategories category, [NotNullWhen(true)] out IReadOnlyList<Marking>? markings)
	{
		markings = null;
		if (Markings.TryGetValue(category, out List<Marking> list))
		{
			markings = list;
			return true;
		}
		return false;
	}

	public bool TryGetMarking(MarkingCategories category, string id, [NotNullWhen(true)] out Marking? marking)
	{
		marking = null;
		if (!Markings.TryGetValue(category, out List<Marking> markings))
		{
			return false;
		}
		foreach (Marking m in markings)
		{
			if (m.MarkingId == id)
			{
				marking = m;
				return true;
			}
		}
		return false;
	}

	public void ShiftRankUp(MarkingCategories category, int idx)
	{
		if (Markings.TryGetValue(category, out List<Marking> markings) && idx >= 0 && idx < markings.Count && idx - 1 >= 0)
		{
			List<Marking> list = markings;
			int index = idx - 1;
			List<Marking> list2 = markings;
			Marking value = markings[idx];
			Marking value2 = markings[idx - 1];
			list[index] = value;
			list2[idx] = value2;
		}
	}

	public void ShiftRankUpFromEnd(MarkingCategories category, int idx)
	{
		if (Markings.TryGetValue(category, out List<Marking> markings))
		{
			ShiftRankUp(category, markings.Count - idx - 1);
		}
	}

	public void ShiftRankDown(MarkingCategories category, int idx)
	{
		if (Markings.TryGetValue(category, out List<Marking> markings) && idx >= 0 && idx < markings.Count && idx + 1 < markings.Count)
		{
			List<Marking> list = markings;
			int index = idx + 1;
			List<Marking> list2 = markings;
			Marking value = markings[idx];
			Marking value2 = markings[idx + 1];
			list[index] = value;
			list2[idx] = value2;
		}
	}

	public void ShiftRankDownFromEnd(MarkingCategories category, int idx)
	{
		if (Markings.TryGetValue(category, out List<Marking> markings))
		{
			ShiftRankDown(category, markings.Count - idx - 1);
		}
	}

	public ForwardMarkingEnumerator GetForwardEnumerator()
	{
		List<Marking> markings = new List<Marking>();
		foreach (var (_, list2) in Markings)
		{
			markings.AddRange(list2);
		}
		return new ForwardMarkingEnumerator(markings);
	}

	public ForwardMarkingEnumerator GetForwardEnumerator(MarkingCategories category)
	{
		List<Marking> markings = new List<Marking>();
		if (Markings.TryGetValue(category, out List<Marking> listing))
		{
			markings = new List<Marking>(listing);
		}
		return new ForwardMarkingEnumerator(markings);
	}

	public ReverseMarkingEnumerator GetReverseEnumerator()
	{
		List<Marking> markings = new List<Marking>();
		foreach (var (_, list2) in Markings)
		{
			markings.AddRange(list2);
		}
		return new ReverseMarkingEnumerator(markings);
	}

	public ReverseMarkingEnumerator GetReverseEnumerator(MarkingCategories category)
	{
		List<Marking> markings = new List<Marking>();
		if (Markings.TryGetValue(category, out List<Marking> listing))
		{
			markings = new List<Marking>(listing);
		}
		return new ReverseMarkingEnumerator(markings);
	}

	public bool CategoryEquals(MarkingCategories category, MarkingSet other)
	{
		if (!Markings.TryGetValue(category, out List<Marking> markings) || !other.Markings.TryGetValue(category, out List<Marking> markingsOther))
		{
			return false;
		}
		return markings.SequenceEqual(markingsOther);
	}

	public bool Equals(MarkingSet other)
	{
		foreach (var (category, _) in Markings)
		{
			if (!CategoryEquals(category, other))
			{
				return false;
			}
		}
		return true;
	}

	public IEnumerable<MarkingCategories> CategoryDifference(MarkingSet other)
	{
		foreach (var (category, _) in Markings)
		{
			if (!CategoryEquals(category, other))
			{
				yield return category;
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MarkingSet target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MarkingSet>(this, ref target, hookCtx, false, context))
		{
			Dictionary<MarkingCategories, List<Marking>> MarkingsTemp = null;
			if (Markings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MarkingCategories, List<Marking>>>(Markings, ref MarkingsTemp, hookCtx, true, context))
			{
				MarkingsTemp = serialization.CreateCopy<Dictionary<MarkingCategories, List<Marking>>>(Markings, hookCtx, context, false);
			}
			target.Markings = MarkingsTemp;
			Dictionary<MarkingCategories, MarkingPoints> PointsTemp = null;
			if (Points == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MarkingCategories, MarkingPoints>>(Points, ref PointsTemp, hookCtx, true, context))
			{
				PointsTemp = serialization.CreateCopy<Dictionary<MarkingCategories, MarkingPoints>>(Points, hookCtx, context, false);
			}
			target.Points = PointsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MarkingSet target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MarkingSet cast = (MarkingSet)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MarkingSet Instantiate()
	{
		return new MarkingSet();
	}
}
