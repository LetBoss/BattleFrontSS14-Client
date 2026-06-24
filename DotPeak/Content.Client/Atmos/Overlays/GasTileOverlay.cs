// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Overlays.GasTileOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.Components;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Atmos.Overlays;

public sealed class GasTileOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  private readonly IEntityManager _entManager;
  private readonly IMapManager _mapManager;
  private readonly SharedMapSystem _mapSystem;
  private readonly SharedTransformSystem _xformSys;
  private readonly ShaderInstance _shader;
  private readonly float[] _timer;
  private readonly float[][] _frameDelays;
  private readonly int[] _frameCounter;
  private readonly Texture[][] _frames;
  private const int FireStates = 3;
  private const string FireRsiPath = "/Textures/Effects/fire.rsi";
  private readonly float[] _fireTimer = new float[3];
  private readonly float[][] _fireFrameDelays = new float[3][];
  private readonly int[] _fireFrameCounter = new int[3];
  private readonly Texture[][] _fireFrames = new Texture[3][];
  private int _gasCount;
  public const int GasOverlayZIndex = 11;

  public virtual OverlaySpace Space => (OverlaySpace) 272;

  public GasTileOverlay(
    GasTileOverlaySystem system,
    IEntityManager entManager,
    IResourceCache resourceCache,
    IPrototypeManager protoMan,
    SpriteSystem spriteSys,
    SharedTransformSystem xformSys)
  {
    this._entManager = entManager;
    this._mapManager = IoCManager.Resolve<IMapManager>();
    this._mapSystem = entManager.System<SharedMapSystem>();
    this._xformSys = xformSys;
    this._shader = protoMan.Index<ShaderPrototype>(GasTileOverlay.UnshadedShader).Instance();
    this.ZIndex = new int?(11);
    this._gasCount = system.VisibleGasId.Length;
    this._timer = new float[this._gasCount];
    this._frameDelays = new float[this._gasCount][];
    this._frameCounter = new int[this._gasCount];
    this._frames = new Texture[this._gasCount][];
    for (int index = 0; index < this._gasCount; ++index)
    {
      GasPrototype gasPrototype = protoMan.Index<GasPrototype>(system.VisibleGasId[index].ToString());
      SpriteSpecifier spriteSpecifier;
      if (!string.IsNullOrEmpty(gasPrototype.GasOverlaySprite) && !string.IsNullOrEmpty(gasPrototype.GasOverlayState))
        spriteSpecifier = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath(gasPrototype.GasOverlaySprite), gasPrototype.GasOverlayState);
      else if (!string.IsNullOrEmpty(gasPrototype.GasOverlayTexture))
        spriteSpecifier = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath(gasPrototype.GasOverlayTexture));
      else
        continue;
      switch (spriteSpecifier)
      {
        case SpriteSpecifier.Rsi rsi:
          Robust.Client.Graphics.RSI.State state;
          if (resourceCache.GetResource<RSIResource>(rsi.RsiPath, true).RSI.TryGetState(Robust.Client.Graphics.RSI.StateId.op_Implicit(rsi.RsiState), ref state))
          {
            this._frames[index] = state.GetFrames((RsiDirection) 0);
            this._frameDelays[index] = state.GetDelays();
            this._frameCounter[index] = 0;
            continue;
          }
          continue;
        case SpriteSpecifier.Texture texture:
          this._frames[index] = new Texture[1]
          {
            spriteSys.Frame0((SpriteSpecifier) texture)
          };
          this._frameDelays[index] = Array.Empty<float>();
          continue;
        default:
          continue;
      }
    }
    Robust.Client.Graphics.RSI rsi1 = resourceCache.GetResource<RSIResource>("/Textures/Effects/fire.rsi", true).RSI;
    for (int index = 0; index < 3; ++index)
    {
      Robust.Client.Graphics.RSI.State state;
      if (!rsi1.TryGetState(Robust.Client.Graphics.RSI.StateId.op_Implicit((index + 1).ToString()), ref state))
        throw new ArgumentOutOfRangeException($"Fire RSI doesn't have state \"{index}\"!");
      this._fireFrames[index] = state.GetFrames((RsiDirection) 0);
      this._fireFrameDelays[index] = state.GetDelays();
      this._fireFrameCounter[index] = 0;
    }
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    for (int index1 = 0; index1 < this._gasCount; ++index1)
    {
      float[] frameDelay = this._frameDelays[index1];
      if (frameDelay.Length != 0)
      {
        int index2 = this._frameCounter[index1];
        this._timer[index1] += ((FrameEventArgs) ref args).DeltaSeconds;
        float num = frameDelay[index2];
        if ((double) this._timer[index1] >= (double) num)
        {
          this._timer[index1] -= num;
          this._frameCounter[index1] = (index2 + 1) % this._frames[index1].Length;
        }
      }
    }
    for (int index3 = 0; index3 < 3; ++index3)
    {
      float[] fireFrameDelay = this._fireFrameDelays[index3];
      if (fireFrameDelay.Length != 0)
      {
        int index4 = this._fireFrameCounter[index3];
        this._fireTimer[index3] += ((FrameEventArgs) ref args).DeltaSeconds;
        float num = fireFrameDelay[index4];
        if ((double) this._fireTimer[index3] >= (double) num)
        {
          this._fireTimer[index3] -= num;
          this._fireFrameCounter[index3] = (index4 + 1) % this._fireFrames[index3].Length;
        }
      }
    }
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (MapId.op_Equality(args.MapId, MapId.Nullspace))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQuery<TransformComponent> entityQuery1 = this._entManager.GetEntityQuery<TransformComponent>();
    EntityQuery<GasTileOverlayComponent> entityQuery2 = this._entManager.GetEntityQuery<GasTileOverlayComponent>();
    (Box2Rotated, DrawingHandleWorld, int, Texture[][], int[], Texture[][], int[], ShaderInstance, EntityQuery<GasTileOverlayComponent>, EntityQuery<TransformComponent>, SharedTransformSystem) valueTuple = (args.WorldBounds, ((OverlayDrawArgs) ref args).WorldHandle, this._gasCount, this._frames, this._frameCounter, this._fireFrames, this._fireFrameCounter, this._shader, entityQuery2, entityQuery1, this._xformSys);
    EntityUid mapOrInvalid = this._mapSystem.GetMapOrInvalid(new MapId?(args.MapId));
    MapAtmosphereComponent atmos;
    if (this._entManager.TryGetComponent<MapAtmosphereComponent>(mapOrInvalid, ref atmos))
      this.DrawMapOverlay(worldHandle, args, mapOrInvalid, atmos);
    if (args.Space != 16 /*0x10*/)
      return;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    this._mapManager.FindGridsIntersecting<(Box2Rotated, DrawingHandleWorld, int, Texture[][], int[], Texture[][], int[], ShaderInstance, EntityQuery<GasTileOverlayComponent>, EntityQuery<TransformComponent>, SharedTransformSystem)>(args.MapId, args.WorldAABB, ref valueTuple, GasTileOverlay.\u003C\u003Ec.\u003C\u003E9__22_0 ?? (GasTileOverlay.\u003C\u003Ec.\u003C\u003E9__22_0 = new GridCallback<(Box2Rotated, DrawingHandleWorld, int, Texture[][], int[], Texture[][], int[], ShaderInstance, EntityQuery<GasTileOverlayComponent>, EntityQuery<TransformComponent>, SharedTransformSystem)>((object) GasTileOverlay.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDraw\u003Eb__22_0))), false, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private void DrawMapOverlay(
    DrawingHandleWorld handle,
    OverlayDrawArgs args,
    EntityUid map,
    MapAtmosphereComponent atmos)
  {
    bool flag = this._entManager.HasComponent<MapGridComponent>(map);
    if (flag && args.Space != 16 /*0x10*/ || !flag && args.Space != 256 /*0x0100*/)
      return;
    Vector2i vector2i1 = Vector2Helpers.Floored(args.WorldAABB.BottomLeft);
    Vector2i vector2i2 = Vector2Helpers.Ceiled(args.WorldAABB.TopRight);
    for (int x = vector2i1.X; x <= vector2i2.X; ++x)
    {
      for (int y = vector2i1.Y; y <= vector2i2.Y; ++y)
      {
        Vector2 vector2_1 = new Vector2((float) x, (float) y);
        for (int index = 0; index < atmos.OverlayData.Opacity.Length; ++index)
        {
          byte num = atmos.OverlayData.Opacity[index];
          if (num > (byte) 0)
          {
            DrawingHandleWorld drawingHandleWorld = handle;
            Texture texture = this._frames[index][this._frameCounter[index]];
            Vector2 vector2_2 = vector2_1;
            Color white = Color.White;
            Color? nullable = new Color?(((Color) ref white).WithAlpha(num));
            ((DrawingHandleBase) drawingHandleWorld).DrawTexture(texture, vector2_2, nullable);
          }
        }
      }
    }
  }
}
