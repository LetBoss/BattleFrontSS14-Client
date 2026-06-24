// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.MetaDataFlags
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.GameObjects;

[Flags]
public enum MetaDataFlags : byte
{
  None = 0,
  SessionSpecific = 1,
  InContainer = 2,
  Detached = 4,
  Undetachable = 8,
  PvsPriority = 16, // 0x10
  ExtraTransformEvents = 32, // 0x20
}
