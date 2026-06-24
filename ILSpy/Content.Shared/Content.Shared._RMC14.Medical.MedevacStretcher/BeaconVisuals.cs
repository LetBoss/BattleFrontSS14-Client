using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.MedevacStretcher;

[Serializable]
[NetSerializable]
public enum BeaconVisuals : byte
{
	Off,
	On
}
