// Decompiled with JetBrains decompiler
// Type: Content.Client.Mining.MiningOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mining.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Mining;

public sealed class MiningOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  private readonly EntityLookupSystem _lookup;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _xform;
  private readonly EntityQuery<SpriteComponent> _spriteQuery;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly HashSet<Entity<MiningScannerViewableComponent>> _viewableEnts = new HashSet<Entity<MiningScannerViewableComponent>>();

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => false;

  public MiningOverlay()
  {
    IoCManager.InjectDependencies<MiningOverlay>(this);
    this._lookup = this._entityManager.System<EntityLookupSystem>();
    this._sprite = this._entityManager.System<SpriteSystem>();
    this._xform = this._entityManager.System<TransformSystem>();
    this._spriteQuery = this._entityManager.GetEntityQuery<SpriteComponent>();
    this._xformQuery = this._entityManager.GetEntityQuery<TransformComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    MiningScannerViewerComponent scannerViewerComponent;
    if (!localEntity.HasValue || !this._entityManager.TryGetComponent<MiningScannerViewerComponent>(localEntity.GetValueOrDefault(), ref scannerViewerComponent) || !scannerViewerComponent.LastPingLocation.HasValue)
      return;
    Vector2 one = Vector2.One;
    Matrix3x2 scale = Matrix3Helpers.CreateScale(ref one);
    this._viewableEnts.Clear();
    this._lookup.GetEntitiesInRange<MiningScannerViewableComponent>(scannerViewerComponent.LastPingLocation.Value, scannerViewerComponent.ViewRange, this._viewableEnts, (LookupFlags) 110);
    foreach (Entity<MiningScannerViewableComponent> viewableEnt in this._viewableEnts)
    {
      TransformComponent transformComponent1;
      SpriteComponent spriteComponent;
      int num1;
      if (this._xformQuery.TryComp(Entity<MiningScannerViewableComponent>.op_Implicit(viewableEnt), ref transformComponent1) && this._spriteQuery.TryComp(Entity<MiningScannerViewableComponent>.op_Implicit(viewableEnt), ref spriteComponent) && !MapId.op_Inequality(transformComponent1.MapID, args.MapId) && spriteComponent.Visible && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<MiningScannerViewableComponent>.op_Implicit(viewableEnt), spriteComponent)), (Enum) MiningScannerVisualLayers.Overlay, ref num1, false))
      {
        ISpriteLayer ispriteLayer = spriteComponent[num1];
        RSI actualRsi = ispriteLayer.ActualRsi;
        int num2;
        if (actualRsi == null)
        {
          num2 = 1;
        }
        else
        {
          ResPath path = actualRsi.Path;
          num2 = 0;
        }
        if (num2 == 0 && ispriteLayer.RsiState.Name != null)
        {
          EntityUid? gridUid = transformComponent1.GridUid;
          Angle angle;
          if (gridUid.HasValue)
          {
            ref readonly EntityQuery<TransformComponent> local = ref this._xformQuery;
            gridUid = transformComponent1.GridUid;
            EntityUid entityUid = gridUid.Value;
            TransformComponent transformComponent2 = local.CompOrNull(entityUid);
            angle = transformComponent2 != null ? transformComponent2.LocalRotation : Angle.op_Implicit(0.0f);
          }
          else
            angle = Angle.op_Implicit(0.0f);
          Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(angle));
          Matrix3x2 translation = Matrix3Helpers.CreateTranslation(((SharedTransformSystem) this._xform).GetWorldPosition(transformComponent1));
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotation, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(ispriteLayer.ActualRsi.Path, ispriteLayer.RsiState.Name), TimeSpan.FromSeconds((double) ispriteLayer.AnimationTime), true);
          double totalSeconds = (scannerViewerComponent.NextPingTime - this._timing.CurTime).TotalSeconds;
          float num3 = totalSeconds < (double) scannerViewerComponent.AnimationDuration ? 0.0f : (float) Math.Clamp((totalSeconds - (double) scannerViewerComponent.AnimationDuration) / (double) scannerViewerComponent.AnimationDuration, 0.0, 1.0);
          Color white = Color.White;
          Color color = ((Color) ref white).WithAlpha(num3);
          worldHandle.DrawTexture(frame, -Vector2i.op_Implicit(frame.Size) / 2f / 32f, ispriteLayer.Rotation, new Color?(color));
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local1);
  }
}
