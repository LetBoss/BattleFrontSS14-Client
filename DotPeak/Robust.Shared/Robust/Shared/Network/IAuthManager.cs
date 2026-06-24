// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.IAuthManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Network;

internal interface IAuthManager
{
  NetUserId? UserId { get; set; }

  string? Server { get; set; }

  string? Token { get; set; }

  string? PubKey { get; set; }

  bool AllowHwid { get; set; }

  void LoadFromEnv();
}
