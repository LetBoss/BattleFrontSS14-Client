using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Species.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Species;

public sealed class GibActionSystem : EntitySystem
{
	public sealed class GibActionEvent : InstantActionEvent, ISerializationGenerated<GibActionEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref GibActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InstantActionEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (GibActionEvent)definitionCast;
			serialization.TryCustomCopy<GibActionEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref GibActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			GibActionEvent cast = (GibActionEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			GibActionEvent cast = (GibActionEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override GibActionEvent Instantiate()
		{
			return new GibActionEvent();
		}
	}

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedBodySystem _bodySystem;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GibActionComponent, MobStateChangedEvent>((ComponentEventHandler<GibActionComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GibActionComponent, GibActionEvent>((ComponentEventHandler<GibActionComponent, GibActionEvent>)OnGibAction, (Type[])null, (Type[])null);
	}

	private void OnMobStateChanged(EntityUid uid, GibActionComponent comp, MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		EntityPrototype actionProto = default(EntityPrototype);
		if (!((EntitySystem)this).TryComp<MobStateComponent>(uid, ref mobState) || !_protoManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(comp.ActionPrototype), ref actionProto))
		{
			return;
		}
		foreach (MobState allowedState in comp.AllowedStates)
		{
			if (allowedState == mobState.CurrentState)
			{
				_actionsSystem.AddAction(uid, ref comp.ActionEntity, EntProtoId.op_Implicit(comp.ActionPrototype));
				return;
			}
		}
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? actionEntity = comp.ActionEntity;
		actionsSystem.RemoveAction(performer, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}

	private void OnGibAction(EntityUid uid, GibActionComponent comp, GibActionEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		_popupSystem.PopupClient(base.Loc.GetString(comp.PopupText, (ValueTuple<string, object>)("name", uid)), uid, uid);
		_bodySystem.GibBody(uid, gibOrgans: true);
	}
}
