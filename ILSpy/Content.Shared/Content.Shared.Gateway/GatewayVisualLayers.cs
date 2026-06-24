using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Gateway;

[Serializable]
[NetSerializable]
public enum GatewayVisualLayers : byte
{
	Portal
}
