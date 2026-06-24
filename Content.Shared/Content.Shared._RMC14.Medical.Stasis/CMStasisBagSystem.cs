using System;
using Content.Shared._RMC14.Medical.Wounds;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Body.Organ;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Medical.Stasis;

public sealed class CMStasisBagSystem : EntitySystem
{
	[Dependency]
	private SharedXenoParasiteSystem _parasite;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private MobStateSystem _mobstate;

	[Dependency]
	private SharedEntityStorageSystem _entStorage;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private INetManager _net;

	private EntityQuery<OrganComponent> _organQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_organQuery = ((EntitySystem)this).GetEntityQuery<OrganComponent>();
		((EntitySystem)this).SubscribeLocalEvent<CMStasisBagComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<CMStasisBagComponent, ContainerIsInsertingAttemptEvent>)OnStasisInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMStasisBagComponent, ContainerIsRemovingAttemptEvent>((EntityEventRefHandler<CMStasisBagComponent, ContainerIsRemovingAttemptEvent>)OnStasisRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMStasisBagComponent, ExaminedEvent>((EntityEventRefHandler<CMStasisBagComponent, ExaminedEvent>)OnStasisExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMInStasisComponent, CMMetabolizeAttemptEvent>((EntityEventRefHandler<CMInStasisComponent, CMMetabolizeAttemptEvent>)OnBloodstreamMetabolizeAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMInStasisComponent, MapInitEvent>((EntityEventRefHandler<CMInStasisComponent, MapInitEvent>)OnInStasisMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMInStasisComponent, ComponentRemove>((EntityEventRefHandler<CMInStasisComponent, ComponentRemove>)OnInStasisRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMInStasisComponent, GetInfectedIncubationMultiplierEvent>((EntityEventRefHandler<CMInStasisComponent, GetInfectedIncubationMultiplierEvent>)OnInStasisGetInfectedIncubationMultiplier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMInStasisComponent, CMBleedAttemptEvent>((EntityEventRefHandler<CMInStasisComponent, CMBleedAttemptEvent>)OnInStasisBleedAttempt, (Type[])null, (Type[])null);
	}

	private void OnStasisInsert(Entity<CMStasisBagComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OnInsert(ent, ((ContainerAttemptEventBase)args).EntityUid);
	}

	private void OnStasisRemove(Entity<CMStasisBagComponent> ent, ref ContainerIsRemovingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OnRemove(ent, ((ContainerAttemptEventBase)args).EntityUid);
	}

	private void OnStasisExamine(Entity<CMStasisBagComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		string msg = "rmc-stasis-new";
		if (ent.Comp.StasisLeft / ent.Comp.StasisMaxTime < 0.33000001311302185)
		{
			msg = "rmc-stasis-very-used";
		}
		else if (ent.Comp.StasisLeft / ent.Comp.StasisMaxTime < 0.6600000262260437)
		{
			msg = "rmc-stasis-used";
		}
		args.PushMarkup(base.Loc.GetString(msg));
	}

	private void OnBloodstreamMetabolizeAttempt(Entity<CMInStasisComponent> ent, ref CMMetabolizeAttemptEvent args)
	{
		args.Cancel();
	}

	private void OnInStasisMapInit(Entity<CMInStasisComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_parasite.RefreshIncubationMultipliers(Entity<VictimInfectedComponent>.op_Implicit(ent.Owner));
	}

	private void OnInStasisRemove(Entity<CMInStasisComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_parasite.RefreshIncubationMultipliers(Entity<VictimInfectedComponent>.op_Implicit(ent.Owner));
	}

	private void OnInStasisGetInfectedIncubationMultiplier(Entity<CMInStasisComponent> ent, ref GetInfectedIncubationMultiplierEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)ent.Comp).Running)
		{
			float multiplier = ent.Comp.IncubationMultiplier;
			if (args.stage >= ent.Comp.LessEffectiveStage)
			{
				multiplier += multiplier / 3f;
			}
			args.Multiply(multiplier);
		}
	}

	private void OnInStasisBleedAttempt(Entity<CMInStasisComponent> ent, ref CMBleedAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnInsert(Entity<CMStasisBagComponent> bag, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<CMInStasisComponent>(target);
	}

	private void OnRemove(Entity<CMStasisBagComponent> bag, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<CMInStasisComponent>(target);
	}

	public override void Update(float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<CMStasisBagComponent> stasisQuery = ((EntitySystem)this).EntityQueryEnumerator<CMStasisBagComponent>();
		EntityUid uid = default(EntityUid);
		CMStasisBagComponent bag = default(CMStasisBagComponent);
		BaseContainer container = default(BaseContainer);
		while (stasisQuery.MoveNext(ref uid, ref bag))
		{
			if (!_container.TryGetContainer(uid, "entity_storage", ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count <= 0)
			{
				continue;
			}
			bool inStasis = false;
			foreach (EntityUid ent in container.ContainedEntities)
			{
				if (_mobstate.IsDead(ent))
				{
					_entStorage.OpenStorage(uid);
					_popup.PopupEntity(base.Loc.GetString("rmc-stasis-reject-dead"), uid, PopupType.SmallCaution);
				}
				else if (((EntitySystem)this).HasComp<CMInStasisComponent>(ent))
				{
					inStasis = true;
				}
			}
			if (inStasis)
			{
				bag.StasisLeft -= TimeSpan.FromSeconds(frameTime);
				if (bag.StasisLeft <= TimeSpan.Zero)
				{
					_entStorage.EmptyContents(uid);
					((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(bag.UsedBag), uid.ToCoordinates(), (ComponentRegistry)null);
					((EntitySystem)this).QueueDel((EntityUid?)uid);
				}
			}
		}
	}

	public bool CanBodyMetabolize(EntityUid body)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		CMMetabolizeAttemptEvent ev = default(CMMetabolizeAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<CMMetabolizeAttemptEvent>(body, ref ev, false);
		return !ev.Cancelled;
	}

	public bool CanOrganMetabolize(Entity<OrganComponent?> organ)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (_organQuery.Resolve(Entity<OrganComponent>.op_Implicit(organ), ref organ.Comp, false))
		{
			EntityUid? body = organ.Comp.Body;
			if (body.HasValue)
			{
				EntityUid body2 = body.GetValueOrDefault();
				CMMetabolizeAttemptEvent ev = default(CMMetabolizeAttemptEvent);
				((EntitySystem)this).RaiseLocalEvent<CMMetabolizeAttemptEvent>(body2, ref ev, false);
				return !ev.Cancelled;
			}
		}
		return true;
	}
}
