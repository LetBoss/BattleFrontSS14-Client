// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgPartyPingOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgPartyPingOverlay : Overlay
{
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly PubgPartyPingClientSystem _pingSystem;
  private readonly VehicleSystem _vehicles;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public PubgPartyPingOverlay(
    IPlayerManager player,
    SharedTransformSystem transform,
    PubgPartyPingClientSystem pingSystem,
    VehicleSystem vehicles)
  {
    this._player = player;
    this._transform = transform;
    this._pingSystem = pingSystem;
    this._vehicles = vehicles;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    PubgActivePingState? latestPing = this._pingSystem.LatestPing;
    if (!latestPing.HasValue)
      return;
    PubgActivePingState pubgActivePingState = latestPing.Value;
    MapId mapId;
    if (!this._vehicles.TryGetDisplayMapId(localEntity.Value, out mapId) || MapId.op_Inequality(mapId, pubgActivePingState.MapId))
      return;
    Vector2 vector2_1 = new Vector2((float) ((UIBox2i) ref args.ViewportBounds).Width / 2f, (float) ((UIBox2i) ref args.ViewportBounds).Height / 2f);
    Vector2 vector2_2 = args.ViewportControl.WorldToScreen(pubgActivePingState.Position) - vector2_1;
    float num1 = vector2_2.Length();
    if ((double) num1 < 1.0)
      return;
    Vector2 direction = vector2_2 / num1;
    float num2 = MathF.Min(MathF.Min((float) ((UIBox2i) ref args.ViewportBounds).Width, (float) ((UIBox2i) ref args.ViewportBounds).Height) * 0.42f, MathF.Max(34f, num1 - 20f));
    Vector2 center = vector2_1 + direction * num2;
    PubgPartyPingOverlay.DrawArrow(((OverlayDrawArgs) ref args).ScreenHandle, center, direction, PubgPartyPingColorResolver.GetColor(pubgActivePingState.Source));
  }

  private static void DrawArrow(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 direction,
    Color color)
  {
    Vector2 vector2_1 = center + direction * 14f;
    Vector2 vector2_2 = new Vector2(-direction.Y, direction.X);
    Vector2 vector2_3 = center - direction * 7f + vector2_2 * 6f;
    Vector2 vector2_4 = center - direction * 7f - vector2_2 * 6f;
    Vector2[] vector2Array1 = new Vector2[3]
    {
      vector2_1,
      vector2_3,
      vector2_4
    };
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) vector2Array1, color);
    Vector2[] vector2Array2 = new Vector2[4]
    {
      vector2_1,
      vector2_3,
      vector2_4,
      vector2_1
    };
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) vector2Array2, Color.Black);
  }
}
