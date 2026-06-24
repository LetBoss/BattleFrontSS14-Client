using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public sealed class MetaDataComponentState : ComponentState
{
	public TimeSpan? PauseTime;

	public string? Name { get; }

	public string? Description { get; }

	public string? PrototypeId { get; }

	public MetaDataComponentState(string? name, string? description, string? prototypeId, TimeSpan? pauseTime)
	{
		Name = name;
		Description = description;
		PrototypeId = prototypeId;
		PauseTime = pauseTime;
	}
}
