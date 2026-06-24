// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderLabelsOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderLabelsOverlay : Overlay
{
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IResourceCache _cache;
  private readonly IEntityManager _entity;
  private readonly CivCommanderLinesSystem _system;
  private readonly Font _font;
  private readonly Font _lineFont;
  private const float LineLabelStep = 5f;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CivCommanderLabelsOverlay(IEntityManager entity, CivCommanderLinesSystem system)
  {
    IoCManager.InjectDependencies<CivCommanderLabelsOverlay>(this);
    this._entity = entity;
    this._system = system;
    this._font = (Font) new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 18);
    this._lineFont = (Font) new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 11);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    if (!localEntity.HasValue || !this._entity.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || teamMemberComponent.TeamId == 0)
      return;
    int teamId = teamMemberComponent.TeamId;
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    MapId mapId = args.MapId;
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    foreach (CivCommanderLabelState commanderLabelState in this._system.Labels.Values)
    {
      if (commanderLabelState.TeamId == teamId && !MapId.op_Inequality(commanderLabelState.MapId, mapId))
        this.DrawLabel(screenHandle, commanderLabelState.Position, commanderLabelState.Rotation, commanderLabelState.Text, CivCommanderLinesOverlay.GetColor(commanderLabelState.Color), 1f);
    }
    if (this._system.IsCommander() && this._system.IsPlacingLabel)
    {
      Vector2 cursorWorldPosition = this._system.GetCursorWorldPosition();
      Color color = CivCommanderLinesOverlay.GetColor(this._system.SelectedColor);
      this.DrawLabel(screenHandle, cursorWorldPosition, this._system.PendingLabelRotation, this._system.PendingLabelText, color, 0.7f);
    }
    foreach (CivCommanderLineState commanderLineState in this._system.Lines.Values)
    {
      if (commanderLineState.TeamId == teamId && !MapId.op_Inequality(commanderLineState.MapId, mapId))
      {
        Vector2 vector2 = commanderLineState.End - commanderLineState.Start;
        float num1 = vector2.Length();
        if ((double) num1 >= 0.10000000149011612)
        {
          string label = CivCommanderLinesOverlay.GetLabel(commanderLineState.Color);
          Color color = CivCommanderLinesOverlay.GetColor(commanderLineState.Color);
          int num2 = Math.Max(1, (int) ((double) num1 / 5.0));
          for (int index = 0; index < num2; ++index)
          {
            float num3 = ((float) index + 0.5f) / (float) num2;
            Vector2 worldPos = commanderLineState.Start + vector2 * num3;
            this.DrawLineTypeLabel(screenHandle, worldPos, label, color);
          }
        }
      }
    }
  }

  private void DrawLabel(
    DrawingHandleScreen screen,
    Vector2 worldPos,
    float rotation,
    string text,
    Color color,
    float alpha)
  {
    if (string.IsNullOrEmpty(text))
      return;
    Vector2 screen1 = this._eye.WorldToScreen(worldPos);
    Vector2 dimensions = screen.GetDimensions(this._font, (ReadOnlySpan<char>) text, 1f);
    if ((double) dimensions.X <= 0.0)
      return;
    float num1 = dimensions.X * 0.5f;
    float num2 = dimensions.Y * 0.5f;
    float num3 = MathF.Cos(rotation);
    float m12 = MathF.Sin(rotation);
    Matrix3x2 matrix3x2 = new Matrix3x2(num3, m12, -m12, num3, screen1.X, screen1.Y);
    Matrix3x2 transform = ((DrawingHandleBase) screen).GetTransform();
    ((DrawingHandleBase) screen).SetTransform(ref matrix3x2);
    float num4 = 4f;
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(-num1 - num4, -num2 - num4, num1 + num4, num2 + num4);
    DrawingHandleScreen drawingHandleScreen = screen;
    UIBox2 uiBox2_2 = uiBox2_1;
    Color black = Color.Black;
    Color color1 = ((Color) ref black).WithAlpha(0.55f * alpha);
    drawingHandleScreen.DrawRect(uiBox2_2, color1, true);
    screen.DrawRect(uiBox2_1, ((Color) ref color).WithAlpha(alpha), false);
    Vector2 vector2 = new Vector2(-num1, -num2);
    screen.DrawString(this._font, vector2, (ReadOnlySpan<char>) text, 1f, ((Color) ref color).WithAlpha(alpha));
    ((DrawingHandleBase) screen).SetTransform(ref transform);
  }

  private void DrawLineTypeLabel(
    DrawingHandleScreen screen,
    Vector2 worldPos,
    string text,
    Color color)
  {
    if (string.IsNullOrEmpty(text))
      return;
    Vector2 screen1 = this._eye.WorldToScreen(worldPos);
    Vector2 dimensions = screen.GetDimensions(this._lineFont, (ReadOnlySpan<char>) text, 1f);
    if ((double) dimensions.X <= 0.0)
      return;
    float num1 = dimensions.X * 0.5f;
    float num2 = dimensions.Y * 0.5f;
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector((float) ((double) screen1.X - (double) num1 - 3.0), (float) ((double) screen1.Y - (double) num2 - 3.0), (float) ((double) screen1.X + (double) num1 + 3.0), (float) ((double) screen1.Y + (double) num2 + 3.0));
    DrawingHandleScreen drawingHandleScreen = screen;
    UIBox2 uiBox2_2 = uiBox2_1;
    Color black = Color.Black;
    Color color1 = ((Color) ref black).WithAlpha(0.6f);
    drawingHandleScreen.DrawRect(uiBox2_2, color1, true);
    screen.DrawRect(uiBox2_1, ((Color) ref color).WithAlpha(0.6f), false);
    screen.DrawString(this._lineFont, new Vector2(screen1.X - num1, screen1.Y - num2), (ReadOnlySpan<char>) text, 1f, color);
  }
}
