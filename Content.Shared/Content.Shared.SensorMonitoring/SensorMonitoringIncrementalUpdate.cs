using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SensorMonitoring;

[Serializable]
[NetSerializable]
public sealed class SensorMonitoringIncrementalUpdate : BoundUserInterfaceMessage
{
	[Serializable]
	[NetSerializable]
	public sealed class SensorData
	{
		public int NetId;

		public SensorStream[] Streams = Array.Empty<SensorStream>();
	}

	[Serializable]
	[NetSerializable]
	public sealed class SensorStream
	{
		public int NetId;

		public SensorUnit Unit;

		public SensorSample[] Samples = Array.Empty<SensorSample>();
	}

	public TimeSpan RelTime;

	public SensorData[] Sensors = Array.Empty<SensorData>();

	public int[] RemovedSensors = Array.Empty<int>();
}
