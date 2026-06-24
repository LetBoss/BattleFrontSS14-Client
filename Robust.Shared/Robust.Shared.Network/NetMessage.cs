using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network;

public abstract class NetMessage
{
	public virtual string MsgName { get; }

	public virtual MsgGroups MsgGroup { get; }

	public INetChannel MsgChannel { get; set; }

	public int MsgSize { get; set; }

	public virtual NetDeliveryMethod DeliveryMethod
	{
		get
		{
			switch (MsgGroup)
			{
			case MsgGroups.Entity:
				return (NetDeliveryMethod)1;
			case MsgGroups.Core:
			case MsgGroups.Command:
				return (NetDeliveryMethod)34;
			case MsgGroups.String:
			case MsgGroups.EntityEvent:
				return (NetDeliveryMethod)67;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public virtual int SequenceChannel => 0;

	protected NetMessage()
	{
		MsgName = GetType().Name;
	}

	public abstract void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer);

	public abstract void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer);
}
