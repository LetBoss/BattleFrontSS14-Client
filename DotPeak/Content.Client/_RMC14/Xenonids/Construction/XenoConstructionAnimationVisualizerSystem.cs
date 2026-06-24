// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.XenoConstructionAnimationVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoConstructionAnimationVisualizerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<XenoConstructionAnimationStartEvent>(new EntityEventHandler<XenoConstructionAnimationStartEvent>(this.OnAnimateResinBuilding), (Type[]) null, (Type[]) null);
  }

  private void OnAnimateResinBuilding(XenoConstructionAnimationStartEvent ev)
  {
    EntityUid? nullable1;
    EntityUid? nullable2;
    XenoConstructionAnimationComponent animationComponent;
    SpriteComponent spriteComponent;
    if (!this.TryGetEntity(ev.Effect, ref nullable1) || !this.TryGetEntity(ev.Xeno, ref nullable2) || !this.TryComp<XenoConstructionAnimationComponent>(nullable1, ref animationComponent) || !this.TryComp<SpriteComponent>(nullable1, ref spriteComponent))
      return;
    int num;
    this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((nullable1.Value, spriteComponent)), (Enum) XenoConstructionVisualLayers.Animation, ref num, false);
    this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((nullable1.Value, spriteComponent)), num);
    animationComponent.AnimationTime = ev.BuildTime;
    animationComponent.AnimationTimeFinished = this._timing.CurTime + ev.BuildTime;
    SpriteComponent.Layer layer;
    if (!this._sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((nullable1.Value, spriteComponent)), num, ref layer, false) || layer.ActualState == null)
      return;
    animationComponent.TotalFrames = layer.ActualState.DelayCount;
  }

  private void Animate(EntityUid uid, SpriteComponent sprite, Enum layerKey, int frame)
  {
    if (!this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layerKey) || !(sprite[(object) layerKey] is SpriteComponent.Layer layer))
      return;
    this._sprite.LayerSetAutoAnimated(layer, layer.AnimationFrame < frame);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityQueryEnumerator<XenoConstructionAnimationComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoConstructionAnimationComponent, SpriteComponent>();
    EntityUid uid;
    XenoConstructionAnimationComponent animationComponent;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref uid, ref animationComponent, ref sprite))
    {
      double num = (animationComponent.AnimationTimeFinished - this._timing.CurTime) / animationComponent.AnimationTime;
      if (num < 0.0)
        num = 0.0;
      int frame = (int) Math.Min((double) animationComponent.TotalFrames * (1.0 - num), (double) (animationComponent.TotalFrames - 1));
      this.Animate(uid, sprite, (Enum) XenoConstructionVisualLayers.Animation, frame);
    }
  }
}
