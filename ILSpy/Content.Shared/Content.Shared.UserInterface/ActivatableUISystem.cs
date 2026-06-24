using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Managers;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.PowerCell;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.UserInterface;

public sealed class ActivatableUISystem : EntitySystem
{
	[Dependency]
	private ISharedAdminManager _adminManager;

	[Dependency]
	private ActionBlockerSystem _blockerSystem;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedPowerCellSystem _cell;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, ComponentStartup>((EntityEventRefHandler<ActivatableUIComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, UseInHandEvent>((ComponentEventHandler<ActivatableUIComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, ActivateInWorldEvent>((ComponentEventHandler<ActivatableUIComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, InteractUsingEvent>((ComponentEventHandler<ActivatableUIComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, HandDeselectedEvent>((EntityEventRefHandler<ActivatableUIComponent, HandDeselectedEvent>)OnHandDeselected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, GotUnequippedHandEvent>((EntityEventRefHandler<ActivatableUIComponent, GotUnequippedHandEvent>)OnHandUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, BoundUIClosedEvent>((ComponentEventHandler<ActivatableUIComponent, BoundUIClosedEvent>)OnUIClose, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>((ComponentEventHandler<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>)GetActivationVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<ActivatableUIComponent, GetVerbsEvent<Verb>>)GetVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserInterfaceComponent, OpenUiActionEvent>((ComponentEventHandler<UserInterfaceComponent, OpenUiActionEvent>)OnActionPerform, (Type[])null, (Type[])null);
		InitializePower();
	}

	private void OnStartup(Entity<ActivatableUIComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Key == null)
		{
			((EntitySystem)this).Log.Error($"Missing UI Key for entity: {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActivatableUIComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
		}
	}

	private void OnActionPerform(EntityUid uid, UserInterfaceComponent component, OpenUiActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Key != null)
		{
			((HandledEntityEventArgs)args).Handled = _uiSystem.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(uid), args.Key, args.Performer);
		}
	}

	private void GetActivationVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<ActivationVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		if (!component.VerbOnly && ShouldAddVerb(uid, component, args))
		{
			args.Verbs.Add(new ActivationVerb
			{
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					InteractUI(args.User, uid, component);
				},
				Text = base.Loc.GetString(LocId.op_Implicit(component.VerbText)),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"))
			});
		}
	}

	private void GetVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		if (component.VerbOnly && ShouldAddVerb(uid, component, args))
		{
			args.Verbs.Add(new Verb
			{
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					InteractUI(args.User, uid, component);
				},
				Text = base.Loc.GetString(LocId.op_Implicit(component.VerbText)),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"))
			});
		}
	}

	private bool ShouldAddVerb<T>(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<T> args) where T : Verb
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess)
		{
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(component.RequiredItems, args.Using.GetValueOrDefault()))
		{
			return false;
		}
		if (component.RequiresComplex)
		{
			if (args.Hands == null)
			{
				return false;
			}
			if (component.InHandsOnly)
			{
				if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit((args.User, args.Hands)), uid, out string hand))
				{
					return false;
				}
				if (component.RequireActiveHand && args.Hands.ActiveHandId != hand)
				{
					return false;
				}
			}
		}
		if (!args.CanInteract)
		{
			if (((EntitySystem)this).HasComp<GhostComponent>(args.User))
			{
				return !component.BlockSpectators;
			}
			return false;
		}
		return true;
	}

