// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.FogOfWar.PubgFogOfWarResetAlphaOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarResetAlphaOverlay : Overlay
{
  [Dependency]
  private IEntityManager _ent;
  private readonly PubgFogOfWarHideSystem _hide;
  private readonly SpriteSystem _sprite;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public PubgFogOfWarResetAlphaOverlay()
  {
    IoCManager.InjectDependencies<PubgFogOfWarResetAlphaOverlay>(this);
    this._hide = this._ent.System<PubgFogOfWarHideSystem>();
    this._sprite = this._ent.System<SpriteSystem>();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._hide.CachedBaseAlphas.Count != 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    foreach ((Entity<SpriteComponent> Ent, float BaseAlpha) in this._hide.CachedBaseAlphas)
    {
      if (Ent.Comp != null)
      {
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Ent;
        Color color1 = Ent.Comp.Color;
        Color color2 = ((Color) ref color1).WithAlpha(BaseAlpha);
        sprite.SetColor(entity, color2);
      }
    }
    this._hide.CachedBaseAlphas.Clear();
  }
}
