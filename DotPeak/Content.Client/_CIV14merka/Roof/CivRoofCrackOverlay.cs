// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Roof.CivRoofCrackOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Roof;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Roof;

public sealed class CivRoofCrackOverlay : Overlay
{
  private static readonly Color LeakColor = Color.FromHex((ReadOnlySpan<char>) "#FFF4C8", new Color?());
  private static readonly Color CoreColor = Color.White;
  private readonly IEntityManager _ent;
  [Dependency]
  private IMapManager _maps;
  private readonly EntityLookupSystem _lookup;
  private readonly SharedMapSystem _map;
  private readonly SharedRoofSystem _roof;
  private readonly SharedTransformSystem _xform;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public CivRoofCrackOverlay(IEntityManager ent)
  {
    this._ent = ent;
    IoCManager.InjectDependencies<CivRoofCrackOverlay>(this);
    this._lookup = ent.System<EntityLookupSystem>();
    this._map = ent.System<SharedMapSystem>();
    this._roof = ent.System<SharedRoofSystem>();
    this._xform = ent.System<SharedTransformSystem>();
    this.ZIndex = new int?(-5);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    MapLightComponent light;
    if (args.Viewport.Eye == null || !this._ent.TryGetComponent<MapLightComponent>(args.MapUid, ref light))
      return;
    float sun = CivRoofCrackOverlay.GetSun(light);
    if ((double) sun <= 0.019999999552965164)
      return;
    IClydeViewport viewport = args.Viewport;
    Box2Rotated bounds = args.WorldBounds;
    IRenderTexture target = viewport.LightRenderTarget;
    DrawingHandleWorld handle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._grids.Clear();
    this._maps.FindGridsIntersecting(args.MapId, bounds, ref this._grids, true, true);
    if (this._grids.Count == 0)
      return;
    Vector2 vector2 = Vector2i.op_Implicit(((IRenderTarget) viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
    Vector2 scale = viewport.RenderScale / (Vector2.One / vector2);
    ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) target, (Action) (() =>
    {
      Matrix3x2 worldToLocalMatrix = ((IRenderTarget) target).GetWorldToLocalMatrix(viewport.Eye, scale);
      using (List<Entity<MapGridComponent>>.Enumerator enumerator = this._grids.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          Entity<MapGridComponent> current = enumerator.Current;
          RoofComponent roofComponent;
          CivRoofGridComponent roofGridComponent;
          if (this._ent.TryGetComponent<RoofComponent>(current.Owner, ref roofComponent) && this._ent.TryGetComponent<CivRoofGridComponent>(current.Owner, ref roofGridComponent) && roofGridComponent.Tiles.Count != 0)
          {
            Matrix3x2 matrix3x2 = Matrix3x2.Multiply(this._xform.GetWorldMatrix(current.Owner), worldToLocalMatrix);
            ((DrawingHandleBase) handle).SetTransform(ref matrix3x2);
            SharedMapSystem.TilesEnumerator tilesEnumerator = this._map.GetTilesEnumerator(current.Owner, current.Comp, bounds, true, (Predicate<TileRef>) null);
            while (true)
            {
              TileRef tileRef;
              CivRoofStage stage;
              do
              {
                if (!((SharedMapSystem.TilesEnumerator) ref tilesEnumerator).MoveNext(ref tileRef))
                  goto label_7;
              }
              while (!roofGridComponent.Tiles.TryGetValue(tileRef.GridIndices, out stage) || !this._roof.GetColor(Entity<MapGridComponent, RoofComponent>.op_Implicit((current.Owner, current.Comp, roofComponent)), tileRef.GridIndices).HasValue);
              CivRoofCrackOverlay.DrawLeaks(handle, this._lookup.GetLocalBounds(tileRef, current.Comp.TileSize), sun, stage, CivRoofCrackOverlay.Seed(current.Owner, tileRef.GridIndices));
            }
          }
        }
      }
    }), new Color?());
    DrawingHandleWorld drawingHandleWorld = handle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private static void DrawLeaks(
    DrawingHandleWorld handle,
    Box2 box,
    float sun,
    CivRoofStage stage,
    uint seed)
  {
    Vector2 bottomLeft = box.BottomLeft;
    Vector2 size = ((Box2) ref box).Size;
    float top = bottomLeft.Y + size.Y;
    float num1 = (float) (0.079999998211860657 + (double) sun * 0.18000000715255737);
    if (stage == CivRoofStage.Broken)
    {
      CivRoofCrackOverlay.DrawRect(handle, box, box, CivRoofCrackOverlay.LeakColor, num1 * 0.46f);
      float x1 = bottomLeft.X + size.X * (float) (0.2800000011920929 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.43999999761581421);
      float width = size.X * (float) (0.23999999463558197 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.20000000298023224);
      float height = size.Y * (float) (0.11999999731779099 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.059999998658895493);
      CivRoofCrackOverlay.DrawSource(handle, box, x1, top, width, height, num1 * 2.05f);
      int num2 = 3 + (int) ((double) CivRoofCrackOverlay.Next(ref seed) * 2.0);
      for (int index = 0; index < num2; ++index)
      {
        float x2 = x1 + size.X * (float) (((double) CivRoofCrackOverlay.Next(ref seed) - 0.5) * 0.2199999988079071);
        float len = size.Y * (float) (0.5 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.34999999403953552);
        float drift = size.X * (float) (((double) CivRoofCrackOverlay.Next(ref seed) - 0.5) * 0.15999999642372131);
        float startW = size.X * (float) (0.05000000074505806 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.029999999329447746);
        float endW = startW * (float) (1.8999999761581421 + (double) CivRoofCrackOverlay.Next(ref seed) * 1.2000000476837158);
        float alpha = num1 * (float) (1.2000000476837158 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.60000002384185791);
        CivRoofCrackOverlay.DrawBeam(handle, box, x2, top - height * 0.35f, len, startW, endW, drift, alpha);
      }
      float x3 = x1 + size.X * (float) (((double) CivRoofCrackOverlay.Next(ref seed) - 0.5) * 0.079999998211860657);
      CivRoofCrackOverlay.DrawPool(handle, box, x3, bottomLeft.Y + size.Y * 0.22f, size.X * 0.64f, size.Y * 0.24f, num1 * 0.62f);
    }
    else
    {
      CivRoofCrackOverlay.DrawRect(handle, box, box, CivRoofCrackOverlay.LeakColor, num1 * 0.2f);
      int num3 = (double) CivRoofCrackOverlay.Next(ref seed) > 0.34999999403953552 ? 2 : 1;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        float x4 = bottomLeft.X + size.X * (float) (0.2199999988079071 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.56000000238418579);
        float width = size.X * (float) (0.15000000596046448 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.11999999731779099);
        float height = size.Y * (float) (0.079999998211860657 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.05000000074505806);
        float alpha1 = num1 * (float) (1.0199999809265137 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.550000011920929);
        CivRoofCrackOverlay.DrawSource(handle, box, x4, top, width, height, alpha1);
        int num4 = 2 + (int) ((double) CivRoofCrackOverlay.Next(ref seed) * 2.0);
        for (int index2 = 0; index2 < num4; ++index2)
        {
          float x5 = x4 + size.X * (float) (((double) CivRoofCrackOverlay.Next(ref seed) - 0.5) * 0.15999999642372131);
          float len = size.Y * (float) (0.41999998688697815 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.30000001192092896);
          float drift = size.X * (float) (((double) CivRoofCrackOverlay.Next(ref seed) - 0.5) * 0.11999999731779099);
          float startW = size.X * (float) (0.039999999105930328 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.029999999329447746);
          float endW = startW * (float) (2.0 + (double) CivRoofCrackOverlay.Next(ref seed) * 1.0);
          float alpha2 = num1 * (float) (1.0800000429153442 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.550000011920929);
          CivRoofCrackOverlay.DrawBeam(handle, box, x5, top - height * 0.3f, len, startW, endW, drift, alpha2);
        }
      }
      float x = bottomLeft.X + size.X * (float) (0.30000001192092896 + (double) CivRoofCrackOverlay.Next(ref seed) * 0.40000000596046448);
      CivRoofCrackOverlay.DrawPool(handle, box, x, bottomLeft.Y + size.Y * 0.2f, size.X * 0.46f, size.Y * 0.18f, num1 * 0.34f);
    }
  }

  private static void DrawSource(
    DrawingHandleWorld handle,
    Box2 clip,
    float x,
    float top,
    float width,
    float height,
    float alpha)
  {
    Box2 box1;
    // ISSUE: explicit constructor call
    ((Box2) ref box1).\u002Ector(new Vector2(x - width * 1.15f, top - height * 1.8f), new Vector2(x + width * 1.15f, top + height * 0.08f));
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(new Vector2(x - width * 0.72f, top - height * 1.15f), new Vector2(x + width * 0.72f, top));
    Box2 box3;
    // ISSUE: explicit constructor call
    ((Box2) ref box3).\u002Ector(new Vector2(x - width * 0.36f, top - height * 0.72f), new Vector2(x + width * 0.36f, top));
    CivRoofCrackOverlay.DrawRect(handle, box1, clip, CivRoofCrackOverlay.LeakColor, alpha * 0.24f);
    CivRoofCrackOverlay.DrawRect(handle, box2, clip, CivRoofCrackOverlay.LeakColor, alpha * 0.46f);
    CivRoofCrackOverlay.DrawRect(handle, box3, clip, CivRoofCrackOverlay.CoreColor, alpha * 0.7f);
  }

  private static void DrawBeam(
    DrawingHandleWorld handle,
    Box2 clip,
    float x,
    float top,
    float len,
    float startW,
    float endW,
    float drift,
    float alpha)
  {
    for (int index = 0; index < 5; ++index)
    {
      float num1 = (float) index / 5f;
      float num2 = (float) (((double) index + 1.0) / 5.0);
      float y1 = top - len * num1;
      float y2 = top - len * num2;
      float num3 = x + drift * num1;
      float num4 = x + drift * num2;
      float num5 = MathHelper.Lerp(startW, endW, num1);
      float num6 = MathHelper.Lerp(startW, endW, num2);
      Box2 box1;
      // ISSUE: explicit constructor call
      ((Box2) ref box1).\u002Ector(new Vector2(MathF.Min(num3 - num5, num4 - num6), y2), new Vector2(MathF.Max(num3 + num5, num4 + num6), y1));
      Box2 box2;
      // ISSUE: explicit constructor call
      ((Box2) ref box2).\u002Ector(new Vector2(MathF.Min(num3 - num5 * 0.34f, num4 - num6 * 0.34f), y2), new Vector2(MathF.Max(num3 + num5 * 0.34f, num4 + num6 * 0.34f), y1));
      float num7 = (float) (1.0 - (double) num1 * 0.57999998331069946);
      CivRoofCrackOverlay.DrawRect(handle, box1, clip, CivRoofCrackOverlay.LeakColor, alpha * 0.26f * num7);
      CivRoofCrackOverlay.DrawRect(handle, box2, clip, CivRoofCrackOverlay.CoreColor, alpha * 0.18f * num7);
    }
    float x1 = x + drift;
    float num = top - len;
    CivRoofCrackOverlay.DrawPool(handle, clip, x1, num + len * 0.08f, endW * 2.5f, len * 0.2f, alpha * 0.28f);
  }

  private static void DrawPool(
    DrawingHandleWorld handle,
    Box2 clip,
    float x,
    float y,
    float width,
    float height,
    float alpha)
  {
    Box2 box1;
    // ISSUE: explicit constructor call
    ((Box2) ref box1).\u002Ector(new Vector2(x - width, y - height), new Vector2(x + width, y + height));
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(new Vector2(x - width * 0.55f, y - height * 0.55f), new Vector2(x + width * 0.55f, y + height * 0.55f));
    CivRoofCrackOverlay.DrawRect(handle, box1, clip, CivRoofCrackOverlay.LeakColor, alpha * 0.18f);
    CivRoofCrackOverlay.DrawRect(handle, box2, clip, CivRoofCrackOverlay.CoreColor, alpha * 0.16f);
  }

  private static void DrawRect(
    DrawingHandleWorld handle,
    Box2 box,
    Box2 clip,
    Color color,
    float alpha)
  {
    float x1 = Math.Clamp(box.BottomLeft.X, clip.BottomLeft.X, clip.TopRight.X);
    float y1 = Math.Clamp(box.BottomLeft.Y, clip.BottomLeft.Y, clip.TopRight.Y);
    float x2 = Math.Clamp(box.TopRight.X, clip.BottomLeft.X, clip.TopRight.X);
    float y2 = Math.Clamp(box.TopRight.Y, clip.BottomLeft.Y, clip.TopRight.Y);
    if ((double) x1 >= (double) x2 || (double) y1 >= (double) y2 || (double) alpha <= 0.0)
      return;
    handle.DrawRect(new Box2(new Vector2(x1, y1), new Vector2(x2, y2)), ((Color) ref color).WithAlpha(Math.Clamp(alpha, 0.0f, 1f)), true);
  }

  private static float GetSun(MapLightComponent light)
  {
    return Math.Clamp((float) (((double) light.AmbientLightColor.R + (double) light.AmbientLightColor.G + (double) light.AmbientLightColor.B) / 3.0), 0.0f, 1f);
  }

  private static uint Seed(EntityUid grid, Vector2i tile)
  {
    int num1 = grid.GetHashCode() ^ tile.X * 73856093 ^ tile.Y * 19349663;
    int num2 = (num1 ^ num1 >>> 16 /*0x10*/) * 2146121005;
    int num3 = (num2 ^ num2 >>> 15) * -2073254261;
    return (uint) (num3 ^ num3 >>> 16 /*0x10*/);
  }

  private static float Next(ref uint seed)
  {
    seed = (uint) ((int) seed * 1664525 + 1013904223);
    return (float) (seed & 16777215U /*0xFFFFFF*/) / 16777215f;
  }
}
