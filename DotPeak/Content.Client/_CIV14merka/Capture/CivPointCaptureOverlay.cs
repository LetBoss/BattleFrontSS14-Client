// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Capture.CivPointCaptureOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.UserInterface.Systems.Hud;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Client._CIV14merka.Capture;

public sealed class CivPointCaptureOverlay : Overlay
{
  private readonly IEntityManager _entity;
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly List<Vector2> _arcPoints = new List<Vector2>();

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public CivPointCaptureOverlay(IEntityManager entity, IPlayerManager player)
  {
    IoCManager.InjectDependencies<CivPointCaptureOverlay>(this);
    this._entity = entity;
    this._player = player;
    this._transform = entity.System<SharedTransformSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    int viewerTeamId;
    if (!this.TryGetViewerTeamId(out viewerTeamId))
      return;
    AllEntityQueryEnumerator<CivPointCapturePointComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<CivPointCapturePointComponent, TransformComponent>();
    EntityUid entityUid;
    CivPointCapturePointComponent capturePointComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref capturePointComponent, ref transformComponent))
    {
      if (!MapId.op_Equality(transformComponent.MapID, MapId.Nullspace) && !MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Vector2 position = this._transform.GetMapCoordinates(transformComponent).Position;
        float num1 = Math.Max(0.5f, capturePointComponent.CaptureRadius);
        if (CivPointCaptureOverlay.CircleIntersectsWorld(args.WorldAABB, position, num1 + 0.5f))
        {
          Color relationColor1 = CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, capturePointComponent.OwnerTeamId);
          int num2 = capturePointComponent.CapturingTeamId == 0 ? 0 : ((double) capturePointComponent.CaptureProgress > 0.0 ? 1 : 0);
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawCircle(position, num1, ((Color) ref relationColor1).WithAlpha(0.1f), true);
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawCircle(position, num1, ((Color) ref relationColor1).WithAlpha(0.95f), false);
          DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
          Vector2 vector2 = position;
          double num3 = (double) num1 + 0.079999998211860657;
          Color black = Color.Black;
          Color color = ((Color) ref black).WithAlpha(0.65f);
          ((DrawingHandleBase) worldHandle).DrawCircle(vector2, (float) num3, color, false);
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawCircle(position, 0.18f, ((Color) ref relationColor1).WithAlpha(0.82f), true);
          if (num2 != 0)
          {
            Color relationColor2 = CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, capturePointComponent.CapturingTeamId);
            this.DrawProgressArc(((OverlayDrawArgs) ref args).WorldHandle, position, num1 + 0.18f, capturePointComponent.CaptureProgress, relationColor2);
          }
        }
      }
    }
  }

  private bool TryGetViewerTeamId(out int viewerTeamId)
  {
    viewerTeamId = 0;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return false;
    CivTeamMemberComponent teamMemberComponent;
    if (this._entity.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent))
    {
      viewerTeamId = teamMemberComponent.TeamId;
      return viewerTeamId > 0;
    }
    if (!this._entity.HasComponent<GhostComponent>(localEntity.Value))
      return false;
    CivHudEventsSystem civHudEventsSystem = this._entity.System<CivHudEventsSystem>();
    ref int local = ref viewerTeamId;
    CivHudStatusEvent lastStatus = civHudEventsSystem.LastStatus;
    int viewerTeamId1 = lastStatus != null ? lastStatus.ViewerTeamId : 0;
    local = viewerTeamId1;
    return viewerTeamId > 0;
  }

  private void DrawProgressArc(
    DrawingHandleWorld handle,
    Vector2 center,
    float radius,
    float progress,
    Color color)
  {
    progress = Math.Clamp(progress, 0.0f, 1f);
    if ((double) progress <= 0.0)
      return;
    int num = Math.Max(6, (int) MathF.Ceiling(28f * progress));
    this._arcPoints.Clear();
    for (int index = 0; index <= num; ++index)
    {
      float x = (float) (6.2831854820251465 * (double) (progress * (float) index / (float) num) - 1.5707963705062866);
      this._arcPoints.Add(center + new Vector2(MathF.Cos(x), MathF.Sin(x)) * radius);
    }
    if (this._arcPoints.Count < 2)
      return;
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) CollectionsMarshal.AsSpan<Vector2>(this._arcPoints), color);
  }

  private static bool CircleIntersectsWorld(Box2 worldBounds, Vector2 center, float radius)
  {
    float x = Math.Clamp(center.X, worldBounds.Left, worldBounds.Right);
    float y = Math.Clamp(center.Y, worldBounds.Bottom, worldBounds.Top);
    return (double) (center - new Vector2(x, y)).LengthSquared() <= (double) radius * (double) radius;
  }
}
