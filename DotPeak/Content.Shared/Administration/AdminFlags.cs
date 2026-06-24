// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.AdminFlags
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Administration;

[Flags]
public enum AdminFlags : uint
{
  None = 0,
  Admin = 1,
  Ban = 2,
  Debug = 4,
  Fun = 8,
  Permissions = 16, // 0x00000010
  Server = 32, // 0x00000020
  Spawn = 64, // 0x00000040
  VarEdit = 128, // 0x00000080
  Mapping = 256, // 0x00000100
  Logs = 512, // 0x00000200
  Round = 1024, // 0x00000400
  Query = 2048, // 0x00000800
  Adminhelp = 4096, // 0x00001000
  ViewNotes = 8192, // 0x00002000
  EditNotes = 16384, // 0x00004000
  MassBan = 32768, // 0x00008000
  Stealth = 65536, // 0x00010000
  Adminchat = 131072, // 0x00020000
  Pii = 262144, // 0x00040000
  Moderator = 524288, // 0x00080000
  AdminWho = 1048576, // 0x00100000
  NameColor = 2097152, // 0x00200000
  Anticheat = 4194304, // 0x00400000
  MentorHelp = 1073741824, // 0x40000000
  Host = 2147483648, // 0x80000000
}
