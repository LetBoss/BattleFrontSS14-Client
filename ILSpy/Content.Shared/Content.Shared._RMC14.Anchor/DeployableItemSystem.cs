using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Inventory;
using Content.Shared.ActionBlocker;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Anchor;

public sealed class DeployableItemSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private AnchorableSystem _anchorable;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private FoldableSystem _foldable;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedCMInventorySystem _cmInventory;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly HashSet<Entity<DeployableItemComponent>> _deployables = new HashSet<Entity<DeployableItemComponent>>();

	private static readonly ProtoId<TagPrototype> CatwalkTag = ProtoId<TagPrototype>.op_Implicit("Catwalk");

	private static readonly ProtoId<TagPrototype> StairsTag = ProtoId<TagPrototype>.op_Implicit("RMCStairs");

	private static readonly ProtoId<TagPrototype> CarpetTag = ProtoId<TagPrototype>.op_Implicit("Carpet");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, AfterInteractEvent>((EntityEventRefHandler<DeployableItemComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, InteractHandEvent>((EntityEventRefHandler<DeployableItemComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, UseInHandEvent>((EntityEventRefHandler<DeployableItemComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, CanDragEvent>((EntityEventRefHandler<DeployableItemComponent, CanDragEvent>)OnCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, CanDropDraggedEvent>((EntityEventRefHandler<DeployableItemComponent, CanDropDraggedEvent>)OnCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, DragDropDraggedEvent>((EntityEventRefHandler<DeployableItemComponent, DragDropDraggedEvent>)OnDragDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, ExaminedEvent>((EntityEventRefHandler<DeployableItemComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableItemComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DeployableItemComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployFoldableComponent, UseInHandEvent>((EntityEventRefHandler<DeployFoldableComponent, UseInHandEvent>)OnFoldableUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, CanDropTargetEvent>((EntityEventRefHandler<HandsComponent, CanDropTargetEvent>)OnCanDropTarget, (Type[])null, (Type[])null);
	}

	private void OnCanDrag(Entity<DeployableItemComponent> ent, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnCanDropDragged(Entity<DeployableItemComponent> ent, ref CanDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Transform(Entity<DeployableItemComponent>.op_Implicit(ent)).Anchored)
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnCanDropTarget(Entity<HandsComponent> ent, ref CanDropTargetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!(ent.Owner != args.User) && CanPickup(args.Dragged, args.User))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnDragDropDragged(Entity<DeployableItemComponent> ent, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != args.Target) && CanPickup(Entity<DeployableItemComponent>.op_Implicit(ent), args.User))
		{
			args.Handled = true;
			Pickup(ent, args.User);
		}
	}

	private void OnExamined(Entity<DeployableItemComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		var (filled, total) = _cmInventory.GetItemSlotsFilled(Entity<ItemSlotsComponent>.op_Implicit(ent.Owner));
		using (args.PushGroup("DeployableItemComponent"))
		{
			if (ent.Comp.Position != DeployableItemPosition.None)
			{
				args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-deployed-click"));
				args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-deployed-drag"));
				if (ent.Comp.MagazineExamine)
				{
					args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-magazines", (ValueTuple<string, object>)("filled", filled), (ValueTuple<string, object>)("total", total)));
				}
				return;
			}
			args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-not-deployed"));
			if (ent.Comp.MagazineExamine)
			{
				if (filled == 0)
				{
					args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-empty"));
				}
				else if (filled < total * ent.Comp.AlmostEmptyThreshold)
				{
					args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-almost-empty"));
				}
				else if (filled < total * ent.Comp.HalfFullThreshold)
				{
					args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-half-full"));
				}
				else
				{
					args.PushMarkup(base.Loc.GetString("cm-magazine-box-examine-almost-full"));
				}
			}
		}
	}

	private void OnGetAlternativeVerbs(Entity<DeployableItemComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && ent.Comp.Position != DeployableItemPosition.None)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("cm-magazine-box-pick-up"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					Pickup(ent, user);
				},
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png"))
			});
		}
	}

	private void OnAfterInteract(Entity<DeployableItemComponent> ent, ref AfterInteractEvent args)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && (!args.Target.HasValue || _tag.HasAnyTag(args.Target.Value, CatwalkTag, StairsTag, CarpetTag)))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Deploy(ent, args.User, args.ClickLocation);
		}
	}

	private void OnInteractHand(Entity<DeployableItemComponent> ent, ref InteractHandEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.LeftClickPickup && ent.Comp.Position != DeployableItemPosition.None)
		{
			((HandledEntityEventArgs)args).Handled = true;
			Pickup(ent, args.User);
		}
	}

	private void OnUseInHand(Entity<DeployableItemComponent> ent, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		Deploy(ent, args.User, _transform.GetMoverCoordinates(Entity<DeployableItemComponent>.op_Implicit(ent)));
	}

	private void OnFoldableUseInHand(Entity<DeployFoldableComponent> ent, ref UseInHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<FoldableComponent>(Entity<DeployFoldableComponent>.op_Implicit(ent), ref foldable))
		{
			return;
		}
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(args.User);
		PhysicsComponent anchorBody = default(PhysicsComponent);
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(ent.Owner, ref anchorBody) || !_anchorable.TileFree(coordinates, anchorBody))
		{
			_popup.PopupPredicted(base.Loc.GetString("foldable-deploy-fail", (ValueTuple<string, object>)("object", ent)), Entity<DeployFoldableComponent>.op_Implicit(ent), args.User);
		}
		else if (((EntitySystem)this).TryComp<HandsComponent>(args.User, ref hands) && _hands.TryDrop(Entity<HandsComponent>.op_Implicit((args.User, hands)), Entity<DeployFoldableComponent>.op_Implicit(ent), coordinates))
		{
			if (!_foldable.TrySetFolded(Entity<DeployFoldableComponent>.op_Implicit(ent), foldable, state: false))
			{
				_hands.TryPickup(args.User, Entity<DeployFoldableComponent>.op_Implicit(ent), null, checkActionBlocker: true, animateUser: false, animate: true, hands);
			}
			else
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void Deploy(Entity<DeployableItemComponent> ent, EntityUid user, EntityCoordinates location)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		location = _transform.GetMoverCoordinates(location).SnapToGrid();
		TransformComponent transform = ((EntitySystem)this).Transform(Entity<DeployableItemComponent>.op_Implicit(ent));
		Entity<TransformComponent> transformEnt = default(Entity<TransformComponent>);
		transformEnt._002Ector(Entity<DeployableItemComponent>.op_Implicit(ent), transform);
		if (!_transform.GetGrid(transformEnt).HasValue)
		{
			return;
		}
		MapId map = _transform.GetMapId(transformEnt);
		Vector2 worldPos = _transform.ToMapCoordinates(location, true).Position;
		_deployables.Clear();
		_entityLookup.GetEntitiesInRange<DeployableItemComponent>(map, worldPos, 0.3f, _deployables, (LookupFlags)110);
		bool lower = false;
		bool upper = false;
		foreach (Entity<DeployableItemComponent> deployable in _deployables)
		{
			if (!(deployable.Owner == ent.Owner))
			{
				switch (deployable.Comp.Position)
				{
				case DeployableItemPosition.Lower:
					lower = true;
					break;
				case DeployableItemPosition.Upper:
					upper = true;
					break;
				}
				if (lower && upper)
				{
					_popup.PopupClient(base.Loc.GetString("cm-magazine-box-no-space"), user, PopupType.SmallCaution);
					return;
				}
			}
		}
		DeployableItemPosition position;
		Vector2 offset;
		if (!lower)
		{
			position = DeployableItemPosition.Lower;
			offset = new Vector2(0f, -0.25f);
		}
		else
		{
			if (upper)
			{
				return;
			}
			position = DeployableItemPosition.Upper;
			offset = new Vector2(0f, 0.25f);
		}
		location = ((EntityCoordinates)(ref location)).Offset(offset);
		if (_hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), Entity<DeployableItemComponent>.op_Implicit(ent), location))
		{
			_transform.SetCoordinates(Entity<DeployableItemComponent>.op_Implicit(ent), location);
			_physics.SetBodyType(Entity<DeployableItemComponent>.op_Implicit(ent), (BodyType)4, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
			ent.Comp.Position = position;
			_appearance.SetData(Entity<DeployableItemComponent>.op_Implicit(ent), (Enum)DeployableItemVisuals.Deployed, (object)true, (AppearanceComponent)null);
			((EntitySystem)this).Dirty<DeployableItemComponent>(ent, (MetaDataComponent)null);
		}
	}

	private bool CanPickup(EntityUid deployable, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		DeployableItemComponent deployableComp = default(DeployableItemComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(deployable, (MetaDataComponent)null) && _hands.TryGetEmptyHand(Entity<HandsComponent>.op_Implicit(user), out string _) && _actionBlocker.CanPickup(user, deployable) && ((EntitySystem)this).TryComp<DeployableItemComponent>(deployable, ref deployableComp))
		{
			return deployableComp.Position != DeployableItemPosition.None;
		}
		return false;
	}

	private void Pickup(Entity<DeployableItemComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (CanPickup(Entity<DeployableItemComponent>.op_Implicit(ent), user))
		{
			_physics.SetBodyType(Entity<DeployableItemComponent>.op_Implicit(ent), (BodyType)8, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
			if (!_hands.TryPickupAnyHand(user, Entity<DeployableItemComponent>.op_Implicit(ent)))
			{
				_physics.SetBodyType(Entity<DeployableItemComponent>.op_Implicit(ent), (BodyType)4, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
				return;
			}
			ent.Comp.Position = DeployableItemPosition.None;
			_appearance.SetData(Entity<DeployableItemComponent>.op_Implicit(ent), (Enum)DeployableItemVisuals.Deployed, (object)false, (AppearanceComponent)null);
			((EntitySystem)this).Dirty<DeployableItemComponent>(ent, (MetaDataComponent)null);
		}
	}
}
