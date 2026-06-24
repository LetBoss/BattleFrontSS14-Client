// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.VisibilityFlags
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Eye;

[Flags]
[FlagsFor(typeof (VisibilityMaskLayer))]
public enum VisibilityFlags
{
  None = 0,
  Normal = 1,
  Ghost = 2,
  Subfloor = 4,
  Commander = 8,
  Xeno = 32768, // 0x00008000
}
