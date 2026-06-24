// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.PathfindingBreadcrumbFlag
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.NPC;

[Flags]
public enum PathfindingBreadcrumbFlag : ushort
{
  None = 0,
  Invalid = 1,
  Space = 2,
  Door = 4,
  Access = 8,
  Climb = 16, // 0x0010
}
