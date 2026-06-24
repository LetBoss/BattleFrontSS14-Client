// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Watch.SharedXenoWatchSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Watch;

public abstract class SharedXenoWatchSystem : EntitySystem
{
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedPlayerManager _player;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoComponent, XenoWatchActionEvent>(new EntityEventRefHandler<XenoComponent, XenoWatchActionEvent>(this.OnXenoWatchAction));
    this.SubscribeLocalEvent<XenoWatchingComponent, MoveInputEvent>(new EntityEventRefHandler<XenoWatchingComponent, MoveInputEvent>(this.OnXenoMoveInput));
    this.Subs.BuiEvents<XenoComponent>((object) XenoWatchUIKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoComponent>) (subs => subs.Event<XenoWatchBuiMsg>(new EntityEventRefHandler<XenoComponent, XenoWatchBuiMsg>(this.OnXenoWatchBui))));
  }

  private void OnXenoMoveInput(Entity<XenoWatchingComponent> xeno, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    if (this._net.IsClient)
    {
      EntityUid? localEntity = this._player.LocalEntity;
      EntityUid owner = xeno.Owner;
      if ((localEntity.HasValue ? (localEntity.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0 && this._player.LocalSession != null)
      {
        this.Unwatch((Entity<EyeComponent>) xeno.Owner, this._player.LocalSession);
        return;
      }
    }
    ActorComponent comp1;
    if (this.TryComp<ActorComponent>((EntityUid) xeno, out comp1))
    {
      this.Unwatch((Entity<EyeComponent>) xeno.Owner, comp1.PlayerSession);
    }
    else
    {
      QueenEyeComponent comp2;
      ActorComponent comp3;
      if (!this.TryComp<QueenEyeComponent>((EntityUid) xeno, out comp2) || !this.TryComp<ActorComponent>(comp2.Queen, out comp3))
        return;
      this.Unwatch((Entity<EyeComponent>) xeno.Owner, comp3.PlayerSession);
    }
  }

  private void OnXenoWatchBui(Entity<XenoComponent> xeno, ref XenoWatchBuiMsg args)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(args.Target, out entity))
      return;
    this.Watch((Entity<HiveMemberComponent, ActorComponent, EyeComponent>) xeno.Owner, (Entity<HiveMemberComponent>) entity.Value);
  }

  protected virtual void OnXenoWatchAction(Entity<XenoComponent> ent, ref XenoWatchActionEvent args)
  {
  }

  public virtual void Watch(
    Entity<HiveMemberComponent?, ActorComponent?, EyeComponent?> watcher,
    Entity<HiveMemberComponent?> toWatch)
  {
  }

  protected virtual void Unwatch(Entity<EyeComponent?> watcher, ICommonSession player)
  {
    if (!this.Resolve<EyeComponent>((EntityUid) watcher, ref watcher.Comp))
      return;
    this._eye.SetTarget((EntityUid) watcher, new EntityUid?(), (EyeComponent) watcher);
    XenoUnwatchEvent args = new XenoUnwatchEvent();
    this.RaiseLocalEvent<XenoUnwatchEvent>((EntityUid) watcher, ref args);
  }

  public bool TryGetWatched(Entity<XenoWatchingComponent?> watching, out EntityUid watched)
  {
    if (!this.Resolve<XenoWatchingComponent>((EntityUid) watching, ref watching.Comp, false) || !watching.Comp.Watching.HasValue)
    {
      watched = new EntityUid();
      return false;
    }
    watched = watching.Comp.Watching.Value;
    return true;
  }

  public void SetWatching(Entity<XenoWatchingComponent?> watching, EntityUid target)
  {
    watching.Comp = this.EnsureComp<XenoWatchingComponent>((EntityUid) watching);
    watching.Comp.Watching = new EntityUid?(target);
    this.Dirty<XenoWatchingComponent>(watching);
  }
}
