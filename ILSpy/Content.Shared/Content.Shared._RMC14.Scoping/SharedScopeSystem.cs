using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Emplacements;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Camera;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Scoping;

public abstract class SharedScopeSystem : EntitySystem
{
	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedContentEyeSystem _contentEye;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedWeaponMountSystem _weaponMount;

	public override void Initialize()
	{
		InitializeUser();
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, MapInitEvent>((EntityEventRefHandler<ScopeComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, ComponentRemove>((EntityEventRefHandler<ScopeComponent, ComponentRemove>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, EntityTerminatingEvent>((EntityEventRefHandler<ScopeComponent, EntityTerminatingEvent>)OnScopeEntityTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, GotUnequippedHandEvent>((EntityEventRefHandler<ScopeComponent, GotUnequippedHandEvent>)OnUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, HandDeselectedEvent>((EntityEventRefHandler<ScopeComponent, HandDeselectedEvent>)OnDeselectHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, ItemUnwieldedEvent>((EntityEventRefHandler<ScopeComponent, ItemUnwieldedEvent>)OnUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, GetItemActionsEvent>((EntityEventRefHandler<ScopeComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, ToggleActionEvent>((EntityEventRefHandler<ScopeComponent, ToggleActionEvent>)OnToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, ScopeCycleZoomLevelEvent>((EntityEventRefHandler<ScopeComponent, ScopeCycleZoomLevelEvent>)OnCycleZoomLevel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, ActivateInWorldEvent>((EntityEventRefHandler<ScopeComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, GunShotEvent>((EntityEventRefHandler<ScopeComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopeComponent, ScopeDoAfterEvent>((EntityEventRefHandler<ScopeComponent, ScopeDoAfterEvent>)OnScopeDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunScopingComponent, GotUnequippedHandEvent>((EntityEventRefHandler<GunScopingComponent, GotUnequippedHandEvent>)OnGunUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunScopingComponent, HandDeselectedEvent>((EntityEventRefHandler<GunScopingComponent, HandDeselectedEvent>)OnGunDeselectHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunScopingComponent, ItemUnwieldedEvent>((EntityEventRefHandler<GunScopingComponent, ItemUnwieldedEvent>)OnGunUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunScopingComponent, GunShotEvent>((EntityEventRefHandler<GunScopingComponent, GunShotEvent>)OnGunGunShot, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ScopeComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		_actionContainer.EnsureAction(ent.Owner, ref ent.Comp.ScopingToggleActionEntity, EntProtoId.op_Implicit(ent.Comp.ScopingToggleAction));
		if (ent.Comp.ZoomLevels.Count > 1)
		{
			_actionContainer.EnsureAction(ent.Owner, ref ent.Comp.CycleZoomLevelActionEntity, EntProtoId.op_Implicit(ent.Comp.CycleZoomLevelAction));
		}
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	private void OnShutdown(Entity<ScopeComponent> ent, ref ComponentRemove args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = ent.Comp.User;
		if (user.HasValue)
		{
			EntityUid user2 = user.GetValueOrDefault();
			Unscope(ent);
			_actionsSystem.RemoveProvidedActions(user2, ent.Owner);
		}
	}

	private void OnScopeEntityTerminating(Entity<ScopeComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Unscope(ent);
	}

	private void OnUnequip(Entity<ScopeComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Unscope(ent);
	}

	private void OnDeselectHand(Entity<ScopeComponent> ent, ref HandDeselectedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Unscope(ent);
	}

	private void OnUnwielded(Entity<ScopeComponent> ent, ref ItemUnwieldedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.RequireWielding)
		{
			Unscope(ent);
		}
	}

	private void OnGetActions(Entity<ScopeComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.ScopingToggleActionEntity, EntProtoId.op_Implicit(ent.Comp.ScopingToggleAction));
		if (ent.Comp.ZoomLevels.Count > 1)
		{
			args.AddAction(ref ent.Comp.CycleZoomLevelActionEntity, EntProtoId.op_Implicit(ent.Comp.CycleZoomLevelAction));
		}
	}

	private void OnToggleAction(Entity<ScopeComponent> ent, ref ToggleActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleScoping(ent, args.Performer);
		}
	}

	private void OnCycleZoomLevel(Entity<ScopeComponent> scope, ref ScopeCycleZoomLevelEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (scope.Comp.CurrentZoomLevel >= scope.Comp.ZoomLevels.Count - 1)
			{
				scope.Comp.CurrentZoomLevel = 0;
			}
			else
			{
				scope.Comp.CurrentZoomLevel++;
			}
			ScopeZoomLevel zoomLevel = GetCurrentZoomLevel(scope);
			if (zoomLevel.Name != null)
			{
				_popup.PopupClient(base.Loc.GetString("rcm-action-popup-scope-cycle-zoom", (ValueTuple<string, object>)("zoom", zoomLevel.Name)), args.Performer, args.Performer);
			}
			((EntitySystem)this).Dirty<ScopeComponent>(scope, (MetaDataComponent)null);
		}
	}

	private void OnActivateInWorld(Entity<ScopeComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex && ent.Comp.UseInHand)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleScoping(ent, args.User);
		}
	}

	private void OnGunShot(Entity<ScopeComponent> ent, ref GunShotEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<WeaponControllerComponent>(args.User))
		{
			Angle localRotation = ((EntitySystem)this).Transform(args.User).LocalRotation;
			Direction cardinalDir = ((Angle)(ref localRotation)).GetCardinalDir();
			if (ent.Comp.ScopingDirection != (Direction?)cardinalDir)
			{
				Unscope(ent);
			}
		}
	}

	private void OnScopeDoAfter(Entity<ScopeComponent> ent, ref ScopeDoAfterEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (args.Cancelled)
		{
			DeleteRelay(ent, args.User);
			return;
		}
		EntityUid user = args.User;
		if (!CanScopePopup(ent, user))
		{
			DeleteRelay(ent, args.User);
		}
		else
		{
			Scope(ent, user, args.Direction);
		}
	}

	private void OnGunUnequip(Entity<GunScopingComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UnscopeGun(ent);
	}

	private void OnGunDeselectHand(Entity<GunScopingComponent> ent, ref HandDeselectedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UnscopeGun(ent);
	}

	private void OnGunUnwielded(Entity<GunScopingComponent> ent, ref ItemUnwieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UnscopeGun(ent);
	}

	private void OnGunGunShot(Entity<GunScopingComponent> ent, ref GunShotEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Angle localRotation = ((EntitySystem)this).Transform(args.User).LocalRotation;
		Direction dir = ((Angle)(ref localRotation)).GetCardinalDir();
		ScopeComponent scope = default(ScopeComponent);
		if (((EntitySystem)this).TryComp<ScopeComponent>(ent.Comp.Scope, ref scope) && scope.ScopingDirection != (Direction?)dir)
		{
			UnscopeGun(ent);
		}
	}

	private bool CanScopePopup(Entity<ScopeComponent> scope, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		EntityUid ent = scope.Owner;
		if (scope.Comp.Attachment && !TryGetActiveEntity(scope, out ent))
		{
			string msgError = base.Loc.GetString("cm-action-popup-scoping-must-attach", (ValueTuple<string, object>)("scope", ent));
			_popup.PopupClient(msgError, user, user);
			return false;
		}
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var heldItem))
		{
			goto IL_009f;
		}
		if (!scope.Comp.Attachment)
		{
			EntityUid? val = heldItem;
			EntityUid owner = scope.Owner;
			if (!val.HasValue || val.GetValueOrDefault() != owner)
			{
				goto IL_009f;
			}
		}
		goto IL_00e5;
		IL_00e5:
		if (_pulling.IsPulled(user))
		{
			string msgError2 = base.Loc.GetString("cm-action-popup-scoping-user-must-not-pulled", (ValueTuple<string, object>)("scope", ent));
			_popup.PopupClient(msgError2, user, user);
			return false;
		}
		if (_container.IsEntityInContainer(user, (MetaDataComponent)null) && !scope.Comp.CanUseInsideContainer)
		{
			string msgError3 = base.Loc.GetString("cm-action-popup-scoping-user-must-not-contained", (ValueTuple<string, object>)("scope", ent));
			_popup.PopupClient(msgError3, user, user);
			return false;
		}
		WieldableComponent wieldable = default(WieldableComponent);
		if (scope.Comp.RequireWielding && ((EntitySystem)this).TryComp<WieldableComponent>(ent, ref wieldable) && !wieldable.Wielded)
		{
			string msgError4 = base.Loc.GetString("cm-action-popup-scoping-user-must-wield", (ValueTuple<string, object>)("scope", ent));
			_popup.PopupClient(msgError4, user, user);
			return false;
		}
		if (!GetCurrentZoomLevel(scope).AllowMovement && IsMoveHeld(user))
		{
			string msgError5 = base.Loc.GetString("cm-action-popup-scoping-user-must-not-move", (ValueTuple<string, object>)("scope", ent));
			_popup.PopupClient(msgError5, user, user);
			return false;
		}
		return true;
		IL_009f:
		if (!scope.Comp.CanUseInsideContainer)
		{
			string msgError6 = base.Loc.GetString("cm-action-popup-scoping-user-must-hold", (ValueTuple<string, object>)("scope", ent));
			_popup.PopupClient(msgError6, user, user);
			return false;
		}
		goto IL_00e5;
	}

	public virtual Direction? StartScoping(Entity<ScopeComponent> scope, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanScopePopup(scope, user))
		{
			return null;
		}
		Angle worldRotation = _transform.GetWorldRotation(user);
		Direction cardinalDir = ((Angle)(ref worldRotation)).GetCardinalDir();
		ScopeDoAfterEvent ev = new ScopeDoAfterEvent(cardinalDir);
		ScopeZoomLevel zoomLevel = GetCurrentZoomLevel(scope);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, zoomLevel.DoAfter, ev, Entity<ScopeComponent>.op_Implicit(scope), null, Entity<ScopeComponent>.op_Implicit(scope))
		{
			BreakOnMove = !zoomLevel.AllowMovement
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			return cardinalDir;
		}
		return null;
	}

	private void Scope(Entity<ScopeComponent> scope, EntityUid user, Direction direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		ScopingComponent scoping = default(ScopingComponent);
		if (((EntitySystem)this).TryComp<ScopingComponent>(user, ref scoping))
		{
			UserStopScoping(Entity<ScopingComponent>.op_Implicit((user, scoping)));
		}
		ScopeZoomLevel zoomLevel = GetCurrentZoomLevel(scope);
		bool num = ((EntitySystem)this).HasComp<MountableWeaponComponent>(scope.Owner);
		scope.Comp.User = user;
		scope.Comp.ScopingDirection = direction;
		((EntitySystem)this).Dirty<ScopeComponent>(scope, (MetaDataComponent)null);
		scoping = ((EntitySystem)this).EnsureComp<ScopingComponent>(user);
		scoping.Scope = Entity<ScopeComponent>.op_Implicit(scope);
		scoping.AllowMovement = zoomLevel.AllowMovement;
		((EntitySystem)this).Dirty(user, (IComponent)(object)scoping, (MetaDataComponent)null);
		if (scope.Comp.Attachment && TryGetActiveEntity(scope, out var active))
		{
			GunScopingComponent gunScoping = ((EntitySystem)this).EnsureComp<GunScopingComponent>(active);
			gunScoping.Scope = Entity<ScopeComponent>.op_Implicit(scope);
			((EntitySystem)this).Dirty(active, (IComponent)(object)gunScoping, (MetaDataComponent)null);
		}
		Angle mountRot;
		Vector2 targetOffset = ((num && _weaponMount.TryGetMountSeatingRotation(scope.Owner, out mountRot)) ? (((Angle)(ref mountRot)).ToWorldVec() * GetScopeOffsetMagnitude(scope)) : GetScopeOffset(scope, direction));
		scoping.EyeOffset = targetOffset;
		if (scope.Comp.ScopePopup != null)
		{
			string msgUser = base.Loc.GetString(scope.Comp.ScopePopup, (ValueTuple<string, object>)("scope", scope.Owner));
			_popup.PopupClient(msgUser, user, user);
		}
		SharedActionsSystem actionsSystem = _actionsSystem;
		EntityUid? scopingToggleActionEntity = scope.Comp.ScopingToggleActionEntity;
		actionsSystem.SetToggled(scopingToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(scopingToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: true);
		_contentEye.SetZoom(user, Vector2.One * zoomLevel.Zoom, ignoreLimits: true);
		UpdateOffset(user);
		OnScoped(scope, user, direction);
		ScopedEvent ev = new ScopedEvent(user, scope);
		((EntitySystem)this).RaiseLocalEvent<ScopedEvent>(user, ref ev, false);
	}

	public virtual bool Unscope(Entity<ScopeComponent> scope)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = scope.Comp.User;
		if (user.HasValue)
		{
			EntityUid user2 = user.GetValueOrDefault();
			((EntitySystem)this).RemCompDeferred<ScopingComponent>(user2);
			if (scope.Comp.Attachment && TryGetActiveEntity(scope, out var active))
			{
				((EntitySystem)this).RemCompDeferred<GunScopingComponent>(active);
			}
			if (scope.Comp.Attachment && scope.Comp.User.HasValue)
			{
				AttachableToggleableInterruptEvent interruptEvent = new AttachableToggleableInterruptEvent(scope.Comp.User.Value);
				((EntitySystem)this).RaiseLocalEvent<AttachableToggleableInterruptEvent>(scope.Owner, ref interruptEvent, false);
			}
			scope.Comp.User = null;
			scope.Comp.ScopingDirection = null;
			((EntitySystem)this).Dirty<ScopeComponent>(scope, (MetaDataComponent)null);
			if (scope.Comp.UnScopePopup != null)
			{
				string msgUser = base.Loc.GetString(scope.Comp.UnScopePopup, (ValueTuple<string, object>)("scope", scope.Owner));
				_popup.PopupClient(msgUser, user2, user2);
			}
			SharedActionsSystem actionsSystem = _actionsSystem;
			user = scope.Comp.ScopingToggleActionEntity;
			actionsSystem.SetToggled(user.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(user.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
			_contentEye.ResetZoom(user2);
			return true;
		}
		return false;
	}

	private void UnscopeGun(Entity<GunScopingComponent> gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ScopeComponent scope = default(ScopeComponent);
		if (((EntitySystem)this).TryComp<ScopeComponent>(gun.Comp.Scope, ref scope))
		{
			Unscope(Entity<ScopeComponent>.op_Implicit((gun.Comp.Scope.Value, scope)));
		}
	}

	private void ToggleScoping(Entity<ScopeComponent> scope, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ScopingComponent>(user))
		{
			Unscope(scope);
			ScopingComponent scoping = default(ScopingComponent);
			if (((EntitySystem)this).TryComp<ScopingComponent>(user, ref scoping))
			{
				UserStopScoping(Entity<ScopingComponent>.op_Implicit((user, scoping)));
			}
		}
		else
		{
			StartScoping(scope, user);
		}
	}

	private bool TryGetActiveEntity(Entity<ScopeComponent> scope, out EntityUid active)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!scope.Comp.Attachment)
		{
			active = Entity<ScopeComponent>.op_Implicit(scope);
			return true;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<ScopeComponent>.op_Implicit(scope), null)), ref container) || !((EntitySystem)this).HasComp<GunComponent>(container.Owner))
		{
			active = default(EntityUid);
			return false;
		}
		active = container.Owner;
		return true;
	}

	protected Vector2 GetScopeOffset(Entity<ScopeComponent> scope, Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return DirectionExtensions.ToVec(direction) * GetScopeOffsetMagnitude(scope);
	}

	private float GetScopeOffsetMagnitude(Entity<ScopeComponent> scope)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ScopeZoomLevel zoomLevel = GetCurrentZoomLevel(scope);
		return (zoomLevel.Offset * zoomLevel.Zoom - 1f) / 2f;
	}

	protected virtual void DeleteRelay(Entity<ScopeComponent> scope, EntityUid? user)
	{
	}

	protected virtual void OnScoped(Entity<ScopeComponent> scope, EntityUid user, Direction direction)
	{
	}

	private ScopeZoomLevel GetCurrentZoomLevel(Entity<ScopeComponent> scope)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ValidateCurrentZoomLevel(scope);
		return scope.Comp.ZoomLevels[scope.Comp.CurrentZoomLevel];
	}

	private void ValidateCurrentZoomLevel(Entity<ScopeComponent> scope)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		bool dirty = false;
		if (scope.Comp.ZoomLevels == null || scope.Comp.ZoomLevels.Count <= 0)
		{
			scope.Comp.ZoomLevels = new List<ScopeZoomLevel>
			{
				new ScopeZoomLevel(null, 1f, 15f, AllowMovement: false, TimeSpan.FromSeconds(1L))
			};
			dirty = true;
		}
		if (scope.Comp.CurrentZoomLevel >= scope.Comp.ZoomLevels.Count)
		{
			scope.Comp.CurrentZoomLevel = 0;
			dirty = true;
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty<ScopeComponent>(scope, (MetaDataComponent)null);
		}
	}

	private void UpdateOffset(EntityUid user)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GetEyeOffsetEvent ev = default(GetEyeOffsetEvent);
		((EntitySystem)this).RaiseLocalEvent<GetEyeOffsetEvent>(user, ref ev, false);
		_eye.SetOffset(user, ev.Offset, (EyeComponent)null);
	}

	private bool IsMoveHeld(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent mover = default(InputMoverComponent);
		if (((EntitySystem)this).TryComp<InputMoverComponent>(user, ref mover))
		{
			return (mover.HeldMoveButtons & MoveButtons.AnyDirection) != 0;
		}
		return false;
	}

	private void InitializeUser()
	{
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, ComponentRemove>((EntityEventRefHandler<ScopingComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, MoveInputEvent>((EntityEventRefHandler<ScopingComponent, MoveInputEvent>)OnMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, PullStartedMessage>((EntityEventRefHandler<ScopingComponent, PullStartedMessage>)OnPullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, EntParentChangedMessage>((EntityEventRefHandler<ScopingComponent, EntParentChangedMessage>)OnParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, ContainerGettingInsertedAttemptEvent>((EntityEventRefHandler<ScopingComponent, ContainerGettingInsertedAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, EntityTerminatingEvent>((EntityEventRefHandler<ScopingComponent, EntityTerminatingEvent>)OnEntityTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, GetEyeOffsetEvent>((EntityEventRefHandler<ScopingComponent, GetEyeOffsetEvent>)OnGetEyeOffset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, PlayerDetachedEvent>((EntityEventRefHandler<ScopingComponent, PlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, KnockedDownEvent>((EntityEventRefHandler<ScopingComponent, KnockedDownEvent>)OnKnockedDown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, StunnedEvent>((EntityEventRefHandler<ScopingComponent, StunnedEvent>)OnStunned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScopingComponent, MobStateChangedEvent>((EntityEventRefHandler<ScopingComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnRemove(Entity<ScopingComponent> user, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<ScopingComponent>.op_Implicit(user), (MetaDataComponent)null))
		{
			UpdateOffset(Entity<ScopingComponent>.op_Implicit(user));
		}
	}

	private void OnMoveInput(Entity<ScopingComponent> ent, ref MoveInputEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement && !ent.Comp.AllowMovement)
		{
			UserStopScoping(ent);
		}
	}

	private void OnPullStarted(Entity<ScopingComponent> ent, ref PullStartedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner))
		{
			UserStopScoping(ent);
		}
	}

	private void OnParentChanged(Entity<ScopingComponent> ent, ref EntParentChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<WeaponControllerComponent>(Entity<ScopingComponent>.op_Implicit(ent)))
		{
			UserStopScoping(ent);
		}
	}

	private void OnInsertAttempt(Entity<ScopingComponent> ent, ref ContainerGettingInsertedAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopScoping(ent);
	}

	private void OnEntityTerminating(Entity<ScopingComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopScoping(ent);
	}

	private void OnGetEyeOffset(Entity<ScopingComponent> ent, ref GetEyeOffsetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? scope = ent.Comp.Scope;
		if (scope.HasValue)
		{
			EntityUid scopeUid = scope.GetValueOrDefault();
			ScopeComponent scope2 = default(ScopeComponent);
			if (((EntitySystem)this).TryComp<ScopeComponent>(scopeUid, ref scope2) && ((EntitySystem)this).HasComp<MountableWeaponComponent>(scopeUid) && _weaponMount.TryGetMountSeatingRotation(scopeUid, out var mountRot))
			{
				args.Offset += ((Angle)(ref mountRot)).ToWorldVec() * GetScopeOffsetMagnitude(Entity<ScopeComponent>.op_Implicit((scopeUid, scope2)));
				return;
			}
		}
		args.Offset += ent.Comp.EyeOffset;
	}

	private void OnPlayerDetached(Entity<ScopingComponent> ent, ref PlayerDetachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopScoping(ent);
	}

	private void OnKnockedDown(Entity<ScopingComponent> ent, ref KnockedDownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopScoping(ent);
	}

	private void OnStunned(Entity<ScopingComponent> ent, ref StunnedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopScoping(ent);
	}

	private void OnMobStateChanged(Entity<ScopingComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Alive)
		{
			UserStopScoping(ent);
		}
	}

	private void UserStopScoping(Entity<ScopingComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? scope = ent.Comp.Scope;
		((EntitySystem)this).RemCompDeferred<ScopingComponent>(Entity<ScopingComponent>.op_Implicit(ent));
		ScopeComponent scopeComponent = default(ScopeComponent);
		if (((EntitySystem)this).TryComp<ScopeComponent>(scope, ref scopeComponent))
		{
			EntityUid? user = scopeComponent.User;
			EntityUid val = Entity<ScopingComponent>.op_Implicit(ent);
			if (user.HasValue && user.GetValueOrDefault() == val)
			{
				Unscope(Entity<ScopeComponent>.op_Implicit((scope.Value, scopeComponent)));
			}
		}
	}
}
