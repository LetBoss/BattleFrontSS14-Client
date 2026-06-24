// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVar
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Configuration;

[Flags]
public enum CVar : short
{
  NONE = 0,
  CHEAT = 1,
  SERVER = 2,
  NOT_CONNECTED = 4,
  REPLICATED = 8,
  ARCHIVE = 16, // 0x0010
  NOTIFY = 32, // 0x0020
  SERVERONLY = 64, // 0x0040
  CLIENTONLY = 128, // 0x0080
  CONFIDENTIAL = 256, // 0x0100
  CLIENT = 512, // 0x0200
}
