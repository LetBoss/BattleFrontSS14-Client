using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

public sealed class MsgUpdateAdminStatus : NetMessage
{
	public AdminData? Admin;

	public string[] AvailableCommands = Array.Empty<string>();

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int count = ((NetBuffer)buffer).ReadVariableInt32();
		AvailableCommands = new string[count];
		for (int i = 0; i < count; i++)
		{
			AvailableCommands[i] = ((NetBuffer)buffer).ReadString();
		}
		if (((NetBuffer)buffer).ReadBoolean())
		{
			bool active = ((NetBuffer)buffer).ReadBoolean();
			((NetBuffer)buffer).ReadPadBits();
			AdminFlags flags = (AdminFlags)((NetBuffer)buffer).ReadUInt32();
			string title = ((NetBuffer)buffer).ReadString();
			Admin = new AdminData
			{
				Active = active,
				Title = title,
				Flags = flags
			};
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(AvailableCommands.Length);
		string[] availableCommands = AvailableCommands;
		foreach (string cmd in availableCommands)
		{
			((NetBuffer)buffer).Write(cmd);
		}
		((NetBuffer)buffer).Write(Admin != null);
		if (Admin != null)
		{
			((NetBuffer)buffer).Write(Admin.Active);
			((NetBuffer)buffer).WritePadBits();
			((NetBuffer)buffer).Write((uint)Admin.Flags);
			((NetBuffer)buffer).Write(Admin.Title);
		}
	}
}
