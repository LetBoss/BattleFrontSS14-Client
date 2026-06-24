// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Airdrop.PubgAirdropOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Airdrop;

public sealed class PubgAirdropOverlay : Overlay
{
  private const string TextFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";
  private const int TextFontSize = 12;
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly Font _font;
  public bool Active;
  public Vector2 Target;
  public int RemainingSeconds;
  public MapId MapId;
  private int _cachedSeconds = int.MinValue;
  private string _cachedText = string.Empty;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public PubgAirdropOverlay(
    IResourceCache cache,
    IPlayerManager player,
    SharedTransformSystem transform)
  {
    this._player = player;
    this._transform = transform;
    this._font = (Font) new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 12);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this.Active || args.ViewportControl == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue || MapId.op_Inequality(this._transform.GetMapId(Entity<TransformComponent>.op_Implicit(localEntity.Value)), this.MapId))
      return;
    Vector2 screen = args.ViewportControl.WorldToScreen(this.Target);
    if (this.RemainingSeconds != this._cachedSeconds)
    {
      this._cachedSeconds = this.RemainingSeconds;
      this._cachedText = Loc.GetString("pubg-airdrop-countdown", new (string, object)[1]
      {
        ("time", (object) PubgAirdropOverlay.FormatTime(this.RemainingSeconds))
      });
    }
    ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, screen + new Vector2(0.0f, -20f), this._cachedText, Color.White);
  }

  private static string FormatTime(int seconds)
  {
    if (seconds < 0)
      seconds = 0;
    return $"{seconds / 60:D2}:{seconds % 60:D2}";
  }
}
