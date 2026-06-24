// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.ParallaxSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax.Data;
using Content.Client.Parallax.Managers;
using Content.Shared.Parallax;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Parallax;

public sealed class ParallaxSystem : SharedParallaxSystem
{
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IParallaxManager _parallax;
  [Dependency]
  private SharedMapSystem _map;
  private static readonly ProtoId<ParallaxPrototype> Fallback = ProtoId<ParallaxPrototype>.op_Implicit("Default");
  public const int ParallaxZIndex = 0;

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new ParallaxOverlay());
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnReload), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ParallaxComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<ParallaxComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterAutoHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnReload(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<ParallaxPrototype>())
      return;
    this._parallax.UnloadParallax(ProtoId<ParallaxPrototype>.op_Implicit(ParallaxSystem.Fallback));
    this._parallax.LoadDefaultParallax();
    foreach (ParallaxComponent parallaxComponent in this.EntityQuery<ParallaxComponent>(true))
    {
      this._parallax.UnloadParallax(parallaxComponent.Parallax);
      this._parallax.LoadParallaxByName(parallaxComponent.Parallax);
    }
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<ParallaxOverlay>();
  }

  private void OnAfterAutoHandleState(
    EntityUid uid,
    ParallaxComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    if (this._parallax.IsLoaded(component.Parallax))
      return;
    this._parallax.LoadParallaxByName(component.Parallax);
  }

  public ParallaxLayerPrepared[] GetParallaxLayers(MapId mapId)
  {
    return this._parallax.GetParallaxLayers(this.GetParallax(this._map.GetMapOrInvalid(new MapId?(mapId))));
  }

  public string GetParallax(MapId mapId)
  {
    return this.GetParallax(this._map.GetMapOrInvalid(new MapId?(mapId)));
  }

  public string GetParallax(EntityUid mapUid)
  {
    ParallaxComponent parallaxComponent;
    return !this.TryComp<ParallaxComponent>(mapUid, ref parallaxComponent) ? ProtoId<ParallaxPrototype>.op_Implicit(ParallaxSystem.Fallback) : parallaxComponent.Parallax;
  }

  public void DrawParallax(
    DrawingHandleWorld worldHandle,
    Box2 worldAABB,
    Texture sprite,
    TimeSpan curTime,
    Vector2 position,
    Vector2 scrolling,
    float scale = 1f,
    float slowness = 0.0f,
    Color? modulate = null)
  {
    Vector2 vector2_1 = Vector2i.op_Division(sprite.Size, 32f) * scale;
    Vector2 vector2_2 = scrolling * (float) curTime.TotalSeconds;
    Vector2 vector2_3 = position * slowness + vector2_2 - vector2_1 / 2f;
    Vector2 vector2_4 = Vector2i.op_Implicit(Vector2Helpers.Floored((worldAABB.BottomLeft - vector2_3) / vector2_1)) * vector2_1 + vector2_3;
    for (float x = vector2_4.X; (double) x < (double) worldAABB.Right; x += vector2_1.X)
    {
      for (float y = vector2_4.Y; (double) y < (double) worldAABB.Top; y += vector2_1.Y)
      {
        Box2 box2 = Box2.FromDimensions(new Vector2(x, y), vector2_1);
        worldHandle.DrawTextureRect(sprite, box2, modulate);
      }
    }
  }
}
