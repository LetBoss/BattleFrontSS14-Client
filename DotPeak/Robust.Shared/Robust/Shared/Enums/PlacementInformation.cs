// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Enums.PlacementInformation
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Robust.Shared.Enums;

public sealed class PlacementInformation
{
  public string? EntityType { get; set; }

  public bool IsTile { get; set; }

  public EntityUid MobUid { get; set; }

  public string? PlacementOption { get; set; }

  public int Range { get; set; }

  public int TileType { get; set; }

  public int Uses { get; set; } = 1;

  public bool UseEditorContext { get; set; } = true;
}
