// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingSet
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class MarkingSet : ISerializationGenerated<MarkingSet>, ISerializationGenerated
{
  [DataField("markings", false, 1, false, false, null)]
  public Dictionary<MarkingCategories, List<Marking>> Markings = new Dictionary<MarkingCategories, List<Marking>>();
  [DataField("points", false, 1, false, false, null)]
  public Dictionary<MarkingCategories, MarkingPoints> Points = new Dictionary<MarkingCategories, MarkingPoints>();

  public MarkingSet()
  {
  }

  public MarkingSet(
    List<Marking> markings,
    string pointsPrototype,
    MarkingManager? markingManager = null,
    IPrototypeManager? prototypeManager = null)
  {
    IoCManager.Resolve<MarkingManager, IPrototypeManager>(ref markingManager, ref prototypeManager);
    MarkingPointsPrototype prototype;
    if (!prototypeManager.TryIndex<MarkingPointsPrototype>(pointsPrototype, out prototype))
      return;
    this.Points = MarkingPoints.CloneMarkingPointDictionary(prototype.Points);
    foreach (Marking marking in markings)
    {
      MarkingPrototype markingResult;
      if (markingManager.TryGetMarking(marking, out markingResult))
        this.AddBack(markingResult.MarkingCategory, marking);
    }
  }

  public MarkingSet(List<Marking> markings, MarkingManager? markingManager = null)
  {
    IoCManager.Resolve<MarkingManager>(ref markingManager);
    foreach (Marking marking in markings)
    {
      MarkingPrototype markingResult;
      if (markingManager.TryGetMarking(marking, out markingResult))
        this.AddBack(markingResult.MarkingCategory, marking);
    }
  }

  public MarkingSet(
    string pointsPrototype,
    MarkingManager? markingManager = null,
    IPrototypeManager? prototypeManager = null)
  {
    IoCManager.Resolve<MarkingManager, IPrototypeManager>(ref markingManager, ref prototypeManager);
    MarkingPointsPrototype prototype;
    if (!prototypeManager.TryIndex<MarkingPointsPrototype>(pointsPrototype, out prototype))
      return;
    this.Points = MarkingPoints.CloneMarkingPointDictionary(prototype.Points);
  }

  public MarkingSet(MarkingSet other)
  {
    foreach ((MarkingCategories markingCategories, List<Marking> markingList) in other.Markings)
    {
      foreach (Marking other1 in markingList)
        this.AddBack(markingCategories, new Marking(other1));
    }
    this.Points = MarkingPoints.CloneMarkingPointDictionary(other.Points);
  }

  public void EnsureSpecies(
    string species,
    Color? skinColor,
    MarkingManager? markingManager = null,
    IPrototypeManager? prototypeManager = null)
  {
    IoCManager.Resolve<MarkingManager>(ref markingManager);
    IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
    List<(MarkingCategories, string)> valueTupleList = new List<(MarkingCategories, string)>();
    SpeciesPrototype speciesPrototype = prototypeManager.Index<SpeciesPrototype>(species);
    bool onlyWhitelisted = prototypeManager.Index<MarkingPointsPrototype>(speciesPrototype.MarkingPoints).OnlyWhitelisted;
    foreach ((MarkingCategories key, List<Marking> markingList) in this.Markings)
    {
      MarkingCategories markingCategories = key;
      foreach (Marking marking in markingList)
      {
        MarkingPrototype markingResult;
        if (!markingManager.TryGetMarking(marking, out markingResult))
        {
          valueTupleList.Add((markingCategories, marking.MarkingId));
        }
        else
        {
          if (onlyWhitelisted && markingResult.SpeciesRestrictions == null)
            valueTupleList.Add((markingCategories, marking.MarkingId));
          if (markingResult.SpeciesRestrictions != null && !markingResult.SpeciesRestrictions.Contains(species))
            valueTupleList.Add((markingCategories, marking.MarkingId));
        }
      }
    }
    foreach ((MarkingCategories, string) valueTuple in valueTupleList)
      this.Remove(valueTuple.Item1, valueTuple.Item2);
    if (!skinColor.HasValue)
      return;
    foreach ((key, markingList) in this.Markings)
    {
      foreach (Marking marking1 in markingList)
      {
        MarkingPrototype markingResult;
        float alpha;
        if (markingManager.TryGetMarking(marking1, out markingResult) && markingManager.MustMatchSkin(species, markingResult.BodyPart, out alpha, prototypeManager))
        {
          Marking marking2 = marking1;
          Color color1 = skinColor.Value;
          Color color2 = ((Color) ref color1).WithAlpha(alpha);
          marking2.SetColor(color2);
        }
      }
    }
  }

  public void EnsureSexes(Sex sex, MarkingManager? markingManager = null)
  {
    IoCManager.Resolve<MarkingManager>(ref markingManager);
    List<(MarkingCategories, string)> valueTupleList = new List<(MarkingCategories, string)>();
    foreach ((MarkingCategories key, List<Marking> markingList) in this.Markings)
    {
      foreach (Marking marking in markingList)
      {
        MarkingPrototype markingResult;
        if (!markingManager.TryGetMarking(marking, out markingResult))
        {
          valueTupleList.Add((key, marking.MarkingId));
        }
        else
        {
          Sex? sexRestriction = markingResult.SexRestriction;
          if (sexRestriction.HasValue)
          {
            sexRestriction = markingResult.SexRestriction;
            Sex sex1 = sex;
            if (!(sexRestriction.GetValueOrDefault() == sex1 & sexRestriction.HasValue))
              valueTupleList.Add((key, marking.MarkingId));
          }
        }
      }
    }
    foreach ((MarkingCategories, string) valueTuple in valueTupleList)
      this.Remove(valueTuple.Item1, valueTuple.Item2);
  }

  public void EnsureValid(MarkingManager? markingManager = null)
  {
    IoCManager.Resolve<MarkingManager>(ref markingManager);
    List<int> intList = new List<int>();
    foreach ((MarkingCategories markingCategories, List<Marking> markingList) in this.Markings)
    {
      for (int index = 0; index < markingList.Count; ++index)
      {
        MarkingPrototype markingResult;
        if (!markingManager.TryGetMarking(markingList[index], out markingResult))
          intList.Add(index);
        else if (markingResult.Sprites.Count != markingList[index].MarkingColors.Count)
          markingList[index] = new Marking(markingResult.ID, markingResult.Sprites.Count);
      }
      foreach (int idx in intList)
        this.Remove(markingCategories, idx);
    }
  }

  public void EnsureDefault(Color? skinColor = null, Color? eyeColor = null, MarkingManager? markingManager = null)
  {
    IoCManager.Resolve<MarkingManager>(ref markingManager);
    foreach ((MarkingCategories markingCategories, MarkingPoints markingPoints) in this.Points)
    {
      if (markingPoints.Points > 0 && markingPoints.DefaultMarkings.Count > 0)
      {
        for (int index = 0; markingPoints.Points > 0 || index < markingPoints.DefaultMarkings.Count; ++index)
        {
          MarkingPrototype prototype;
          if (markingManager.Markings.TryGetValue((string) markingPoints.DefaultMarkings[index], out prototype))
          {
            List<Color> markingLayerColors = MarkingColoring.GetMarkingLayerColors(prototype, skinColor, eyeColor, this);
            Marking marking = new Marking((string) markingPoints.DefaultMarkings[index], markingLayerColors);
            this.AddBack(markingCategories, marking);
          }
        }
      }
    }
  }

  public int PointsLeft(MarkingCategories category)
  {
    MarkingPoints markingPoints;
    return !this.Points.TryGetValue(category, out markingPoints) ? -1 : markingPoints.Points;
  }

  public void AddFront(MarkingCategories category, Marking marking)
  {
    MarkingPoints markingPoints;
    if (!marking.Forced && this.Points.TryGetValue(category, out markingPoints))
    {
      if (markingPoints.Points <= 0)
        return;
      --markingPoints.Points;
    }
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
    {
      markingList = new List<Marking>();
      this.Markings[category] = markingList;
    }
    markingList.Insert(0, marking);
  }

  public void AddBack(MarkingCategories category, Marking marking)
  {
    MarkingPoints markingPoints;
    if (!marking.Forced && this.Points.TryGetValue(category, out markingPoints))
    {
      if (markingPoints.Points <= 0)
        return;
      --markingPoints.Points;
    }
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
    {
      markingList = new List<Marking>();
      this.Markings[category] = markingList;
    }
    markingList.Add(marking);
  }

  public List<Marking> AddCategory(MarkingCategories category)
  {
    List<Marking> markingList = new List<Marking>();
    this.Markings.Add(category, markingList);
    return markingList;
  }

  public void Replace(MarkingCategories category, int index, Marking marking)
  {
    List<Marking> markingList;
    if (index < 0 || !this.Markings.TryGetValue(category, out markingList) || index >= markingList.Count)
      return;
    markingList[index] = marking;
  }

  public bool Remove(MarkingCategories category, string id)
  {
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
      return false;
    for (int index = 0; index < markingList.Count; ++index)
    {
      if (!(markingList[index].MarkingId != id))
      {
        MarkingPoints markingPoints;
        if (!markingList[index].Forced && this.Points.TryGetValue(category, out markingPoints))
          ++markingPoints.Points;
        markingList.RemoveAt(index);
        return true;
      }
    }
    return false;
  }

  public void Remove(MarkingCategories category, int idx)
  {
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList) || idx < 0 || idx >= markingList.Count)
      return;
    MarkingPoints markingPoints;
    if (!markingList[idx].Forced && this.Points.TryGetValue(category, out markingPoints))
      ++markingPoints.Points;
    markingList.RemoveAt(idx);
  }

  public bool RemoveCategory(MarkingCategories category)
  {
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
      return false;
    MarkingPoints markingPoints;
    if (this.Points.TryGetValue(category, out markingPoints))
    {
      foreach (Marking marking in markingList)
      {
        if (!marking.Forced)
          ++markingPoints.Points;
      }
    }
    this.Markings.Remove(category);
    return true;
  }

  public void Clear()
  {
    foreach (MarkingCategories category in Enum.GetValues<MarkingCategories>())
      this.RemoveCategory(category);
  }

  public int FindIndexOf(MarkingCategories category, string id)
  {
    List<Marking> markingList;
    return !this.Markings.TryGetValue(category, out markingList) ? -1 : markingList.FindIndex((Predicate<Marking>) (m => m.MarkingId == id));
  }

  public bool TryGetCategory(MarkingCategories category, [NotNullWhen(true)] out IReadOnlyList<Marking>? markings)
  {
    markings = (IReadOnlyList<Marking>) null;
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
      return false;
    markings = (IReadOnlyList<Marking>) markingList;
    return true;
  }

  public bool TryGetMarking(MarkingCategories category, string id, [NotNullWhen(true)] out Marking? marking)
  {
    marking = (Marking) null;
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
      return false;
    foreach (Marking marking1 in markingList)
    {
      if (marking1.MarkingId == id)
      {
        marking = marking1;
        return true;
      }
    }
    return false;
  }

  public void ShiftRankUp(MarkingCategories category, int idx)
  {
    List<Marking> markingList1;
    if (!this.Markings.TryGetValue(category, out markingList1) || idx < 0 || idx >= markingList1.Count || idx - 1 < 0)
      return;
    List<Marking> markingList2 = markingList1;
    int index1 = idx - 1;
    List<Marking> markingList3 = markingList1;
    int num = idx;
    Marking marking1 = markingList1[idx];
    Marking marking2 = markingList1[idx - 1];
    markingList2[index1] = marking1;
    int index2 = num;
    Marking marking3 = marking2;
    markingList3[index2] = marking3;
  }

  public void ShiftRankUpFromEnd(MarkingCategories category, int idx)
  {
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
      return;
    this.ShiftRankUp(category, markingList.Count - idx - 1);
  }

  public void ShiftRankDown(MarkingCategories category, int idx)
  {
    List<Marking> markingList1;
    if (!this.Markings.TryGetValue(category, out markingList1) || idx < 0 || idx >= markingList1.Count || idx + 1 >= markingList1.Count)
      return;
    List<Marking> markingList2 = markingList1;
    int index1 = idx + 1;
    List<Marking> markingList3 = markingList1;
    int num = idx;
    Marking marking1 = markingList1[idx];
    Marking marking2 = markingList1[idx + 1];
    markingList2[index1] = marking1;
    int index2 = num;
    Marking marking3 = marking2;
    markingList3[index2] = marking3;
  }

  public void ShiftRankDownFromEnd(MarkingCategories category, int idx)
  {
    List<Marking> markingList;
    if (!this.Markings.TryGetValue(category, out markingList))
      return;
    this.ShiftRankDown(category, markingList.Count - idx - 1);
  }

  public ForwardMarkingEnumerator GetForwardEnumerator()
  {
    List<Marking> markings = new List<Marking>();
    foreach ((MarkingCategories _, List<Marking> collection) in this.Markings)
      markings.AddRange((IEnumerable<Marking>) collection);
    return new ForwardMarkingEnumerator(markings);
  }

  public ForwardMarkingEnumerator GetForwardEnumerator(MarkingCategories category)
  {
    List<Marking> markings = new List<Marking>();
    List<Marking> collection;
    if (this.Markings.TryGetValue(category, out collection))
      markings = new List<Marking>((IEnumerable<Marking>) collection);
    return new ForwardMarkingEnumerator(markings);
  }

  public ReverseMarkingEnumerator GetReverseEnumerator()
  {
    List<Marking> markings = new List<Marking>();
    foreach ((MarkingCategories _, List<Marking> collection) in this.Markings)
      markings.AddRange((IEnumerable<Marking>) collection);
    return new ReverseMarkingEnumerator(markings);
  }

  public ReverseMarkingEnumerator GetReverseEnumerator(MarkingCategories category)
  {
    List<Marking> markings = new List<Marking>();
    List<Marking> collection;
    if (this.Markings.TryGetValue(category, out collection))
      markings = new List<Marking>((IEnumerable<Marking>) collection);
    return new ReverseMarkingEnumerator(markings);
  }

  public bool CategoryEquals(MarkingCategories category, MarkingSet other)
  {
    List<Marking> first;
    List<Marking> second;
    return this.Markings.TryGetValue(category, out first) && other.Markings.TryGetValue(category, out second) && first.SequenceEqual<Marking>((IEnumerable<Marking>) second);
  }

  public bool Equals(MarkingSet other)
  {
    foreach ((MarkingCategories markingCategories, List<Marking> _) in this.Markings)
    {
      if (!this.CategoryEquals(markingCategories, other))
        return false;
    }
    return true;
  }

  public IEnumerable<MarkingCategories> CategoryDifference(MarkingSet other)
  {
    foreach ((MarkingCategories markingCategories, List<Marking> _) in this.Markings)
    {
      if (!this.CategoryEquals(markingCategories, other))
        yield return markingCategories;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarkingSet target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MarkingSet>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<MarkingCategories, List<Marking>> target1 = (Dictionary<MarkingCategories, List<Marking>>) null;
    if (this.Markings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MarkingCategories, List<Marking>>>(this.Markings, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<Dictionary<MarkingCategories, List<Marking>>>(this.Markings, hookCtx, context);
    target.Markings = target1;
    Dictionary<MarkingCategories, MarkingPoints> target2 = (Dictionary<MarkingCategories, MarkingPoints>) null;
    if (this.Points == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MarkingCategories, MarkingPoints>>(this.Points, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<MarkingCategories, MarkingPoints>>(this.Points, hookCtx, context);
    target.Points = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarkingSet target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MarkingSet target1 = (MarkingSet) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MarkingSet Instantiate() => new MarkingSet();
}
