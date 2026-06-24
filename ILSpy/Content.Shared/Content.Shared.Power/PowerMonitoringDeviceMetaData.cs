using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public struct PowerMonitoringDeviceMetaData(string name, NetCoordinates coordinates, PowerMonitoringConsoleGroup group, string spritePath, string spriteState)
{
	public string EntityName = name;

	public NetCoordinates Coordinates = coordinates;

	public PowerMonitoringConsoleGroup Group = group;

	public string SpritePath = spritePath;

	public string SpriteState = spriteState;

	public NetEntity? CollectionMaster = null;
}
