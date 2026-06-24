// Decompiled with JetBrains decompiler
// Type: Content.Client.DoAfter.DoAfterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.DoAfter;

public sealed class DoAfterSystem : SharedDoAfterSystem
{
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private MetaDataSystem _metadata;

  public override void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new DoAfterOverlay((IEntityManager) this.EntityManager, this._prototype, this.GameTiming, this._player));
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<DoAfterOverlay>();
  }

  public override void Update(float frameTime)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    ActiveDoAfterComponent active;
    if (!this.TryComp<ActiveDoAfterComponent>(localEntity, ref active) || this._metadata.EntityPaused(localEntity.Value, (MetaDataComponent) null))
      return;
    TimeSpan curTime = this.GameTiming.CurTime;
    DoAfterComponent comp = this.Comp<DoAfterComponent>(localEntity.Value);
    EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    EntityQuery<HandsComponent> entityQuery2 = this.GetEntityQuery<HandsComponent>();
    this.Update(localEntity.Value, active, comp, curTime, entityQuery1, entityQuery2);
  }

  public bool TryFindActiveDoAfter<T>(
    EntityUid entity,
    [NotNullWhen(true)] out Content.Shared.DoAfter.DoAfter? doAfter,
    [NotNullWhen(true)] out T? @event,
    out float progress)
    where T : DoAfterEvent
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    doAfter = (Content.Shared.DoAfter.DoAfter) null;
    @event = default (T);
    progress = 0.0f;
    ActiveDoAfterComponent doAfterComponent1;
    if (!this.TryComp<ActiveDoAfterComponent>(localEntity, ref doAfterComponent1) || this._metadata.EntityPaused(localEntity.Value, (MetaDataComponent) null))
      return false;
    DoAfterComponent doAfterComponent2 = this.Comp<DoAfterComponent>(localEntity.Value);
    TimeSpan curTime = this.GameTiming.CurTime;
    foreach (Content.Shared.DoAfter.DoAfter doAfter1 in doAfterComponent2.DoAfters.Values)
    {
      if (!doAfter1.Cancelled)
      {
        EntityUid? target = doAfter1.Args.Target;
        EntityUid entityUid = entity;
        if ((target.HasValue ? (EntityUid.op_Inequality(target.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) == 0 && doAfter1.Args.Event is T obj)
        {
          @event = obj;
          doAfter = doAfter1;
          TimeSpan timeSpan = curTime - doAfter.StartTime;
          progress = (float) Math.Min(1.0, timeSpan.TotalSeconds / doAfter.Args.Delay.TotalSeconds);
          return true;
        }
      }
    }
    return false;
  }
}
