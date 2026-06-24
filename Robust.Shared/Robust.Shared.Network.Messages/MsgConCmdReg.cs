using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgConCmdReg : NetMessage
{
	public sealed class Command
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Help { get; set; }
	}

	public override MsgGroups MsgGroup => MsgGroups.String;

	public List<Command> Commands { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		ushort num = ((NetBuffer)buffer).ReadUInt16();
		Commands = new List<Command>(num);
		for (int i = 0; i < num; i++)
		{
			Commands.Add(new Command
			{
				Name = ((NetBuffer)buffer).ReadString(),
				Description = ((NetBuffer)buffer).ReadString(),
				Help = ((NetBuffer)buffer).ReadString()
			});
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		if (Commands == null)
		{
			((NetBuffer)buffer).Write((ushort)0);
			return;
		}
		((NetBuffer)buffer).Write((ushort)Commands.Count);
		foreach (Command command in Commands)
		{
			((NetBuffer)buffer).Write(command.Name);
			((NetBuffer)buffer).Write(command.Description);
			((NetBuffer)buffer).Write(command.Help);
		}
	}
}
