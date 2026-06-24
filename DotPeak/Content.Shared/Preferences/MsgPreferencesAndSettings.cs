// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.MsgPreferencesAndSettings
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

public sealed class MsgPreferencesAndSettings : NetMessage
{
  public PlayerPreferences Preferences;
  public GameSettings Settings;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int length1 = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memStream = new MemoryStream())
    {
      buffer.ReadAlignedMemory(memStream, length1);
      serializer.DeserializeDirect<PlayerPreferences>((Stream) memStream, out this.Preferences);
    }
    int length2 = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memStream = new MemoryStream())
    {
      buffer.ReadAlignedMemory(memStream, length2);
      serializer.DeserializeDirect<GameSettings>((Stream) memStream, out this.Settings);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      serializer.SerializeDirect<PlayerPreferences>((Stream) memoryStream, this.Preferences);
      ((NetBuffer) buffer).WriteVariableInt32((int) memoryStream.Length);
      ArraySegment<byte> buffer1;
      memoryStream.TryGetBuffer(out buffer1);
      ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) buffer1);
    }
    using (MemoryStream memoryStream = new MemoryStream())
    {
      serializer.SerializeDirect<GameSettings>((Stream) memoryStream, this.Settings);
      ((NetBuffer) buffer).WriteVariableInt32((int) memoryStream.Length);
      ArraySegment<byte> buffer2;
      memoryStream.TryGetBuffer(out buffer2);
      ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) buffer2);
    }
  }
}
