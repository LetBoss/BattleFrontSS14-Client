using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Telephone;

[Serializable]
[NetSerializable]
public sealed class RMCTelephoneCallBuiMsg(NetEntity id) : BoundUserInterfaceMessage
{
	public readonly NetEntity Id = id;
}
