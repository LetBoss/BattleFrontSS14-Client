using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Utility.Components;

[Serializable]
[NetSerializable]
public enum EquipmentDeployState
{
	UnDeployed,
	Deployed
}
