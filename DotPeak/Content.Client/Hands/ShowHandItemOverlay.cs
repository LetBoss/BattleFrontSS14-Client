// Decompiled with JetBrains decompiler
// Type: Content.Client.Hands.ShowHandItemOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Hands;

public sealed class ShowHandItemOverlay : Overlay
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entMan;
  private HandsSystem? _hands;
  private readonly IRenderTexture _renderBackbuffer;
  public Texture? IconOverride;
  public EntityUid? EntityOverride;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public ShowHandItemOverlay()
  {
    IoCManager.InjectDependencies<ShowHandItemOverlay>(this);
    IClyde clyde = this._clyde;
    Vector2i vector2i = Vector2i.op_Implicit((64 /*0x40*/, 64 /*0x40*/));
    RenderTargetFormatParameters formatParameters = new RenderTargetFormatParameters((RenderTargetColorFormat) 1, true);
    TextureSampleParameters sampleParameters = new TextureSampleParameters();
    ((TextureSampleParameters) ref sampleParameters).Filter = true;
    TextureSampleParameters? nullable = new TextureSampleParameters?(sampleParameters);
    this._renderBackbuffer = clyde.CreateRenderTarget(vector2i, formatParameters, nullable, nameof (ShowHandItemOverlay));
  }

  protected virtual void DisposeBehavior()
  {
    base.DisposeBehavior();
    ((IDisposable) this._renderBackbuffer).Dispose();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._cfg.GetCVar<bool>(CCVars.HudHeldItemShow) && base.BeforeDraw(ref args);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    if (WindowId.op_Equality(mouseScreenPosition.Window, WindowId.Invalid))
      return;
    DrawingHandleScreen screen = ((OverlayDrawArgs) ref args).ScreenHandle;
    float cvar = this._cfg.GetCVar<float>(CCVars.HudHeldItemOffset);
    Vector2 vector2_1 = new Vector2(cvar, cvar);
    if (this.IconOverride != null)
    {
      DrawingHandleScreen drawingHandleScreen = screen;
      Texture iconOverride = this.IconOverride;
      Vector2 vector2_2 = mouseScreenPosition.Position - Vector2i.op_Implicit(Vector2i.op_Division(this.IconOverride.Size, 2)) + vector2_1;
      Color white = Color.White;
      Color? nullable = new Color?(((Color) ref white).WithAlpha(0.75f));
      ((DrawingHandleBase) drawingHandleScreen).DrawTexture(iconOverride, vector2_2, nullable);
    }
    else
    {
      if (this._hands == null)
        this._hands = this._entMan.System<HandsSystem>();
      EntityUid? handEntity = this._hands.GetActiveHandEntity();
      SpriteComponent sprite;
      if (!handEntity.HasValue || !this._entMan.TryGetComponent<SpriteComponent>(handEntity, ref sprite))
        return;
      Vector2i halfSize = Vector2i.op_Division(((IRenderTarget) this._renderBackbuffer).Size, 2);
      float uiScale = args.ViewportControl is Control viewportControl ? viewportControl.UIScale : 1f;
      ((DrawingHandleBase) screen).RenderInRenderTarget((IRenderTarget) this._renderBackbuffer, (Action) (() => screen.DrawEntity(handEntity.Value, Vector2i.op_Implicit(halfSize), new Vector2(1f, 1f) * uiScale, new Angle?(Angle.Zero), Angle.Zero, new Direction?((Direction) 0), sprite, (TransformComponent) null, (SharedTransformSystem) null)), new Color?(Color.Transparent));
      DrawingHandleScreen drawingHandleScreen = screen;
      Texture texture = this._renderBackbuffer.Texture;
      Vector2 vector2_3 = mouseScreenPosition.Position - Vector2i.op_Implicit(halfSize) + vector2_1;
      Color white = Color.White;
      Color? nullable = new Color?(((Color) ref white).WithAlpha(0.75f));
      ((DrawingHandleBase) drawingHandleScreen).DrawTexture(texture, vector2_3, nullable);
    }
  }
}
