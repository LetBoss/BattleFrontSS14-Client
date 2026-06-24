// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Pvo.CivPvoOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Factions;
using Content.Shared._CIV14merka.Pvo;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoOverlay : Overlay
{
  private static readonly Color FallbackColor = Color.FromHex((ReadOnlySpan<char>) "#C8C8C8", new Color?());
  private const int FilledCircleSegments = 40;
  private const int OutlineSegments = 80 /*0x50*/;
  private readonly IEntityManager _entityManager;
  private readonly SharedTransformSystem _transform;
  private readonly IPrototypeManager _prototype;
  private readonly Vector2[] _filledTriangle = new Vector2[3];
  private readonly Vector2[] _outlinePoints = new Vector2[81];

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public CivPvoOverlay(
    IEntityManager entityManager,
    SharedTransformSystem transform,
    IPrototypeManager prototype)
  {
    this._entityManager = entityManager;
    this._transform = transform;
    this._prototype = prototype;
    this.ZIndex = new int?(-5);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<CivPvoComponent, TransformComponent> entityQueryEnumerator = this._entityManager.EntityQueryEnumerator<CivPvoComponent, TransformComponent>();
    EntityUid entityUid;
    CivPvoComponent civPvoComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref civPvoComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && (double) civPvoComponent.Radius > 0.0)
      {
        Vector2 position = this._transform.GetMapCoordinates(transformComponent).Position;
        Color color = this.GetColor(civPvoComponent.SideId);
        this.DrawFilledCircle(worldHandle, position, civPvoComponent.Radius, ((Color) ref color).WithAlpha(0.12f));
        this.DrawCircleOutline(worldHandle, position, civPvoComponent.Radius, ((Color) ref color).WithAlpha(0.9f));
      }
    }
  }

  private void DrawFilledCircle(
    DrawingHandleWorld handle,
    Vector2 center,
    float radius,
    Color color)
  {
    this._filledTriangle[0] = center;
    for (int index = 0; index < 40; ++index)
    {
      float x1 = (float) ((double) index / 40.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 40.0 * 3.1415927410125732 * 2.0);
      this._filledTriangle[1] = new Vector2(center.X + MathF.Cos(x1) * radius, center.Y + MathF.Sin(x1) * radius);
      this._filledTriangle[2] = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) this._filledTriangle, color);
    }
  }

  private void DrawCircleOutline(
    DrawingHandleWorld handle,
    Vector2 center,
    float radius,
    Color color)
  {
    for (int index = 0; index <= 80 /*0x50*/; ++index)
    {
      float x = (float) ((double) index / 80.0 * 3.1415927410125732 * 2.0);
      this._outlinePoints[index] = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
    }
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) this._outlinePoints, color);
  }

  private Color GetColor(string sideId)
  {
    CivFactionPrototype factionPrototype;
    return !string.IsNullOrWhiteSpace(sideId) && this._prototype.TryIndex<CivFactionPrototype>(sideId, ref factionPrototype) ? factionPrototype.Color : CivPvoOverlay.FallbackColor;
  }
}
