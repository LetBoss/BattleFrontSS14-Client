// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.Systems.EmergencyShuttleOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Shuttles.Systems;

public sealed class EmergencyShuttleOverlay : Overlay
{
  private readonly EntityQuery<TransformComponent> _transformQuery;
  private readonly SharedTransformSystem _transformSystem;
  public EntityUid? StationUid;
  public Box2? Position;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public EmergencyShuttleOverlay(
    EntityQuery<TransformComponent> transformQuery,
    SharedTransformSystem transformSystem)
  {
    this._transformQuery = transformQuery;
    this._transformSystem = transformSystem;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    TransformComponent transformComponent;
    if (!this.Position.HasValue || !this._transformQuery.TryGetComponent(this.StationUid, ref transformComponent))
      return;
    DrawingHandleWorld worldHandle1 = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 worldMatrix = this._transformSystem.GetWorldMatrix(transformComponent);
    ref Matrix3x2 local1 = ref worldMatrix;
    ((DrawingHandleBase) worldHandle1).SetTransform(ref local1);
    DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2 box2 = this.Position.Value;
    Color red = Color.Red;
    Color color = ((Color) ref red).WithAlpha((byte) 100);
    worldHandle2.DrawRect(box2, color, true);
    DrawingHandleWorld worldHandle3 = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local2 = ref identity;
    ((DrawingHandleBase) worldHandle3).SetTransform(ref local2);
  }
}
