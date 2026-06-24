// Decompiled with JetBrains decompiler
// Type: Content.Client.Placement.Modes.WallmountLight
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Placement;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Placement.Modes;

public sealed class WallmountLight(PlacementManager pMan) : PlacementMode(pMan)
{
  public virtual void AlignPlacementMode(ScreenCoordinates mouseScreen)
  {
    this.MouseCoords = this.ScreenToCursorGrid(mouseScreen);
    this.CurrentTile = this.GetTileRef(this.MouseCoords);
    if (this.pManager.CurrentPermission.IsTile)
      return;
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(this.MouseCoords.EntityId, Vector2i.op_Implicit(this.CurrentTile.GridIndices));
    Vector2 vector2;
    switch ((int) this.pManager.Direction)
    {
      case 0:
        vector2 = new Vector2(0.5f, 0.0f);
        break;
      case 1:
        return;
      case 2:
        vector2 = new Vector2(1f, 0.5f);
        break;
      case 3:
        return;
      case 4:
        vector2 = new Vector2(0.5f, 1f);
        break;
      case 5:
        return;
      case 6:
        vector2 = new Vector2(0.0f, 0.5f);
        break;
      default:
        return;
    }
    entityCoordinates = ((EntityCoordinates) ref entityCoordinates).Offset(vector2);
    this.MouseCoords = entityCoordinates;
  }

  public virtual bool IsValidPosition(EntityCoordinates position)
  {
    return !this.pManager.CurrentPermission.IsTile && this.RangeCheck(position);
  }
}
