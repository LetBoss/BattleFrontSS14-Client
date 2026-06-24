using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Construction;
using Content.Shared.Administration.Logs;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Construction.EntitySystems;

public sealed class AnchorableSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class TryUnanchorCompletedEvent : SimpleDoAfterEvent, ISerializationGenerated<TryUnanchorCompletedEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref TryUnanchorCompletedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (TryUnanchorCompletedEvent)definitionCast;
			serialization.TryCustomCopy<TryUnanchorCompletedEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref TryUnanchorCompletedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			TryUnanchorCompletedEvent cast = (TryUnanchorCompletedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			TryUnanchorCompletedEvent cast = (TryUnanchorCompletedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override TryUnanchorCompletedEvent Instantiate()
		{
			return new TryUnanchorCompletedEvent();
		}
	}

	[Serializable]
	[NetSerializable]
	private sealed class TryAnchorCompletedEvent : SimpleDoAfterEvent, ISerializationGenerated<TryAnchorCompletedEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref TryAnchorCompletedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (TryAnchorCompletedEvent)definitionCast;
			serialization.TryCustomCopy<TryAnchorCompletedEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref TryAnchorCompletedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			TryAnchorCompletedEvent cast = (TryAnchorCompletedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			TryAnchorCompletedEvent cast = (TryAnchorCompletedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override TryAnchorCompletedEvent Instantiate()
		{
			return new TryAnchorCompletedEvent();
		}
	}

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	public readonly ProtoId<TagPrototype> Unstackable = ProtoId<TagPrototype>.op_Implicit("Unstackable");

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<AnchorableComponent, InteractUsingEvent>((ComponentEventHandler<AnchorableComponent, InteractUsingEvent>)OnInteractUsing, new Type[1] { typeof(ItemSlotsSystem) }, new Type[1] { typeof(SharedConstructionSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AnchorableComponent, TryAnchorCompletedEvent>((ComponentEventHandler<AnchorableComponent, TryAnchorCompletedEvent>)OnAnchorComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnchorableComponent, TryUnanchorCompletedEvent>((ComponentEventHandler<AnchorableComponent, TryUnanchorCompletedEvent>)OnUnanchorComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnchorableComponent, ExaminedEvent>((ComponentEventHandler<AnchorableComponent, ExaminedEvent>)OnAnchoredExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnchorableComponent, ComponentStartup>((ComponentEventHandler<AnchorableComponent, ComponentStartup>)OnAnchorStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnchorableComponent, AnchorStateChangedEvent>((ComponentEventHandler<AnchorableComponent, AnchorStateChangedEvent>)OnAnchorStateChange, (Type[])null, (Type[])null);
	}

	private void OnAnchorStartup(EntityUid uid, AnchorableComponent comp, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)AnchorVisuals.Anchored, (object)((EntitySystem)this).Transform(uid).Anchored, (AppearanceComponent)null);
	}

	private void OnAnchorStateChange(EntityUid uid, AnchorableComponent comp, AnchorStateChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)AnchorVisuals.Anchored, (object)((AnchorStateChangedEvent)(ref args)).Anchored, (AppearanceComponent)null);
	}

	private void TryUnAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent? anchorable = null, TransformComponent? transform = null, ToolComponent? usingTool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnchorableComponent, TransformComponent>(uid, ref anchorable, ref transform, true) && ((EntitySystem)this).Resolve<ToolComponent>(usingUid, ref usingTool, true) && Valid(uid, userUid, usingUid, anchoring: false))
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(29, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userUid)), "user", "ToPrettyString(userUid)");
			handler.AppendLiteral(" is trying to unanchor ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "targetlocation", "transform.Coordinates");
			adminLogger.Add(LogType.Anchor, LogImpact.Low, ref handler);
			_tool.UseTool(usingUid, userUid, uid, anchorable.Delay, (IEnumerable<string>)usingTool.Qualities, new TryUnanchorCompletedEvent());
		}
	}

	private void OnInteractUsing(EntityUid uid, AnchorableComponent anchorable, InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ToolComponent usedTool = default(ToolComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<ToolComponent>(args.Used, ref usedTool) && _tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(anchorable.Tool), usedTool))
		{
			((HandledEntityEventArgs)args).Handled = true;
			TryToggleAnchor(uid, args.User, args.Used, anchorable, null, null, usedTool);
		}
	}

	private void OnAnchoredExamine(EntityUid uid, AnchorableComponent component, ExaminedEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (component.Flags == AnchorableFlags.None)
		{
			args.PushMarkup(base.Loc.GetString("rmc-construction-non-anchorable"));
			return;
		}
		string messageId = (((EntitySystem)this).Comp<TransformComponent>(uid).Anchored ? "examinable-anchored" : "examinable-unanchored");
		args.PushMarkup(base.Loc.GetString(messageId, (ValueTuple<string, object>)("target", uid)));
	}

	private void OnUnanchorComplete(EntityUid uid, AnchorableComponent component, TryUnanchorCompletedEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid? used = args.Used;
			if (used.HasValue)
			{
				EntityUid used2 = used.GetValueOrDefault();
				TransformComponent xform = ((EntitySystem)this).Transform(uid);
				((EntitySystem)this).RaiseLocalEvent<BeforeUnanchoredEvent>(uid, new BeforeUnanchoredEvent(args.User, used2), false);
				_transformSystem.Unanchor(uid, xform, true);
				((EntitySystem)this).RaiseLocalEvent<UserUnanchoredEvent>(uid, new UserUnanchoredEvent(args.User, used2), false);
				_popup.PopupClient(base.Loc.GetString("anchorable-unanchored"), uid, args.User);
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(19, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
				handler.AppendLiteral(" unanchored ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "anchored", "ToPrettyString(uid)");
				handler.AppendLiteral(" using ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used2)), "using", "ToPrettyString(used)");
				adminLogger.Add(LogType.Unanchor, LogImpact.Low, ref handler);
			}
		}
	}

	private void OnAnchorComplete(EntityUid uid, AnchorableComponent component, TryAnchorCompletedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (!used.HasValue)
		{
			return;
		}
		EntityUid used2 = used.GetValueOrDefault();
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		PhysicsComponent anchorBody = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref anchorBody) && !TileFree(xform.Coordinates, anchorBody, uid))
		{
			_popup.PopupClient(base.Loc.GetString("anchorable-occupied"), uid, args.User);
			return;
		}
		Angle rot = xform.LocalRotation;
		xform.LocalRotation = Angle.op_Implicit(Math.Round(Angle.op_Implicit(rot) / (Math.PI / 2.0)) * (Math.PI / 2.0));
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(uid, ref pullable) && pullable.Puller.HasValue)
		{
			_pulling.TryStopPull(uid, pullable);
		}
		if (component.Snap)
		{
			EntityCoordinates coordinates = xform.Coordinates.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
			if (AnyUnstackable(uid, coordinates))
			{
				_popup.PopupClient(base.Loc.GetString("construction-step-condition-no-unstackable-in-tile"), uid, args.User);
				return;
			}
			_transformSystem.SetCoordinates(uid, coordinates);
		}
		((EntitySystem)this).RaiseLocalEvent<BeforeAnchoredEvent>(uid, new BeforeAnchoredEvent(args.User, used2), false);
		if (!xform.Anchored)
		{
			_transformSystem.AnchorEntity(uid, xform);
		}
		((EntitySystem)this).RaiseLocalEvent<UserAnchoredEvent>(uid, new UserAnchoredEvent(args.User, used2), false);
		_popup.PopupClient(base.Loc.GetString("anchorable-anchored"), uid, args.User);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(17, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
		handler.AppendLiteral(" anchored ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "anchored", "ToPrettyString(uid)");
		handler.AppendLiteral(" using ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used2)), "using", "ToPrettyString(used)");
		adminLogger.Add(LogType.Anchor, LogImpact.Low, ref handler);
	}

	public void TryToggleAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent? anchorable = null, TransformComponent? transform = null, PullableComponent? pullable = null, ToolComponent? usingTool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve(uid, ref transform, true))
		{
			if (transform.Anchored)
			{
				TryUnAnchor(uid, userUid, usingUid, anchorable, transform, usingTool);
			}
			else
			{
				TryAnchor(uid, userUid, usingUid, anchorable, transform, pullable, usingTool);
			}
		}
	}

	private void TryAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent? anchorable = null, TransformComponent? transform = null, PullableComponent? pullable = null, ToolComponent? usingTool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<AnchorableComponent, TransformComponent>(uid, ref anchorable, ref transform, true))
		{
			return;
		}
		((EntitySystem)this).Resolve<PullableComponent>(uid, ref pullable, false);
		if (((EntitySystem)this).Resolve<ToolComponent>(usingUid, ref usingTool, true) && Valid(uid, userUid, usingUid, anchoring: true, anchorable, usingTool))
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(25, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userUid)), "user", "ToPrettyString(userUid)");
			handler.AppendLiteral(" is trying to anchor ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "targetlocation", "transform.Coordinates");
			adminLogger.Add(LogType.Anchor, LogImpact.Low, ref handler);
			PhysicsComponent anchorBody = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref anchorBody) && !TileFree(transform.Coordinates, anchorBody, uid))
			{
				_popup.PopupClient(base.Loc.GetString("anchorable-occupied"), uid, userUid);
			}
			else if (AnyUnstackable(uid, transform.Coordinates))
			{
				_popup.PopupClient(base.Loc.GetString("construction-step-condition-no-unstackable-in-tile"), uid, userUid);
			}
			else
			{
				_tool.UseTool(usingUid, userUid, uid, anchorable.Delay, (IEnumerable<string>)usingTool.Qualities, new TryAnchorCompletedEvent());
			}
		}
	}

	private bool Valid(EntityUid uid, EntityUid userUid, EntityUid usingUid, bool anchoring, AnchorableComponent? anchorable = null, ToolComponent? usingTool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<AnchorableComponent>(uid, ref anchorable, true))
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<ToolComponent>(usingUid, ref usingTool, true))
		{
			return false;
		}
		if (anchoring && (anchorable.Flags & AnchorableFlags.Anchorable) == 0)
		{
			return false;
		}
		if (!anchoring && (anchorable.Flags & AnchorableFlags.Unanchorable) == 0)
		{
			return false;
		}
		BaseAnchoredAttemptEvent attempt = (anchoring ? ((BaseAnchoredAttemptEvent)new AnchorAttemptEvent(userUid, usingUid)) : ((BaseAnchoredAttemptEvent)new UnanchorAttemptEvent(userUid, usingUid)));
		if (anchoring)
		{
			((EntitySystem)this).RaiseLocalEvent<AnchorAttemptEvent>(uid, (AnchorAttemptEvent)attempt, false);
		}
		else
		{
			((EntitySystem)this).RaiseLocalEvent<UnanchorAttemptEvent>(uid, (UnanchorAttemptEvent)attempt, false);
		}
		anchorable.Delay += attempt.Delay;
		return !((CancellableEntityEventArgs)attempt).Cancelled;
	}

	public bool TileFree(EntityCoordinates coordinates, PhysicsComponent anchorBody, EntityUid? anchoringEntity = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gridUid = _transformSystem.GetGrid(coordinates);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid))
		{
			return false;
		}
		Vector2i tileIndices = _map.TileIndicesFor(Entity<MapGridComponent>.op_Implicit((gridUid.Value, grid)), coordinates);
		return TileFree(Entity<MapGridComponent>.op_Implicit((gridUid.Value, grid)), tileIndices, anchorBody.CollisionLayer, anchorBody.CollisionMask, anchoringEntity);
	}

	public bool TileFree(Entity<MapGridComponent> grid, Vector2i gridIndices, int collisionLayer = 0, int collisionMask = 0, EntityUid? anchoringEntity = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		AnchoredEntitiesEnumerator enumerator = _map.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), grid.Comp, gridIndices);
		EntityUid? ent = default(EntityUid?);
		PhysicsComponent body = default(PhysicsComponent);
		while (((AnchoredEntitiesEnumerator)(ref enumerator)).MoveNext(ref ent))
		{
			if (!_physicsQuery.TryGetComponent(ent, ref body) || !body.CanCollide || !body.Hard || ((body.CollisionMask & collisionLayer) == 0 && (body.CollisionLayer & collisionMask) == 0))
			{
				continue;
			}
			if (anchoringEntity.HasValue)
			{
				RMCCheckTileFreeEvent ev = new RMCCheckTileFreeEvent(ent.Value);
				((EntitySystem)this).RaiseLocalEvent<RMCCheckTileFreeEvent>(anchoringEntity.Value, ref ev, false);
				if (ev.IsTileFree)
				{
					continue;
				}
			}
			return false;
		}
		return true;
	}

	[Obsolete("Use the Entity<MapGridComponent> version")]
	public bool TileFree(MapGridComponent grid, Vector2i gridIndices, int collisionLayer = 0, int collisionMask = 0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return TileFree(Entity<MapGridComponent>.op_Implicit((((Component)grid).Owner, grid)), gridIndices, collisionLayer, collisionMask);
	}

	public bool AnyUnstackable(EntityUid uid, EntityCoordinates location)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (_tagSystem.HasTag(uid, Unstackable))
		{
			return AnyUnstackablesAnchoredAt(location);
		}
		return false;
	}

	public bool AnyUnstackablesAnchoredAt(EntityCoordinates location)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gridUid = _transformSystem.GetGrid(location);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid))
		{
			return false;
		}
		AnchoredEntitiesEnumerator enumerator = _map.GetAnchoredEntitiesEnumerator(gridUid.Value, grid, _map.LocalToTile(gridUid.Value, grid, location));
		EntityUid? entity = default(EntityUid?);
		while (((AnchoredEntitiesEnumerator)(ref enumerator)).MoveNext(ref entity))
		{
			if (_tagSystem.HasTag(entity.Value, Unstackable))
			{
				return true;
			}
		}
		return false;
	}
}
