using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera;

[Serializable]
[NetSerializable]
public sealed class SurveillanceCameraSetupBoundUiState : BoundUserInterfaceState
{
	public string Name { get; }

	public uint Network { get; }

	public List<string> Networks { get; }

	public bool NameDisabled { get; }

	public bool NetworkDisabled { get; }

	public SurveillanceCameraSetupBoundUiState(string name, uint network, List<string> networks, bool nameDisabled, bool networkDisabled)
	{
		Name = name;
		Network = network;
		Networks = networks;
		NameDisabled = nameDisabled;
		NetworkDisabled = networkDisabled;
	}
}
