using System;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Evasion;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Damage;

public sealed class RMCTemporaryInvincibilitySystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EvasionSystem _evasion;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, RMCIgniteAttemptEvent>((EntityEventRefHandler<RMCTemporaryInvincibilityComponent, RMCIgniteAttemptEvent>)OnIgnite, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<RMCTemporaryInvincibilityComponent, BeforeDamageChangedEvent>)OnBeforeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, EvasionRefreshModifiersEvent>((EntityEventRefHandler<RMCTemporaryInvincibilityComponent, EvasionRefreshModifiersEvent>)OnGetEvasion, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, ComponentStartup>((EntityEventRefHandler<RMCTemporaryInvincibilityComponent, ComponentStartup>)OnAdded, (Type[])null, (Type[])null);
	}

	private void OnAdded(Entity<RMCTemporaryInvincibilityComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_evasion.RefreshEvasionModifiers(Entity<RMCTemporaryInvincibilityComponent>.op_Implicit(ent));
	}

	private void OnIgnite(Entity<RMCTemporaryInvincibilityComponent> ent, ref RMCIgniteAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnBeforeDamage(Entity<RMCTemporaryInvincibilityComponent> ent, ref BeforeDamageChangedEvent args)
	{
		args.Cancelled = true;
	}

	private void OnGetEvasion(Entity<RMCTemporaryInvincibilityComponent> ent, ref EvasionRefreshModifiersEvent args)
	{
		args.Evasion += (FixedPoint2)1000;
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCTemporaryInvincibilityComponent> invQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCTemporaryInvincibilityComponent>();
		EntityUid uid = default(EntityUid);
		RMCTemporaryInvincibilityComponent comp = default(RMCTemporaryInvincibilityComponent);
		while (invQuery.MoveNext(ref uid, ref comp))
		{
			if (!(time < comp.ExpiresAt))
			{
				((EntitySystem)this).RemCompDeferred<RMCTemporaryInvincibilityComponent>(uid);
				_evasion.RefreshEvasionModifiers(uid);
			}
		}
	}
}
