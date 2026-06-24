using System;
using Content.Shared.Mobs;
using Content.Shared.Traits.Assorted;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Medical.Unrevivable;

public sealed class RMCUnrevivableSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCRevivableComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCRevivableComponent, MobStateChangedEvent>)OnMobstateChanged, (Type[])null, (Type[])null);
	}

	private void OnMobstateChanged(Entity<RMCRevivableComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			ent.Comp.UnrevivableAt = _timing.CurTime + ent.Comp.UnrevivableDelay;
		}
		else
		{
			ent.Comp.UnrevivableAt = TimeSpan.Zero;
		}
		((EntitySystem)this).Dirty<RMCRevivableComponent>(ent, (MetaDataComponent)null);
	}

	public void AddRevivableTime(EntityUid uid, TimeSpan time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		RMCRevivableComponent revivable = default(RMCRevivableComponent);
		if (((EntitySystem)this).TryComp<RMCRevivableComponent>(uid, ref revivable) && !(revivable.UnrevivableAt == TimeSpan.Zero))
		{
			revivable.UnrevivableAt += time;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)revivable, (MetaDataComponent)null);
		}
	}

	public bool IsUnrevivable(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).HasComp<UnrevivableComponent>(uid);
	}

	public void MakeUnrevivable(Entity<RMCRevivableComponent?> ent, bool killLarva = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RMCRevivableComponent>(ent.Owner, ref ent.Comp, false))
		{
			UnrevivableComponent unrevivableComponent = ((EntitySystem)this).EnsureComp<UnrevivableComponent>(Entity<RMCRevivableComponent>.op_Implicit(ent));
			unrevivableComponent.Analyzable = false;
			unrevivableComponent.Cloneable = false;
			unrevivableComponent.ReasonMessage = ent.Comp.UnrevivableReasonMessage;
			ent.Comp.KillLarva = killLarva;
			((EntitySystem)this).Dirty<RMCRevivableComponent>(ent, (MetaDataComponent)null);
		}
	}

	public bool DoesKillLarvaOnUnrevivable(Entity<RMCRevivableComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RMCRevivableComponent>(ent.Owner, ref ent.Comp, false))
		{
			return false;
		}
		return ent.Comp.KillLarva;
	}

	public int GetUnrevivableStage(Entity<RMCRevivableComponent?> ent, int maxStages)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RMCRevivableComponent>(ent.Owner, ref ent.Comp, false))
		{
			return 0;
		}
		if (ent.Comp.UnrevivableAt == TimeSpan.Zero)
		{
			return 0;
		}
		TimeSpan curTime = _timing.CurTime;
		TimeSpan end = ent.Comp.UnrevivableAt;
		TimeSpan start = ent.Comp.UnrevivableAt - ent.Comp.UnrevivableDelay;
		double progress = (curTime - start) / (end - start);
		return (int)((double)maxStages * progress);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<RMCRevivableComponent> revivableQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCRevivableComponent>();
		EntityUid uid = default(EntityUid);
		RMCRevivableComponent revivable = default(RMCRevivableComponent);
		while (revivableQuery.MoveNext(ref uid, ref revivable))
		{
			if (!IsUnrevivable(uid) && !(revivable.UnrevivableAt == TimeSpan.Zero) && !(_timing.CurTime < revivable.UnrevivableAt))
			{
				MakeUnrevivable(Entity<RMCRevivableComponent>.op_Implicit((uid, revivable)));
			}
		}
	}
}
