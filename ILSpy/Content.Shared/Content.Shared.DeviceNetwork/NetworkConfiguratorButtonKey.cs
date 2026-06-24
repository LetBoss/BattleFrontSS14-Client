using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public enum NetworkConfiguratorButtonKey
{
	Set,
	Add,
	Edit,
	Clear,
	Copy,
	Show
}
