using System;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Finesse;

public sealed class XenoFinesseSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoFinesseComponent, MeleeHitEvent>((EntityEventRefHandler<XenoFinesseComponent, MeleeHitEvent>)OnFinesseMeleeHit, (Type[])null, (Type[])null);
	}

	private void OnFinesseMeleeHit(Entity<XenoFinesseComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid ent in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoFinesseComponent>.op_Implicit(xeno), ent))
			{
				XenoMarkedComponent xenoMarkedComponent = ((EntitySystem)this).EnsureComp<XenoMarkedComponent>(ent);
				xenoMarkedComponent.WearOffAt = _timing.CurTime + xeno.Comp.MarkedTime;
				xenoMarkedComponent.TimeAdded = _timing.CurTime;
				break;
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoMarkedComponent> markedQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoMarkedComponent>();
		EntityUid uid = default(EntityUid);
		XenoMarkedComponent mark = default(XenoMarkedComponent);
		while (markedQuery.MoveNext(ref uid, ref mark))
		{
			if (!(time < mark.WearOffAt))
			{
				((EntitySystem)this).RemCompDeferred<XenoMarkedComponent>(uid);
			}
		}
	}
}
