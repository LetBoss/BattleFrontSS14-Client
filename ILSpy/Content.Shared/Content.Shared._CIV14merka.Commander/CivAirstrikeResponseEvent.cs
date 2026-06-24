using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivAirstrikeResponseEvent : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public CivAirstrikeResponseEvent(bool success, string? error = null)
	{
		Success = success;
		Error = error;
	}
}
