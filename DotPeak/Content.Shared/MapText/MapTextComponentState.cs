// Decompiled with JetBrains decompiler
// Type: Content.Shared.MapText.MapTextComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.MapText;

[NetSerializable]
[Serializable]
public sealed class MapTextComponentState : ComponentState
{
  public string? Text { get; init; }

  public LocId LocText { get; init; }

  public Color Color { get; init; }

  public string FontId { get; init; }

  public int FontSize { get; init; }

  public Vector2 Offset { get; init; }
}
