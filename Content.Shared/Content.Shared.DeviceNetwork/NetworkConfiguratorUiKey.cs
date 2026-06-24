using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public enum NetworkConfiguratorUiKey
{
	List,
	Configure,
	Link
}
