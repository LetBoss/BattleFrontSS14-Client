// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.ChatSelectChannel
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Chat;

[Flags]
public enum ChatSelectChannel : uint
{
  None = 0,
  Local = 1,
  Whisper = 2,
  Radio = 16, // 0x00000010
  LOOC = 32, // 0x00000020
  OOC = 64, // 0x00000040
  Emotes = 512, // 0x00000200
  Dead = 1024, // 0x00000400
  Admin = 8192, // 0x00002000
  Mentor = 32768, // 0x00008000
  Party = 65536, // 0x00010000
  MiniGame = 131072, // 0x00020000
  Lobby = 262144, // 0x00040000
  Console = 16384, // 0x00004000
}
