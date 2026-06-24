// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.LookupFlags
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.GameObjects;

[Flags]
public enum LookupFlags : byte
{
  None = 0,
  Approximate = 1,
  Dynamic = 2,
  Static = 4,
  Sundries = 8,
  Contained = 32, // 0x20
  Sensors = 64, // 0x40
  Uncontained = Sensors | Sundries | Static | Dynamic, // 0x4E
  StaticSundries = Sundries | Static, // 0x0C
  All = StaticSundries | Sensors | Contained | Dynamic, // 0x6E
}
