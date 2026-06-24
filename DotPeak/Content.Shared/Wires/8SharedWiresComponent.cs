// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.StatusLightData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Wires;

[NetSerializable]
[Serializable]
public struct StatusLightData(Color color, StatusLightState state, string text)
{
  public Color Color { get; } = color;

  public StatusLightState State { get; } = state;

  public string Text { get; } = text;

  public override string ToString()
  {
    return $"Color: {this.Color}, State: {this.State}, Text: {this.Text}";
  }
}
