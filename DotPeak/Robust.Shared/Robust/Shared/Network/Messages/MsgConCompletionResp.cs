// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgConCompletionResp
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Console;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Network.Messages;

public sealed class MsgConCompletionResp : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public int Seq { get; set; }

  public CompletionResult Result { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Seq = ((NetBuffer) buffer).ReadInt32();
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    CompletionOption[] Options = new CompletionOption[length];
    for (int index = 0; index < length; ++index)
    {
      string str1 = ((NetBuffer) buffer).ReadString();
      string str2 = ((NetBuffer) buffer).ReadString();
      int Flags = ((NetBuffer) buffer).ReadInt32();
      Options[index] = new CompletionOption(str1, str2 == "" ? (string) null : str2, (CompletionOptionFlags) Flags);
    }
    string str = ((NetBuffer) buffer).ReadString();
    this.Result = new CompletionResult(Options, str == "" ? (string) null : str);
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Seq);
    ((NetBuffer) buffer).WriteVariableInt32(this.Result.Options.Length);
    foreach (CompletionOption option in this.Result.Options)
    {
      ((NetBuffer) buffer).Write(option.Value);
      ((NetBuffer) buffer).Write(option.Hint);
      ((NetBuffer) buffer).Write((int) option.Flags);
    }
    ((NetBuffer) buffer).Write(this.Result.Hint);
  }
}
