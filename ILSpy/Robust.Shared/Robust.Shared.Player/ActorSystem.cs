using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Player;

public sealed class ActorSystem : EntitySystem
{
	[Dependency]
	private readonly ISharedPlayerManager _playerManager;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<ActorComponent, ComponentShutdown>(OnActorShutdown);
	}

	private void OnActorShutdown(EntityUid entity, ActorComponent component, ComponentShutdown args)
	{
		_playerManager.SetAttachedEntity(component.PlayerSession, null);
	}

	public bool TryGetSession(EntityUid? uid, out ICommonSession? session)
	{
		if (TryComp(uid, out ActorComponent comp))
		{
			session = comp.PlayerSession;
			return true;
		}
		session = null;
		return false;
	}

	public ICommonSession? GetSession(EntityUid? uid)
	{
		if (TryComp(uid, out ActorComponent comp))
		{
			return comp.PlayerSession;
		}
		return null;
	}
}
