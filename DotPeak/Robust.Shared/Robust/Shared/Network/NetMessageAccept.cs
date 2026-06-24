// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetMessageAccept
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Network;

[Flags]
public enum NetMessageAccept : byte
{
  None = 0,
  Server = 1,
  Client = 2,
  Both = Client | Server, // 0x03
  Handshake = 4,
}
