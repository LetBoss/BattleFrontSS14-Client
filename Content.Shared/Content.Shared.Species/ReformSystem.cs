using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Species.Components;
using Content.Shared.Stunnable;
using Content.Shared.Zombies;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Shared.Species;

public sealed class ReformSystem : EntitySystem
{
	public sealed class ReformEvent : InstantActionEvent, ISerializationGenerated<ReformEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref ReformEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InstantActionEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (ReformEvent)definitionCast;
			serialization.TryCustomCopy<ReformEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref ReformEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ReformEvent cast = (ReformEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ReformEvent cast = (ReformEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override ReformEvent Instantiate()
		{
			return new ReformEvent();
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class ReformDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<ReformDoAfterEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref ReformDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (ReformDoAfterEvent)definitionCast;
			serialization.TryCustomCopy<ReformDoAfterEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref ReformDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ReformDoAfterEvent cast = (ReformDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ReformDoAfterEvent cast = (ReformDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override ReformDoAfterEvent Instantiate()
		{
			return new ReformDoAfterEvent();
		}
	}

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private SharedStunSystem _stunSystem;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedMindSystem _mindSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ReformComponent, MapInitEvent>((ComponentEventHandler<ReformComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReformComponent, ComponentShutdown>((ComponentEventHandler<ReformComponent, ComponentShutdown>)OnCompRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReformComponent, ReformEvent>((ComponentEventHandler<ReformComponent, ReformEvent>)OnReform, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReformComponent, ReformDoAfterEvent>((ComponentEventHandler<ReformComponent, ReformDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReformComponent, EntityZombifiedEvent>((ComponentEventRefHandler<ReformComponent, EntityZombifiedEvent>)OnZombified, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, ReformComponent comp, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype actionProto = default(EntityPrototype);
		if (!(comp.ActionPrototype != default(EntProtoId)) || _protoManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(comp.ActionPrototype), ref actionProto))
		{
			_actionsSystem.AddAction(uid, ref comp.ActionEntity, out ActionComponent reformAction, EntProtoId.op_Implicit(comp.ActionPrototype));
			if (comp.StartDelayed && reformAction != null && reformAction.UseDelay.HasValue)
			{
				TimeSpan start = _gameTiming.CurTime;
				TimeSpan end = _gameTiming.CurTime + reformAction.UseDelay.Value;
				_actionsSystem.SetCooldown(Entity<ActionComponent>.op_Implicit(comp.ActionEntity.Value), start, end);
			}
		}
	}

	private void OnCompRemove(EntityUid uid, ReformComponent comp, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? actionEntity = comp.ActionEntity;
		actionsSystem.RemoveAction(performer, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}

	private void OnReform(EntityUid uid, ReformComponent comp, ReformEvent args)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (comp.ShouldStun)
		{
			_stunSystem.TryStun(uid, TimeSpan.FromSeconds(comp.ReformTime), refresh: true);
		}
		_popupSystem.PopupClient(base.Loc.GetString(comp.PopupText, (ValueTuple<string, object>)("name", uid)), uid, uid);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, uid, comp.ReformTime, new ReformDoAfterEvent(), uid)
		{
			BreakOnMove = true,
			BlockDuplicate = true,
			BreakOnDamage = true,
			CancelDuplicate = true,
			RequireCanInteract = false
		};
		_doAfterSystem.TryStartDoAfter(doAfter);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnDoAfter(EntityUid uid, ReformComponent comp, ReformDoAfterEvent args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && !((Component)comp).Deleted && !_netMan.IsClient)
		{
			EntityUid child = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(comp.ReformPrototype), ((EntitySystem)this).Transform(uid).Coordinates);
			if (_mindSystem.TryGetMind(uid, out EntityUid mindId, out MindComponent mind))
			{
				_mindSystem.TransferTo(mindId, child, ghostCheckOverride: false, createGhost: true, mind);
			}
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
	}

	private void OnZombified(EntityUid uid, ReformComponent comp, ref EntityZombifiedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? actionEntity = comp.ActionEntity;
		actionsSystem.RemoveAction(performer, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}
}
