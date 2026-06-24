// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.NavMapBeaconConfigureBuiMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Pinpointer;

[NetSerializable]
[Serializable]
public sealed class NavMapBeaconConfigureBuiMessage : BoundUserInterfaceMessage
{
  public string? Text;
  public bool Enabled;
  public Color Color;

  public NavMapBeaconConfigureBuiMessage(string? text, bool enabled, Color color)
  {
    this.Text = text;
    this.Enabled = enabled;
    this.Color = color;
  }
}
