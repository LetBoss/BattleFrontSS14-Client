using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class DamageSpecifier : IEquatable<DamageSpecifier>, ISerializationGenerated<DamageSpecifier>, ISerializationGenerated
{
	[JsonPropertyName("types")]
	[DataField("types", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, DamageTypePrototype>))]
	private Dictionary<string, FixedPoint2>? _damageTypeDictionary;

	[JsonPropertyName("groups")]
	[DataField("groups", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, DamageGroupPrototype>))]
	private Dictionary<string, FixedPoint2>? _damageGroupDictionary;

	[JsonIgnore]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[IncludeDataField(true, 1, false, typeof(DamageSpecifierDictionarySerializer))]
	public Dictionary<string, FixedPoint2> DamageDict { get; set; } = new Dictionary<string, FixedPoint2>();

	[JsonIgnore]
	public bool Empty => DamageDict.Count == 0;

	public FixedPoint2 this[string key] => DamageDict[key];

	public FixedPoint2 GetTotal()
	{
		FixedPoint2 total = FixedPoint2.Zero;
		foreach (FixedPoint2 value in DamageDict.Values)
		{
			total += value;
		}
		return total;
	}

	public bool AnyPositive()
	{
		foreach (FixedPoint2 value in DamageDict.Values)
		{
			if (value > FixedPoint2.Zero)
			{
				return true;
			}
		}
		return false;
	}

	public override string ToString()
	{
		return "DamageSpecifier(" + string.Join("; ", DamageDict.Select<KeyValuePair<string, FixedPoint2>, string>((KeyValuePair<string, FixedPoint2> x) => x.Key + ":" + x.Value)) + ")";
	}

	public DamageSpecifier()
	{
	}

	public DamageSpecifier(DamageSpecifier damageSpec)
	{
		DamageDict = new Dictionary<string, FixedPoint2>(damageSpec.DamageDict);
	}

	public DamageSpecifier(DamageTypePrototype type, FixedPoint2 value)
	{
		DamageDict = new Dictionary<string, FixedPoint2> { { type.ID, value } };
	}

	public DamageSpecifier(DamageGroupPrototype group, FixedPoint2 value)
	{
		int remainingTypes = group.DamageTypes.Count;
		FixedPoint2 remainingDamage = value;
		foreach (string damageType in group.DamageTypes)
		{
			FixedPoint2 damage = remainingDamage / FixedPoint2.New(remainingTypes);
			DamageDict.Add(damageType, damage);
			remainingDamage -= damage;
			remainingTypes--;
		}
	}

	public static DamageSpecifier ApplyModifierSet(DamageSpecifier damageSpec, DamageModifierSet modifierSet)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		newDamage.DamageDict.EnsureCapacity(damageSpec.DamageDict.Count);
		foreach (var (key, value) in damageSpec.DamageDict)
		{
			if (value == 0)
			{
				continue;
			}
			if (value < 0)
			{
				newDamage.DamageDict[key] = value;
				continue;
			}
			float newValue = value.Float();
			if (modifierSet.FlatReduction.TryGetValue(key, out var reduction))
			{
				newValue = Math.Max(0f, newValue - reduction);
			}
			if (modifierSet.Coefficients.TryGetValue(key, out var coefficient))
			{
				newValue *= coefficient;
			}
			if (newValue != 0f)
			{
				newDamage.DamageDict[key] = FixedPoint2.New(newValue);
			}
		}
		return newDamage;
	}

	public static DamageSpecifier ApplyModifierSets(DamageSpecifier damageSpec, IEnumerable<DamageModifierSet> modifierSets)
	{
		bool any = false;
		DamageSpecifier newDamage = damageSpec;
		foreach (DamageModifierSet set in modifierSets)
		{
			newDamage = ApplyModifierSet(newDamage, set);
			any = true;
		}
		if (!any)
		{
			newDamage = new DamageSpecifier(damageSpec);
		}
		return newDamage;
	}

	public static DamageSpecifier GetPositive(DamageSpecifier damageSpec)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		foreach (var (key, value) in damageSpec.DamageDict)
		{
			if (value > 0)
			{
				newDamage.DamageDict[key] = value;
			}
		}
		return newDamage;
	}

	public static DamageSpecifier GetNegative(DamageSpecifier damageSpec)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		foreach (var (key, value) in damageSpec.DamageDict)
		{
			if (value < 0)
			{
				newDamage.DamageDict[key] = value;
			}
		}
		return newDamage;
	}

	public void TrimZeros()
	{
		foreach (var (key, fixedPoint2) in DamageDict)
		{
			if (fixedPoint2 == 0)
			{
				DamageDict.Remove(key);
			}
		}
	}

	public void Clamp(FixedPoint2 minValue, FixedPoint2 maxValue)
	{
		ClampMax(maxValue);
		ClampMin(minValue);
	}

	public void ClampMin(FixedPoint2 minValue)
	{
		foreach (var (key, fixedPoint2) in DamageDict)
		{
			if (fixedPoint2 < minValue)
			{
				DamageDict[key] = minValue;
			}
		}
	}

	public void ClampMax(FixedPoint2 maxValue)
	{
		foreach (var (key, fixedPoint2) in DamageDict)
		{
			if (fixedPoint2 > maxValue)
			{
				DamageDict[key] = maxValue;
			}
		}
	}

	public void ExclusiveAdd(DamageSpecifier other)
	{
		foreach (var (type, value) in other.DamageDict)
		{
			if (DamageDict.TryGetValue(type, out var existing))
			{
				DamageDict[type] = existing + value;
			}
		}
	}

	public bool TryGetDamageInGroup(DamageGroupPrototype group, out FixedPoint2 total)
	{
		bool containsMemeber = false;
		total = FixedPoint2.Zero;
		foreach (string type in group.DamageTypes)
		{
			if (DamageDict.TryGetValue(type, out var value))
			{
				total += value;
				containsMemeber = true;
			}
		}
		return containsMemeber;
	}

	public Dictionary<string, FixedPoint2> GetDamagePerGroup(IPrototypeManager protoManager)
	{
		Dictionary<string, FixedPoint2> dict = new Dictionary<string, FixedPoint2>();
		GetDamagePerGroup(protoManager, dict);
		return dict;
	}

	public void GetDamagePerGroup(IPrototypeManager protoManager, Dictionary<string, FixedPoint2> dict)
	{
		dict.Clear();
		foreach (DamageGroupPrototype group in protoManager.EnumeratePrototypes<DamageGroupPrototype>())
		{
			if (TryGetDamageInGroup(group, out var value))
			{
				dict.Add(group.ID, value);
			}
		}
	}

	public static DamageSpecifier operator *(DamageSpecifier damageSpec, FixedPoint2 factor)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
		{
			newDamage.DamageDict.Add(entry.Key, entry.Value * factor);
		}
		return newDamage;
	}

	public static DamageSpecifier operator *(DamageSpecifier damageSpec, float factor)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
		{
			newDamage.DamageDict.Add(entry.Key, entry.Value * factor);
		}
		return newDamage;
	}

	public static DamageSpecifier operator /(DamageSpecifier damageSpec, FixedPoint2 factor)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
		{
			newDamage.DamageDict.Add(entry.Key, entry.Value / factor);
		}
		return newDamage;
	}

	public static DamageSpecifier operator /(DamageSpecifier damageSpec, float factor)
	{
		DamageSpecifier newDamage = new DamageSpecifier();
		foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
		{
			newDamage.DamageDict.Add(entry.Key, entry.Value / factor);
		}
		return newDamage;
	}

	public static DamageSpecifier operator +(DamageSpecifier damageSpecA, DamageSpecifier damageSpecB)
	{
		DamageSpecifier newDamage = new DamageSpecifier(damageSpecA);
		foreach (KeyValuePair<string, FixedPoint2> entry in damageSpecB.DamageDict)
		{
			if (!newDamage.DamageDict.TryAdd(entry.Key, entry.Value))
			{
				newDamage.DamageDict[entry.Key] += entry.Value;
			}
		}
		return newDamage;
	}

	public static DamageSpecifier operator -(DamageSpecifier damageSpecA, DamageSpecifier damageSpecB)
	{
		DamageSpecifier newDamage = new DamageSpecifier(damageSpecA);
		foreach (KeyValuePair<string, FixedPoint2> entry in damageSpecB.DamageDict)
		{
			if (!newDamage.DamageDict.TryAdd(entry.Key, -entry.Value))
			{
				newDamage.DamageDict[entry.Key] -= entry.Value;
			}
		}
		return newDamage;
	}

	public static DamageSpecifier operator +(DamageSpecifier damageSpec)
	{
		return damageSpec;
	}

	public static DamageSpecifier operator -(DamageSpecifier damageSpec)
	{
		return damageSpec * -1f;
	}

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
		if (other == null || DamageDict.Count != other.DamageDict.Count)
		{
			return false;
		}
		foreach (var (key, value) in DamageDict)
		{
			if (!other.DamageDict.TryGetValue(key, out var otherValue) || value != otherValue)
			{
				return false;
			}
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DamageSpecifier>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, FixedPoint2> _damageTypeDictionaryTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, FixedPoint2>>(_damageTypeDictionary, ref _damageTypeDictionaryTemp, hookCtx, true, context))
			{
				_damageTypeDictionaryTemp = serialization.CreateCopy<Dictionary<string, FixedPoint2>>(_damageTypeDictionary, hookCtx, context, false);
			}
			target._damageTypeDictionary = _damageTypeDictionaryTemp;
			Dictionary<string, FixedPoint2> _damageGroupDictionaryTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, FixedPoint2>>(_damageGroupDictionary, ref _damageGroupDictionaryTemp, hookCtx, true, context))
			{
				_damageGroupDictionaryTemp = serialization.CreateCopy<Dictionary<string, FixedPoint2>>(_damageGroupDictionary, hookCtx, context, false);
			}
			target._damageGroupDictionary = _damageGroupDictionaryTemp;
			Dictionary<string, FixedPoint2> DamageDictTemp = null;
			if (DamageDict == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, FixedPoint2>>(DamageDict, ref DamageDictTemp, hookCtx, true, context))
			{
				DamageDictTemp = serialization.CreateCopy<Dictionary<string, FixedPoint2>>(DamageDict, hookCtx, context, false);
			}
			target.DamageDict = DamageDictTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageSpecifier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageSpecifier cast = (DamageSpecifier)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DamageSpecifier Instantiate()
	{
		return new DamageSpecifier();
	}
}
