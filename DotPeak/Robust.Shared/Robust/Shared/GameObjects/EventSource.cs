// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EventSource
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.GameObjects;

[Flags]
public enum EventSource : byte
{
  None = 0,
  Local = 1,
  Network = 2,
  All = Network | Local, // 0x03
}
