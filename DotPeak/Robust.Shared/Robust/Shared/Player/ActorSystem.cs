// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.ActorSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Robust.Shared.Player;

public sealed class ActorSystem : EntitySystem
{
  [Dependency]
  private readonly ISharedPlayerManager _playerManager;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActorComponent, ComponentShutdown>(new ComponentEventHandler<ActorComponent, ComponentShutdown>(this.OnActorShutdown));
  }

  private void OnActorShutdown(EntityUid entity, ActorComponent component, ComponentShutdown args)
  {
    this._playerManager.SetAttachedEntity(component.PlayerSession, new EntityUid?());
  }

  public bool TryGetSession(EntityUid? uid, out ICommonSession? session)
  {
    ActorComponent comp;
    if (this.TryComp<ActorComponent>(uid, out comp))
    {
      session = comp.PlayerSession;
      return true;
    }
    session = (ICommonSession) null;
    return false;
  }

  public ICommonSession? GetSession(EntityUid? uid)
  {
    ActorComponent comp;
    return this.TryComp<ActorComponent>(uid, out comp) ? comp.PlayerSession : (ICommonSession) null;
  }
}
