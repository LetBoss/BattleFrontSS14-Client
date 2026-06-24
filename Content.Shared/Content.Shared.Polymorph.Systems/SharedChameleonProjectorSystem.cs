using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Polymorph.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Polymorph.Systems;

public abstract class SharedChameleonProjectorSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private ISerializationManager _serMan;

	[Dependency]
	private MetaDataSystem _meta;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _xform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguiseComponent, InteractHandEvent>((EntityEventRefHandler<ChameleonDisguiseComponent, InteractHandEvent>)OnDisguiseInteractHand, new Type[1] { typeof(SharedItemSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguiseComponent, DamageChangedEvent>((EntityEventRefHandler<ChameleonDisguiseComponent, DamageChangedEvent>)OnDisguiseDamaged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguiseComponent, InsertIntoEntityStorageAttemptEvent>((EntityEventRefHandler<ChameleonDisguiseComponent, InsertIntoEntityStorageAttemptEvent>)OnDisguiseInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguiseComponent, ComponentShutdown>((EntityEventRefHandler<ChameleonDisguiseComponent, ComponentShutdown>)OnDisguiseShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguisedComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<ChameleonDisguisedComponent, EntGotInsertedIntoContainerMessage>)OnDisguisedInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, AfterInteractEvent>((EntityEventRefHandler<ChameleonProjectorComponent, AfterInteractEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, GetVerbsEvent<UtilityVerb>>((EntityEventRefHandler<ChameleonProjectorComponent, GetVerbsEvent<UtilityVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, DisguiseToggleNoRotEvent>((EntityEventRefHandler<ChameleonProjectorComponent, DisguiseToggleNoRotEvent>)OnToggleNoRot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, DisguiseToggleAnchoredEvent>((EntityEventRefHandler<ChameleonProjectorComponent, DisguiseToggleAnchoredEvent>)OnToggleAnchored, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, HandDeselectedEvent>((EntityEventRefHandler<ChameleonProjectorComponent, HandDeselectedEvent>)OnDeselected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, GotUnequippedHandEvent>((EntityEventRefHandler<ChameleonProjectorComponent, GotUnequippedHandEvent>)OnUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonProjectorComponent, ComponentShutdown>((EntityEventRefHandler<ChameleonProjectorComponent, ComponentShutdown>)OnProjectorShutdown, (Type[])null, (Type[])null);
	}

	private void OnDisguiseInteractHand(Entity<ChameleonDisguiseComponent> ent, ref InteractHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TryReveal(Entity<ChameleonDisguisedComponent>.op_Implicit(ent.Comp.User));
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnDisguiseDamaged(Entity<ChameleonDisguiseComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = args.DamageDelta;
		if (damage != null)
		{
			_damageable.TryChangeDamage(ent.Comp.User, damage);
		}
	}

	private void OnDisguiseInsertAttempt(Entity<ChameleonDisguiseComponent> ent, ref InsertIntoEntityStorageAttemptEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		args.Cancelled = true;
		TryReveal(Entity<ChameleonDisguisedComponent>.op_Implicit(ent.Comp.User));
	}

	private void OnDisguiseShutdown(Entity<ChameleonDisguiseComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_actions.RemoveProvidedActions(ent.Comp.User, ent.Comp.Projector);
	}

	private void OnDisguisedInserted(Entity<ChameleonDisguisedComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TryReveal(Entity<ChameleonDisguisedComponent>.op_Implicit((Entity<ChameleonDisguisedComponent>.op_Implicit(ent), Entity<ChameleonDisguisedComponent>.op_Implicit(ent))));
	}

	private void OnInteract(Entity<ChameleonProjectorComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				((HandledEntityEventArgs)args).Handled = true;
				TryDisguise(ent, args.User, target2);
			}
		}
	}

	private void OnGetVerbs(Entity<ChameleonProjectorComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess)
		{
			EntityUid user = args.User;
			EntityUid target = args.Target;
			args.Verbs.Add(new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					TryDisguise(ent, user, target);
				},
				Text = base.Loc.GetString("chameleon-projector-set-disguise")
			});
		}
	}

	public bool TryDisguise(Entity<ChameleonProjectorComponent> ent, EntityUid user, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (_container.IsEntityInContainer(target, (MetaDataComponent)null) || _container.IsEntityInContainer(user, (MetaDataComponent)null))
		{
			_popup.PopupClient(base.Loc.GetString("chameleon-projector-inside-container"), target, user);
			return false;
		}
		if (IsInvalid(ent.Comp, target))
		{
			_popup.PopupClient(base.Loc.GetString("chameleon-projector-invalid"), target, user);
			return false;
		}
		_popup.PopupClient(base.Loc.GetString("chameleon-projector-success"), target, user);
		Disguise(ent, user, target);
		return true;
	}

	private void OnToggleNoRot(Entity<ChameleonProjectorComponent> ent, ref DisguiseToggleNoRotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? disguised = ent.Comp.Disguised;
		if (disguised.HasValue)
		{
			EntityUid uid = disguised.GetValueOrDefault();
			TransformComponent xform = ((EntitySystem)this).Transform(uid);
			_xform.SetLocalRotationNoLerp(uid, Angle.op_Implicit(0f), xform);
			xform.NoLocalRotation = !xform.NoLocalRotation;
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnToggleAnchored(Entity<ChameleonProjectorComponent> ent, ref DisguiseToggleAnchoredEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? disguised = ent.Comp.Disguised;
		if (disguised.HasValue)
		{
			EntityUid uid = disguised.GetValueOrDefault();
			TransformComponent xform = ((EntitySystem)this).Transform(uid);
			if (xform.Anchored)
			{
				_xform.Unanchor(uid, xform, true);
			}
			else
			{
				_xform.AnchorEntity(Entity<TransformComponent>.op_Implicit((uid, xform)), (Entity<MapGridComponent>?)null);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnDeselected(Entity<ChameleonProjectorComponent> ent, ref HandDeselectedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RevealProjector(ent);
	}

	private void OnUnequipped(Entity<ChameleonProjectorComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RevealProjector(ent);
	}

	private void OnProjectorShutdown(Entity<ChameleonProjectorComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RevealProjector(ent);
	}

	public bool IsInvalid(ChameleonProjectorComponent comp, EntityUid target)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!_whitelist.IsWhitelistFail(comp.Whitelist, target))
		{
			return _whitelist.IsBlacklistPass(comp.Blacklist, target);
		}
		return true;
	}

	public void Disguise(Entity<ChameleonProjectorComponent> ent, EntityUid user, EntityUid entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		ChameleonProjectorComponent proj = ent.Comp;
		if (!_net.IsClient)
		{
			TryReveal(Entity<ChameleonDisguisedComponent>.op_Implicit(user));
			_actions.AddAction(user, ref proj.NoRotActionEntity, EntProtoId.op_Implicit(proj.NoRotAction), Entity<ChameleonProjectorComponent>.op_Implicit(ent));
			_actions.AddAction(user, ref proj.AnchorActionEntity, EntProtoId.op_Implicit(proj.AnchorAction), Entity<ChameleonProjectorComponent>.op_Implicit(ent));
			proj.Disguised = user;
			EntityUid disguise = ((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(proj.DisguiseProto), user.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			ChameleonDisguisedComponent disguised = ((EntitySystem)this).AddComp<ChameleonDisguisedComponent>(user);
			disguised.Disguise = disguise;
			((EntitySystem)this).Dirty(user, (IComponent)(object)disguised, (MetaDataComponent)null);
			MetaDataComponent meta = ((EntitySystem)this).MetaData(entity);
			_meta.SetEntityName(disguise, meta.EntityName, (MetaDataComponent)null, true);
			_meta.SetEntityDescription(disguise, meta.EntityDescription, (MetaDataComponent)null);
			ChameleonDisguiseComponent comp = ((EntitySystem)this).EnsureComp<ChameleonDisguiseComponent>(disguise);
			comp.User = user;
			comp.Projector = Entity<ChameleonProjectorComponent>.op_Implicit(ent);
			comp.SourceEntity = entity;
			EntityPrototype obj = ((EntitySystem)this).Prototype(entity, (MetaDataComponent)null);
			comp.SourceProto = EntProtoId.op_Implicit((obj != null) ? obj.ID : null);
			((EntitySystem)this).Dirty(disguise, (IComponent)(object)comp, (MetaDataComponent)null);
			CopyComp<ItemComponent>(Entity<ChameleonDisguiseComponent>.op_Implicit((disguise, comp)));
			_appearance.CopyData(Entity<AppearanceComponent>.op_Implicit(entity), Entity<AppearanceComponent>.op_Implicit(disguise));
		}
	}

	public bool TryReveal(Entity<ChameleonDisguisedComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ChameleonDisguisedComponent>(Entity<ChameleonDisguisedComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		ChameleonDisguiseComponent disguise = default(ChameleonDisguiseComponent);
		ChameleonProjectorComponent proj = default(ChameleonProjectorComponent);
		if (((EntitySystem)this).TryComp<ChameleonDisguiseComponent>(ent.Comp.Disguise, ref disguise) && ((EntitySystem)this).TryComp<ChameleonProjectorComponent>(disguise.Projector, ref proj))
		{
			proj.Disguised = null;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<ChameleonDisguisedComponent>.op_Implicit(ent));
		xform.NoLocalRotation = false;
		_xform.Unanchor(Entity<ChameleonDisguisedComponent>.op_Implicit(ent), xform, true);
		((EntitySystem)this).Del((EntityUid?)ent.Comp.Disguise);
		((EntitySystem)this).RemComp<ChameleonDisguisedComponent>(Entity<ChameleonDisguisedComponent>.op_Implicit(ent));
		return true;
	}

	public void RevealProjector(Entity<ChameleonProjectorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? disguised = ent.Comp.Disguised;
		if (disguised.HasValue)
		{
			EntityUid user = disguised.GetValueOrDefault();
			TryReveal(Entity<ChameleonDisguisedComponent>.op_Implicit(user));
		}
	}

	protected bool CopyComp<T>(Entity<ChameleonDisguiseComponent> ent) where T : Component, new()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!GetSrcComp<T>(ent.Comp, out T src))
		{
			return true;
		}
		((EntitySystem)this).RemComp<T>(Entity<ChameleonDisguiseComponent>.op_Implicit(ent));
		T dest = ((EntitySystem)this).AddComp<T>(Entity<ChameleonDisguiseComponent>.op_Implicit(ent));
		_serMan.CopyTo<T>(src, ref dest, (ISerializationContext)null, false, true);
		((EntitySystem)this).Dirty(Entity<ChameleonDisguiseComponent>.op_Implicit(ent), (IComponent)(object)dest, (MetaDataComponent)null);
		return false;
	}

	private bool GetSrcComp<T>(ChameleonDisguiseComponent comp, [NotNullWhen(true)] out T? src) where T : Component, new()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).TryComp<T>(comp.SourceEntity, ref src))
		{
			return true;
		}
		EntProtoId? sourceProto = comp.SourceProto;
		if (sourceProto.HasValue)
		{
			EntProtoId protoId = sourceProto.GetValueOrDefault();
			EntityPrototype proto = default(EntityPrototype);
			if (!_proto.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(protoId), ref proto))
			{
				return false;
			}
			return proto.TryGetComponent<T>(ref src, base.EntityManager.ComponentFactory);
		}
		return false;
	}
}
