using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.DragDrop;

public abstract class SharedDragDropSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private SharedInteractionSystem _interaction;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeAllEvent<DragDropRequestEvent>((EntitySessionEventHandler<DragDropRequestEvent>)OnDragDropRequestEvent, (Type[])null, (Type[])null);
	}

	private void OnDragDropRequestEvent(DragDropRequestEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		EntityUid dragged = ((EntitySystem)this).GetEntity(msg.Dragged);
		EntityUid target = ((EntitySystem)this).GetEntity(msg.Target);
		if (((EntitySystem)this).Deleted(dragged, (MetaDataComponent)null) || ((EntitySystem)this).Deleted(target, (MetaDataComponent)null))
		{
			return;
		}
		EntityUid? user = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (user.HasValue && _actionBlockerSystem.CanInteract(user.Value, target) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user.Value), Entity<TransformComponent>.op_Implicit(dragged), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user.Value), Entity<TransformComponent>.op_Implicit(target), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			DragDropDraggedEvent dragArgs = new DragDropDraggedEvent(user.Value, target);
			((EntitySystem)this).RaiseLocalEvent<DragDropDraggedEvent>(dragged, ref dragArgs, false);
			if (!dragArgs.Handled)
			{
				DragDropTargetEvent dropArgs = new DragDropTargetEvent(user.Value, dragged);
				((EntitySystem)this).RaiseLocalEvent<DragDropTargetEvent>(((EntitySystem)this).GetEntity(msg.Target), ref dropArgs, false);
			}
		}
	}
}
