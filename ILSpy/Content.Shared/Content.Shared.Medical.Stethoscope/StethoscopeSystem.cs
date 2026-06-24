using System;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Medical.Stethoscope.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Medical.Stethoscope;

public sealed class StethoscopeSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedContainerSystem _container;

	private const string DamageToListenFor = "Asphyxiation";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StethoscopeComponent, InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>>((EntityEventRefHandler<StethoscopeComponent, InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>>)AddStethoscopeVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StethoscopeComponent, GetItemActionsEvent>((EntityEventRefHandler<StethoscopeComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StethoscopeComponent, StethoscopeActionEvent>((EntityEventRefHandler<StethoscopeComponent, StethoscopeActionEvent>)OnStethoscopeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StethoscopeComponent, StethoscopeDoAfterEvent>((EntityEventRefHandler<StethoscopeComponent, StethoscopeDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnGetActions(Entity<StethoscopeComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.ActionEntity, EntProtoId.op_Implicit(ent.Comp.Action));
	}

	private void OnStethoscopeAction(Entity<StethoscopeComponent> ent, ref StethoscopeActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		StartListening(ent, args.Target);
	}

	private void AddStethoscopeVerb(Entity<StethoscopeComponent> ent, ref InventoryRelayedEvent<GetVerbsEvent<InnateVerb>> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (args.Args.CanInteract && args.Args.CanAccess && ((EntitySystem)this).HasComp<MobStateComponent>(args.Args.Target))
		{
			EntityUid target = args.Args.Target;
			InnateVerb verb = new InnateVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					StartListening(ent, target);
				},
				Text = base.Loc.GetString("stethoscope-verb"),
				IconEntity = ((EntitySystem)this).GetNetEntity(Entity<StethoscopeComponent>.op_Implicit(ent), (MetaDataComponent)null),
				Priority = 2
			};
			args.Args.Verbs.Add(verb);
		}
	}

	private void StartListening(Entity<StethoscopeComponent> ent, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<StethoscopeComponent>.op_Implicit(ent), null, null)), ref container))
		{
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, container.Owner, ent.Comp.Delay, new StethoscopeDoAfterEvent(), Entity<StethoscopeComponent>.op_Implicit(ent), target, Entity<StethoscopeComponent>.op_Implicit(ent))
			{
				DuplicateCondition = DuplicateConditions.SameEvent,
				BreakOnMove = true,
				Hidden = true,
				BreakOnHandChange = false
			});
		}
	}

	private void OnDoAfter(Entity<StethoscopeComponent> ent, ref StethoscopeDoAfterEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (((HandledEntityEventArgs)args).Handled || !target.HasValue || args.Cancelled)
		{
			ent.Comp.LastMeasuredDamage = null;
			return;
		}
		ExamineWithStethoscope(ent, args.Args.User, target.Value);
		args.Repeat = true;
	}

	private void ExamineWithStethoscope(Entity<StethoscopeComponent> stethoscope, EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		DamageableComponent damageComp = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState) || !((EntitySystem)this).TryComp<DamageableComponent>(target, ref damageComp) || _mobState.IsDead(target, mobState) || !damageComp.Damage.DamageDict.TryGetValue("Asphyxiation", out var asphyxDmg))
		{
			_popup.PopupPredicted(base.Loc.GetString("stethoscope-nothing"), target, user);
			stethoscope.Comp.LastMeasuredDamage = null;
			return;
		}
		string absString = GetAbsoluteDamageString(asphyxDmg);
		if (!stethoscope.Comp.LastMeasuredDamage.HasValue)
		{
			_popup.PopupPredicted(absString, target, user);
		}
		else
		{
			string deltaString = GetDeltaDamageString(stethoscope.Comp.LastMeasuredDamage.Value, asphyxDmg);
			_popup.PopupPredicted(base.Loc.GetString("stethoscope-combined-status", (ValueTuple<string, object>)("absolute", absString), (ValueTuple<string, object>)("delta", deltaString)), target, user);
		}
		stethoscope.Comp.LastMeasuredDamage = asphyxDmg;
	}

	private string GetAbsoluteDamageString(FixedPoint2 asphyxDmg)
	{
		int num = (int)asphyxDmg;
		string text = ((num < 60) ? ((num < 10) ? "stethoscope-normal" : ((num >= 30) ? "stethoscope-hyper" : "stethoscope-raggedy")) : ((num >= 80) ? "stethoscope-fucked" : "stethoscope-irregular"));
		string msg = text;
		return base.Loc.GetString(msg);
	}

	private string GetDeltaDamageString(FixedPoint2 lastDamage, FixedPoint2 currentDamage)
	{
		if (lastDamage > currentDamage)
		{
			return base.Loc.GetString("stethoscope-delta-improving");
		}
		if (lastDamage < currentDamage)
		{
			return base.Loc.GetString("stethoscope-delta-worsening");
		}
		return base.Loc.GetString("stethoscope-delta-steady");
	}
}
