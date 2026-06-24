// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.SelectiveFire
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Weapons.Ranged.Components;

[Flags]
public enum SelectiveFire : byte
{
  Invalid = 0,
  SemiAuto = 1,
  Burst = 2,
  FullAuto = 4,
}
