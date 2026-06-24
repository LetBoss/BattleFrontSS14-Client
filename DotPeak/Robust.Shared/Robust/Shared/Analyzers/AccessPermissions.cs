// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Analyzers.AccessPermissions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Analyzers;

[Flags]
public enum AccessPermissions : byte
{
  None = 0,
  Read = 1,
  Write = 2,
  Execute = 4,
  ReadWrite = Write | Read, // 0x03
  ReadExecute = Execute | Read, // 0x05
  WriteExecute = Execute | Write, // 0x06
  ReadWriteExecute = WriteExecute | Read, // 0x07
}
