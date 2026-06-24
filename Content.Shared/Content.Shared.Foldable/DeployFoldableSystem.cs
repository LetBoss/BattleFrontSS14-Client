using System;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Foldable;

public sealed class DeployFoldableSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private FoldableSystem _foldable;

	[Dependency]
	private AnchorableSystem _anchorable;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private TagSystem _tag;

	private static readonly ProtoId<TagPrototype> CatwalkTag = ProtoId<TagPrototype>.op_Implicit("Catwalk");

	private static readonly ProtoId<TagPrototype> StairsTag = ProtoId<TagPrototype>.op_Implicit("RMCStairs");

	private static readonly ProtoId<TagPrototype> CarpetTag = ProtoId<TagPrototype>.op_Implicit("Carpet");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeployFoldableComponent, AfterInteractEvent>((EntityEventRefHandler<DeployFoldableComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployFoldableComponent, CanDragEvent>((EntityEventRefHandler<DeployFoldableComponent, CanDragEvent>)OnCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployFoldableComponent, DragDropDraggedEvent>((EntityEventRefHandler<DeployFoldableComponent, DragDropDraggedEvent>)OnDragDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployFoldableComponent, CanDropDraggedEvent>((EntityEventRefHandler<DeployFoldableComponent, CanDropDraggedEvent>)OnCanDropDragged, (Type[])null, (Type[])null);
	}

	private void OnCanDropDragged(Entity<DeployFoldableComponent> ent, ref CanDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != args.Target))
		{
			args.Handled = true;
			args.CanDrop = true;
		}
	}

	private void OnDragDropDragged(Entity<DeployFoldableComponent> ent, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (!(args.User != args.Target) && ((EntitySystem)this).TryComp<FoldableComponent>(Entity<DeployFoldableComponent>.op_Implicit(ent), ref foldable) && _foldable.TrySetFolded(Entity<DeployFoldableComponent>.op_Implicit(ent), foldable, state: true))
		{
			_hands.PickupOrDrop(args.User, ent.Owner);
			args.Handled = true;
		}
	}

	private void OnCanDrag(Entity<DeployFoldableComponent> ent, ref CanDragEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<DeployFoldableComponent>.op_Implicit(ent), ref foldable) && !foldable.IsFolded)
		{
			args.Handled = true;
		}
	}

	private void OnAfterInteract(Entity<DeployFoldableComponent> ent, ref AfterInteractEvent args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || (args.Target.HasValue && !_tag.HasAnyTag(args.Target.Value, CatwalkTag, StairsTag, CarpetTag)) || !((EntitySystem)this).TryComp<FoldableComponent>(Entity<DeployFoldableComponent>.op_Implicit(ent), ref foldable))
		{
			return;
		}
		PhysicsComponent anchorBody = default(PhysicsComponent);
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(ent.Owner, ref anchorBody) || !_anchorable.TileFree(args.ClickLocation, anchorBody))
		{
			_popup.PopupPredicted(base.Loc.GetString("foldable-deploy-fail", (ValueTuple<string, object>)("object", ent)), Entity<DeployFoldableComponent>.op_Implicit(ent), args.User);
		}
		else if (((EntitySystem)this).TryComp<HandsComponent>(args.User, ref hands) && _hands.TryDrop(Entity<HandsComponent>.op_Implicit((args.User, hands)), args.Used, args.ClickLocation))
		{
			if (!_foldable.TrySetFolded(Entity<DeployFoldableComponent>.op_Implicit(ent), foldable, state: false))
			{
				_hands.TryPickup(args.User, args.Used, null, checkActionBlocker: true, animateUser: false, animate: true, hands);
			}
			else
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}
}
