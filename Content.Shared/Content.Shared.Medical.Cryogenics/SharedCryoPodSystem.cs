using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Medical.Cryogenics;

public abstract class SharedCryoPodSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class CryoPodPryFinished : SimpleDoAfterEvent, ISerializationGenerated<CryoPodPryFinished>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref CryoPodPryFinished target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (CryoPodPryFinished)definitionCast;
			serialization.TryCustomCopy<CryoPodPryFinished>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref CryoPodPryFinished target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			CryoPodPryFinished cast = (CryoPodPryFinished)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			CryoPodPryFinished cast = (CryoPodPryFinished)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override CryoPodPryFinished Instantiate()
		{
			return new CryoPodPryFinished();
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class CryoPodDragFinished : SimpleDoAfterEvent, ISerializationGenerated<CryoPodDragFinished>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref CryoPodDragFinished target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (CryoPodDragFinished)definitionCast;
			serialization.TryCustomCopy<CryoPodDragFinished>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref CryoPodDragFinished target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			CryoPodDragFinished cast = (CryoPodDragFinished)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			CryoPodDragFinished cast = (CryoPodDragFinished)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override CryoPodDragFinished Instantiate()
		{
			return new CryoPodDragFinished();
		}
	}

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private StandingStateSystem _standingStateSystem;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private MobStateSystem _mobStateSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedPointLightSystem _light;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CryoPodComponent, CanDropTargetEvent>((ComponentEventRefHandler<CryoPodComponent, CanDropTargetEvent>)OnCryoPodCanDropOn, (Type[])null, (Type[])null);
		InitializeInsideCryoPod();
	}

	private void OnCryoPodCanDropOn(EntityUid uid, CryoPodComponent component, ref CanDropTargetEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			args.CanDrop = ((EntitySystem)this).HasComp<BodyComponent>(args.Dragged);
			args.Handled = true;
		}
	}

	protected void OnComponentInit(EntityUid uid, CryoPodComponent cryoPodComponent, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		cryoPodComponent.BodyContainer = _containerSystem.EnsureContainer<ContainerSlot>(uid, "scanner-body", (ContainerManagerComponent)null);
	}

	protected void UpdateAppearance(EntityUid uid, CryoPodComponent? cryoPod = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CryoPodComponent>(uid, ref cryoPod, true))
		{
			bool cryoPodEnabled = ((EntitySystem)this).HasComp<ActiveCryoPodComponent>(uid);
			SharedPointLightComponent light = default(SharedPointLightComponent);
			if (_light.TryGetLight(uid, ref light))
			{
				_light.SetEnabled(uid, cryoPodEnabled && cryoPod.BodyContainer.ContainedEntity.HasValue, light, (MetaDataComponent)null);
			}
			if (((EntitySystem)this).Resolve<AppearanceComponent>(uid, ref appearance, true))
			{
				_appearanceSystem.SetData(uid, (Enum)CryoPodComponent.CryoPodVisuals.ContainsEntity, (object)(!cryoPod.BodyContainer.ContainedEntity.HasValue), appearance);
				_appearanceSystem.SetData(uid, (Enum)CryoPodComponent.CryoPodVisuals.IsOn, (object)cryoPodEnabled, appearance);
			}
		}
	}

	public bool InsertBody(EntityUid uid, EntityUid target, CryoPodComponent cryoPodComponent)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (cryoPodComponent.BodyContainer.ContainedEntity.HasValue)
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<MobStateComponent>(target))
		{
			return false;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(target);
		_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit((target, xform)), (BaseContainer)(object)cryoPodComponent.BodyContainer, (TransformComponent)null, false);
		((EntitySystem)this).EnsureComp<InsideCryoPodComponent>(target);
		_standingStateSystem.Stand(target, null, null, force: true);
		UpdateAppearance(uid, cryoPodComponent);
		return true;
	}

	public void TryEjectBody(EntityUid uid, EntityUid userId, CryoPodComponent? cryoPodComponent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CryoPodComponent>(uid, ref cryoPodComponent, true))
		{
			return;
		}
		if (cryoPodComponent.Locked)
		{
			_popupSystem.PopupEntity(base.Loc.GetString("cryo-pod-locked"), uid, userId);
			return;
		}
		EntityUid? ejected = EjectBody(uid, cryoPodComponent);
		if (ejected.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(18, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ejected.Value)), "ToPrettyString(ejected.Value)");
			handler.AppendLiteral(" ejected from ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			handler.AppendLiteral(" by ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userId)), "ToPrettyString(userId)");
			adminLogger.Add(LogType.Action, LogImpact.Medium, ref handler);
		}
	}

	public virtual EntityUid? EjectBody(EntityUid uid, CryoPodComponent? cryoPodComponent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CryoPodComponent>(uid, ref cryoPodComponent, true))
		{
			return null;
		}
		EntityUid? containedEntity = cryoPodComponent.BodyContainer.ContainedEntity;
		if (containedEntity.HasValue)
		{
			EntityUid contained = containedEntity.GetValueOrDefault();
			if (((EntityUid)(ref contained)).Valid)
			{
				_containerSystem.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained), (BaseContainer)(object)cryoPodComponent.BodyContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
				if (((EntitySystem)this).HasComp<KnockedDownComponent>(contained) || _mobStateSystem.IsIncapacitated(contained))
				{
					_standingStateSystem.Down(contained);
				}
				else
				{
					_standingStateSystem.Stand(contained);
				}
				UpdateAppearance(uid, cryoPodComponent);
				return contained;
			}
		}
		return null;
	}

	protected void AddAlternativeVerbs(EntityUid uid, CryoPodComponent cryoPodComponent, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && cryoPodComponent.BodyContainer.ContainedEntity.HasValue)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("cryo-pod-verb-noun-occupant"),
				Category = VerbCategory.Eject,
				Priority = 1,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryEjectBody(uid, args.User, cryoPodComponent);
				}
			});
		}
	}

	protected void OnEmagged(EntityUid uid, CryoPodComponent? cryoPodComponent, ref GotEmaggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CryoPodComponent>(uid, ref cryoPodComponent, true) && _emag.CompareFlag(args.Type, EmagType.Interaction) && (!cryoPodComponent.PermaLocked || !cryoPodComponent.Locked))
		{
			cryoPodComponent.PermaLocked = true;
			cryoPodComponent.Locked = true;
			args.Handled = true;
		}
	}

	protected void OnCryoPodPryFinished(EntityUid uid, CryoPodComponent cryoPodComponent, CryoPodPryFinished args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid? ejected = EjectBody(uid, cryoPodComponent);
			if (ejected.HasValue)
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(18, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ejected.Value)), "ToPrettyString(ejected.Value)");
				handler.AppendLiteral(" pried out of ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
				handler.AppendLiteral(" by ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "ToPrettyString(args.User)");
				adminLogger.Add(LogType.Action, LogImpact.Medium, ref handler);
			}
		}
	}

	public virtual void InitializeInsideCryoPod()
	{
		((EntitySystem)this).SubscribeLocalEvent<InsideCryoPodComponent, DownAttemptEvent>((ComponentEventHandler<InsideCryoPodComponent, DownAttemptEvent>)HandleDown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InsideCryoPodComponent, EntGotRemovedFromContainerMessage>((ComponentEventHandler<InsideCryoPodComponent, EntGotRemovedFromContainerMessage>)OnEntGotRemovedFromContainer, (Type[])null, (Type[])null);
	}

	private void HandleDown(EntityUid uid, InsideCryoPodComponent component, DownAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnEntGotRemovedFromContainer(EntityUid uid, InsideCryoPodComponent component, EntGotRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Terminating(uid, (MetaDataComponent)null))
		{
			((EntitySystem)this).RemComp<InsideCryoPodComponent>(uid);
		}
	}
}
