// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderLinesOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
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
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderLinesOverlay : Overlay
{
  [Dependency]
  private IPlayerManager _player;
  private readonly IEntityManager _entity;
  private readonly CivCommanderLinesSystem _system;
  private const float LineThickness = 0.36f;
  private const float EndpointRadius = 0.22f;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public CivCommanderLinesOverlay(IEntityManager entity, CivCommanderLinesSystem system)
  {
    IoCManager.InjectDependencies<CivCommanderLinesOverlay>(this);
    this._entity = entity;
    this._system = system;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    if (!localEntity.HasValue || !this._entity.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || teamMemberComponent.TeamId == 0)
      return;
    int teamId = teamMemberComponent.TeamId;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    foreach (CivCommanderLineState commanderLineState in this._system.Lines.Values)
    {
      if (commanderLineState.TeamId == teamId && !MapId.op_Inequality(commanderLineState.MapId, args.MapId))
      {
        Color color = CivCommanderLinesOverlay.GetColor(commanderLineState.Color);
        CivCommanderLinesOverlay.DrawThickLine(worldHandle, commanderLineState.Start, commanderLineState.End, 0.36f, color);
        ((DrawingHandleBase) worldHandle).DrawCircle(commanderLineState.Start, 0.22f, color, true);
        ((DrawingHandleBase) worldHandle).DrawCircle(commanderLineState.End, 0.22f, color, true);
      }
    }
    if (!this._system.IsCommander() || !this._system.IsDrawing || !MapId.op_Equality(this._system.DrawStart.MapId, args.MapId))
      return;
    Vector2 cursorWorldPosition = this._system.GetCursorWorldPosition();
    Color color1 = CivCommanderLinesOverlay.GetColor(this._system.SelectedColor);
    CivCommanderLinesOverlay.DrawThickLine(worldHandle, this._system.DrawStart.Position, cursorWorldPosition, 0.36f, ((Color) ref color1).WithAlpha(0.65f));
    ((DrawingHandleBase) worldHandle).DrawCircle(this._system.DrawStart.Position, 0.22f, color1, true);
    ((DrawingHandleBase) worldHandle).DrawCircle(cursorWorldPosition, 0.22f, ((Color) ref color1).WithAlpha(0.7f), true);
  }

  private static void DrawThickLine(
    DrawingHandleWorld handle,
    Vector2 a,
    Vector2 b,
    float thickness,
    Color color)
  {
    Vector2 vector2_1 = b - a;
    float num1 = vector2_1.Length();
    if ((double) num1 < 1.0 / 1000.0)
      return;
    Vector2 vector2_2 = vector2_1 / num1;
    Vector2 vector2_3 = new Vector2(-vector2_2.Y, vector2_2.X);
    float x = thickness * 0.5f;
    ((DrawingHandleBase) handle).DrawLine(a, b, color);
    int num2 = (int) MathF.Ceiling(x / 0.04f);
    for (int index = 1; index <= num2; ++index)
    {
      float num3 = MathF.Min(x, (float) index * 0.04f);
      Vector2 vector2_4 = vector2_3 * num3;
      ((DrawingHandleBase) handle).DrawLine(a + vector2_4, b + vector2_4, color);
      ((DrawingHandleBase) handle).DrawLine(a - vector2_4, b - vector2_4, color);
    }
  }

  public static Color GetColor(CivCommanderLineColor color)
  {
    Color color1;
    switch (color)
    {
      case CivCommanderLineColor.Attack:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#E63946", new Color?());
        break;
      case CivCommanderLineColor.Defense:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#4DA6FF", new Color?());
        break;
      case CivCommanderLineColor.Route:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#FFD23F", new Color?());
        break;
      case CivCommanderLineColor.Border:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#F5F5F5", new Color?());
        break;
      default:
        color1 = Color.White;
        break;
    }
    return color1;
  }

  public static string GetLabel(CivCommanderLineColor color)
  {
    string label;
    switch (color)
    {
      case CivCommanderLineColor.Attack:
        label = Loc.GetString("civ-cmd-lines-color-attack");
        break;
      case CivCommanderLineColor.Defense:
        label = Loc.GetString("civ-cmd-lines-color-defense");
        break;
      case CivCommanderLineColor.Route:
        label = Loc.GetString("civ-cmd-lines-color-route");
        break;
      case CivCommanderLineColor.Border:
        label = Loc.GetString("civ-cmd-lines-color-border");
        break;
      default:
        label = color.ToString();
        break;
    }
    return label;
  }
}