	private void OnUseInHand(EntityUid uid, ActivatableUIComponent component, UseInHandEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !component.VerbOnly && component.RequiredItems == null)
		{
			((HandledEntityEventArgs)args).Handled = InteractUI(args.User, uid, component);
		}
	}

	private void OnActivate(EntityUid uid, ActivatableUIComponent component, ActivateInWorldEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex && !component.VerbOnly && component.RequiredItems == null)
		{
			((HandledEntityEventArgs)args).Handled = InteractUI(args.User, uid, component);
		}
	}

	private void OnInteractUsing(EntityUid uid, ActivatableUIComponent component, InteractUsingEvent args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !component.VerbOnly && component.RequiredItems != null && !_whitelistSystem.IsWhitelistFail(component.RequiredItems, args.Used))
		{
			((HandledEntityEventArgs)args).Handled = InteractUI(args.User, uid, component);
		}
	}

	private void OnUIClose(EntityUid uid, ActivatableUIComponent component, BoundUIClosedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		EntityUid? currentSingleUser = component.CurrentSingleUser;
		if (currentSingleUser.HasValue && !(actor != currentSingleUser.GetValueOrDefault()) && object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, component.Key))
		{
			SetCurrentSingleUser(uid, null, component);
		}
	}

	private bool InteractUI(EntityUid user, EntityUid uiEntity, ActivatableUIComponent aui)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		if (aui.Key == null || !_uiSystem.HasUi(uiEntity, aui.Key, (UserInterfaceComponent)null))
		{
			return false;
		}
		if (_uiSystem.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uiEntity), aui.Key, user))
		{
			_uiSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uiEntity), aui.Key, (EntityUid?)user, false);
			return true;
		}
		if (!_blockerSystem.CanInteract(user, uiEntity) && (!((EntitySystem)this).HasComp<GhostComponent>(user) || aui.BlockSpectators))
		{
			return false;
		}
		if (aui.RequiresComplex && !_blockerSystem.CanComplexInteract(user))
		{
			return false;
		}
		if (aui.InHandsOnly)
		{
			HandsComponent hands = default(HandsComponent);
			if (!((EntitySystem)this).TryComp<HandsComponent>(user, ref hands))
			{
				return false;
			}
			if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit((user, hands)), uiEntity, out string hand))
			{
				return false;
			}
			if (aui.RequireActiveHand && hands.ActiveHandId != hand)
			{
				return false;
			}
		}
		if (aui.AdminOnly && !_adminManager.IsAdmin(user))
		{
			return false;
		}
		if (aui.SingleUser && aui.CurrentSingleUser.HasValue)
		{
			EntityUid? currentSingleUser = aui.CurrentSingleUser;
			if (!currentSingleUser.HasValue || user != currentSingleUser.GetValueOrDefault())
			{
				string message = base.Loc.GetString("machine-already-in-use", (ValueTuple<string, object>)("machine", uiEntity));
				_popupSystem.PopupClient(message, uiEntity, user);
				if (_uiSystem.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uiEntity), aui.Key))
				{
					return true;
				}
				((EntitySystem)this).Log.Error($"Activatable UI has user without being opened? Entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uiEntity))}. User: {aui.CurrentSingleUser}, Key: {aui.Key}");
			}
		}
		ActivatableUIOpenAttemptEvent oae = new ActivatableUIOpenAttemptEvent(user);
		UserOpenActivatableUIAttemptEvent uae = new UserOpenActivatableUIAttemptEvent(user, uiEntity);
		((EntitySystem)this).RaiseLocalEvent<UserOpenActivatableUIAttemptEvent>(user, uae, false);
		((EntitySystem)this).RaiseLocalEvent<ActivatableUIOpenAttemptEvent>(uiEntity, oae, false);
		if (((CancellableEntityEventArgs)oae).Cancelled || ((CancellableEntityEventArgs)uae).Cancelled)
		{
			return false;
		}
		BeforeActivatableUIOpenEvent bae = new BeforeActivatableUIOpenEvent(user);
		((EntitySystem)this).RaiseLocalEvent<BeforeActivatableUIOpenEvent>(uiEntity, bae, false);
		SetCurrentSingleUser(uiEntity, user, aui);
		_uiSystem.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(uiEntity), aui.Key, (EntityUid?)user, false);
		AfterActivatableUIOpenEvent aae = new AfterActivatableUIOpenEvent(user, user);
		((EntitySystem)this).RaiseLocalEvent<AfterActivatableUIOpenEvent>(uiEntity, aae, false);
		return true;
	}

	public void SetCurrentSingleUser(EntityUid uid, EntityUid? user, ActivatableUIComponent? aui = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActivatableUIComponent>(uid, ref aui, true) && aui.SingleUser)
		{
			aui.CurrentSingleUser = user;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)aui, (MetaDataComponent)null);
			((EntitySystem)this).RaiseLocalEvent<ActivatableUIPlayerChangedEvent>(uid, new ActivatableUIPlayerChangedEvent(), false);
		}
	}

	public void CloseAll(EntityUid uid, ActivatableUIComponent? aui = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActivatableUIComponent>(uid, ref aui, false))
		{
			if (aui.Key == null)
			{
				((EntitySystem)this).Log.Error($"Encountered null key in activatable ui on entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			else
			{
				_uiSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), aui.Key);
			}
		}
	}

	private void OnHandDeselected(Entity<ActivatableUIComponent> ent, ref HandDeselectedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.InHandsOnly && ent.Comp.RequireActiveHand)
		{
			CloseAll(Entity<ActivatableUIComponent>.op_Implicit(ent), Entity<ActivatableUIComponent>.op_Implicit(ent));
		}
	}

	private void OnHandUnequipped(Entity<ActivatableUIComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.InHandsOnly)
		{
			CloseAll(Entity<ActivatableUIComponent>.op_Implicit(ent), Entity<ActivatableUIComponent>.op_Implicit(ent));
		}
	}

	private void InitializePower()
	{
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, ActivatableUIOpenAttemptEvent>((ComponentEventHandler<ActivatableUIRequiresPowerCellComponent, ActivatableUIOpenAttemptEvent>)OnBatteryOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, BoundUIOpenedEvent>((ComponentEventHandler<ActivatableUIRequiresPowerCellComponent, BoundUIOpenedEvent>)OnBatteryOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, BoundUIClosedEvent>((ComponentEventHandler<ActivatableUIRequiresPowerCellComponent, BoundUIClosedEvent>)OnBatteryClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, ItemToggledEvent>((EntityEventRefHandler<ActivatableUIRequiresPowerCellComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<ActivatableUIRequiresPowerCellComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		ActivatableUIComponent activatable = default(ActivatableUIComponent);
		if (((EntitySystem)this).TryComp<ActivatableUIComponent>(Entity<ActivatableUIRequiresPowerCellComponent>.op_Implicit(ent), ref activatable) && !args.Activated)
		{
			if (activatable.Key == null)
			{
				((EntitySystem)this).Log.Error($"Encountered null key in activatable ui on entity {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActivatableUIRequiresPowerCellComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
			}
			else
			{
				_uiSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), activatable.Key);
			}
		}
	}

	private void OnBatteryOpened(EntityUid uid, ActivatableUIRequiresPowerCellComponent component, BoundUIOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ActivatableUIComponent activatable = ((EntitySystem)this).Comp<ActivatableUIComponent>(uid);
		if (((BaseBoundUserInterfaceEvent)args).UiKey.Equals(activatable.Key))
		{
			_toggle.TryActivate(Entity<ItemToggleComponent>.op_Implicit(uid));
		}
	}

	private void OnBatteryClosed(EntityUid uid, ActivatableUIRequiresPowerCellComponent component, BoundUIClosedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ActivatableUIComponent activatable = ((EntitySystem)this).Comp<ActivatableUIComponent>(uid);
		if (((BaseBoundUserInterfaceEvent)args).UiKey.Equals(activatable.Key) && !_uiSystem.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), activatable.Key))
		{
			_toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(uid));
		}
	}

	public void CheckUsage(EntityUid uid, ActivatableUIComponent? active = null, ActivatableUIRequiresPowerCellComponent? component = null, PowerCellDrawComponent? draw = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActivatableUIRequiresPowerCellComponent, PowerCellDrawComponent, ActivatableUIComponent>(uid, ref component, ref draw, ref active, false))
		{
			if (active.Key == null)
			{
				((EntitySystem)this).Log.Error($"Encountered null key in activatable ui on entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			else if (!_cell.HasActivatableCharge(uid))
			{
				_uiSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), active.Key);
			}
		}
	}

	private void OnBatteryOpenAttempt(EntityUid uid, ActivatableUIRequiresPowerCellComponent component, ActivatableUIOpenAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		PowerCellDrawComponent draw = default(PowerCellDrawComponent);
		if (((EntitySystem)this).TryComp<PowerCellDrawComponent>(uid, ref draw) && (((CancellableEntityEventArgs)args).Cancelled || !_cell.HasActivatableCharge(uid, draw, null, args.User) || !_cell.HasDrawCharge(uid, draw, null, args.User)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
