// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Figurines.FigurineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Shared._RMC14.Figurines;
using Content.Shared.Administration;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Figurines;

public sealed class FigurineSystem : EntitySystem
{
  [Dependency]
  private IClientAdminManager _adminManager;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private SpriteSystem _sprite;
  private readonly FigurineSystem.ContentSpriteControl _control = new FigurineSystem.ContentSpriteControl();

  public virtual void Initialize()
  {
  }

  public virtual void Shutdown()
  {
  }

  private void OnFigurineRequest(FigurineRequestEvent ev)
  {
    if (!this._timing.IsFirstTimePredicted || !this._adminManager.HasFlag(AdminFlags.Host))
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(valueOrDefault, ref spriteComponent))
      return;
    this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((valueOrDefault, spriteComponent)), Vector2.One);
    Vector2i vector2i = Vector2i.Zero;
    foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
    {
      if (allLayer.Visible)
        vector2i = Vector2i.ComponentMax(vector2i, allLayer.PixelSize);
    }
    if (((Vector2i) ref vector2i).Equals(Vector2i.Zero))
      return;
    this._control.QueuedTextures.Enqueue((this._clyde.CreateRenderTarget(new Vector2i(vector2i.X, vector2i.Y), new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "export"), (Direction) 0, valueOrDefault));
  }

  private sealed class ContentSpriteControl : Control
  {
    [Dependency]
    private IEntityManager _entManager;
    internal readonly Queue<(IRenderTexture Texture, Direction Direction, EntityUid Entity)> QueuedTextures = new Queue<(IRenderTexture, Direction, EntityUid)>();

    public ContentSpriteControl()
    {
      IoCManager.InjectDependencies<FigurineSystem.ContentSpriteControl>(this);
    }

    protected virtual void Draw(DrawingHandleScreen handle)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      FigurineSystem.ContentSpriteControl.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new FigurineSystem.ContentSpriteControl.\u003C\u003Ec__DisplayClass3_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.handle = handle;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      base.Draw(cDisplayClass30.handle);
      ISawmill log = this._entManager.System<FigurineSystem>().Log;
      (IRenderTexture Texture, Direction Direction, EntityUid Entity) result1;
      while (this.QueuedTextures.TryDequeue(out result1))
      {
        try
        {
          (IRenderTexture Texture, Direction Direction, EntityUid Entity) result = result1;
          ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) result1.Texture, (Action) (() =>
          {
            DrawingHandleScreen drawingHandleScreen = handle;
            EntityUid entity = result.Entity;
            Vector2 vector2 = Vector2i.op_Implicit(Vector2i.op_Division(((IRenderTarget) result.Texture).Size, 2));
            Vector2 one = Vector2.One;
            Angle? nullable1 = new Angle?(Angle.Zero);
            Direction? nullable2 = new Direction?(result.Direction);
            Angle angle = new Angle();
            Direction? nullable3 = nullable2;
            drawingHandleScreen.DrawEntity(entity, vector2, one, nullable1, angle, nullable3, (SpriteComponent) null, (TransformComponent) null, (SharedTransformSystem) null);
          }), new Color?());
          // ISSUE: method pointer
          ((IRenderTarget) result1.Texture).CopyPixelsToMemory<Rgba32>(closure_0 ?? (closure_0 = new CopyPixelsDelegate<Rgba32>((object) cDisplayClass30, __methodptr(\u003CDraw\u003Eb__1))), new UIBox2i?());
        }
        catch (Exception ex)
        {
          ((IDisposable) result1.Texture).Dispose();
          if (!string.IsNullOrEmpty(ex.StackTrace))
            log.Fatal(ex.StackTrace);
        }
      }
    }
  }
}
