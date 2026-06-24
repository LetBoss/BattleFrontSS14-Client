// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.SlotFlags
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Inventory;

[NetSerializable]
[Flags]
[Serializable]
public enum SlotFlags
{
  NONE = 0,
  PREVENTEQUIP = 1,
  HEAD = 2,
  EYES = 4,
  EARS = 8,
  MASK = 16, // 0x00000010
  OUTERCLOTHING = 32, // 0x00000020
  INNERCLOTHING = 64, // 0x00000040
  NECK = 128, // 0x00000080
  BACK = 256, // 0x00000100
  BELT = 512, // 0x00000200
  GLOVES = 1024, // 0x00000400
  IDCARD = 2048, // 0x00000800
  POCKET = 4096, // 0x00001000
  LEGS = 8192, // 0x00002000
  FEET = 16384, // 0x00004000
  SUITSTORAGE = 32768, // 0x00008000
  All = -1, // 0xFFFFFFFF
  WITHOUT_POCKET = -4097, // 0xFFFFEFFF
}
