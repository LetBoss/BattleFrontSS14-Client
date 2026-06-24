using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client._RMC14.TacticalMap;

[Serializable]
[DataDefinition]
public struct TacticalMapSettingRegistration : ISerializationGenerated<TacticalMapSettingRegistration>, ISerializationGenerated
{
	[DataField("Key", false, 1, false, false, null)]
	public string? Key { get; set; } = null;

	[DataField("Value", false, 1, false, false, null)]
	public object? Value { get; set; } = null;

	[DataField("PlanetId", false, 1, false, false, null)]
	public string? PlanetId { get; set; } = null;

	public TacticalMapSettingRegistration()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TacticalMapSettingRegistration target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<TacticalMapSettingRegistration>(this, ref target, hookCtx, false, context))
		{
			string key = null;
			if (!serialization.TryCustomCopy<string>(Key, ref key, hookCtx, false, context))
			{
				key = Key;
			}
			object value = null;
			if (!serialization.TryCustomCopy<object>(Value, ref value, hookCtx, true, context))
			{
				value = serialization.CreateCopy(Value, hookCtx, context, false);
			}
			string planetId = null;
			if (!serialization.TryCustomCopy<string>(PlanetId, ref planetId, hookCtx, false, context))
			{
				planetId = PlanetId;
			}
			TacticalMapSettingRegistration tacticalMapSettingRegistration = target;
			tacticalMapSettingRegistration.Key = key;
			tacticalMapSettingRegistration.Value = value;
			tacticalMapSettingRegistration.PlanetId = planetId;
			target = tacticalMapSettingRegistration;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TacticalMapSettingRegistration target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TacticalMapSettingRegistration target2 = (TacticalMapSettingRegistration)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public TacticalMapSettingRegistration Instantiate()
	{
		return new TacticalMapSettingRegistration();
	}
}
