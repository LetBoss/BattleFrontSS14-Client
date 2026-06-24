using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivSmokeSupportResponseEvent : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public CivSmokeSupportResponseEvent(bool success, string? error = null)
	{
		Success = success;
		Error = error;
	}
}
