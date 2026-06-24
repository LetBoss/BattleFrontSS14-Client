// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.ShuttleButtons
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Movement.Systems;

[Flags]
public enum ShuttleButtons : byte
{
  None = 0,
  StrafeUp = 1,
  StrafeDown = 2,
  StrafeLeft = 4,
  StrafeRight = 8,
  RotateLeft = 16, // 0x10
  RotateRight = 32, // 0x20
  Brake = 64, // 0x40
}
