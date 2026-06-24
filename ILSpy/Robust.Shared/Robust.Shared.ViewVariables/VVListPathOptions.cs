using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public readonly struct VVListPathOptions
{
	public VVAccess MinimumAccess { get; init; } = VVAccess.ReadOnly;

	public bool ListIndexers { get; init; } = true;

	public int RemoteListLength { get; init; } = 500;

	public VVListPathOptions()
	{
	}
}
