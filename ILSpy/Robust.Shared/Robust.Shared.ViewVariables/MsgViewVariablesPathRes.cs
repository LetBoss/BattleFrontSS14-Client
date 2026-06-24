using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPathRes : MsgViewVariablesPath
{
	public string[] Response { get; set; } = Array.Empty<string>();

	public ViewVariablesResponseCode ResponseCode { get; set; } = ViewVariablesResponseCode.Ok;

	internal MsgViewVariablesPathRes()
	{
	}

	internal MsgViewVariablesPathRes(MsgViewVariablesPathReq req)
	{
		base.Path = req.Path;
		base.RequestId = req.RequestId;
	}

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		base.ReadFromBuffer(buffer, serializer);
		ResponseCode = (ViewVariablesResponseCode)((NetBuffer)buffer).ReadUInt16();
		int num = ((NetBuffer)buffer).ReadInt32();
		Response = new string[num];
		for (int i = 0; i < num; i++)
		{
			Response[i] = ((NetBuffer)buffer).ReadString();
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		base.WriteToBuffer(buffer, serializer);
		((NetBuffer)buffer).Write((ushort)ResponseCode);
		((NetBuffer)buffer).Write(Response.Length);
		string[] response = Response;
		foreach (string text in response)
		{
			((NetBuffer)buffer).Write(text);
		}
	}
}
