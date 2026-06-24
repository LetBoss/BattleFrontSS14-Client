// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.MsgUpdateCharacter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.IO;

#nullable enable
namespace Content.Shared.Preferences;

public sealed class MsgUpdateCharacter : NetMessage
{
  public int Slot;
  public ICharacterProfile Profile;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Slot = ((NetBuffer) buffer).ReadInt32();
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memStream = new MemoryStream(num))
    {
      buffer.ReadAlignedMemory(memStream, num);
      this.Profile = serializer.Deserialize<ICharacterProfile>((Stream) memStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Slot);
    using (MemoryStream memoryStream = new MemoryStream())
    {
      serializer.Serialize((Stream) memoryStream, (object) this.Profile);
      ((NetBuffer) buffer).WriteVariableInt32((int) memoryStream.Length);
      ArraySegment<byte> buffer1;
      memoryStream.TryGetBuffer(out buffer1);
      ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) buffer1);
    }
  }
}
