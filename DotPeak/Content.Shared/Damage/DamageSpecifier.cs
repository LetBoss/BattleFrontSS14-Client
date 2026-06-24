// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageSpecifier
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace Content.Shared.Damage;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class DamageSpecifier : 
  IEquatable<DamageSpecifier>,
  ISerializationGenerated<DamageSpecifier>,
  ISerializationGenerated
{
  [JsonPropertyName("types")]
  [DataField("types", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, DamageTypePrototype>))]
  private System.Collections.Generic.Dictionary<string, FixedPoint2>? _damageTypeDictionary;
  [JsonPropertyName("groups")]
  [DataField("groups", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, DamageGroupPrototype>))]
  private System.Collections.Generic.Dictionary<string, FixedPoint2>? _damageGroupDictionary;

  [JsonIgnore]
  [Robust.Shared.ViewVariables.ViewVariables]
  [IncludeDataField(true, 1, false, typeof (DamageSpecifierDictionarySerializer))]
  public System.Collections.Generic.Dictionary<string, FixedPoint2> DamageDict { get; set; } = new System.Collections.Generic.Dictionary<string, FixedPoint2>();

  public FixedPoint2 GetTotal()
  {
    FixedPoint2 zero = FixedPoint2.Zero;
    foreach (FixedPoint2 fixedPoint2 in this.DamageDict.Values)
      zero += fixedPoint2;
    return zero;
  }

  public bool AnyPositive()
  {
    foreach (FixedPoint2 fixedPoint2 in this.DamageDict.Values)
    {
      if (fixedPoint2 > FixedPoint2.Zero)
        return true;
    }
    return false;
  }

  [JsonIgnore]
  public bool Empty => this.DamageDict.Count == 0;

  public override string ToString()
  {
    return $"DamageSpecifier({string.Join("; ", this.DamageDict.Select<KeyValuePair<string, FixedPoint2>, string>((Func<KeyValuePair<string, FixedPoint2>, string>) (x => $"{x.Key}:{x.Value.ToString()}")))})";
  }

  public DamageSpecifier()
  {
  }

  public DamageSpecifier(DamageSpecifier damageSpec)
  {
    this.DamageDict = new System.Collections.Generic.Dictionary<string, FixedPoint2>((IDictionary<string, FixedPoint2>) damageSpec.DamageDict);
  }

  public DamageSpecifier(DamageTypePrototype type, FixedPoint2 value)
  {
    this.DamageDict = new System.Collections.Generic.Dictionary<string, FixedPoint2>()
    {
      {
        type.ID,
        value
      }
    };
  }

  public DamageSpecifier(DamageGroupPrototype group, FixedPoint2 value)
  {
    int count = group.DamageTypes.Count;
    FixedPoint2 fixedPoint2_1 = value;
    foreach (string damageType in group.DamageTypes)
    {
      FixedPoint2 fixedPoint2_2 = fixedPoint2_1 / FixedPoint2.New(count);
      this.DamageDict.Add(damageType, fixedPoint2_2);
      fixedPoint2_1 -= fixedPoint2_2;
      --count;
    }
  }

  public static DamageSpecifier ApplyModifierSet(
    DamageSpecifier damageSpec,
    DamageModifierSet modifierSet)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    damageSpecifier.DamageDict.EnsureCapacity(damageSpec.DamageDict.Count);
    foreach ((string key, FixedPoint2 fixedPoint2) in damageSpec.DamageDict)
    {
      if (!(fixedPoint2 == 0))
      {
        if (fixedPoint2 < 0)
        {
          damageSpecifier.DamageDict[key] = fixedPoint2;
        }
        else
        {
          float num1 = fixedPoint2.Float();
          float num2;
          if (modifierSet.FlatReduction.TryGetValue(key, out num2))
            num1 = Math.Max(0.0f, num1 - num2);
          float num3;
          if (modifierSet.Coefficients.TryGetValue(key, out num3))
            num1 *= num3;
          if ((double) num1 != 0.0)
            damageSpecifier.DamageDict[key] = FixedPoint2.New(num1);
        }
      }
    }
    return damageSpecifier;
  }

  public static DamageSpecifier ApplyModifierSets(
    DamageSpecifier damageSpec,
    IEnumerable<DamageModifierSet> modifierSets)
  {
    bool flag = false;
    DamageSpecifier damageSpec1 = damageSpec;
    foreach (DamageModifierSet modifierSet in modifierSets)
    {
      damageSpec1 = DamageSpecifier.ApplyModifierSet(damageSpec1, modifierSet);
      flag = true;
    }
    if (!flag)
      damageSpec1 = new DamageSpecifier(damageSpec);
    return damageSpec1;
  }

  public static DamageSpecifier GetPositive(DamageSpecifier damageSpec)
  {
    DamageSpecifier positive = new DamageSpecifier();
    foreach ((string key, FixedPoint2 fixedPoint2) in damageSpec.DamageDict)
    {
      if (fixedPoint2 > 0)
        positive.DamageDict[key] = fixedPoint2;
    }
    return positive;
  }

  public static DamageSpecifier GetNegative(DamageSpecifier damageSpec)
  {
    DamageSpecifier negative = new DamageSpecifier();
    foreach ((string key, FixedPoint2 fixedPoint2) in damageSpec.DamageDict)
    {
      if (fixedPoint2 < 0)
        negative.DamageDict[key] = fixedPoint2;
    }
    return negative;
  }

  public void TrimZeros()
  {
    foreach ((string key, FixedPoint2 fixedPoint2) in this.DamageDict)
    {
      if (fixedPoint2 == 0)
        this.DamageDict.Remove(key);
    }
  }

  public void Clamp(FixedPoint2 minValue, FixedPoint2 maxValue)
  {
    this.ClampMax(maxValue);
    this.ClampMin(minValue);
  }

  public void ClampMin(FixedPoint2 minValue)
  {
    foreach ((string key, FixedPoint2 fixedPoint2) in this.DamageDict)
    {
      if (fixedPoint2 < minValue)
        this.DamageDict[key] = minValue;
    }
  }

  public void ClampMax(FixedPoint2 maxValue)
  {
    foreach ((string key, FixedPoint2 fixedPoint2) in this.DamageDict)
    {
      if (fixedPoint2 > maxValue)
        this.DamageDict[key] = maxValue;
    }
  }

  public void ExclusiveAdd(DamageSpecifier other)
  {
    foreach ((string key, FixedPoint2 fixedPoint2_1) in other.DamageDict)
    {
      FixedPoint2 fixedPoint2_2;
      if (this.DamageDict.TryGetValue(key, out fixedPoint2_2))
        this.DamageDict[key] = fixedPoint2_2 + fixedPoint2_1;
    }
  }

  public bool TryGetDamageInGroup(DamageGroupPrototype group, out FixedPoint2 total)
  {
    bool damageInGroup = false;
    total = FixedPoint2.Zero;
    foreach (string damageType in group.DamageTypes)
    {
      FixedPoint2 fixedPoint2;
      if (this.DamageDict.TryGetValue(damageType, out fixedPoint2))
      {
        total += fixedPoint2;
        damageInGroup = true;
      }
    }
    return damageInGroup;
  }

  public System.Collections.Generic.Dictionary<string, FixedPoint2> GetDamagePerGroup(
    IPrototypeManager protoManager)
  {
    System.Collections.Generic.Dictionary<string, FixedPoint2> dict = new System.Collections.Generic.Dictionary<string, FixedPoint2>();
    this.GetDamagePerGroup(protoManager, dict);
    return dict;
  }

  public void GetDamagePerGroup(
    IPrototypeManager protoManager,
    System.Collections.Generic.Dictionary<string, FixedPoint2> dict)
  {
    dict.Clear();
    foreach (DamageGroupPrototype enumeratePrototype in protoManager.EnumeratePrototypes<DamageGroupPrototype>())
    {
      FixedPoint2 total;
      if (this.TryGetDamageInGroup(enumeratePrototype, out total))
        dict.Add(enumeratePrototype.ID, total);
    }
  }

  public static DamageSpecifier operator *(DamageSpecifier damageSpec, FixedPoint2 factor)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpec.DamageDict)
      damageSpecifier.DamageDict.Add(keyValuePair.Key, keyValuePair.Value * factor);
    return damageSpecifier;
  }

  public static DamageSpecifier operator *(DamageSpecifier damageSpec, float factor)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpec.DamageDict)
      damageSpecifier.DamageDict.Add(keyValuePair.Key, keyValuePair.Value * factor);
    return damageSpecifier;
  }

  public static DamageSpecifier operator /(DamageSpecifier damageSpec, FixedPoint2 factor)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpec.DamageDict)
      damageSpecifier.DamageDict.Add(keyValuePair.Key, keyValuePair.Value / factor);
    return damageSpecifier;
  }

  public static DamageSpecifier operator /(DamageSpecifier damageSpec, float factor)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpec.DamageDict)
      damageSpecifier.DamageDict.Add(keyValuePair.Key, keyValuePair.Value / factor);
    return damageSpecifier;
  }

  public static DamageSpecifier operator +(DamageSpecifier damageSpecA, DamageSpecifier damageSpecB)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier(damageSpecA);
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpecB.DamageDict)
    {
      if (!damageSpecifier.DamageDict.TryAdd(keyValuePair.Key, keyValuePair.Value))
        damageSpecifier.DamageDict[keyValuePair.Key] += keyValuePair.Value;
    }
    return damageSpecifier;
  }

  public static DamageSpecifier operator -(DamageSpecifier damageSpecA, DamageSpecifier damageSpecB)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier(damageSpecA);
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpecB.DamageDict)
    {
      if (!damageSpecifier.DamageDict.TryAdd(keyValuePair.Key, -keyValuePair.Value))
        damageSpecifier.DamageDict[keyValuePair.Key] -= keyValuePair.Value;
    }
    return damageSpecifier;
  }

  public static DamageSpecifier operator +(DamageSpecifier damageSpec) => damageSpec;

  public static DamageSpecifier operator -(DamageSpecifier damageSpec) => damageSpec * -1f;

  public static DamageSpecifier operator *(float factor, DamageSpecifier damageSpec)
  {
    return damageSpec * factor;
  }

  public static DamageSpecifier operator *(FixedPoint2 factor, DamageSpecifier damageSpec)
  {
    return damageSpec * factor;
  }

  public bool Equals(DamageSpecifier? other)
  {
    if (other == null || this.DamageDict.Count != other.DamageDict.Count)
      return false;
    foreach ((string key, FixedPoint2 fixedPoint2_1) in this.DamageDict)
    {
      FixedPoint2 fixedPoint2_2;
      if (!other.DamageDict.TryGetValue(key, out fixedPoint2_2) || fixedPoint2_1 != fixedPoint2_2)
        return false;
    }
    return true;
  }

  public FixedPoint2 this[string key] => this.DamageDict[key];

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageSpecifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DamageSpecifier>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.Dictionary<string, FixedPoint2> dictionary1 = (System.Collections.Generic.Dictionary<string, FixedPoint2>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this._damageTypeDictionary, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this._damageTypeDictionary, hookCtx, context, false);
    target._damageTypeDictionary = dictionary1;
    System.Collections.Generic.Dictionary<string, FixedPoint2> dictionary2 = (System.Collections.Generic.Dictionary<string, FixedPoint2>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this._damageGroupDictionary, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this._damageGroupDictionary, hookCtx, context, false);
    target._damageGroupDictionary = dictionary2;
    System.Collections.Generic.Dictionary<string, FixedPoint2> dictionary3 = (System.Collections.Generic.Dictionary<string, FixedPoint2>) null;
    if (this.DamageDict == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this.DamageDict, ref dictionary3, hookCtx, true, context))
      dictionary3 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this.DamageDict, hookCtx, context, false);
    target.DamageDict = dictionary3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageSpecifier target,
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
    DamageSpecifier target1 = (DamageSpecifier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public DamageSpecifier Instantiate() => new DamageSpecifier();
}
