// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.ParallaxOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax.Managers;
using Content.Shared.CCVar;
using Content.Shared.Parallax.Biomes;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Content.Client.Parallax;

public sealed class ParallaxOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _configurationManager;
  [Dependency]
  private IParallaxManager _manager;
  private readonly SharedMapSystem _mapSystem;
  private readonly ParallaxSystem _parallax;

  public virtual OverlaySpace Space => (OverlaySpace) 256 /*0x0100*/;

  public ParallaxOverlay()
  {
    this.ZIndex = new int?(0);
    IoCManager.InjectDependencies<ParallaxOverlay>(this);
    this._mapSystem = this._entManager.System<SharedMapSystem>();
    this._parallax = this._entManager.System<ParallaxSystem>();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return !MapId.op_Equality(args.MapId, MapId.Nullspace) && !this._entManager.HasComponent<BiomeComponent>(this._mapSystem.GetMapOrInvalid(new MapId?(args.MapId)));
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (MapId.op_Equality(args.MapId, MapId.Nullspace) || !this._configurationManager.GetCVar<bool>(CCVars.ParallaxEnabled))
      return;
    IEye eye = args.Viewport.Eye;
    Vector2 vector2_1 = eye != null ? eye.Position.Position : Vector2.Zero;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ParallaxLayerPrepared[] parallaxLayers = this._parallax.GetParallaxLayers(args.MapId);
    float totalSeconds = (float) this._timing.RealTime.TotalSeconds;
    foreach (ParallaxLayerPrepared parallaxLayerPrepared in parallaxLayers)
    {
      ShaderInstance shaderInstance = string.IsNullOrEmpty(parallaxLayerPrepared.Config.Shader) ? (ShaderInstance) null : this._prototypeManager.Index<ShaderPrototype>(parallaxLayerPrepared.Config.Shader).Instance();
      ((DrawingHandleBase) worldHandle).UseShader(shaderInstance);
      Texture texture = parallaxLayerPrepared.Texture;
      Vector2 vector2_2 = Vector2i.op_Division(texture.Size, 32f) * parallaxLayerPrepared.Config.Scale;
      Vector2 vector2_3 = parallaxLayerPrepared.Config.WorldHomePosition + this._manager.ParallaxAnchor;
      Vector2 vector2_4 = parallaxLayerPrepared.Config.Scrolling * totalSeconds;
      Vector2 vector2_5 = (vector2_1 - vector2_3) * parallaxLayerPrepared.Config.Slowness + vector2_4 + vector2_3 + parallaxLayerPrepared.Config.WorldAdjustPosition - vector2_2 / 2f;
      if (parallaxLayerPrepared.Config.Tiled)
      {
        Vector2 vector2_6 = Vector2i.op_Implicit(Vector2Helpers.Floored((args.WorldAABB.BottomLeft - vector2_5) / vector2_2)) * vector2_2 + vector2_5;
        for (float x = vector2_6.X; (double) x < (double) args.WorldAABB.Right; x += vector2_2.X)
        {
          for (float y = vector2_6.Y; (double) y < (double) args.WorldAABB.Top; y += vector2_2.Y)
            worldHandle.DrawTextureRect(texture, Box2.FromDimensions(new Vector2(x, y), vector2_2), new Color?());
        }
      }
      else
        worldHandle.DrawTextureRect(texture, Box2.FromDimensions(vector2_5, vector2_2), new Color?());
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
