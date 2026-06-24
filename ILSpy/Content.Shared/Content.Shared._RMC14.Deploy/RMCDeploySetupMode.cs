using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Deploy;

[Serializable]
[NetSerializable]
public enum RMCDeploySetupMode
{
	Default,
	Reactive,
	ReactiveParental
}
