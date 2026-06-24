// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgPlacement
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
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
    this.PlaceType = (PlacementManagerMessage) ((NetBuffer) buffer).ReadByte();
    switch (this.PlaceType)
    {
      case PlacementManagerMessage.StartPlacement:
        this.Range = ((NetBuffer) buffer).ReadInt32();
        this.IsTile = ((NetBuffer) buffer).ReadBoolean();
        this.ObjType = ((NetBuffer) buffer).ReadString();
        this.AlignOption = ((NetBuffer) buffer).ReadString();
        break;
      case PlacementManagerMessage.CancelPlacement:
      case PlacementManagerMessage.PlacementFailed:
        throw new NotImplementedException();
      case PlacementManagerMessage.RequestPlacement:
        this.Align = ((NetBuffer) buffer).ReadString();
        this.IsTile = ((NetBuffer) buffer).ReadBoolean();
        this.Replacement = ((NetBuffer) buffer).ReadBoolean();
        if (this.IsTile)
          this.TileType = ((NetBuffer) buffer).ReadInt32();
        else
          this.EntityTemplateName = ((NetBuffer) buffer).ReadString();
        this.NetCoordinates = buffer.ReadNetCoordinates();
        this.DirRcv = (Direction) (int) (sbyte) ((NetBuffer) buffer).ReadByte();
        this.Mirrored = ((NetBuffer) buffer).ReadBoolean();
        break;
      case PlacementManagerMessage.RequestEntRemove:
        this.EntityUid = new NetEntity(((NetBuffer) buffer).ReadInt32());
        break;
      case PlacementManagerMessage.RequestRectRemove:
        this.NetCoordinates = buffer.ReadNetCoordinates();
        this.RectSize = buffer.ReadVector2();
        break;
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write((byte) this.PlaceType);
    switch (this.PlaceType)
    {
      case PlacementManagerMessage.StartPlacement:
        ((NetBuffer) buffer).Write(this.Range);
        ((NetBuffer) buffer).Write(this.IsTile);
        ((NetBuffer) buffer).Write(this.ObjType);
        ((NetBuffer) buffer).Write(this.AlignOption);
        break;
      case PlacementManagerMessage.CancelPlacement:
      case PlacementManagerMessage.PlacementFailed:
        throw new NotImplementedException();
      case PlacementManagerMessage.RequestPlacement:
        ((NetBuffer) buffer).Write(this.Align);
        ((NetBuffer) buffer).Write(this.IsTile);
        ((NetBuffer) buffer).Write(this.Replacement);
        if (this.IsTile)
          ((NetBuffer) buffer).Write(this.TileType);
        else
          ((NetBuffer) buffer).Write(this.EntityTemplateName);
        buffer.Write(this.NetCoordinates);
        ((NetBuffer) buffer).Write((byte) this.DirRcv);
        ((NetBuffer) buffer).Write(this.Mirrored);
        break;
      case PlacementManagerMessage.RequestEntRemove:
        ((NetBuffer) buffer).Write((int) this.EntityUid);
        break;
      case PlacementManagerMessage.RequestRectRemove:
        buffer.Write(this.NetCoordinates);
        buffer.Write(this.RectSize);
        break;
    }
  }
}
