using System;
using Content.Shared.Actions;
using Content.Shared.Gravity;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared.Movement.Systems;

public abstract class SharedJetpackSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedContainerSystem Container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<JetpackComponent, GetItemActionsEvent>((ComponentEventHandler<JetpackComponent, GetItemActionsEvent>)OnJetpackGetAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackComponent, DroppedEvent>((ComponentEventHandler<JetpackComponent, DroppedEvent>)OnJetpackDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackComponent, ToggleJetpackEvent>((ComponentEventHandler<JetpackComponent, ToggleJetpackEvent>)OnJetpackToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackUserComponent, RefreshWeightlessModifiersEvent>((EntityEventRefHandler<JetpackUserComponent, RefreshWeightlessModifiersEvent>)OnJetpackUserWeightlessMovement, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackUserComponent, CanWeightlessMoveEvent>((ComponentEventRefHandler<JetpackUserComponent, CanWeightlessMoveEvent>)OnJetpackUserCanWeightless, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackUserComponent, EntParentChangedMessage>((ComponentEventRefHandler<JetpackUserComponent, EntParentChangedMessage>)OnJetpackUserEntParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<JetpackComponent, EntGotInsertedIntoContainerMessage>)OnJetpackMoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GravityChangedEvent>((EntityEventRefHandler<GravityChangedEvent>)OnJetpackUserGravityChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JetpackComponent, MapInitEvent>((ComponentEventHandler<JetpackComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnJetpackUserWeightlessMovement(Entity<JetpackUserComponent> ent, ref RefreshWeightlessModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		args.WeightlessAcceleration = ent.Comp.WeightlessAcceleration;
		args.WeightlessModifier = ent.Comp.WeightlessModifier;
		args.WeightlessFriction = ent.Comp.WeightlessFriction;
		args.WeightlessFrictionNoInput = ent.Comp.WeightlessFrictionNoInput;
	}

	private void OnMapInit(EntityUid uid, JetpackComponent component, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_actionContainer.EnsureAction(uid, ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnJetpackUserGravityChanged(ref GravityChangedEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gridUid = ev.ChangedGridIndex;
		EntityQuery<JetpackComponent> jetpackQuery = ((EntitySystem)this).GetEntityQuery<JetpackComponent>();
		EntityQueryEnumerator<JetpackUserComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<JetpackUserComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		JetpackUserComponent user = default(JetpackUserComponent);
		TransformComponent transform = default(TransformComponent);
		JetpackComponent jetpack = default(JetpackComponent);
		while (query.MoveNext(ref uid, ref user, ref transform))
		{
			EntityUid? gridUid2 = transform.GridUid;
			EntityUid val = gridUid;
			if (gridUid2.HasValue && gridUid2.GetValueOrDefault() == val && ev.HasGravity && jetpackQuery.TryGetComponent(user.Jetpack, ref jetpack))
			{
				_popup.PopupClient(base.Loc.GetString("jetpack-to-grid"), uid, uid);
				SetEnabled(user.Jetpack, jetpack, enabled: false, uid);
			}
		}
	}

	private void OnJetpackDropped(EntityUid uid, JetpackComponent component, DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		SetEnabled(uid, component, enabled: false, args.User);
	}

	private void OnJetpackMoved(Entity<JetpackComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = ((ContainerModifiedMessage)args).Container.Owner;
		EntityUid? jetpackUser = ent.Comp.JetpackUser;
		if (!jetpackUser.HasValue || owner != jetpackUser.GetValueOrDefault())
		{
			SetEnabled(Entity<JetpackComponent>.op_Implicit(ent), ent.Comp, enabled: false, ent.Comp.JetpackUser);
		}
	}

	private void OnJetpackUserCanWeightless(EntityUid uid, JetpackUserComponent component, ref CanWeightlessMoveEvent args)
	{
		args.CanMove = true;
	}

	private void OnJetpackUserEntParentChanged(EntityUid uid, JetpackUserComponent component, ref EntParentChangedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		JetpackComponent jetpack = default(JetpackComponent);
		if (((EntitySystem)this).TryComp<JetpackComponent>(component.Jetpack, ref jetpack) && !CanEnableOnGrid(((EntParentChangedMessage)(ref args)).Transform.GridUid))
		{
			SetEnabled(component.Jetpack, jetpack, enabled: false, uid);
			_popup.PopupClient(base.Loc.GetString("jetpack-to-grid"), uid, uid);
		}
	}

	private void SetupUser(EntityUid user, EntityUid jetpackUid, JetpackComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		JetpackUserComponent userComp = default(JetpackUserComponent);
		((EntitySystem)this).EnsureComp<JetpackUserComponent>(user, ref userComp);
		component.JetpackUser = user;
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(user, ref physics))
		{
			_physics.SetBodyStatus(user, physics, (BodyStatus)1, true);
		}
		userComp.Jetpack = jetpackUid;
		userComp.WeightlessAcceleration = component.Acceleration;
		userComp.WeightlessModifier = component.WeightlessModifier;
		userComp.WeightlessFriction = component.Friction;
		userComp.WeightlessFrictionNoInput = component.Friction;
		_movementSpeedModifier.RefreshWeightlessModifiers(user);
	}

	private void RemoveUser(EntityUid uid, JetpackComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).RemComp<JetpackUserComponent>(uid))
		{
			component.JetpackUser = null;
			PhysicsComponent physics = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref physics))
			{
				_physics.SetBodyStatus(uid, physics, (BodyStatus)0, true);
			}
			_movementSpeedModifier.RefreshWeightlessModifiers(uid);
		}
	}

	private void OnJetpackToggle(EntityUid uid, JetpackComponent component, ToggleJetpackEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			TransformComponent xform = default(TransformComponent);
			if (((EntitySystem)this).TryComp(uid, ref xform) && !CanEnableOnGrid(xform.GridUid))
			{
				_popup.PopupClient(base.Loc.GetString("jetpack-no-station"), uid, args.Performer);
			}
			else
			{
				SetEnabled(uid, component, !IsEnabled(uid));
			}
		}
	}

	private bool CanEnableOnGrid(EntityUid? gridUid)
	{
		if (gridUid.HasValue)
		{
			return !((EntitySystem)this).HasComp<GravityComponent>(gridUid);
		}
		return true;
	}

	private void OnJetpackGetAction(EntityUid uid, JetpackComponent component, GetItemActionsEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
	}

	private bool IsEnabled(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).HasComp<ActiveJetpackComponent>(uid);
	}

	public void SetEnabled(EntityUid uid, JetpackComponent component, bool enabled, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnabled(uid) == enabled || (enabled && !CanEnable(uid, component)))
		{
			return;
		}
		if (!user.HasValue)
		{
			BaseContainer container = default(BaseContainer);
			if (!Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container))
			{
				return;
			}
			user = container.Owner;
		}
		if (enabled)
		{
			SetupUser(user.Value, uid, component);
			((EntitySystem)this).EnsureComp<ActiveJetpackComponent>(uid);
		}
		else
		{
			RemoveUser(user.Value, component);
			((EntitySystem)this).RemComp<ActiveJetpackComponent>(uid);
		}
		Appearance.SetData(uid, (Enum)JetpackVisuals.Enabled, (object)enabled, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	public bool IsUserFlying(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).HasComp<JetpackUserComponent>(uid);
	}

	protected virtual bool CanEnable(EntityUid uid, JetpackComponent component)
	{
		return true;
	}
}
