// Decompiled with JetBrains decompiler
// Type: Content.Shared.Info.SendRulesInformationMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Content.Shared.Info;

public sealed class SendRulesInformationMessage : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public float PopupTime { get; set; }

  public string CoreRules { get; set; } = string.Empty;

  public bool ShouldShowRules { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.PopupTime = ((NetBuffer) buffer).ReadFloat();
    this.CoreRules = ((NetBuffer) buffer).ReadString();
    this.ShouldShowRules = ((NetBuffer) buffer).ReadBoolean();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.PopupTime);
    ((NetBuffer) buffer).Write(this.CoreRules);
    ((NetBuffer) buffer).Write(this.ShouldShowRules);
  }
}
