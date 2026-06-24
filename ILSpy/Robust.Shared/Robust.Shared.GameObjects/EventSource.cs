using System;

namespace Robust.Shared.GameObjects;

[Flags]
public enum EventSource : byte
{
	None = 0,
	Local = 1,
	Network = 2,
	All = 3
}
