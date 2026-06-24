using System;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Construction.Components;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared.Lock;

public sealed class LockSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private ActivatableUISystem _activatableUI;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _sharedPopupSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, ComponentStartup>((ComponentEventHandler<LockComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, ActivateInWorldEvent>((ComponentEventHandler<LockComponent, ActivateInWorldEvent>)OnActivated, new Type[1] { typeof(ActivatableUISystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, StorageOpenAttemptEvent>((ComponentEventRefHandler<LockComponent, StorageOpenAttemptEvent>)OnStorageOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, ExaminedEvent>((ComponentEventHandler<LockComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<LockComponent, GetVerbsEvent<AlternativeVerb>>)AddToggleLockVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, GotEmaggedEvent>((ComponentEventRefHandler<LockComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, LockDoAfter>((ComponentEventHandler<LockComponent, LockDoAfter>)OnDoAfterLock, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, UnlockDoAfter>((ComponentEventHandler<LockComponent, UnlockDoAfter>)OnDoAfterUnlock, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockComponent, StorageInteractAttemptEvent>((EntityEventRefHandler<LockComponent, StorageInteractAttemptEvent>)OnStorageInteractAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockedWiresPanelComponent, LockToggleAttemptEvent>((EntityEventRefHandler<LockedWiresPanelComponent, LockToggleAttemptEvent>)OnLockToggleAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockedWiresPanelComponent, AttemptChangePanelEvent>((EntityEventRefHandler<LockedWiresPanelComponent, AttemptChangePanelEvent>)OnAttemptChangePanel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LockedAnchorableComponent, UnanchorAttemptEvent>((EntityEventRefHandler<LockedAnchorableComponent, UnanchorAttemptEvent>)OnUnanchorAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresLockComponent, ActivatableUIOpenAttemptEvent>((ComponentEventHandler<ActivatableUIRequiresLockComponent, ActivatableUIOpenAttemptEvent>)OnUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresLockComponent, LockToggledEvent>((ComponentEventHandler<ActivatableUIRequiresLockComponent, LockToggledEvent>)LockToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleRequiresLockComponent, ItemToggleActivateAttemptEvent>((ComponentEventRefHandler<ItemToggleRequiresLockComponent, ItemToggleActivateAttemptEvent>)OnActivateAttempt, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, LockComponent lockComp, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearanceSystem.SetData(uid, (Enum)LockVisuals.Locked, (object)lockComp.Locked, (AppearanceComponent)null);
	}

	private void OnActivated(EntityUid uid, LockComponent lockComp, ActivateInWorldEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			if (lockComp.Locked && lockComp.UnlockOnClick)
			{
				((HandledEntityEventArgs)args).Handled = TryUnlock(uid, args.User, lockComp);
			}
			else if (!lockComp.Locked && lockComp.LockOnClick)
			{
				((HandledEntityEventArgs)args).Handled = TryLock(uid, args.User, lockComp);
			}
		}
	}

	private void OnStorageOpenAttempt(EntityUid uid, LockComponent component, ref StorageOpenAttemptEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (component.Locked)
		{
			if (!args.Silent)
			{
				_sharedPopupSystem.PopupClient(base.Loc.GetString("entity-storage-component-locked-message"), uid, args.User);
			}
			args.Cancelled = true;
		}
	}

	private void OnExamined(EntityUid uid, LockComponent lockComp, ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		args.PushText(base.Loc.GetString(lockComp.Locked ? "lock-comp-on-examined-is-locked" : "lock-comp-on-examined-is-unlocked", (ValueTuple<string, object>)("entityName", Identity.Name(uid, (IEntityManager)(object)base.EntityManager))));
	}

	public bool TryLock(EntityUid uid, EntityUid user, LockComponent? lockComp = null, bool skipDoAfter = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LockComponent>(uid, ref lockComp, true))
		{
			return false;
		}
		if (!CanToggleLock(uid, user, quiet: false))
		{
			return false;
		}
		if (lockComp.UseAccess && !HasUserAccess(uid, user, null, quiet: false))
		{
			return false;
		}
		if (!skipDoAfter && lockComp.LockTime != TimeSpan.Zero)
		{
			return _doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, lockComp.LockTime, new LockDoAfter(), uid, uid)
			{
				BreakOnDamage = true,
				BreakOnMove = true,
				NeedHand = true,
				BreakOnDropItem = false
			});
		}
		Lock(uid, user, lockComp);
		return true;
	}

	public void Lock(EntityUid uid, EntityUid? user, LockComponent? lockComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LockComponent>(uid, ref lockComp, true) || lockComp.Locked)
		{
			return;
		}
		if (user.HasValue)
		{
			EntityUid valueOrDefault = user.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault)).Valid)
			{
				_sharedPopupSystem.PopupClient(base.Loc.GetString("lock-comp-do-lock-success", (ValueTuple<string, object>)("entityName", Identity.Name(uid, (IEntityManager)(object)base.EntityManager))), uid, user);
			}
		}
		_audio.PlayPredicted(lockComp.LockSound, uid, user, (AudioParams?)null);
		lockComp.Locked = true;
		_appearanceSystem.SetData(uid, (Enum)LockVisuals.Locked, (object)true, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)lockComp, (MetaDataComponent)null);
		LockToggledEvent ev = new LockToggledEvent(Locked: true);
		((EntitySystem)this).RaiseLocalEvent<LockToggledEvent>(uid, ref ev, true);
	}

	public void Unlock(EntityUid uid, EntityUid? user, LockComponent? lockComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LockComponent>(uid, ref lockComp, true) || !lockComp.Locked)
		{
			return;
		}
		if (user.HasValue)
		{
			EntityUid valueOrDefault = user.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault)).Valid)
			{
				_sharedPopupSystem.PopupClient(base.Loc.GetString("lock-comp-do-unlock-success", (ValueTuple<string, object>)("entityName", Identity.Name(uid, (IEntityManager)(object)base.EntityManager))), uid, user.Value);
			}
		}
		_audio.PlayPredicted(lockComp.UnlockSound, uid, user, (AudioParams?)null);
		lockComp.Locked = false;
		_appearanceSystem.SetData(uid, (Enum)LockVisuals.Locked, (object)false, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)lockComp, (MetaDataComponent)null);
		LockToggledEvent ev = new LockToggledEvent(Locked: false);
		((EntitySystem)this).RaiseLocalEvent<LockToggledEvent>(uid, ref ev, true);
	}

	public bool TryUnlock(EntityUid uid, EntityUid user, LockComponent? lockComp = null, bool skipDoAfter = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LockComponent>(uid, ref lockComp, true))
		{
			return false;
		}
		if (!CanToggleLock(uid, user, quiet: false))
		{
			return false;
		}
		if (lockComp.UseAccess && !HasUserAccess(uid, user, null, quiet: false))
		{
			return false;
		}
		if (!skipDoAfter && lockComp.UnlockTime != TimeSpan.Zero)
		{
			return _doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, lockComp.LockTime, new UnlockDoAfter(), uid, uid)
			{
				BreakOnDamage = true,
				BreakOnMove = true,
				NeedHand = true,
				BreakOnDropItem = false
			});
		}
		Unlock(uid, user, lockComp);
		return true;
	}

	public bool IsLocked(Entity<LockComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LockComponent>(Entity<LockComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		return ent.Comp.Locked;
	}

	public bool CanToggleLock(EntityUid uid, EntityUid user, bool quiet = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!_actionBlocker.CanComplexInteract(user))
		{
			return false;
		}
		LockToggleAttemptEvent ev = new LockToggleAttemptEvent(user, quiet);
		((EntitySystem)this).RaiseLocalEvent<LockToggleAttemptEvent>(uid, ref ev, true);
		if (ev.Cancelled)
		{
			return false;
		}
		UserLockToggleAttemptEvent userEv = new UserLockToggleAttemptEvent(uid, quiet);
		((EntitySystem)this).RaiseLocalEvent<UserLockToggleAttemptEvent>(user, ref userEv, true);
		return !userEv.Cancelled;
	}

	private bool HasUserAccess(EntityUid uid, EntityUid user, AccessReaderComponent? reader = null, bool quiet = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<AccessReaderComponent>(uid, ref reader, false))
		{
			return true;
		}
		if (_accessReader.IsAllowed(user, uid, reader))
		{
			return true;
		}
		if (!quiet)
		{
			_sharedPopupSystem.PopupClient(base.Loc.GetString("lock-comp-has-user-access-fail"), uid, user);
		}
		return false;
	}

	private void AddToggleLockVerb(EntityUid uid, LockComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && args.CanComplexInteract)
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = (component.Locked ? ((Action)delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryUnlock(uid, args.User, component);
				}) : ((Action)delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryLock(uid, args.User, component);
				})),
				Text = base.Loc.GetString(component.Locked ? "toggle-lock-verb-unlock" : "toggle-lock-verb-lock"),
				Icon = (SpriteSpecifier?)((!component.Locked) ? new Texture(new ResPath("/Textures/Interface/VerbIcons/lock.svg.192dpi.png")) : new Texture(new ResPath("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png")))
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnEmagged(EntityUid uid, LockComponent component, ref GotEmaggedEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Access) && component.Locked && component.BreakOnAccessBreaker)
		{
			_audio.PlayPredicted(component.UnlockSound, uid, (EntityUid?)args.UserUid, (AudioParams?)null);
			component.Locked = false;
			_appearanceSystem.SetData(uid, (Enum)LockVisuals.Locked, (object)false, (AppearanceComponent)null);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			LockToggledEvent ev = new LockToggledEvent(Locked: false);
			((EntitySystem)this).RaiseLocalEvent<LockToggledEvent>(uid, ref ev, true);
			args.Repeatable = true;
			args.Handled = true;
		}
	}

	private void OnDoAfterLock(EntityUid uid, LockComponent component, LockDoAfter args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			TryLock(uid, args.User, null, skipDoAfter: true);
		}
	}

	private void OnDoAfterUnlock(EntityUid uid, LockComponent component, UnlockDoAfter args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			TryUnlock(uid, args.User, null, skipDoAfter: true);
		}
	}

	private void OnStorageInteractAttempt(Entity<LockComponent> ent, ref StorageInteractAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Locked)
		{
			args.Cancelled = true;
		}
	}

	private void OnLockToggleAttempt(Entity<LockedWiresPanelComponent> ent, ref LockToggleAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		WiresPanelComponent panel = default(WiresPanelComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<WiresPanelComponent>(Entity<LockedWiresPanelComponent>.op_Implicit(ent), ref panel) && panel.Open)
		{
			if (!args.Silent)
			{
				_sharedPopupSystem.PopupClient(base.Loc.GetString("construction-step-condition-wire-panel-close"), Entity<LockedWiresPanelComponent>.op_Implicit(ent), args.User);
			}
			args.Cancelled = true;
		}
	}

	private void OnAttemptChangePanel(Entity<LockedWiresPanelComponent> ent, ref AttemptChangePanelEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComp = default(LockComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<LockComponent>(Entity<LockedWiresPanelComponent>.op_Implicit(ent), ref lockComp) && lockComp.Locked)
		{
			_sharedPopupSystem.PopupClient(base.Loc.GetString("lock-comp-generic-fail", (ValueTuple<string, object>)("target", Identity.Entity(Entity<LockedWiresPanelComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager))), Entity<LockedWiresPanelComponent>.op_Implicit(ent), args.User);
			args.Cancelled = true;
		}
	}

	private void OnUnanchorAttempt(Entity<LockedAnchorableComponent> ent, ref UnanchorAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComp = default(LockComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && ((EntitySystem)this).TryComp<LockComponent>(Entity<LockedAnchorableComponent>.op_Implicit(ent), ref lockComp) && lockComp.Locked)
		{
			_sharedPopupSystem.PopupClient(base.Loc.GetString("lock-comp-generic-fail", (ValueTuple<string, object>)("target", Identity.Entity(Entity<LockedAnchorableComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager))), Entity<LockedAnchorableComponent>.op_Implicit(ent), args.User);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUIOpenAttempt(EntityUid uid, ActivatableUIRequiresLockComponent component, ActivatableUIOpenAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComp = default(LockComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && ((EntitySystem)this).TryComp<LockComponent>(uid, ref lockComp) && lockComp.Locked != component.RequireLocked)
		{
			((CancellableEntityEventArgs)args).Cancel();
			if (lockComp.Locked)
			{
				_sharedPopupSystem.PopupClient(base.Loc.GetString("entity-storage-component-locked-message"), uid, args.User);
			}
			_audio.PlayPredicted(component.AccessDeniedSound, uid, (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void LockToggled(EntityUid uid, ActivatableUIRequiresLockComponent component, LockToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComp = default(LockComponent);
		if (((EntitySystem)this).TryComp<LockComponent>(uid, ref lockComp) && lockComp.Locked != component.RequireLocked)
		{
			_activatableUI.CloseAll(uid);
		}
	}

	private void OnActivateAttempt(EntityUid uid, ItemToggleRequiresLockComponent component, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComp = default(LockComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<LockComponent>(uid, ref lockComp) && lockComp.Locked != component.RequireLocked)
		{
			args.Cancelled = true;
			if (lockComp.Locked)
			{
				_sharedPopupSystem.PopupClient(base.Loc.GetString("lock-comp-generic-fail", (ValueTuple<string, object>)("target", Identity.Entity(uid, (IEntityManager)(object)base.EntityManager))), uid, args.User);
			}
		}
	}
}
