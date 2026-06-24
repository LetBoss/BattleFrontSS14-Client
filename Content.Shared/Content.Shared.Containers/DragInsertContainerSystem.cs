using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Climbing.Systems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Containers;

public sealed class DragInsertContainerSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class DragInsertContainerDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<DragInsertContainerDoAfterEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref DragInsertContainerDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (DragInsertContainerDoAfterEvent)definitionCast;
			serialization.TryCustomCopy<DragInsertContainerDoAfterEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref DragInsertContainerDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DragInsertContainerDoAfterEvent cast = (DragInsertContainerDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DragInsertContainerDoAfterEvent cast = (DragInsertContainerDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override DragInsertContainerDoAfterEvent Instantiate()
		{
			return new DragInsertContainerDoAfterEvent();
		}
	}

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private ClimbSystem _climb;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DragInsertContainerComponent, DragDropTargetEvent>((EntityEventRefHandler<DragInsertContainerComponent, DragDropTargetEvent>)OnDragDropOn, new Type[1] { typeof(ClimbSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DragInsertContainerComponent, DragInsertContainerDoAfterEvent>((EntityEventRefHandler<DragInsertContainerComponent, DragInsertContainerDoAfterEvent>)OnDragFinished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DragInsertContainerComponent, CanDropTargetEvent>((EntityEventRefHandler<DragInsertContainerComponent, CanDropTargetEvent>)OnCanDragDropOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DragInsertContainerComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DragInsertContainerComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerb, (Type[])null, (Type[])null);
	}

	private void OnDragDropOn(Entity<DragInsertContainerComponent> ent, ref DragDropTargetEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		Entity<DragInsertContainerComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		DragInsertContainerComponent dragInsertContainerComponent = default(DragInsertContainerComponent);
		val.Deconstruct(ref val2, ref dragInsertContainerComponent);
		DragInsertContainerComponent comp = dragInsertContainerComponent;
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<DragInsertContainerComponent>.op_Implicit(ent), comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			if (comp.EntryDelay <= TimeSpan.Zero || (!comp.DelaySelfEntry && args.User == args.Dragged))
			{
				args.Handled = Insert(args.Dragged, args.User, Entity<DragInsertContainerComponent>.op_Implicit(ent), container);
				return;
			}
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, comp.EntryDelay, new DragInsertContainerDoAfterEvent(), Entity<DragInsertContainerComponent>.op_Implicit(ent), args.Dragged, Entity<DragInsertContainerComponent>.op_Implicit(ent))
			{
				BreakOnDamage = true,
				BreakOnMove = true,
				NeedHand = false
			};
			_doAfter.TryStartDoAfter(doAfterArgs);
			args.Handled = true;
		}
	}

	private void OnDragFinished(Entity<DragInsertContainerComponent> ent, ref DragInsertContainerDoAfterEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && args.Args.Target.HasValue && _container.TryGetContainer(Entity<DragInsertContainerComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			Insert(args.Args.Target.Value, args.User, Entity<DragInsertContainerComponent>.op_Implicit(ent), container);
		}
	}

	private void OnCanDragDropOn(Entity<DragInsertContainerComponent> ent, ref CanDropTargetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Entity<DragInsertContainerComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		DragInsertContainerComponent dragInsertContainerComponent = default(DragInsertContainerComponent);
		val.Deconstruct(ref val2, ref dragInsertContainerComponent);
		DragInsertContainerComponent comp = dragInsertContainerComponent;
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<DragInsertContainerComponent>.op_Implicit(ent), comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			args.Handled = true;
			args.CanDrop |= _container.CanInsert(args.Dragged, container, false, (TransformComponent)null);
		}
	}

	private void OnGetAlternativeVerb(Entity<DragInsertContainerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Entity<DragInsertContainerComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		DragInsertContainerComponent dragInsertContainerComponent = default(DragInsertContainerComponent);
		val.Deconstruct(ref val2, ref dragInsertContainerComponent);
		EntityUid uid = val2;
		DragInsertContainerComponent comp = dragInsertContainerComponent;
		BaseContainer container = default(BaseContainer);
		if (!comp.UseVerbs || !args.CanInteract || !args.CanAccess || args.Hands == null || !_container.TryGetContainer(uid, comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		EntityUid user = args.User;
		if (!_actionBlocker.CanInteract(user, Entity<DragInsertContainerComponent>.op_Implicit(ent)))
		{
			return;
		}
		if (container.ContainedEntities.Count > 0)
		{
			int emptyableCount = 0;
			foreach (EntityUid contained in container.ContainedEntities)
			{
				if (_container.CanRemove(contained, container))
				{
					emptyableCount++;
				}
			}
			if (emptyableCount > 0)
			{
				AlternativeVerb verb = new AlternativeVerb
				{
					Act = delegate
					{
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						//IL_0059: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
						//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
						//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
						ISharedAdminLogManager adminLog = _adminLog;
						LogStringHandler handler = new LogStringHandler(19, 2);
						handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
						handler.AppendLiteral(" emptied container ");
						handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DragInsertContainerComponent>.op_Implicit(ent), (MetaDataComponent)null), "ToPrettyString(ent)");
						adminLog.Add(LogType.Action, LogImpact.Low, ref handler);
						foreach (EntityUid current in _container.EmptyContainer(container, false, (EntityCoordinates?)null, true))
						{
							_climb.ForciblySetClimbing(current, Entity<DragInsertContainerComponent>.op_Implicit(ent));
						}
					},
					Category = VerbCategory.Eject,
					Text = base.Loc.GetString("container-verb-text-empty"),
					Priority = 1
				};
				args.Verbs.Add(verb);
			}
		}
		if (_container.CanInsert(user, container, false, (TransformComponent)null) && _actionBlocker.CanMove(user))
		{
			AlternativeVerb verb2 = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					Insert(user, user, Entity<DragInsertContainerComponent>.op_Implicit(ent), container);
				},
				Text = base.Loc.GetString("container-verb-text-enter"),
				Priority = 2
			};
			args.Verbs.Add(verb2);
		}
	}

	public bool Insert(EntityUid target, EntityUid user, EntityUid containerEntity, BaseContainer container)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(target), container, (TransformComponent)null, false))
		{
			return false;
		}
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(26, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
		handler.AppendLiteral(" inserted ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
		handler.AppendLiteral(" into container ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(containerEntity)), "ToPrettyString(containerEntity)");
		adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
		return true;
	}
}
