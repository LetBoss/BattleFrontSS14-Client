using System;
using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorMessagesReceivedMsg : NetMessage
{
	public List<MentorMessage> Messages = new List<MentorMessage>();

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int count = ((NetBuffer)buffer).ReadInt32();
		Messages.EnsureCapacity(count);
		NetUserId destination = default(NetUserId);
		for (int i = 0; i < count; i++)
		{
			((NetUserId)(ref destination))._002Ector(NetMessageExt.ReadGuid(buffer));
			string destinationName = ((NetBuffer)buffer).ReadString();
			bool num = ((NetBuffer)buffer).ReadBoolean();
			NetUserId? author = null;
			if (num)
			{
				author = new NetUserId(NetMessageExt.ReadGuid(buffer));
			}
			string authorName = ((NetBuffer)buffer).ReadString();
			string text = ((NetBuffer)buffer).ReadString();
			DateTime time = DateTime.FromBinary(((NetBuffer)buffer).ReadInt64());
			bool isMentor = ((NetBuffer)buffer).ReadBoolean();
			bool isAdmin = ((NetBuffer)buffer).ReadBoolean();
			bool create = ((NetBuffer)buffer).ReadBoolean();
			MentorMessage message = new MentorMessage(destination, destinationName, author, authorName, text, time, isMentor, isAdmin, create);
			Messages.Add(message);
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		((NetBuffer)buffer).Write(Messages.Count);
		foreach (MentorMessage message in Messages)
		{
			NetMessageExt.Write(buffer, message.Destination.UserId);
			((NetBuffer)buffer).Write(message.DestinationName);
			if (message.Author.HasValue)
			{
				((NetBuffer)buffer).Write(true);
				NetMessageExt.Write(buffer, NetUserId.op_Implicit(message.Author.Value));
			}
			else
			{
				((NetBuffer)buffer).Write(false);
			}
			((NetBuffer)buffer).Write(message.AuthorName);
			((NetBuffer)buffer).Write(message.Text);
			((NetBuffer)buffer).Write(message.Time.ToBinary());
			((NetBuffer)buffer).Write(message.IsMentor);
			((NetBuffer)buffer).Write(message.IsAdmin);
			((NetBuffer)buffer).Write(message.Create);
		}
	}
}
