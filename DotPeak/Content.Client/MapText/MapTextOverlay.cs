// Decompiled with JetBrains decompiler
// Type: Content.Client.MapText.MapTextOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.MapText;

public sealed class MapTextOverlay : Overlay
{
  private readonly IConfigurationManager _configManager;
  private readonly IEntityManager _entManager;
  private readonly IUserInterfaceManager _uiManager;
  private readonly SharedTransformSystem _transform;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public MapTextOverlay(
    IConfigurationManager configManager,
    IEntityManager entManager,
    IUserInterfaceManager uiManager,
    SharedTransformSystem transform,
    IResourceCache resourceCache,
    IPrototypeManager prototypeManager)
  {
    this._configManager = configManager;
    this._entManager = entManager;
    this._uiManager = uiManager;
    this._transform = transform;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null)
      return;
    DrawingHandleBase drawingHandle = args.DrawingHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    drawingHandle.SetTransform(ref local);
    float scale = this._configManager.GetCVar<float>(CVars.DisplayUIScale);
    if ((double) scale == 0.0)
      scale = this._uiManager.DefaultUIScale;
    this.DrawWorld(((OverlayDrawArgs) ref args).ScreenHandle, args, scale);
    args.DrawingHandle.UseShader((ShaderInstance) null);
  }

  private void DrawWorld(DrawingHandleScreen handle, OverlayDrawArgs args, float scale)
  {
    if (args.ViewportControl == null)
      return;
    Matrix3x2 worldToScreenMatrix = args.ViewportControl.GetWorldToScreenMatrix();
    AllEntityQueryEnumerator<MapTextComponent> entityQueryEnumerator = this._entManager.AllEntityQueryEnumerator<MapTextComponent>();
    Box2Rotated box2Rotated = ((Box2Rotated) ref args.WorldBounds).Enlarged(2f);
    EntityUid entityUid;
    MapTextComponent mapTextComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref mapTextComponent))
    {
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(entityUid, (TransformComponent) null);
      if (!MapId.op_Inequality(mapCoordinates.MapId, args.MapId) && ((Box2Rotated) ref box2Rotated).Contains(mapCoordinates.Position) && mapTextComponent.CachedFont != null)
      {
        Vector2 vector2 = Vector2.Transform(mapCoordinates.Position, worldToScreenMatrix) + mapTextComponent.Offset;
        Vector2 dimensions = handle.GetDimensions((Font) mapTextComponent.CachedFont, (ReadOnlySpan<char>) mapTextComponent.CachedText, scale);
        handle.DrawString((Font) mapTextComponent.CachedFont, vector2 - dimensions / 2f, (ReadOnlySpan<char>) mapTextComponent.CachedText, scale, mapTextComponent.Color);
      }
    }
  }
}
