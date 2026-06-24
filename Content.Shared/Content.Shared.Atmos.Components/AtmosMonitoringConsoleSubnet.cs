using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public record AtmosMonitoringConsoleSubnet(int NetId, AtmosPipeLayer PipeLayer, string HexCode);
