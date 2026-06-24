// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.AppearanceComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
public sealed class AppearanceComponentState : ComponentState
{
  public readonly Dictionary<Enum, object> Data;

  public AppearanceComponentState(Dictionary<Enum, object> data) => this.Data = data;
}
