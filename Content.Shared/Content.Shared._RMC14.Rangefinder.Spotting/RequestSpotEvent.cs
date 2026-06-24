using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Rangefinder.Spotting;

[Serializable]
[NetSerializable]
public sealed class RequestSpotEvent : EntityEventArgs
{
	public NetEntity SpottingTool;

	public NetEntity User;

	public NetEntity Target;
}
