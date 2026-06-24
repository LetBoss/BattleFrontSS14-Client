using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Medical.IV;

public abstract class SharedIVDripSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly HashSet<EntityUid> _packsToUpdate = new HashSet<EntityUid>();

	private EntityQuery<BloodPackComponent> _bloodPackQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_bloodPackQuery = ((EntitySystem)this).GetEntityQuery<BloodPackComponent>();
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<IVDripComponent, EntInsertedIntoContainerMessage>)OnIVDripEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<IVDripComponent, EntRemovedFromContainerMessage>)OnIVDripEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<IVDripComponent, AfterAutoHandleStateEvent>)OnIVDripAfterHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, CanDragEvent>((EntityEventRefHandler<IVDripComponent, CanDragEvent>)OnIVDripCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, CanDropDraggedEvent>((EntityEventRefHandler<IVDripComponent, CanDropDraggedEvent>)OnIVDripCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, DragDropDraggedEvent>((EntityEventRefHandler<IVDripComponent, DragDropDraggedEvent>)OnIVDripDragDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, InteractHandEvent>((EntityEventRefHandler<IVDripComponent, InteractHandEvent>)OnIVInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<IVDripComponent, GetVerbsEvent<InteractionVerb>>)OnIVVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripComponent, ExaminedEvent>((EntityEventRefHandler<IVDripComponent, ExaminedEvent>)OnIVExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IVDripTargetComponent, CanDropTargetEvent>((EntityEventRefHandler<IVDripTargetComponent, CanDropTargetEvent>)OnIVTargetCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, MapInitEvent>((EntityEventRefHandler<BloodPackComponent, MapInitEvent>)OnBloodPackMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<BloodPackComponent, AfterAutoHandleStateEvent>)OnBloodPackAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, SolutionContainerChangedEvent>((EntityEventRefHandler<BloodPackComponent, SolutionContainerChangedEvent>)OnBloodPackSolutionChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, AfterInteractEvent>((EntityEventRefHandler<BloodPackComponent, AfterInteractEvent>)OnBloodPackAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, AttachBloodPackDoAfterEvent>((EntityEventRefHandler<BloodPackComponent, AttachBloodPackDoAfterEvent>)OnBloodPackAttachDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, GotUnequippedHandEvent>((EntityEventRefHandler<BloodPackComponent, GotUnequippedHandEvent>)OnBloodPackUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<BloodPackComponent, GetVerbsEvent<InteractionVerb>>)OnBloodPackVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodPackComponent, ExaminedEvent>((EntityEventRefHandler<BloodPackComponent, ExaminedEvent>)OnBloodPackExamine, (Type[])null, (Type[])null);
	}

	private void OnIVDripEntInserted(Entity<IVDripComponent> iv, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateIVVisuals(iv);
	}

	private void OnIVDripEntRemoved(Entity<IVDripComponent> iv, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateIVVisuals(iv);
	}

	private void OnIVDripAfterHandleState(Entity<IVDripComponent> iv, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateIVAppearance(iv);
	}

	private void OnIVDripCanDrag(Entity<IVDripComponent> iv, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnIVDripCanDropDragged(Entity<IVDripComponent> iv, ref CanDropDraggedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<IVDripTargetComponent>(args.Target) && InRange(Entity<IVDripComponent>.op_Implicit(iv), args.Target, iv.Comp.Range))
		{
			args.Handled = true;
			args.CanDrop = true;
		}
	}

	private void OnIVTargetCanDropTarget(Entity<IVDripTargetComponent> marine, ref CanDropTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		EntityUid iv = args.Dragged;
		IVDripComponent ivComp = default(IVDripComponent);
		if (((EntitySystem)this).TryComp<IVDripComponent>(iv, ref ivComp) && InRange(iv, Entity<IVDripTargetComponent>.op_Implicit(marine), ivComp.Range))
		{
			args.Handled = true;
			args.CanDrop = true;
		}
	}

	private void OnIVDripDragDropDragged(Entity<IVDripComponent> iv, ref DragDropDraggedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			if (!iv.Comp.AttachedTo.HasValue)
			{
				AttachIV(iv, args.User, args.Target);
			}
			else
			{
				DetachIV(iv, args.User, rip: false, predict: true);
			}
		}
	}

	private void OnIVInteractHand(Entity<IVDripComponent> iv, ref InteractHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		DetachIV(iv, args.User, rip: false, predict: true);
	}

	private void OnIVVerbs(Entity<IVDripComponent> iv, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					ToggleInject(iv, user);
				},
				Text = base.Loc.GetString("cm-iv-verb-toggle-inject")
			});
		}
	}

	private void OnIVExamine(Entity<IVDripComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("IVDripComponent"))
		{
			string injectingMsg = (ent.Comp.Injecting ? "cm-iv-examine-injecting" : "cm-iv-examine-drawing");
			args.PushMarkup(base.Loc.GetString(injectingMsg, (ValueTuple<string, object>)("iv", ent.Owner)));
			string chemicalsMsg = base.Loc.GetString("cm-iv-examine-chemicals-none");
			BaseContainer container = default(BaseContainer);
			if (_containers.TryGetContainer(Entity<IVDripComponent>.op_Implicit(ent), ent.Comp.Slot, ref container, (ContainerManagerComponent)null))
			{
				EntityUid packId = container.ContainedEntities.FirstOrDefault();
				BloodPackComponent pack = default(BloodPackComponent);
				if (((EntityUid)(ref packId)).Valid && ((EntitySystem)this).TryComp<BloodPackComponent>(packId, ref pack) && _solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(packId), pack.Solution, out Entity<SolutionComponent>? _, out Solution solution))
				{
					chemicalsMsg = base.Loc.GetString("cm-iv-examine-chemicals", (ValueTuple<string, object>)("attached", packId), (ValueTuple<string, object>)("units", solution.Volume.Int()));
				}
			}
			args.PushMarkup(chemicalsMsg);
			EntityUid? attachedTo = ent.Comp.AttachedTo;
			string text;
			if (attachedTo.HasValue)
			{
				EntityUid attached = attachedTo.GetValueOrDefault();
				text = base.Loc.GetString("cm-iv-examine-attached", (ValueTuple<string, object>)("attached", attached));
			}
			else
			{
				text = base.Loc.GetString("cm-iv-examine-attached-none");
			}
			string attachedMsg = text;
			args.PushMarkup(attachedMsg);
		}
	}

	private void OnBloodPackMapInit(Entity<BloodPackComponent> pack, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_packsToUpdate.Add(Entity<BloodPackComponent>.op_Implicit(pack));
	}

	private void OnBloodPackAfterState(Entity<BloodPackComponent> pack, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdatePackVisuals(pack);
	}

	private void OnBloodPackSolutionChanged(Entity<BloodPackComponent> pack, ref SolutionContainerChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdatePackVisuals(pack);
	}

	private void OnBloodPackAfterInteract(Entity<BloodPackComponent> pack, ref AfterInteractEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (!InRange(Entity<BloodPackComponent>.op_Implicit(pack), target2, pack.Comp.Range) || !((EntitySystem)this).HasComp<IVDripTargetComponent>(target2))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		if (pack.Comp.AttachedTo.HasValue)
		{
			DetachPack(Entity<BloodPackComponent>.op_Implicit((Entity<BloodPackComponent>.op_Implicit(pack), Entity<BloodPackComponent>.op_Implicit(pack))), user, rip: false, predict: true);
			return;
		}
		if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user), pack.Comp.SkillRequired))
		{
			_popup.PopupClient(base.Loc.GetString("cm-iv-attach-no-skill"), user, user);
			return;
		}
		if (user == target2)
		{
			_popup.PopupClient(base.Loc.GetString("cm-blood-pack-cannot-self"), user, user);
			return;
		}
		TimeSpan delay = pack.Comp.AttachDelay;
		if (delay > TimeSpan.Zero)
		{
			string selfPoke = base.Loc.GetString("cm-blood-pack-poke-self", (ValueTuple<string, object>)("pack", pack.Owner), (ValueTuple<string, object>)("target", target2));
			string othersPoke = base.Loc.GetString("cm-blood-pack-poke-others", new(string, object)[3]
			{
				("user", user),
				("pack", pack.Owner),
				("target", target2)
			});
			_popup.PopupPredicted(selfPoke, othersPoke, target2, user);
		}
		AttachBloodPackDoAfterEvent ev = new AttachBloodPackDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<BloodPackComponent>.op_Implicit(pack), target2, Entity<BloodPackComponent>.op_Implicit(pack))
		{
			BreakOnMove = true,
			BreakOnDamage = true,
			BreakOnHandChange = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent,
			TargetEffect = EntProtoId.op_Implicit("RMCEffectHealBusy")
		};
		_doAfter.TryStartDoAfter(doAfter);
	}

	private void OnBloodPackAttachDoAfter(Entity<BloodPackComponent> pack, ref AttachBloodPackDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				AttachPack(pack, args.User, target2);
			}
		}
	}

	private void OnBloodPackUnequippedHand(Entity<BloodPackComponent> pack, ref GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		DetachPack(Entity<BloodPackComponent>.op_Implicit((Entity<BloodPackComponent>.op_Implicit(pack), Entity<BloodPackComponent>.op_Implicit(pack))), args.User, rip: true, predict: true);
	}

	private void OnBloodPackVerbs(Entity<BloodPackComponent> pack, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					ToggleInject(pack, user);
				},
				Text = base.Loc.GetString("cm-iv-verb-toggle-inject")
			});
		}
	}

	private void OnBloodPackExamine(Entity<BloodPackComponent> pack, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("BloodPackComponent"))
		{
			string injectingMsg = (pack.Comp.Injecting ? "cm-iv-examine-injecting" : "cm-iv-examine-drawing");
			args.PushMarkup(base.Loc.GetString(injectingMsg, (ValueTuple<string, object>)("iv", pack.Owner)));
			EntityUid? attachedTo = pack.Comp.AttachedTo;
			string text;
			if (attachedTo.HasValue)
			{
				EntityUid attached = attachedTo.GetValueOrDefault();
				text = base.Loc.GetString("cm-iv-examine-attached", (ValueTuple<string, object>)("attached", attached));
			}
			else
			{
				text = base.Loc.GetString("cm-iv-examine-attached-none");
			}
			string attachedMsg = text;
			args.PushMarkup(attachedMsg);
			if (_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(pack.Owner), pack.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution))
			{
				args.PushMarkup(base.Loc.GetString("cm-blood-pack-contains", (ValueTuple<string, object>)("units", solution.Volume.Int())));
			}
		}
	}

	protected bool InRange(EntityUid iv, EntityUid to, float range)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates ivPos = _transform.GetMapCoordinates(iv, (TransformComponent)null);
		MapCoordinates toPos = _transform.GetMapCoordinates(to, (TransformComponent)null);
		return ((MapCoordinates)(ref ivPos)).InRange(toPos, range);
	}

	private void AttachIV(Entity<IVDripComponent> iv, EntityUid user, EntityUid to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (InRange(Entity<IVDripComponent>.op_Implicit(iv), to, iv.Comp.Range))
		{
			if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user), iv.Comp.SkillRequired))
			{
				_popup.PopupClient(base.Loc.GetString("cm-iv-attach-no-skill"), user, user);
				return;
			}
			iv.Comp.AttachedTo = to;
			((EntitySystem)this).Dirty<IVDripComponent>(iv, (MetaDataComponent)null);
			AttachFeedback(Entity<IVDripComponent>.op_Implicit(iv), user, to, iv.Comp.Injecting);
		}
	}

	protected void DetachIV(Entity<IVDripComponent> iv, EntityUid? user, bool rip, bool predict)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedTo = iv.Comp.AttachedTo;
		if (!attachedTo.HasValue)
		{
			return;
		}
		EntityUid target = attachedTo.GetValueOrDefault();
		if (user.HasValue && !_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user.Value), iv.Comp.SkillRequired))
		{
			_popup.PopupClient(base.Loc.GetString("cm-iv-detach-no-skill"), user.Value, user.Value);
			return;
		}
		iv.Comp.AttachedTo = null;
		((EntitySystem)this).Dirty<IVDripComponent>(iv, (MetaDataComponent)null);
		if (rip)
		{
			DoRip(iv.Comp.RipDamage, target, user, iv.Comp.RipEmote, predict);
		}
		else
		{
			DoDetachFeedback(Entity<IVDripComponent>.op_Implicit(iv), target, user, predict);
		}
	}

	private void AttachPack(Entity<BloodPackComponent> pack, EntityUid user, EntityUid to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (InRange(Entity<BloodPackComponent>.op_Implicit(pack), to, pack.Comp.Range))
		{
			if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user), pack.Comp.SkillRequired))
			{
				_popup.PopupClient(base.Loc.GetString("cm-iv-attach-no-skill"), user, user);
				return;
			}
			pack.Comp.AttachedTo = to;
			((EntitySystem)this).Dirty<BloodPackComponent>(pack, (MetaDataComponent)null);
			AttachFeedback(Entity<BloodPackComponent>.op_Implicit(pack), user, to, pack.Comp.Injecting);
		}
	}

	protected void DetachPack(Entity<BloodPackComponent> pack, EntityUid? user, bool rip, bool predict)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedTo = pack.Comp.AttachedTo;
		if (!attachedTo.HasValue)
		{
			return;
		}
		EntityUid target = attachedTo.GetValueOrDefault();
		if (user.HasValue && !_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user.Value), pack.Comp.SkillRequired))
		{
			_popup.PopupClient(base.Loc.GetString("cm-iv-detach-no-skill"), user.Value, user.Value);
			return;
		}
		pack.Comp.AttachedTo = null;
		((EntitySystem)this).Dirty<BloodPackComponent>(pack, (MetaDataComponent)null);
		if (rip)
		{
			DoRip(pack.Comp.RipDamage, target, user, pack.Comp.RipEmote, predict);
		}
		else
		{
			DoDetachFeedback(Entity<BloodPackComponent>.op_Implicit(pack), target, user, predict);
		}
	}

	private void ToggleInject(Entity<IVDripComponent> iv, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ToggleInject(Entity<IVDripComponent>.op_Implicit(iv), ref iv.Comp.Injecting, user);
		((EntitySystem)this).Dirty<IVDripComponent>(iv, (MetaDataComponent)null);
	}

	private void ToggleInject(Entity<BloodPackComponent> pack, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ToggleInject(Entity<BloodPackComponent>.op_Implicit(pack), ref pack.Comp.Injecting, user);
		((EntitySystem)this).Dirty<BloodPackComponent>(pack, (MetaDataComponent)null);
	}

	private void ToggleInject(EntityUid iv, ref bool injecting, EntityUid user)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		injecting = !injecting;
		string msg = (injecting ? base.Loc.GetString("cm-iv-now-injecting") : base.Loc.GetString("cm-iv-now-taking"));
		_popup.PopupClient(msg, iv, user);
	}

	protected void UpdatePackVisuals(Entity<BloodPackComponent> pack)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(pack.Owner), pack.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution))
		{
			UpdatePackAppearance(pack);
			return;
		}
		BaseContainer container = default(BaseContainer);
		IVDripComponent iv = default(IVDripComponent);
		if (_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<BloodPackComponent>.op_Implicit(pack), null)), ref container) && ((EntitySystem)this).TryComp<IVDripComponent>(container.Owner, ref iv))
		{
			iv.FillColor = solution.GetColor(_prototype);
			iv.FillPercentage = (int)(solution.Volume / solution.MaxVolume * 100);
			((EntitySystem)this).Dirty(container.Owner, (IComponent)(object)iv, (MetaDataComponent)null);
			UpdateIVAppearance(Entity<IVDripComponent>.op_Implicit((container.Owner, iv)));
		}
		UpdatePackAppearance(pack);
	}

	protected void UpdateIVVisuals(Entity<IVDripComponent> iv)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			UpdateIVAppearance(iv);
		}
		else
		{
			BaseContainer container = default(BaseContainer);
			if (!_containers.TryGetContainer(Entity<IVDripComponent>.op_Implicit(iv), iv.Comp.Slot, ref container, (ContainerManagerComponent)null))
			{
				return;
			}
			BloodPackComponent pack = default(BloodPackComponent);
			foreach (EntityUid entity in container.ContainedEntities)
			{
				if (((EntitySystem)this).TryComp<BloodPackComponent>(entity, ref pack) && _solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity), pack.Solution, out Entity<SolutionComponent>? _, out Solution solution))
				{
					iv.Comp.FillColor = solution.GetColor(_prototype);
					iv.Comp.FillPercentage = (int)(solution.Volume / solution.MaxVolume * 100);
					((EntitySystem)this).Dirty<IVDripComponent>(iv, (MetaDataComponent)null);
					UpdateIVAppearance(iv);
					return;
				}
			}
			iv.Comp.FillColor = Color.White;
			iv.Comp.FillPercentage = 0;
			((EntitySystem)this).Dirty<IVDripComponent>(iv, (MetaDataComponent)null);
			UpdateIVAppearance(iv);
		}
	}

	protected virtual void UpdateIVAppearance(Entity<IVDripComponent> iv)
	{
	}

	protected virtual void UpdatePackAppearance(Entity<BloodPackComponent> pack)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			if (_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(pack.Owner), pack.Comp.Solution, out Entity<SolutionComponent>? solEnt))
			{
				Solution solution = solEnt.Value.Comp.Solution;
				pack.Comp.FillPercentage = solution.Volume / solution.MaxVolume;
				pack.Comp.FillColor = solution.GetColor(_prototype);
			}
			else
			{
				pack.Comp.FillPercentage = FixedPoint2.Zero;
				pack.Comp.FillColor = Color.Transparent;
			}
			((EntitySystem)this).Dirty<BloodPackComponent>(pack, (MetaDataComponent)null);
		}
	}

	protected virtual void DoRip(DamageSpecifier? damage, EntityUid attached, EntityUid? user, ProtoId<EmotePrototype> ripEmote, bool predict)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (damage != null)
		{
			_damageable.TryChangeDamage(attached, damage, ignoreResistances: true);
		}
		if (_timing.IsFirstTimePredicted)
		{
			string message = base.Loc.GetString("cm-iv-rip", (ValueTuple<string, object>)("target", attached));
			if (predict)
			{
				_popup.PopupClient(message, attached, user);
				Filter others = ((!user.HasValue) ? Filter.Pvs(attached, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null) : Filter.PvsExcept(user.Value, 2f, (IEntityManager)null));
				_popup.PopupEntity(message, attached, others, recordReplay: true);
			}
			else
			{
				_popup.PopupEntity(message, attached);
			}
		}
	}

	private void AttachFeedback(EntityUid iv, EntityUid user, EntityUid to, bool injecting)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			string selfMessage = "cm-iv-attach-self-drawing";
			string othersMessage = "cm-iv-attach-others-drawing";
			if (injecting)
			{
				selfMessage = "cm-iv-attach-self-injecting";
				othersMessage = "cm-iv-attach-others-injecting";
			}
			_popup.PopupClient(base.Loc.GetString(selfMessage, (ValueTuple<string, object>)("iv", iv), (ValueTuple<string, object>)("target", to)), to, user);
			Filter others = Filter.PvsExcept(user, 2f, (IEntityManager)null);
			_popup.PopupEntity(base.Loc.GetString(othersMessage, new(string, object)[3]
			{
				("iv", iv),
				("user", user),
				("target", to)
			}), to, others, recordReplay: true);
		}
	}

	private void DoDetachFeedback(EntityUid iv, EntityUid attached, EntityUid? user, bool predict)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		string selfMessage = base.Loc.GetString("cm-iv-detach-self", (ValueTuple<string, object>)("iv", iv), (ValueTuple<string, object>)("target", attached));
		if (predict)
		{
			_popup.PopupClient(selfMessage, attached, user);
		}
		else
		{
			_popup.PopupEntity(selfMessage, attached);
		}
		if (user.HasValue)
		{
			Filter others = Filter.PvsExcept(user.Value, 2f, (IEntityManager)null);
			_popup.PopupEntity(base.Loc.GetString("cm-iv-detach-others", new(string, object)[3]
			{
				("iv", iv),
				("user", user),
				("target", attached)
			}), attached, others, recordReplay: true);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		BloodPackComponent comp = default(BloodPackComponent);
		foreach (EntityUid pack in _packsToUpdate)
		{
			if (_bloodPackQuery.TryComp(pack, ref comp))
			{
				UpdatePackVisuals(Entity<BloodPackComponent>.op_Implicit((pack, comp)));
			}
		}
		_packsToUpdate.Clear();
	}
}
