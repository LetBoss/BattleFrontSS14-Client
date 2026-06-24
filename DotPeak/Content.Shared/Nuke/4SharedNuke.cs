// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nuke.NukeStatus
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable disable
namespace Content.Shared.Nuke;

public enum NukeStatus : byte
{
  AWAIT_DISK,
  AWAIT_CODE,
  AWAIT_ARM,
  ARMED,
  COOLDOWN,
}
