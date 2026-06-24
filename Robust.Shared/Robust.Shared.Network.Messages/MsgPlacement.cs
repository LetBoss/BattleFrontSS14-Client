using System;
using System.Numerics;
using Lidgren.Network;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgPlacement : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public PlacementManagerMessage PlaceType { get; set; }

	public string Align { get; set; }

	public bool Replacement { get; set; }

	public bool IsTile { get; set; }

	public int TileType { get; set; }

	public string EntityTemplateName { get; set; }

	public NetCoordinates NetCoordinates { get; set; }

	public Direction DirRcv { get; set; }

	public NetEntity EntityUid { get; set; }

	public int Range { get; set; }

	public string ObjType { get; set; }

	public string AlignOption { get; set; }

	public Vector2 RectSize { get; set; }

	public bool Mirrored { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		PlaceType = (PlacementManagerMessage)((NetBuffer)buffer).ReadByte();
		switch (PlaceType)
		{
		case PlacementManagerMessage.RequestPlacement:
			Align = ((NetBuffer)buffer).ReadString();
			IsTile = ((NetBuffer)buffer).ReadBoolean();
			Replacement = ((NetBuffer)buffer).ReadBoolean();
			if (IsTile)
			{
				TileType = ((NetBuffer)buffer).ReadInt32();
			}
			else
			{
				EntityTemplateName = ((NetBuffer)buffer).ReadString();
			}
			NetCoordinates = buffer.ReadNetCoordinates();
			DirRcv = (Direction)(sbyte)((NetBuffer)buffer).ReadByte();
			Mirrored = ((NetBuffer)buffer).ReadBoolean();
			break;
		case PlacementManagerMessage.StartPlacement:
			Range = ((NetBuffer)buffer).ReadInt32();
			IsTile = ((NetBuffer)buffer).ReadBoolean();
			ObjType = ((NetBuffer)buffer).ReadString();
			AlignOption = ((NetBuffer)buffer).ReadString();
			break;
		case PlacementManagerMessage.CancelPlacement:
		case PlacementManagerMessage.PlacementFailed:
			throw new NotImplementedException();
		case PlacementManagerMessage.RequestEntRemove:
			EntityUid = new NetEntity(((NetBuffer)buffer).ReadInt32());
			break;
		case PlacementManagerMessage.RequestRectRemove:
			NetCoordinates = buffer.ReadNetCoordinates();
			RectSize = buffer.ReadVector2();
			break;
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		((NetBuffer)buffer).Write((byte)PlaceType);
		switch (PlaceType)
		{
		case PlacementManagerMessage.RequestPlacement:
			((NetBuffer)buffer).Write(Align);
			((NetBuffer)buffer).Write(IsTile);
			((NetBuffer)buffer).Write(Replacement);
			if (IsTile)
			{
				((NetBuffer)buffer).Write(TileType);
			}
			else
			{
				((NetBuffer)buffer).Write(EntityTemplateName);
			}
			buffer.Write(NetCoordinates);
			((NetBuffer)buffer).Write((byte)DirRcv);
			((NetBuffer)buffer).Write(Mirrored);
			break;
		case PlacementManagerMessage.StartPlacement:
			((NetBuffer)buffer).Write(Range);
			((NetBuffer)buffer).Write(IsTile);
			((NetBuffer)buffer).Write(ObjType);
			((NetBuffer)buffer).Write(AlignOption);
			break;
		case PlacementManagerMessage.CancelPlacement:
		case PlacementManagerMessage.PlacementFailed:
			throw new NotImplementedException();
		case PlacementManagerMessage.RequestEntRemove:
			((NetBuffer)buffer).Write((int)EntityUid);
			break;
		case PlacementManagerMessage.RequestRectRemove:
			buffer.Write(NetCoordinates);
			buffer.Write(RectSize);
			break;
		}
	}
}
