using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tools.Systems;

public sealed class WeldableSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedToolSystem _toolSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPhysicsSystem _physics;

	private EntityQuery<WeldableComponent> _query;

	public override void Initialize()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WeldableComponent, InteractUsingEvent>((ComponentEventHandler<WeldableComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeldableComponent, WeldFinishedEvent>((ComponentEventHandler<WeldableComponent, WeldFinishedEvent>)OnWeldFinished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LayerChangeOnWeldComponent, WeldableChangedEvent>((ComponentEventRefHandler<LayerChangeOnWeldComponent, WeldableChangedEvent>)OnWeldChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeldableComponent, ExaminedEvent>((ComponentEventHandler<WeldableComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		_query = ((EntitySystem)this).GetEntityQuery<WeldableComponent>();
	}

	public bool IsWelded(EntityUid uid, WeldableComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref component, false))
		{
			return component.IsWelded;
		}
		return false;
	}

	private void OnExamine(EntityUid uid, WeldableComponent component, ExaminedEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsWelded && component.WeldedExamineMessage.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? weldedExamineMessage = component.WeldedExamineMessage;
			args.PushText(loc.GetString(weldedExamineMessage.HasValue ? LocId.op_Implicit(weldedExamineMessage.GetValueOrDefault()) : null));
		}
	}

	private void OnInteractUsing(EntityUid uid, WeldableComponent component, InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryWeld(uid, args.Used, args.User, component);
		}
	}

	private bool CanWeld(EntityUid uid, EntityUid tool, EntityUid user, WeldableComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(uid, ref component, true))
		{
			return false;
		}
		WeldableAttemptEvent attempt = new WeldableAttemptEvent(user, tool);
		((EntitySystem)this).RaiseLocalEvent<WeldableAttemptEvent>(uid, attempt, false);
		if (((CancellableEntityEventArgs)attempt).Cancelled)
		{
			return false;
		}
		return true;
	}

	private bool TryWeld(EntityUid uid, EntityUid tool, EntityUid user, WeldableComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(uid, ref component, true))
		{
			return false;
		}
		if (!CanWeld(uid, tool, user, component))
		{
			return false;
		}
		if (!_toolSystem.UseTool(tool, user, uid, component.Time.Seconds, ProtoId<ToolQualityPrototype>.op_Implicit(component.WeldingQuality), new WeldFinishedEvent(), component.Fuel))
		{
			return false;
		}
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(16, 4);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
		handler.AppendLiteral(" is ");
		handler.AppendFormatted(component.IsWelded ? "un" : "");
		handler.AppendLiteral("welding ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
		handler.AppendLiteral(" at ");
		handler.AppendFormatted<EntityCoordinates>(((EntitySystem)this).Transform(uid).Coordinates, "targetlocation", "Transform(uid).Coordinates");
		adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		return true;
	}

	private void OnWeldFinished(EntityUid uid, WeldableComponent component, WeldFinishedEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && args.Used.HasValue && CanWeld(uid, args.Used.Value, args.User, component))
		{
			SetWeldedState(uid, !component.IsWelded, component);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(8, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" ");
			handler.AppendFormatted((!component.IsWelded) ? "un" : "");
			handler.AppendLiteral("welded ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
	}

	private void OnWeldChanged(EntityUid uid, LayerChangeOnWeldComponent component, ref WeldableChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtures = default(FixturesComponent);
		if (!((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixtures))
		{
			return;
		}
		foreach (var (id, fixture) in fixtures.Fixtures)
		{
			if (args.IsWelded)
			{
				if (fixture.CollisionLayer == (int)component.UnWeldedLayer)
				{
					_physics.SetCollisionLayer(uid, id, fixture, (int)component.WeldedLayer, (FixturesComponent)null, (PhysicsComponent)null);
				}
			}
			else if (fixture.CollisionLayer == (int)component.WeldedLayer)
			{
				_physics.SetCollisionLayer(uid, id, fixture, (int)component.UnWeldedLayer, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
	}

	private void UpdateAppearance(EntityUid uid, WeldableComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref component, true))
		{
			_appearance.SetData(uid, (Enum)WeldableVisuals.IsWelded, (object)component.IsWelded, (AppearanceComponent)null);
		}
	}

	public void SetWeldedState(EntityUid uid, bool state, WeldableComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref component, true) && component.IsWelded != state)
		{
			component.IsWelded = state;
			WeldableChangedEvent ev = new WeldableChangedEvent(component.IsWelded);
			((EntitySystem)this).RaiseLocalEvent<WeldableChangedEvent>(uid, ref ev, false);
			UpdateAppearance(uid, component);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void SetWeldingTime(EntityUid uid, TimeSpan time, WeldableComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref component, true) && !component.Time.Equals(time))
		{
			component.Time = time;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
