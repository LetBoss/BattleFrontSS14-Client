// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.MoveButtons
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Movement.Systems;

[Flags]
[NetSerializable]
[Serializable]
public enum MoveButtons : byte
{
  None = 0,
  Up = 1,
  Down = 2,
  Left = 4,
  Right = 8,
  Walk = 16, // 0x10
  AnyDirection = Right | Left | Down | Up, // 0x0F
}
