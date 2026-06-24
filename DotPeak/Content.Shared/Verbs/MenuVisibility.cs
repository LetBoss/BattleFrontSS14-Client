// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.MenuVisibility
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Verbs;

[Flags]
public enum MenuVisibility
{
  Default = 0,
  NoFov = 1,
  InContainer = 2,
  Invisible = 4,
  All = Invisible | InContainer | NoFov, // 0x00000007
}
