// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Enums.OverlaySpace
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Enums;

[Flags]
public enum OverlaySpace : ushort
{
  None = 0,
  ScreenSpace = 2,
  WorldSpace = 4,
  WorldSpaceBelowFOV = 8,
  WorldSpaceEntities = 16, // 0x0010
  WorldSpaceGrids = 32, // 0x0020
  WorldSpaceBelowEntities = 64, // 0x0040
  ScreenSpaceBelowWorld = 128, // 0x0080
  WorldSpaceBelowWorld = 256, // 0x0100
  BeforeLighting = 512, // 0x0200
}
