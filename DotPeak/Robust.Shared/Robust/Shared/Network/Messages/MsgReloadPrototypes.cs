// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgReloadPrototypes
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Network.Messages;

public sealed class MsgReloadPrototypes : NetMessage
{
  public ResPath[] Paths;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int length = ((NetBuffer) buffer).ReadInt32();
    this.Paths = new ResPath[length];
    for (int index = 0; index < length; ++index)
      this.Paths[index] = new ResPath(((NetBuffer) buffer).ReadString());
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Paths.Length);
    foreach (ResPath path in this.Paths)
      ((NetBuffer) buffer).Write(path.ToString());
  }
}
