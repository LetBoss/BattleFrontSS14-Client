// Decompiled with JetBrains decompiler
// Type: Content.Shared.Payload.Components.ChemicalPayloadFilledSlots
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Payload.Components;

[Flags]
[NetSerializable]
[Serializable]
public enum ChemicalPayloadFilledSlots : byte
{
  None = 0,
  Left = 1,
  Right = 2,
  Both = Right | Left, // 0x03
}
