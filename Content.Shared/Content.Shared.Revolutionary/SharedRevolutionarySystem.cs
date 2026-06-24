using System;
using Content.Shared.Antag;
using Content.Shared.IdentityManagement;
using Content.Shared.Mindshield.Components;
using Content.Shared.Popups;
using Content.Shared.Revolutionary.Components;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared.Revolutionary;

public abstract class SharedRevolutionarySystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedStunSystem _sharedStun;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MindShieldComponent, MapInitEvent>((ComponentEventHandler<MindShieldComponent, MapInitEvent>)MindShieldImplanted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolutionaryComponent, ComponentGetStateAttemptEvent>((ComponentEventRefHandler<RevolutionaryComponent, ComponentGetStateAttemptEvent>)OnRevCompGetStateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadRevolutionaryComponent, ComponentGetStateAttemptEvent>((ComponentEventRefHandler<HeadRevolutionaryComponent, ComponentGetStateAttemptEvent>)OnRevCompGetStateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolutionaryComponent, ComponentStartup>((ComponentEventHandler<RevolutionaryComponent, ComponentStartup>)DirtyRevComps, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadRevolutionaryComponent, ComponentStartup>((ComponentEventHandler<HeadRevolutionaryComponent, ComponentStartup>)DirtyRevComps, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShowAntagIconsComponent, ComponentStartup>((ComponentEventHandler<ShowAntagIconsComponent, ComponentStartup>)DirtyRevComps, (Type[])null, (Type[])null);
	}

	private void MindShieldImplanted(EntityUid uid, MindShieldComponent comp, MapInitEvent init)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<HeadRevolutionaryComponent>(uid))
		{
			((EntitySystem)this).RemCompDeferred<MindShieldComponent>(uid);
		}
		else if (((EntitySystem)this).HasComp<RevolutionaryComponent>(uid))
		{
			TimeSpan stunTime = TimeSpan.FromSeconds(4L);
			EntityUid name = Identity.Entity(uid, (IEntityManager)(object)base.EntityManager);
			((EntitySystem)this).RemComp<RevolutionaryComponent>(uid);
			_sharedStun.TryParalyze(uid, stunTime, refresh: true);
			_popupSystem.PopupEntity(base.Loc.GetString("rev-break-control", (ValueTuple<string, object>)("name", name)), uid);
		}
	}

	private void OnRevCompGetStateAttempt(EntityUid uid, HeadRevolutionaryComponent comp, ref ComponentGetStateAttemptEvent args)
	{
		args.Cancelled = !CanGetState(args.Player);
	}

	private void OnRevCompGetStateAttempt(EntityUid uid, RevolutionaryComponent comp, ref ComponentGetStateAttemptEvent args)
	{
		args.Cancelled = !CanGetState(args.Player);
	}

	private bool CanGetState(ICommonSession? player)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = ((player != null) ? player.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid uid = val.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<RevolutionaryComponent>(uid) || ((EntitySystem)this).HasComp<HeadRevolutionaryComponent>(uid))
			{
				return true;
			}
			return ((EntitySystem)this).HasComp<ShowAntagIconsComponent>(uid);
		}
		return true;
	}

	private void DirtyRevComps<T>(EntityUid someUid, T someComp, ComponentStartup ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<RevolutionaryComponent> revComps = ((EntitySystem)this).AllEntityQuery<RevolutionaryComponent>();
		EntityUid uid = default(EntityUid);
		RevolutionaryComponent comp = default(RevolutionaryComponent);
		while (revComps.MoveNext(ref uid, ref comp))
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
		AllEntityQueryEnumerator<HeadRevolutionaryComponent> headRevComps = ((EntitySystem)this).AllEntityQuery<HeadRevolutionaryComponent>();
		EntityUid uid2 = default(EntityUid);
		HeadRevolutionaryComponent comp2 = default(HeadRevolutionaryComponent);
		while (headRevComps.MoveNext(ref uid2, ref comp2))
		{
			((EntitySystem)this).Dirty(uid2, (IComponent)(object)comp2, (MetaDataComponent)null);
		}
	}
}
