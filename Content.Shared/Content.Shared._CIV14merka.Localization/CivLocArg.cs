using System;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Localization;

[Serializable]
[NetSerializable]
public struct CivLocArg
{
	public string Name;

	public string Value;

	public bool IsLoc;

	public CivLocMessage? Sub;

	public static CivLocArg Text(string name, object? value)
	{
		return new CivLocArg
		{
			Name = name,
			Value = (value?.ToString() ?? string.Empty),
			IsLoc = false
		};
	}

	public static CivLocArg LocRef(string name, string locId)
	{
		return new CivLocArg
		{
			Name = name,
			Value = locId,
			IsLoc = true
		};
	}

	public static CivLocArg Msg(string name, CivLocMessage sub)
	{
		return new CivLocArg
		{
			Name = name,
			Sub = sub
		};
	}

	public object ResolveValue()
	{
		if (Sub != null)
		{
			return Sub.Resolve();
		}
		if (!IsLoc)
		{
			return Value;
		}
		return Loc.GetString(Value);
	}
}
