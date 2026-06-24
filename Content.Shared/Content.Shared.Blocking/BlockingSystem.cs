using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Blocking;

public sealed class BlockingSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private FixtureSystem _fixtureSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeUser();
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, GotEquippedHandEvent>((ComponentEventHandler<BlockingComponent, GotEquippedHandEvent>)OnEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, GotUnequippedHandEvent>((ComponentEventHandler<BlockingComponent, GotUnequippedHandEvent>)OnUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, DroppedEvent>((ComponentEventHandler<BlockingComponent, DroppedEvent>)OnDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, GetItemActionsEvent>((ComponentEventHandler<BlockingComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, ToggleActionEvent>((ComponentEventHandler<BlockingComponent, ToggleActionEvent>)OnToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, ComponentShutdown>((ComponentEventHandler<BlockingComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<BlockingComponent, GetVerbsEvent<ExamineVerb>>)OnVerbExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, MapInitEvent>((ComponentEventHandler<BlockingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, BlockingComponent component, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_actionContainer.EnsureAction(uid, ref component.BlockingToggleActionEntity, EntProtoId.op_Implicit(component.BlockingToggleAction));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnEquip(EntityUid uid, BlockingComponent component, GotEquippedHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		component.User = args.User;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(args.User, ref physicsComponent) && (int)physicsComponent.BodyType != 4 && !((EntitySystem)this).HasComp<BlockingUserComponent>(args.User))
		{
			BlockingUserComponent blockingUserComponent = ((EntitySystem)this).EnsureComp<BlockingUserComponent>(args.User);
			blockingUserComponent.BlockingItem = uid;
			blockingUserComponent.OriginalBodyType = physicsComponent.BodyType;
		}
	}

	private void OnUnequip(EntityUid uid, BlockingComponent component, GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		StopBlockingHelper(uid, component, args.User);
	}

	private void OnDrop(EntityUid uid, BlockingComponent component, DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		StopBlockingHelper(uid, component, args.User);
	}

	private void OnGetActions(EntityUid uid, BlockingComponent component, GetItemActionsEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref component.BlockingToggleActionEntity, EntProtoId.op_Implicit(component.BlockingToggleAction));
	}

	private void OnToggleAction(EntityUid uid, BlockingComponent component, ToggleActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityQuery<BlockingComponent> blockQuery = ((EntitySystem)this).GetEntityQuery<BlockingComponent>();
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).GetEntityQuery<HandsComponent>().TryGetComponent(args.Performer, ref hands))
		{
			return;
		}
		EntityUid[] array = _handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit((args.Performer, hands))).ToArray();
		BlockingComponent otherBlockComp = default(BlockingComponent);
		foreach (EntityUid shield in array)
		{
			if (!(shield == uid) && blockQuery.TryGetComponent(shield, ref otherBlockComp) && otherBlockComp.IsBlocking)
			{
				CantBlockError(args.Performer);
				return;
			}
		}
		if (component.IsBlocking)
		{
			StopBlocking(uid, component, args.Performer);
		}
		else
		{
			StartBlocking(uid, component, args.Performer);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnShutdown(EntityUid uid, BlockingComponent component, ComponentShutdown args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (component.User.HasValue)
		{
			_actionsSystem.RemoveProvidedActions(component.User.Value, uid);
			StopBlockingHelper(uid, component, component.User.Value);
		}
	}

	public bool StartBlocking(EntityUid item, BlockingComponent component, EntityUid user)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsBlocking)
		{
			return false;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(user);
		string shieldName = ((EntitySystem)this).Name(item, (MetaDataComponent)null);
		EntityUid blockerName = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
		string msgUser = base.Loc.GetString("action-popup-blocking-user", (ValueTuple<string, object>)("shield", shieldName));
		string msgOther = base.Loc.GetString("action-popup-blocking-other", (ValueTuple<string, object>)("blockerName", blockerName), (ValueTuple<string, object>)("shield", shieldName));
		EntityUid? gridUid = xform.GridUid;
		EntityUid parentUid = xform.ParentUid;
		if (!gridUid.HasValue || gridUid.GetValueOrDefault() != parentUid)
		{
			CantBlockError(user);
			return false;
		}
		if (!_handsSystem.IsHolding(Entity<HandsComponent>.op_Implicit(user), item, out string _))
		{
			CantBlockError(user);
			return false;
		}
		TileRef? playerTileRef = _turf.GetTileRef(xform.Coordinates);
		if (playerTileRef.HasValue)
		{
			IEnumerable<EntityUid> localEntitiesIntersecting = _lookup.GetLocalEntitiesIntersecting(playerTileRef.Value, 0f, (LookupFlags)110);
			EntityQuery<MobStateComponent> mobQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
			foreach (EntityUid uid in localEntitiesIntersecting)
			{
				if (uid != user && mobQuery.HasComponent(uid))
				{
					TooCloseError(user);
					return false;
				}
			}
		}
		_transformSystem.AnchorEntity(user, xform);
		if (!xform.Anchored)
		{
			CantBlockError(user);
			return false;
		}
		SharedActionsSystem actionsSystem = _actionsSystem;
		gridUid = component.BlockingToggleActionEntity;
		actionsSystem.SetToggled(gridUid.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(gridUid.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: true);
		_popupSystem.PopupPredicted(msgUser, msgOther, user, user);
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(user, ref physicsComponent))
		{
			_fixtureSystem.TryCreateFixture(user, component.Shape, "blocking-active", 1f, true, 223, 0, 0.4f, 0f, true, (FixturesComponent)null, physicsComponent, (TransformComponent)null);
		}
		component.IsBlocking = true;
		((EntitySystem)this).Dirty(item, (IComponent)(object)component, (MetaDataComponent)null);
		return true;
	}

	private void CantBlockError(EntityUid user)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		string msgError = base.Loc.GetString("action-popup-blocking-user-cant-block");
		_popupSystem.PopupClient(msgError, user, user);
	}

	private void TooCloseError(EntityUid user)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		string msgError = base.Loc.GetString("action-popup-blocking-user-too-close");
		_popupSystem.PopupClient(msgError, user, user);
	}

	public bool StopBlocking(EntityUid item, BlockingComponent component, EntityUid user)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (!component.IsBlocking)
		{
			return false;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(user);
		string shieldName = ((EntitySystem)this).Name(item, (MetaDataComponent)null);
		EntityUid blockerName = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
		string msgUser = base.Loc.GetString("action-popup-blocking-disabling-user", (ValueTuple<string, object>)("shield", shieldName));
		string msgOther = base.Loc.GetString("action-popup-blocking-disabling-other", (ValueTuple<string, object>)("blockerName", blockerName), (ValueTuple<string, object>)("shield", shieldName));
		BlockingUserComponent blockingUserComponent = default(BlockingUserComponent);
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<BlockingUserComponent>(user, ref blockingUserComponent) && ((EntitySystem)this).TryComp<PhysicsComponent>(user, ref physicsComponent))
		{
			if (xform.Anchored)
			{
				_transformSystem.Unanchor(user, xform, true);
			}
			SharedActionsSystem actionsSystem = _actionsSystem;
			EntityUid? blockingToggleActionEntity = component.BlockingToggleActionEntity;
			actionsSystem.SetToggled(blockingToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(blockingToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
			_fixtureSystem.DestroyFixture(user, "blocking-active", true, physicsComponent, (FixturesComponent)null, (TransformComponent)null);
			_physics.SetBodyType(user, blockingUserComponent.OriginalBodyType, (FixturesComponent)null, physicsComponent, (TransformComponent)null);
			_popupSystem.PopupPredicted(msgUser, msgOther, user, user);
		}
		component.IsBlocking = false;
		((EntitySystem)this).Dirty(item, (IComponent)(object)component, (MetaDataComponent)null);
		return true;
	}

	private void StopBlockingHelper(EntityUid uid, BlockingComponent component, EntityUid user)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsBlocking)
		{
			StopBlocking(uid, component, user);
		}
		EntityQuery<BlockingUserComponent> userQuery = ((EntitySystem)this).GetEntityQuery<BlockingUserComponent>();
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).GetEntityQuery<HandsComponent>().TryGetComponent(user, ref hands))
		{
			return;
		}
		EntityUid[] array = _handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit((user, hands))).ToArray();
		BlockingUserComponent blockingUserComponent = default(BlockingUserComponent);
		foreach (EntityUid shield in array)
		{
			if (((EntitySystem)this).HasComp<BlockingComponent>(shield) && userQuery.TryGetComponent(user, ref blockingUserComponent))
			{
				blockingUserComponent.BlockingItem = shield;
				return;
			}
		}
		((EntitySystem)this).RemComp<BlockingUserComponent>(user);
		component.User = null;
	}

	private void OnVerbExamine(EntityUid uid, BlockingComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		if (args.CanInteract && args.CanAccess)
		{
			float fraction = (component.IsBlocking ? component.ActiveBlockFraction : component.PassiveBlockFraction);
			DamageModifierSet modifier = (component.IsBlocking ? component.ActiveBlockDamageModifier : component.PassiveBlockDamageModifer);
			FormattedMessage msg = new FormattedMessage();
			msg.AddMarkupOrThrow(base.Loc.GetString("blocking-fraction", (ValueTuple<string, object>)("value", MathF.Round(fraction * 100f, 1))));
			AppendCoefficients(modifier, msg);
			_examine.AddDetailedExamineVerb(args, (Component)(object)component, msg, base.Loc.GetString("blocking-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png", base.Loc.GetString("blocking-examinable-verb-message"));
		}
	}

	private void AppendCoefficients(DamageModifierSet modifiers, FormattedMessage msg)
	{
		foreach (KeyValuePair<string, float> coefficient in modifiers.Coefficients)
		{
			msg.PushNewline();
			msg.AddMarkupOrThrow(Loc.GetString("blocking-coefficient-value", new(string, object)[2]
			{
				("type", coefficient.Key),
				("value", MathF.Round(coefficient.Value * 100f, 1))
			}));
		}
		foreach (KeyValuePair<string, float> flat in modifiers.FlatReduction)
		{
			msg.PushNewline();
			msg.AddMarkupOrThrow(Loc.GetString("blocking-reduction-value", new(string, object)[2]
			{
				("type", flat.Key),
				("value", flat.Value)
			}));
		}
	}

	private void InitializeUser()
	{
		((EntitySystem)this).SubscribeLocalEvent<BlockingUserComponent, DamageModifyEvent>((ComponentEventHandler<BlockingUserComponent, DamageModifyEvent>)OnUserDamageModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingComponent, DamageModifyEvent>((ComponentEventHandler<BlockingComponent, DamageModifyEvent>)OnDamageModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingUserComponent, EntParentChangedMessage>((ComponentEventRefHandler<BlockingUserComponent, EntParentChangedMessage>)OnParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingUserComponent, ContainerGettingInsertedAttemptEvent>((ComponentEventHandler<BlockingUserComponent, ContainerGettingInsertedAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingUserComponent, AnchorStateChangedEvent>((ComponentEventRefHandler<BlockingUserComponent, AnchorStateChangedEvent>)OnAnchorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockingUserComponent, EntityTerminatingEvent>((ComponentEventRefHandler<BlockingUserComponent, EntityTerminatingEvent>)OnEntityTerminating, (Type[])null, (Type[])null);
	}

	private void OnParentChanged(EntityUid uid, BlockingUserComponent component, ref EntParentChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopBlocking(uid, component);
	}

	private void OnInsertAttempt(EntityUid uid, BlockingUserComponent component, ContainerGettingInsertedAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserStopBlocking(uid, component);
	}

	private void OnAnchorChanged(EntityUid uid, BlockingUserComponent component, ref AnchorStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((AnchorStateChangedEvent)(ref args)).Anchored)
		{
			UserStopBlocking(uid, component);
		}
	}

	private void OnUserDamageModified(EntityUid uid, BlockingUserComponent component, DamageModifyEvent args)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		BlockingComponent blocking = default(BlockingComponent);
		DamageableComponent dmgComp = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<BlockingComponent>(component.BlockingItem, ref blocking) || args.Damage.GetTotal() <= 0 || !((EntitySystem)this).TryComp<DamageableComponent>(component.BlockingItem, ref dmgComp))
		{
			return;
		}
		float blockFraction = (blocking.IsBlocking ? blocking.ActiveBlockFraction : blocking.PassiveBlockFraction);
		blockFraction = Math.Clamp(blockFraction, 0f, 1f);
		_damageable.TryChangeDamage(component.BlockingItem, blockFraction * args.OriginalDamage);
		DamageModifierSet modify = new DamageModifierSet();
		foreach (string key in dmgComp.Damage.DamageDict.Keys)
		{
			modify.Coefficients.TryAdd(key, 1f - blockFraction);
		}
		args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, modify);
		if (blocking.IsBlocking && !args.Damage.Equals(args.OriginalDamage))
		{
			_audio.PlayPvs(blocking.BlockSound, uid, (AudioParams?)null);
		}
	}

	private void OnDamageModified(EntityUid uid, BlockingComponent component, DamageModifyEvent args)
	{
		DamageModifierSet modifier = (component.IsBlocking ? component.ActiveBlockDamageModifier : component.PassiveBlockDamageModifer);
		if (modifier != null)
		{
			args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, modifier);
		}
	}

	private void OnEntityTerminating(EntityUid uid, BlockingUserComponent component, ref EntityTerminatingEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		BlockingComponent blockingComponent = default(BlockingComponent);
		if (((EntitySystem)this).TryComp<BlockingComponent>(component.BlockingItem, ref blockingComponent))
		{
			StopBlockingHelper(component.BlockingItem.Value, blockingComponent, uid);
		}
	}

	private void UserStopBlocking(EntityUid uid, BlockingUserComponent component)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		BlockingComponent blockComp = default(BlockingComponent);
		if (((EntitySystem)this).TryComp<BlockingComponent>(component.BlockingItem, ref blockComp) && blockComp.IsBlocking)
		{
			StopBlocking(component.BlockingItem.Value, blockComp, uid);
		}
	}
}
