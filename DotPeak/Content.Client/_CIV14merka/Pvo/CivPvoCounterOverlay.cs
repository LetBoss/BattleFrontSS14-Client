// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Pvo.CivPvoCounterOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Pvo;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoCounterOverlay : Overlay
{
  [Dependency]
  private readonly IEyeManager _eye;
  [Dependency]
  private readonly IPlayerManager _player;
  [Dependency]
  private readonly IResourceCache _cache;
  [Dependency]
  private readonly IEntityManager _entity;
  private readonly SharedTransformSystem _xform;
  private readonly Font _font;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CivPvoCounterOverlay()
  {
    IoCManager.InjectDependencies<CivPvoCounterOverlay>(this);
    this._xform = this._entity.System<SharedTransformSystem>();
    this._font = (Font) new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 12);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    if (!localEntity.HasValue || !this._entity.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || string.IsNullOrWhiteSpace(teamMemberComponent.SideId))
      return;
    MapId mapId = args.MapId;
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    EntityQueryEnumerator<CivPvoComponent, TransformComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<CivPvoComponent, TransformComponent>();
    EntityUid entityUid;
    CivPvoComponent civPvoComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref civPvoComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, mapId) && !string.IsNullOrWhiteSpace(civPvoComponent.SideId) && string.Equals(civPvoComponent.SideId, teamMemberComponent.SideId, StringComparison.OrdinalIgnoreCase))
      {
        int num1 = civPvoComponent.Tier1Charges + civPvoComponent.Tier2Charges;
        string str1;
        if (!civPvoComponent.Infinite)
          str1 = Loc.GetString("civ-eq-pvo-counter", new (string, object)[3]
          {
            ("total", (object) num1),
            ("max", (object) civPvoComponent.MaxCharges),
            ("tier2", (object) civPvoComponent.Tier2Charges)
          });
        else
          str1 = Loc.GetString("civ-eq-pvo-counter-infinite");
        string str2 = str1;
        Vector2 screen = this._eye.WorldToScreen(this._xform.GetWorldPosition(entityUid) + new Vector2(0.0f, 1.1f));
        Vector2 dimensions = screenHandle.GetDimensions(this._font, (ReadOnlySpan<char>) str2, 1f);
        if ((double) dimensions.X > 0.0)
        {
          float num2 = dimensions.X * 0.5f;
          Vector2 vector2 = new Vector2(screen.X - num2, screen.Y - dimensions.Y);
          Color color1 = num1 > 0 ? Color.LightGreen : Color.OrangeRed;
          UIBox2 uiBox2_1;
          // ISSUE: explicit constructor call
          ((UIBox2) ref uiBox2_1).\u002Ector(vector2.X - 3f, vector2.Y - 2f, (float) ((double) vector2.X + (double) dimensions.X + 3.0), (float) ((double) vector2.Y + (double) dimensions.Y + 2.0));
          DrawingHandleScreen drawingHandleScreen = screenHandle;
          UIBox2 uiBox2_2 = uiBox2_1;
          Color black = Color.Black;
          Color color2 = ((Color) ref black).WithAlpha(0.55f);
          drawingHandleScreen.DrawRect(uiBox2_2, color2, true);
          screenHandle.DrawString(this._font, vector2, (ReadOnlySpan<char>) str2, 1f, color1);
        }
      }
    }
  }
}
