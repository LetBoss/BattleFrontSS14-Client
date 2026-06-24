using System;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Sticky.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Sticky.Systems;

public sealed class StickySystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedPopupSystem _popup;

	private const string StickerSlotId = "stickers_container";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StickyComponent, AfterInteractEvent>((EntityEventRefHandler<StickyComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StickyComponent, StickyDoAfterEvent>((EntityEventRefHandler<StickyComponent, StickyDoAfterEvent>)OnStickyDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StickyComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<StickyComponent, GetVerbsEvent<Verb>>)OnGetVerbs, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(Entity<StickyComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				((HandledEntityEventArgs)args).Handled = StartSticking(ent, target2, args.User);
			}
		}
	}

	private void OnGetVerbs(Entity<StickyComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Entity<StickyComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		StickyComponent stickyComponent = default(StickyComponent);
		val.Deconstruct(ref val2, ref stickyComponent);
		EntityUid uid = val2;
		StickyComponent comp = stickyComponent;
		if (!comp.StuckTo.HasValue || !comp.CanUnstick || !args.CanInteract || args.Hands == null)
		{
			return;
		}
		EntityUid user = args.User;
		if (_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(uid), Entity<TransformComponent>.op_Implicit(user), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, delegate(EntityUid entity)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? stuckTo = comp.StuckTo;
			return stuckTo.HasValue && stuckTo.GetValueOrDefault() == entity;
		}))
		{
			args.Verbs.Add(new Verb
			{
				DoContactInteraction = true,
				Text = base.Loc.GetString(LocId.op_Implicit(comp.VerbText)),
				Icon = comp.VerbIcon,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					StartUnsticking(ent, user);
				}
			});
		}
	}

	private bool StartSticking(Entity<StickyComponent> ent, EntityUid target, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Entity<StickyComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		StickyComponent stickyComponent = default(StickyComponent);
		val.Deconstruct(ref val2, ref stickyComponent);
		EntityUid uid = val2;
		StickyComponent comp = stickyComponent;
		if (_whitelist.IsWhitelistFail(comp.Whitelist, target) || _whitelist.IsBlacklistPass(comp.Blacklist, target))
		{
			return false;
		}
		AttemptEntityStickEvent attemptEv = new AttemptEntityStickEvent(target, user);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityStickEvent>(uid, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			return false;
		}
		if (comp.StickDelay <= TimeSpan.Zero)
		{
			StickToEntity(ent, target, user);
			return true;
		}
		if (comp.StickPopupStart.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? stickPopupStart = comp.StickPopupStart;
			string msg = loc.GetString(stickPopupStart.HasValue ? LocId.op_Implicit(stickPopupStart.GetValueOrDefault()) : null);
			_popup.PopupClient(msg, user, user);
		}
		_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, comp.StickDelay, new StickyDoAfterEvent(), uid, target, uid)
		{
			BreakOnMove = true,
			NeedHand = true,
			ForceVisible = true
		});
		return true;
	}

	private void OnStickyDoAfter(Entity<StickyComponent> ent, ref StickyDoAfterEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			EntityUid user = args.User;
			if (!ent.Comp.StuckTo.HasValue)
			{
				StickToEntity(ent, target2, user);
			}
			else
			{
				UnstickFromEntity(ent, user);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void StartUnsticking(Entity<StickyComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Entity<StickyComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		StickyComponent stickyComponent = default(StickyComponent);
		val.Deconstruct(ref val2, ref stickyComponent);
		EntityUid uid = val2;
		StickyComponent comp = stickyComponent;
		EntityUid? stuckTo = comp.StuckTo;
		if (!stuckTo.HasValue)
		{
			return;
		}
		EntityUid stuckTo2 = stuckTo.GetValueOrDefault();
		AttemptEntityUnstickEvent attemptEv = new AttemptEntityUnstickEvent(stuckTo2, user);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityUnstickEvent>(uid, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			return;
		}
		if (comp.UnstickDelay <= TimeSpan.Zero)
		{
			UnstickFromEntity(ent, user);
			return;
		}
		if (comp.UnstickPopupStart.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? unstickPopupStart = comp.UnstickPopupStart;
			string msg = loc.GetString(unstickPopupStart.HasValue ? LocId.op_Implicit(unstickPopupStart.GetValueOrDefault()) : null);
			_popup.PopupClient(msg, user, user);
		}
		_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, comp.UnstickDelay, new StickyDoAfterEvent(), uid, uid)
		{
			BreakOnMove = true,
			NeedHand = true
		});
	}

	public void StickToEntity(Entity<StickyComponent> ent, EntityUid target, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Entity<StickyComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		StickyComponent stickyComponent = default(StickyComponent);
		val.Deconstruct(ref val2, ref stickyComponent);
		EntityUid uid = val2;
		StickyComponent comp = stickyComponent;
		AttemptEntityStickEvent attemptEv = new AttemptEntityStickEvent(target, user);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityStickEvent>(uid, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			return;
		}
		Container container = _container.EnsureContainer<Container>(target, "stickers_container", (ContainerManagerComponent)null);
		((BaseContainer)container).ShowContents = true;
		if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			if (comp.StickPopupSuccess.HasValue)
			{
				ILocalizationManager loc = base.Loc;
				LocId? stickPopupSuccess = comp.StickPopupSuccess;
				string msg = loc.GetString(stickPopupSuccess.HasValue ? LocId.op_Implicit(stickPopupSuccess.GetValueOrDefault()) : null);
				_popup.PopupClient(msg, user, user);
			}
			_appearance.SetData(uid, (Enum)StickyVisuals.IsStuck, (object)true, (AppearanceComponent)null);
			comp.StuckTo = target;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			EntityStuckEvent ev = new EntityStuckEvent(target, user);
			((EntitySystem)this).RaiseLocalEvent<EntityStuckEvent>(uid, ref ev, false);
		}
	}

	public void UnstickFromEntity(Entity<StickyComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		Entity<StickyComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		StickyComponent stickyComponent = default(StickyComponent);
		val.Deconstruct(ref val2, ref stickyComponent);
		EntityUid uid = val2;
		StickyComponent comp = stickyComponent;
		EntityUid? stuckTo = comp.StuckTo;
		if (!stuckTo.HasValue)
		{
			return;
		}
		EntityUid stuckTo2 = stuckTo.GetValueOrDefault();
		AttemptEntityUnstickEvent attemptEv = new AttemptEntityUnstickEvent(stuckTo2, user);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityUnstickEvent>(uid, ref attemptEv, false);
		BaseContainer container = default(BaseContainer);
		if (!attemptEv.Cancelled && _container.TryGetContainer(stuckTo2, "stickers_container", ref container, (ContainerManagerComponent)null) && _container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			if (container.ContainedEntities.Count == 0)
			{
				_container.ShutdownContainer(container);
			}
			_hands.PickupOrDrop(user, uid);
			_appearance.SetData(uid, (Enum)StickyVisuals.IsStuck, (object)false, (AppearanceComponent)null);
			if (comp.UnstickPopupSuccess.HasValue)
			{
				ILocalizationManager loc = base.Loc;
				LocId? unstickPopupSuccess = comp.UnstickPopupSuccess;
				string msg = loc.GetString(unstickPopupSuccess.HasValue ? LocId.op_Implicit(unstickPopupSuccess.GetValueOrDefault()) : null);
				_popup.PopupClient(msg, user, user);
			}
			comp.StuckTo = null;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			EntityUnstuckEvent ev = new EntityUnstuckEvent(stuckTo2, user);
			((EntitySystem)this).RaiseLocalEvent<EntityUnstuckEvent>(uid, ref ev, false);
		}
	}
}
