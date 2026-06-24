using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPathReq : MsgViewVariablesPath
{
	public Guid Session { get; set; } = Guid.Empty;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		base.ReadFromBuffer(buffer, serializer);
		Session = buffer.ReadGuid();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		base.WriteToBuffer(buffer, serializer);
		buffer.Write(Session);
	}
}
