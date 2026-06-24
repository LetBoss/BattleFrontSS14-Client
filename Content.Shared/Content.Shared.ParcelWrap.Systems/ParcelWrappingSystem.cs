using System;
using System.Collections.Generic;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Materials;
using Content.Shared.ParcelWrap.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.ParcelWrap.Systems;

public sealed class ParcelWrappingSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedChargesSystem _charges;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeParcelWrap();
		InitializeWrappedParcel();
	}

	public bool IsWrappable(Entity<ParcelWrapComponent> wrapper, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (wrapper.Owner != target && !_charges.IsEmpty(Entity<LimitedChargesComponent>.op_Implicit(wrapper.Owner)) && _whitelist.IsWhitelistPass(wrapper.Comp.Whitelist, target))
		{
			return _whitelist.IsBlacklistFail(wrapper.Comp.Blacklist, target);
		}
		return false;
	}

	private void InitializeParcelWrap()
	{
		((EntitySystem)this).SubscribeLocalEvent<ParcelWrapComponent, AfterInteractEvent>((EntityEventRefHandler<ParcelWrapComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParcelWrapComponent, GetVerbsEvent<UtilityVerb>>((EntityEventRefHandler<ParcelWrapComponent, GetVerbsEvent<UtilityVerb>>)OnGetVerbsForParcelWrap, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParcelWrapComponent, ParcelWrapItemDoAfterEvent>((EntityEventRefHandler<ParcelWrapComponent, ParcelWrapItemDoAfterEvent>)OnWrapItemDoAfter, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(Entity<ParcelWrapComponent> entity, ref AfterInteractEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (args.CanReach && IsWrappable(entity, target2))
			{
				((HandledEntityEventArgs)args).Handled = TryStartWrapDoAfter(args.User, entity, target2);
			}
		}
	}

	private void OnGetVerbsForParcelWrap(Entity<ParcelWrapComponent> entity, ref GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && IsWrappable(entity, args.Target))
		{
			EntityUid user = args.User;
			EntityUid target = args.Target;
			args.Verbs.Add(new UtilityVerb
			{
				Text = base.Loc.GetString("parcel-wrap-verb-wrap"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					TryStartWrapDoAfter(user, entity, target);
				}
			});
		}
	}

	private void OnWrapItemDoAfter(Entity<ParcelWrapComponent> wrapper, ref ParcelWrapItemDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				WrapInternal(args.User, wrapper, target2);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private bool TryStartWrapDoAfter(EntityUid user, Entity<ParcelWrapComponent> wrapper, EntityUid target)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return _doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, wrapper.Comp.WrapDelay, new ParcelWrapItemDoAfterEvent(), Entity<ParcelWrapComponent>.op_Implicit(wrapper), target, Entity<ParcelWrapComponent>.op_Implicit(wrapper))
		{
			NeedHand = true,
			BreakOnMove = true,
			BreakOnDamage = true
		});
	}

	private void WrapInternal(EntityUid user, Entity<ParcelWrapComponent> wrapper, EntityUid target)
	{
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			EntityUid spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(wrapper.Comp.ParcelPrototype), ((EntitySystem)this).Transform(target).Coordinates);
			ItemComponent targetItemComp = default(ItemComponent);
			((EntitySystem)this).TryComp<ItemComponent>(target, ref targetItemComp);
			ProtoId<ItemSizePrototype> size = wrapper.Comp.FallbackItemSize;
			if (wrapper.Comp.WrappedItemsMaintainSize && targetItemComp != null)
			{
				size = targetItemComp.Size;
			}
			ItemComponent item = ((EntitySystem)this).Comp<ItemComponent>(spawned);
			_item.SetSize(spawned, size, item);
			_appearance.SetData(spawned, (Enum)WrappedParcelVisuals.Size, (object)size.Id, (AppearanceComponent)null);
			if (wrapper.Comp.WrappedItemsMaintainShape && targetItemComp != null)
			{
				List<Box2i> shape = targetItemComp.Shape;
				if (shape != null)
				{
					_item.SetShape(spawned, shape, item);
				}
			}
			BaseContainer containerOfTarget = default(BaseContainer);
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(target, null, null)), ref containerOfTarget))
			{
				_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(target), containerOfTarget, true, false, (EntityCoordinates?)null, (Angle?)null);
				_container.InsertOrDrop(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(spawned, null, null)), containerOfTarget, (TransformComponent)null);
			}
			WrappedParcelComponent parcel = ((EntitySystem)this).EnsureComp<WrappedParcelComponent>(spawned);
			if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(target), (BaseContainer)(object)parcel.Contents, (TransformComponent)null, false))
			{
				((EntitySystem)this).QueueDel((EntityUid?)spawned);
			}
		}
		_charges.TryUseCharges(Entity<LimitedChargesComponent>.op_Implicit(wrapper.Owner), 1);
		if (_net.IsServer && _charges.IsEmpty(Entity<LimitedChargesComponent>.op_Implicit(wrapper.Owner)))
		{
			((EntitySystem)this).QueueDel((EntityUid?)Entity<ParcelWrapComponent>.op_Implicit(wrapper));
		}
		_audio.PlayPredicted(wrapper.Comp.WrapSound, target, (EntityUid?)user, (AudioParams?)null);
	}

	private void InitializeWrappedParcel()
	{
		((EntitySystem)this).SubscribeLocalEvent<WrappedParcelComponent, ComponentInit>((EntityEventRefHandler<WrappedParcelComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WrappedParcelComponent, UseInHandEvent>((EntityEventRefHandler<WrappedParcelComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WrappedParcelComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<WrappedParcelComponent, GetVerbsEvent<InteractionVerb>>)OnGetVerbsForWrappedParcel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WrappedParcelComponent, UnwrapWrappedParcelDoAfterEvent>((EntityEventRefHandler<WrappedParcelComponent, UnwrapWrappedParcelDoAfterEvent>)OnUnwrapParcelDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WrappedParcelComponent, DestructionEventArgs>((EntityEventRefHandler<WrappedParcelComponent, DestructionEventArgs>)OnDestroyed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WrappedParcelComponent, GotReclaimedEvent>((EntityEventRefHandler<WrappedParcelComponent, GotReclaimedEvent>)OnDestroyed, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(Entity<WrappedParcelComponent> entity, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Contents = _container.EnsureContainer<ContainerSlot>(Entity<WrappedParcelComponent>.op_Implicit(entity), entity.Comp.ContainerId, (ContainerManagerComponent)null);
	}

	private void OnUseInHand(Entity<WrappedParcelComponent> entity, ref UseInHandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryStartUnwrapDoAfter(args.User, entity);
		}
	}

	private void OnGetVerbsForWrappedParcel(Entity<WrappedParcelComponent> entity, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new InteractionVerb
			{
				Text = base.Loc.GetString("parcel-wrap-verb-unwrap"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					TryStartUnwrapDoAfter(user, entity);
				}
			});
		}
	}

	private void OnUnwrapParcelDoAfter(Entity<WrappedParcelComponent> entity, ref UnwrapWrappedParcelDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			WrappedParcelComponent parcel = default(WrappedParcelComponent);
			if (((EntitySystem)this).TryComp<WrappedParcelComponent>(target2, ref parcel))
			{
				UnwrapInternal(args.User, Entity<WrappedParcelComponent>.op_Implicit((target2, parcel)));
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnDestroyed<T>(Entity<WrappedParcelComponent> parcel, ref T args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = UnwrapInternal(null, parcel);
		if (val.HasValue)
		{
			EntityUid contents = val.GetValueOrDefault();
			_popup.PopupPredicted(base.Loc.GetString("parcel-wrap-popup-parcel-destroyed", (ValueTuple<string, object>)("contents", contents)), contents, null, PopupType.MediumCaution);
		}
	}

	private bool TryStartUnwrapDoAfter(EntityUid user, Entity<WrappedParcelComponent> parcel)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		return _doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, parcel.Comp.UnwrapDelay, new UnwrapWrappedParcelDoAfterEvent(), Entity<WrappedParcelComponent>.op_Implicit(parcel), Entity<WrappedParcelComponent>.op_Implicit(parcel))
		{
			NeedHand = true
		});
	}

	private EntityUid? UnwrapInternal(EntityUid? user, Entity<WrappedParcelComponent> parcel)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? containedEntity = parcel.Comp.Contents.ContainedEntity;
		_audio.PlayPredicted(parcel.Comp.UnwrapSound, Entity<WrappedParcelComponent>.op_Implicit(parcel), user, (AudioParams?)null);
		if (!_net.IsServer)
		{
			return containedEntity;
		}
		TransformComponent parcelTransform = ((EntitySystem)this).Transform(Entity<WrappedParcelComponent>.op_Implicit(parcel));
		if (containedEntity.HasValue)
		{
			EntityUid parcelContents = containedEntity.GetValueOrDefault();
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(parcelContents), (BaseContainer)(object)parcel.Comp.Contents, true, true, (EntityCoordinates?)parcelTransform.Coordinates, (Angle?)null);
			BaseContainer outerContainer = default(BaseContainer);
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<WrappedParcelComponent>.op_Implicit(parcel), null, null)), ref outerContainer))
			{
				_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<WrappedParcelComponent>.op_Implicit(parcel), null, null)), outerContainer, true, true, (EntityCoordinates?)null, (Angle?)null);
				_container.InsertOrDrop(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(parcelContents, null, null)), outerContainer, (TransformComponent)null);
			}
		}
		EntProtoId? unwrapTrash = parcel.Comp.UnwrapTrash;
		if (unwrapTrash.HasValue)
		{
			EntProtoId trashProto = unwrapTrash.GetValueOrDefault();
			EntityUid trash = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(trashProto), parcelTransform.Coordinates);
			_transform.DropNextTo(Entity<TransformComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(trash, null)), Entity<TransformComponent>.op_Implicit((Entity<WrappedParcelComponent>.op_Implicit(parcel), parcelTransform)));
		}
		((EntitySystem)this).QueueDel((EntityUid?)Entity<WrappedParcelComponent>.op_Implicit(parcel));
		return containedEntity;
	}
}
