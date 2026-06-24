// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Contacts.ContactFlags
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Physics.Dynamics.Contacts;

[Flags]
internal enum ContactFlags : byte
{
  None = 0,
  PreInit = 1,
  Island = 2,
  Filter = 4,
  Grid = 8,
  Deleting = 16, // 0x10
  Deleted = 32, // 0x20
}
