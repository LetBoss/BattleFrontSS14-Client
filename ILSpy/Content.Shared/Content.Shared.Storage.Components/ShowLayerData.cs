using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Components;

[Serializable]
[NetSerializable]
public sealed class ShowLayerData : ICloneable
{
	public readonly IReadOnlyList<string> QueuedEntities;

	public ShowLayerData()
	{
		QueuedEntities = new List<string>();
	}

	public ShowLayerData(IReadOnlyList<string> other)
	{
		QueuedEntities = other;
	}

	public object Clone()
	{
		return this;
	}
}
