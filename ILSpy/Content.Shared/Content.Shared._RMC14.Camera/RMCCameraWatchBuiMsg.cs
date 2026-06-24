using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Camera;

[Serializable]
[NetSerializable]
public sealed class RMCCameraWatchBuiMsg(NetEntity camera) : BoundUserInterfaceMessage
{
	public readonly NetEntity Camera = camera;
}
