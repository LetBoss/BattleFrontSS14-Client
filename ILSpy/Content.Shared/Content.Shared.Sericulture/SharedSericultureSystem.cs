using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Cloning.Events;
using Content.Shared.DoAfter;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared.Sericulture;

public abstract class SharedSericultureSystem : EntitySystem
{
	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private HungerSystem _hungerSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedStackSystem _stackSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SericultureComponent, MapInitEvent>((ComponentEventHandler<SericultureComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SericultureComponent, ComponentShutdown>((ComponentEventHandler<SericultureComponent, ComponentShutdown>)OnCompRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SericultureComponent, SericultureActionEvent>((ComponentEventHandler<SericultureComponent, SericultureActionEvent>)OnSericultureStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SericultureComponent, SericultureDoAfterEvent>((ComponentEventHandler<SericultureComponent, SericultureDoAfterEvent>)OnSericultureDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SericultureComponent, CloningEvent>((EntityEventRefHandler<SericultureComponent, CloningEvent>)OnClone, (Type[])null, (Type[])null);
	}

	private void OnClone(Entity<SericultureComponent> ent, ref CloningEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (args.Settings.EventComponents.Contains(((EntitySystem)this).Factory.GetRegistration(((object)ent.Comp).GetType()).Name))
		{
			SericultureComponent comp = ((EntitySystem)this).EnsureComp<SericultureComponent>(args.CloneUid);
			comp.PopupText = ent.Comp.PopupText;
			comp.ProductionLength = ent.Comp.ProductionLength;
			comp.HungerCost = ent.Comp.HungerCost;
			comp.EntityProduced = ent.Comp.EntityProduced;
			comp.MinHungerThreshold = ent.Comp.MinHungerThreshold;
			((EntitySystem)this).Dirty(args.CloneUid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnMapInit(EntityUid uid, SericultureComponent comp, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_actionsSystem.AddAction(uid, ref comp.ActionEntity, EntProtoId.op_Implicit(comp.Action));
	}

	private void OnCompRemove(EntityUid uid, SericultureComponent comp, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? actionEntity = comp.ActionEntity;
		actionsSystem.RemoveAction(performer, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}

	private void OnSericultureStart(EntityUid uid, SericultureComponent comp, SericultureActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		HungerComponent hungerComp = default(HungerComponent);
		if (((EntitySystem)this).TryComp<HungerComponent>(uid, ref hungerComp) && _hungerSystem.IsHungerBelowState(uid, comp.MinHungerThreshold, _hungerSystem.GetHunger(hungerComp) - comp.HungerCost, hungerComp))
		{
			_popupSystem.PopupClient(base.Loc.GetString(comp.PopupText), uid, uid);
			return;
		}
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, uid, comp.ProductionLength, new SericultureDoAfterEvent(), uid)
		{
			BreakOnMove = true,
			BlockDuplicate = true,
			BreakOnDamage = true,
			CancelDuplicate = true
		};
		_doAfterSystem.TryStartDoAfter(doAfter);
	}

	private void OnSericultureDoAfter(EntityUid uid, SericultureComponent comp, SericultureDoAfterEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || ((Component)comp).Deleted)
		{
			return;
		}
		HungerComponent hungerComp = default(HungerComponent);
		if (((EntitySystem)this).TryComp<HungerComponent>(uid, ref hungerComp) && _hungerSystem.IsHungerBelowState(uid, comp.MinHungerThreshold, _hungerSystem.GetHunger(hungerComp) - comp.HungerCost, hungerComp))
		{
			_popupSystem.PopupClient(base.Loc.GetString(comp.PopupText), uid, uid);
			return;
		}
		_hungerSystem.ModifyHunger(uid, 0f - comp.HungerCost);
		if (!_netManager.IsClient)
		{
			EntityUid newEntity = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(comp.EntityProduced), ((EntitySystem)this).Transform(uid).Coordinates);
			_stackSystem.TryMergeToHands(newEntity, uid);
		}
		args.Repeat = true;
	}
}
