// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.MsgDeleteCharacter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Content.Shared.Preferences;

public sealed class MsgDeleteCharacter : NetMessage
{
  public int Slot;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Slot = ((NetBuffer) buffer).ReadInt32();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Slot);
  }
}
