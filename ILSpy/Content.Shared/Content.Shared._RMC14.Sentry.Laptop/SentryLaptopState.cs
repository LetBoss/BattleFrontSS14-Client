using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public enum SentryLaptopState : byte
{
	Closed,
	Open,
	Active
}
