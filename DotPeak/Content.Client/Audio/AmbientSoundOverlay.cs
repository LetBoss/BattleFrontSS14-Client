// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.AmbientSoundOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Audio;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Audio;

public sealed class AmbientSoundOverlay : Overlay
{
  private readonly IEntityManager _entManager;
  private readonly AmbientSoundSystem _ambient;
  private readonly EntityLookupSystem _lookup;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public AmbientSoundOverlay(
    IEntityManager entManager,
    AmbientSoundSystem ambient,
    EntityLookupSystem lookup)
  {
    this._entManager = entManager;
    this._ambient = ambient;
    this._lookup = lookup;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQuery<AmbientSoundComponent> entityQuery1 = this._entManager.GetEntityQuery<AmbientSoundComponent>();
    EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
    SharedTransformSystem sharedTransformSystem = this._entManager.System<SharedTransformSystem>();
    foreach (EntityUid entityUid in this._lookup.GetEntitiesIntersecting(args.MapId, args.WorldBounds, (LookupFlags) 110))
    {
      AmbientSoundComponent ambientSoundComponent;
      TransformComponent transformComponent;
      if (entityQuery1.TryGetComponent(entityUid, ref ambientSoundComponent) && entityQuery2.TryGetComponent(entityUid, ref transformComponent))
      {
        if (ambientSoundComponent.Enabled)
        {
          if (this._ambient.IsActive(Entity<AmbientSoundComponent>.op_Implicit((entityUid, ambientSoundComponent))))
          {
            DrawingHandleWorld drawingHandleWorld = worldHandle;
            Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(transformComponent);
            Color lightGreen = Color.LightGreen;
            Color color = ((Color) ref lightGreen).WithAlpha(0.5f);
            ((DrawingHandleBase) drawingHandleWorld).DrawCircle(worldPosition, 0.25f, color, true);
          }
          else
          {
            DrawingHandleWorld drawingHandleWorld = worldHandle;
            Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(transformComponent);
            Color orange = Color.Orange;
            Color color = ((Color) ref orange).WithAlpha(0.25f);
            ((DrawingHandleBase) drawingHandleWorld).DrawCircle(worldPosition, 0.25f, color, true);
          }
        }
        else
        {
          DrawingHandleWorld drawingHandleWorld = worldHandle;
          Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(transformComponent);
          Color red = Color.Red;
          Color color = ((Color) ref red).WithAlpha(0.25f);
          ((DrawingHandleBase) drawingHandleWorld).DrawCircle(worldPosition, 0.25f, color, true);
        }
      }
    }
  }
}
