// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Upload.GamePrototypeLoadMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Upload;

public sealed class GamePrototypeLoadMessage : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public string PrototypeData { get; set; } = string.Empty;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.PrototypeData = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.PrototypeData);
  }
}
