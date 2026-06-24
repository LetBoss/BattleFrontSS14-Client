// Decompiled with JetBrains decompiler
// Type: Content.Client.Popups.PopupOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Popups;

public sealed class PopupOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  private readonly IConfigurationManager _configManager;
  private readonly IEntityManager _entManager;
  private readonly IPlayerManager _playerMgr;
  private readonly IUserInterfaceManager _uiManager;
  private readonly PopupSystem _popup;
  private readonly PopupUIController _controller;
  private readonly ExamineSystemShared _examine;
  private readonly SharedTransformSystem _transform;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public PopupOverlay(
    IConfigurationManager configManager,
    IEntityManager entManager,
    IPlayerManager playerMgr,
    IPrototypeManager protoManager,
    IUserInterfaceManager uiManager,
    PopupUIController controller,
    ExamineSystemShared examine,
    SharedTransformSystem transform,
    PopupSystem popup)
  {
    this._configManager = configManager;
    this._entManager = entManager;
    this._playerMgr = playerMgr;
    this._uiManager = uiManager;
    this._examine = examine;
    this._transform = transform;
    this._popup = popup;
    this._controller = controller;
    this._shader = protoManager.Index<ShaderPrototype>(PopupOverlay.UnshadedShader).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null)
      return;
    DrawingHandleBase drawingHandle = args.DrawingHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    drawingHandle.SetTransform(ref local);
    args.DrawingHandle.UseShader(this._shader);
    float scale = this._configManager.GetCVar<float>(CVars.DisplayUIScale);
    if ((double) scale == 0.0)
      scale = this._uiManager.DefaultUIScale;
    this.DrawWorld(((OverlayDrawArgs) ref args).ScreenHandle, args, scale);
    args.DrawingHandle.UseShader((ShaderInstance) null);
  }

  private void DrawWorld(DrawingHandleScreen worldHandle, OverlayDrawArgs args, float scale)
  {
    if (this._popup.WorldLabels.Count == 0 || args.ViewportControl == null)
      return;
    Matrix3x2 worldToScreenMatrix = args.ViewportControl.GetWorldToScreenMatrix();
    EntityUid? ourEntity = ((ISharedPlayerManager) this._playerMgr).LocalEntity;
    MapCoordinates mapCoordinates1;
    // ISSUE: explicit constructor call
    ((MapCoordinates) ref mapCoordinates1).\u002Ector(((Box2) ref args.WorldAABB).Center, args.MapId);
    Vector2 vector2 = ((Box2Rotated) ref args.WorldBounds).Center;
    if (ourEntity.HasValue)
    {
      mapCoordinates1 = this._transform.GetMapCoordinates(ourEntity.Value, (TransformComponent) null);
      vector2 = mapCoordinates1.Position;
    }
    foreach (PopupSystem.WorldPopupLabel worldLabel in (IEnumerable<PopupSystem.WorldPopupLabel>) this._popup.WorldLabels)
    {
      PopupSystem.WorldPopupLabel popup = worldLabel;
      MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(popup.InitialPos, true);
      if (!MapId.op_Inequality(mapCoordinates2.MapId, args.MapId))
      {
        float range = (mapCoordinates2.Position - vector2).Length();
        if (((Box2Rotated) ref args.WorldBounds).Contains(mapCoordinates2.Position) && this._examine.InRangeUnOccluded(mapCoordinates1, mapCoordinates2, range, (SharedInteractionSystem.Ignored) (e =>
        {
          if (EntityUid.op_Equality(e, popup.InitialPos.EntityId))
            return true;
          EntityUid entityUid = e;
          EntityUid? nullable = ourEntity;
          return nullable.HasValue && EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault());
        }), entMan: this._entManager))
        {
          Vector2 position = Vector2.Transform(mapCoordinates2.Position, worldToScreenMatrix);
          this._controller.DrawPopup((PopupSystem.PopupLabel) popup, worldHandle, position, scale);
        }
      }
    }
  }
}
