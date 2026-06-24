// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesResponseCode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.ViewVariables;

public enum ViewVariablesResponseCode : ushort
{
  Ok = 200, // 0x00C8
  InvalidRequest = 400, // 0x0190
  NoAccess = 401, // 0x0191
  NoObject = 404, // 0x0194
}
