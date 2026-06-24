using System;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Input;

public sealed class RMCInputSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	private bool _activeInputMoverEnabled;

	private EntityQuery<ActorComponent> _actorQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_actorQuery = ((EntitySystem)this).GetEntityQuery<ActorComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ActiveInputMoverComponent, MapInitEvent>((EntityEventRefHandler<ActiveInputMoverComponent, MapInitEvent>)OnActiveMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveInputMoverComponent, PlayerAttachedEvent>((EntityEventRefHandler<ActiveInputMoverComponent, PlayerAttachedEvent>)OnActiveAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveInputMoverComponent, PlayerDetachedEvent>((EntityEventRefHandler<ActiveInputMoverComponent, PlayerDetachedEvent>)OnActiveDetached, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCActiveInputMoverEnabled, (Action<bool>)delegate(bool v)
		{
			_activeInputMoverEnabled = v;
		}, true);
	}

	private void OnActiveMapInit(Entity<ActiveInputMoverComponent> ent, ref MapInitEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_activeInputMoverEnabled && !_net.IsClient)
		{
			if (_actorQuery.HasComp(Entity<ActiveInputMoverComponent>.op_Implicit(ent)))
			{
				((EntitySystem)this).EnsureComp<InputMoverComponent>(Entity<ActiveInputMoverComponent>.op_Implicit(ent));
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<InputMoverComponent>(Entity<ActiveInputMoverComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnActiveAttached(Entity<ActiveInputMoverComponent> ent, ref PlayerAttachedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (_activeInputMoverEnabled)
		{
			((EntitySystem)this).EnsureComp<InputMoverComponent>(Entity<ActiveInputMoverComponent>.op_Implicit(ent));
		}
	}

	private void OnActiveDetached(Entity<ActiveInputMoverComponent> ent, ref PlayerDetachedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (_activeInputMoverEnabled)
		{
			((EntitySystem)this).RemCompDeferred<InputMoverComponent>(Entity<ActiveInputMoverComponent>.op_Implicit(ent));
		}
	}
}
