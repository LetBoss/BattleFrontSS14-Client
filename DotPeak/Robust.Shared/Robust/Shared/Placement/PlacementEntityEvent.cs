// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Placement.PlacementEntityEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Network;

#nullable disable
namespace Robust.Shared.Placement;

public readonly struct PlacementEntityEvent(
  EntityUid editedEntity,
  EntityCoordinates coordinates,
  PlacementEventAction placementEventAction,
  NetUserId? placerNetUserId)
{
  public readonly EntityUid EditedEntity = editedEntity;
  public readonly EntityCoordinates Coordinates = coordinates;
  public readonly PlacementEventAction PlacementEventAction = placementEventAction;
  public readonly NetUserId? PlacerNetUserId = placerNetUserId;
}
