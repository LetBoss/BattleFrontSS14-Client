using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SensorMonitoring;

[Serializable]
[NetSerializable]
public sealed class SensorMonitoringConsoleBoundInterfaceState : BoundUserInterfaceState
{
	[Serializable]
	[NetSerializable]
	public sealed class SensorData
	{
		public int NetId;

		public string Name = "";

		public string Address = "";

		public SensorDeviceType DeviceType;

		public SensorStream[] Streams = Array.Empty<SensorStream>();
	}

	[Serializable]
	[NetSerializable]
	public sealed class SensorStream
	{
		public int NetId;

		public string Name = "";

		public SensorUnit Unit;

		public SensorSample[] Samples = Array.Empty<SensorSample>();
	}

	public TimeSpan RetentionTime;

	public SensorData[] Sensors = Array.Empty<SensorData>();
}
