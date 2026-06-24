// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.MotionDetector.MotionDetectorOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Shared._RMC14.MotionDetector;
using Content.Shared.CCVar;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.MotionDetector;

public sealed class MotionDetectorOverlaySystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IClientNetManager _net;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    if (this._overlay.HasOverlay<MotionDetectorOverlay>())
      return;
    this._overlay.AddOverlay((Overlay) new MotionDetectorOverlay());
  }

  public virtual void Shutdown() => this._overlay.RemoveOverlay<MotionDetectorOverlay>();

  public void DrawBlips<T>(
    DrawingHandleWorld handle,
    ref TimeSpan last,
    List<(Vector2 Pos, bool QueenEye)> blips,
    Texture texture,
    Texture queenEyeTexture)
    where T : IComponent, IDetectorComponent
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    MapCoordinates mapCoordinates = ((SharedTransformSystem) this._entity.System<TransformSystem>()).GetMapCoordinates(valueOrDefault, (TransformComponent) null);
    float size1 = 15f;
    float size2 = (float) this._config.GetCVar<int>(CCVars.ViewportWidth);
    IEye currentEye = this._eye.CurrentEye;
    Vector2 zoom = currentEye.Zoom;
    Angle rotation = currentEye.Rotation;
    Direction cardinalDir = ((Angle) ref rotation).GetCardinalDir();
    if (cardinalDir == 2 || cardinalDir == 6)
    {
      double num = (double) size1;
      size1 = size2;
      size2 = (float) num;
    }
    HandsSystem handsSystem = this._entity.System<HandsSystem>();
    InventorySystem inventorySystem = this._entity.System<InventorySystem>();
    TimeSpan curTime = this._timing.CurTime;
    Entity<HandsComponent> ent = Entity<HandsComponent>.op_Implicit(valueOrDefault);
    List<EntityUid> list = handsSystem.EnumerateHeld(ent).ToList<EntityUid>();
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (inventorySystem.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(valueOrDefault), out containerSlotEnumerator))
    {
      EntityUid entityUid;
      while (containerSlotEnumerator.NextItem(out entityUid))
        list.Add(entityUid);
    }
    foreach (EntityUid entityUid in list)
    {
      T obj;
      if (this._entity.TryGetComponent<T>(entityUid, ref obj))
      {
        TimeSpan scanDuration = obj.ScanDuration;
        INetChannel serverChannel = this._net.ServerChannel;
        if (serverChannel != null)
          scanDuration += TimeSpan.FromMilliseconds((double) serverChannel.Ping / 2.0);
        if (!(curTime > obj.LastScan + scanDuration))
        {
          if (last != obj.LastScan)
          {
            last = obj.LastScan;
            blips.Clear();
            foreach (Blip blip in obj.Blips)
            {
              if (!MapId.op_Inequality(mapCoordinates.MapId, blip.Coordinates.MapId))
              {
                size2 *= zoom.X;
                size1 *= zoom.Y;
                Vector2 vector2 = blip.Coordinates.Position - new Vector2(0.5f, 0.5f) - mapCoordinates.Position;
                this.Cap(ref vector2.X, size2);
                this.Cap(ref vector2.Y, size1);
                blips.Add((vector2, blip.QueenEye));
              }
            }
          }
          foreach ((Vector2 Pos, bool QueenEye) blip in blips)
            ((DrawingHandleBase) handle).DrawTexture(blip.QueenEye ? queenEyeTexture : texture, mapCoordinates.Position + blip.Pos, new Color?());
        }
      }
    }
  }

  private void Cap(ref float i, float size)
  {
    float num = (float) ((double) size / 2.0 - 0.5);
    if ((double) i > (double) num)
    {
      i = num;
    }
    else
    {
      if ((double) i >= -(double) num)
        return;
      i = -num;
    }
  }
}
