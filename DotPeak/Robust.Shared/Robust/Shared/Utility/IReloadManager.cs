// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.IReloadManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Utility;

internal interface IReloadManager
{
  event Action<ResPath>? OnChanged;

  internal void Register(string directory, string filter);

  internal void Register(ResPath directory, string filter);

  void Initialize();
}
