using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared.Pinpointer;

public abstract class SharedPinpointerSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private EmagSystem _emag;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PinpointerComponent, GotEmaggedEvent>((ComponentEventRefHandler<PinpointerComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PinpointerComponent, AfterInteractEvent>((ComponentEventHandler<PinpointerComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PinpointerComponent, ExaminedEvent>((ComponentEventHandler<PinpointerComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(EntityUid uid, PinpointerComponent component, AfterInteractEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		target.GetValueOrDefault();
		if (component.CanRetarget && !component.IsActive)
		{
			((HandledEntityEventArgs)args).Handled = true;
			component.Target = args.Target;
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(19, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" set target of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "pinpointer", "ToPrettyString(uid)");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(component.Target.Value)), "target", "ToPrettyString(component.Target.Value)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
			if (component.UpdateTargetName)
			{
				component.TargetName = ((!component.Target.HasValue) ? null : ((string)Identity.Name(component.Target.Value, (IEntityManager)(object)base.EntityManager)));
			}
		}
	}

	public virtual void SetTarget(EntityUid uid, EntityUid? target, PinpointerComponent? pinpointer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PinpointerComponent>(uid, ref pinpointer, true))
		{
			return;
		}
		EntityUid? target2 = pinpointer.Target;
		EntityUid? val = target;
		if (target2.HasValue != val.HasValue || (target2.HasValue && !(target2.GetValueOrDefault() == val.GetValueOrDefault())))
		{
			pinpointer.Target = target;
			if (pinpointer.UpdateTargetName)
			{
				pinpointer.TargetName = ((!target.HasValue) ? null : ((string)Identity.Name(target.Value, (IEntityManager)(object)base.EntityManager)));
			}
			if (pinpointer.IsActive)
			{
				UpdateDirectionToTarget(uid, pinpointer);
			}
		}
	}

	protected virtual void UpdateDirectionToTarget(EntityUid uid, PinpointerComponent? pinpointer = null)
	{
	}

	private void OnExamined(EntityUid uid, PinpointerComponent component, ExaminedEvent args)
	{
		if (args.IsInDetailsRange && component.TargetName != null)
		{
			args.PushMarkup(base.Loc.GetString("examine-pinpointer-linked", (ValueTuple<string, object>)("target", component.TargetName)));
		}
	}

	public void SetDistance(EntityUid uid, Distance distance, PinpointerComponent? pinpointer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PinpointerComponent>(uid, ref pinpointer, true) && distance != pinpointer.DistanceToTarget)
		{
			pinpointer.DistanceToTarget = distance;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)pinpointer, (MetaDataComponent)null);
		}
	}

	public bool TrySetArrowAngle(EntityUid uid, Angle arrowAngle, PinpointerComponent? pinpointer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PinpointerComponent>(uid, ref pinpointer, true))
		{
			return false;
		}
		if (((Angle)(ref pinpointer.ArrowAngle)).EqualsApprox(arrowAngle, pinpointer.Precision))
		{
			return false;
		}
		pinpointer.ArrowAngle = arrowAngle;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)pinpointer, (MetaDataComponent)null);
		return true;
	}

	public void SetActive(EntityUid uid, bool isActive, PinpointerComponent? pinpointer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PinpointerComponent>(uid, ref pinpointer, true) && isActive != pinpointer.IsActive)
		{
			pinpointer.IsActive = isActive;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)pinpointer, (MetaDataComponent)null);
		}
	}

	public virtual bool TogglePinpointer(EntityUid uid, PinpointerComponent? pinpointer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PinpointerComponent>(uid, ref pinpointer, true))
		{
			return false;
		}
		bool isActive = !pinpointer.IsActive;
		SetActive(uid, isActive, pinpointer);
		return isActive;
	}

	private void OnEmagged(EntityUid uid, PinpointerComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(uid, EmagType.Interaction) && !component.CanRetarget)
		{
			args.Handled = true;
			component.CanRetarget = true;
		}
	}
}
