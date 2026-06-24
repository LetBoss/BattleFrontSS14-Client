// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Eye.QueenEyeOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Eye;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Eye;

public sealed class QueenEyeOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _player;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<QueenEyeActionComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<QueenEyeActionComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnUpdated<AfterAutoHandleStateEvent>)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<QueenEyeActionComponent, QueenEyeActionUpdated>(new EntityEventRefHandler<QueenEyeActionComponent, QueenEyeActionUpdated>((object) this, __methodptr(OnUpdated<QueenEyeActionUpdated>)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<QueenEyeActionComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<QueenEyeActionComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<QueenEyeActionComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<QueenEyeActionComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnDetached)), (Type[]) null, (Type[]) null);
  }

  private void OnUpdated<T>(Entity<QueenEyeActionComponent> ent, ref T args) => this.Updated(ent);

  private void OnAttached(Entity<QueenEyeActionComponent> ent, ref LocalPlayerAttachedEvent args)
  {
    this.Updated(ent);
  }

  private void OnDetached(Entity<QueenEyeActionComponent> ent, ref LocalPlayerDetachedEvent args)
  {
    this._overlay.RemoveOverlay<QueenEyeOverlay>();
  }

  private void Updated(Entity<QueenEyeActionComponent> ent)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = Entity<QueenEyeActionComponent>.op_Implicit(ent);
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
      return;
    if (!ent.Comp.Eye.HasValue)
    {
      this._overlay.RemoveOverlay<QueenEyeOverlay>();
    }
    else
    {
      if (this._overlay.HasOverlay<QueenEyeOverlay>())
        return;
      this._overlay.AddOverlay((Overlay) new QueenEyeOverlay());
    }
  }
}
