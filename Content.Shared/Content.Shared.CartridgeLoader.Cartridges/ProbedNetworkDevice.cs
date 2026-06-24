using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class ProbedNetworkDevice
{
	public readonly string Name;

	public readonly string Address;

	public readonly string Frequency;

	public readonly string NetId;

	public ProbedNetworkDevice(string name, string address, string frequency, string netId)
	{
		Name = name;
		Address = address;
		Frequency = frequency;
		NetId = netId;
	}
}
