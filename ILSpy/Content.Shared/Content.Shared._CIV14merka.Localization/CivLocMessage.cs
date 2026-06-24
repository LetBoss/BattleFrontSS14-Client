using System;
using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Localization;

[Serializable]
[NetSerializable]
public sealed class CivLocMessage
{
	public string Id = string.Empty;

	public List<CivLocArg> Args = new List<CivLocArg>();

	public CivLocMessage()
	{
	}

	public CivLocMessage(string id, params CivLocArg[] args)
	{
		Id = id;
		Args = new List<CivLocArg>(args);
	}

	public string Resolve()
	{
		if (Args.Count == 0)
		{
			return Loc.GetString(Id);
		}
		(string, object)[] resolved = new(string, object)[Args.Count];
		for (int i = 0; i < Args.Count; i++)
		{
			resolved[i] = (Args[i].Name, Args[i].ResolveValue());
		}
		return Loc.GetString(Id, resolved);
	}
}
