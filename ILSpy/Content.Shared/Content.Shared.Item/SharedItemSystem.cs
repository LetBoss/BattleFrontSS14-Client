using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Item;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Item;

public abstract class SharedItemSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	protected SharedContainerSystem Container;

	private EntityQuery<FixedItemSizeStorageComponent> _fixedItemSizeStorageQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_fixedItemSizeStorageQuery = ((EntitySystem)this).GetEntityQuery<FixedItemSizeStorageComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<ItemComponent, GetVerbsEvent<InteractionVerb>>)AddPickupVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, InteractHandEvent>((ComponentEventHandler<ItemComponent, InteractHandEvent>)OnHandInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<ItemComponent, AfterAutoHandleStateEvent>)OnItemAutoState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, ExaminedEvent>((ComponentEventHandler<ItemComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleSizeComponent, ItemToggledEvent>((ComponentEventHandler<ItemToggleSizeComponent, ItemToggledEvent>)OnItemToggle, (Type[])null, (Type[])null);
	}

	private void OnItemAutoState(EntityUid uid, ItemComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetHeldPrefix(uid, component.HeldPrefix, force: true, component);
	}

	public void SetSize(EntityUid uid, ProtoId<ItemSizePrototype> size, ItemComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemComponent>(uid, ref component, false) && !(component.Size == size))
		{
			component.Size = size;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			ItemSizeChangedEvent ev = new ItemSizeChangedEvent(uid);
			((EntitySystem)this).RaiseLocalEvent<ItemSizeChangedEvent>(uid, ref ev, true);
		}
	}

	public void SetShape(EntityUid uid, List<Box2i>? shape, ItemComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemComponent>(uid, ref component, false) && component.Shape != shape)
		{
			component.Shape = shape;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			ItemSizeChangedEvent ev = new ItemSizeChangedEvent(uid);
			((EntitySystem)this).RaiseLocalEvent<ItemSizeChangedEvent>(uid, ref ev, true);
		}
	}

	public void SetStoredOffset(EntityUid uid, Vector2i newOffset, ItemComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemComponent>(uid, ref component, false))
		{
			component.StoredOffset = newOffset;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void SetHeldPrefix(EntityUid uid, string? heldPrefix, bool force = false, ItemComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemComponent>(uid, ref component, false) && (force || !(component.HeldPrefix == heldPrefix)))
		{
			component.HeldPrefix = heldPrefix;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			VisualsChanged(uid);
		}
	}

	public void CopyVisuals(EntityUid uid, ItemComponent otherItem, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemComponent>(uid, ref item, true))
		{
			item.RsiPath = otherItem.RsiPath;
			item.InhandVisuals = otherItem.InhandVisuals;
			item.HeldPrefix = otherItem.HeldPrefix;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)item, (MetaDataComponent)null);
			VisualsChanged(uid);
		}
	}

	private void OnHandInteract(EntityUid uid, ItemComponent component, InteractHandEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _handsSystem.TryPickup(args.User, uid))
		{
			((HandledEntityEventArgs)args).Handled = true;
			ItemPickedUpEvent ev = new ItemPickedUpEvent(args.User, uid);
			((EntitySystem)this).RaiseLocalEvent<ItemPickedUpEvent>(uid, ref ev, true);
		}
	}

	private void AddPickupVerb(EntityUid uid, ItemComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (args.Hands != null && !args.Using.HasValue && args.CanAccess && args.CanInteract && _handsSystem.CanPickupAnyHand(args.User, args.Target, checkActionBlocker: true, args.Hands, component))
		{
			InteractionVerb verb = new InteractionVerb();
			verb.Act = delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				_handsSystem.TryPickupAnyHand(args.User, args.Target, checkActionBlocker: false, animateUser: false, animate: true, args.Hands, component);
			};
			verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png"));
			BaseContainer userContainer = default(BaseContainer);
			Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(args.User, null, null)), ref userContainer);
			BaseContainer container = default(BaseContainer);
			if (Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(args.Target, null, null)), ref container) && container != userContainer)
			{
				verb.Text = base.Loc.GetString("pick-up-verb-get-data-text-inventory");
			}
			else
			{
				verb.Text = base.Loc.GetString("pick-up-verb-get-data-text");
			}
			args.Verbs.Add(verb);
		}
	}

	private void OnExamine(EntityUid uid, ItemComponent component, ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!(component.Size == ProtoId<ItemSizePrototype>.op_Implicit("Invalid")))
		{
			args.PushMarkup(base.Loc.GetString("item-component-on-examine-size", (ValueTuple<string, object>)("size", GetItemSizeLocale(component.Size))), -2);
		}
	}

	public ItemSizePrototype GetSizePrototype(ProtoId<ItemSizePrototype> id)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _prototype.Index<ItemSizePrototype>(id);
	}

	public virtual void VisualsChanged(EntityUid owner)
	{
	}

	public string GetItemSizeLocale(ProtoId<ItemSizePrototype> size)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return base.Loc.GetString(LocId.op_Implicit(GetSizePrototype(size).Name));
	}

	public int GetItemSizeWeight(ProtoId<ItemSizePrototype> size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetSizePrototype(size).Weight;
	}

	public IReadOnlyList<Box2i> GetItemShape(Entity<StorageComponent?> storage, Entity<ItemComponent?> uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemComponent>(Entity<ItemComponent>.op_Implicit(uid), ref uid.Comp, true))
		{
			return (IReadOnlyList<Box2i>)(object)new Box2i[0];
		}
		FixedItemSizeStorageComponent fixedComp = default(FixedItemSizeStorageComponent);
		if (_fixedItemSizeStorageQuery.TryComp(Entity<StorageComponent>.op_Implicit(storage), ref fixedComp))
		{
			FixedItemSizeStorageComponent fixedItemSizeStorageComponent = fixedComp;
			if (fixedItemSizeStorageComponent.CachedSize == null)
			{
				fixedItemSizeStorageComponent.CachedSize = (Box2i[]?)(object)new Box2i[1] { Box2i.FromDimensions(Vector2i.Zero, fixedComp.Size - Vector2i.One) };
			}
			return fixedComp.CachedSize;
		}
		IReadOnlyList<Box2i> shape = uid.Comp.Shape;
		return shape ?? GetSizePrototype(uid.Comp.Size).DefaultShape;
	}

	public IReadOnlyList<Box2i> GetItemShape(ItemComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<Box2i> shape = component.Shape;
		return shape ?? GetSizePrototype(component.Size).DefaultShape;
	}

	public IReadOnlyList<Box2i> GetAdjustedItemShape(Entity<StorageComponent?> storage, Entity<ItemComponent?> entity, ItemStorageLocation location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GetAdjustedItemShape(storage, entity, location.Rotation, location.Position);
	}

	public IReadOnlyList<Box2i> GetAdjustedItemShape(Entity<StorageComponent?> storage, Entity<ItemComponent?> entity, Angle rotation, Vector2i position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemComponent>(Entity<ItemComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return (IReadOnlyList<Box2i>)(object)new Box2i[0];
		}
		IReadOnlyList<Box2i> itemShape = GetItemShape(storage, entity);
		Box2i boundingShape = itemShape.GetBoundingBox();
		Box2 val = Box2i.op_Implicit(boundingShape);
		Vector2 boundingCenter = ((Box2)(ref val)).Center;
		Matrix3x2 matty = Matrix3Helpers.CreateTransform(ref boundingCenter, ref rotation);
		Vector2 vector = Vector2i.op_Implicit(boundingShape.BottomLeft);
		val = Box2i.op_Implicit(boundingShape);
		Vector2 drift = vector - Matrix3Helpers.TransformBox(matty, ref val).BottomLeft;
		List<Box2i> adjustedShapes = new List<Box2i>();
		Box2i floored = default(Box2i);
		foreach (Box2i shape in itemShape)
		{
			val = Box2i.op_Implicit(shape);
			Box2 val2 = Matrix3Helpers.TransformBox(matty, ref val);
			Box2 transformed = ((Box2)(ref val2)).Translated(drift);
			((Box2i)(ref floored))._002Ector(Vector2Helpers.Floored(transformed.BottomLeft), Vector2Helpers.Floored(transformed.TopRight));
			Box2i translated = ((Box2i)(ref floored)).Translated(position);
			adjustedShapes.Add(translated);
		}
		return adjustedShapes;
	}

	private void OnItemToggle(EntityUid uid, ItemToggleSizeComponent itemToggleSize, ItemToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		ItemComponent item = default(ItemComponent);
		if (!((EntitySystem)this).TryComp<ItemComponent>(uid, ref item))
		{
			return;
		}
		if (args.Activated)
		{
			if (itemToggleSize.ActivatedShape != null)
			{
				ItemToggleSizeComponent itemToggleSizeComponent = itemToggleSize;
				if (itemToggleSizeComponent.DeactivatedShape == null)
				{
					itemToggleSizeComponent.DeactivatedShape = new List<Box2i>(GetItemShape(item));
				}
				((EntitySystem)this).Dirty(uid, (IComponent)(object)itemToggleSize, (MetaDataComponent)null);
				SetShape(uid, itemToggleSize.ActivatedShape, item);
			}
			if (itemToggleSize.ActivatedSize.HasValue)
			{
				ItemToggleSizeComponent itemToggleSizeComponent = itemToggleSize;
				ProtoId<ItemSizePrototype> valueOrDefault = itemToggleSizeComponent.DeactivatedSize.GetValueOrDefault();
				if (!itemToggleSizeComponent.DeactivatedSize.HasValue)
				{
					valueOrDefault = item.Size;
					itemToggleSizeComponent.DeactivatedSize = valueOrDefault;
				}
				((EntitySystem)this).Dirty(uid, (IComponent)(object)itemToggleSize, (MetaDataComponent)null);
				SetSize(uid, itemToggleSize.ActivatedSize.Value, item);
			}
		}
		else
		{
			if (itemToggleSize.DeactivatedShape != null)
			{
				SetShape(uid, itemToggleSize.DeactivatedShape, item);
			}
			if (itemToggleSize.DeactivatedSize.HasValue)
			{
				SetSize(uid, itemToggleSize.DeactivatedSize.Value, item);
			}
		}
	}
}
