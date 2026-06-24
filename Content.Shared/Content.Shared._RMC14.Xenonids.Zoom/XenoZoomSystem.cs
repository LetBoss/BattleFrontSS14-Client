using System;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Camera;
using Content.Shared.DoAfter;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Xenonids.Zoom;

public sealed class XenoZoomSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedContentEyeSystem _contentEye;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoZoomComponent, XenoZoomActionEvent>((EntityEventRefHandler<XenoZoomComponent, XenoZoomActionEvent>)OnXenoZoomAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoZoomComponent, XenoZoomDoAfterEvent>((EntityEventRefHandler<XenoZoomComponent, XenoZoomDoAfterEvent>)OnXenoZoomDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoZoomComponent, GetEyeOffsetEvent>((EntityEventRefHandler<XenoZoomComponent, GetEyeOffsetEvent>)OnXenoZoomGetEyeOffset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoZoomComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoZoomComponent, RefreshMovementSpeedModifiersEvent>)OnXenoZoomRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoZoomComponent, XenoLeapAttemptEvent>((EntityEventRefHandler<XenoZoomComponent, XenoLeapAttemptEvent>)OnLeapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoZoomComponent, XenoRestEvent>((EntityEventRefHandler<XenoZoomComponent, XenoRestEvent>)OnRest, (Type[])null, (Type[])null);
	}

	private void OnXenoZoomAction(Entity<XenoZoomComponent> xeno, ref XenoZoomActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		XenoZoomDoAfterEvent ev = new XenoZoomDoAfterEvent();
		TimeSpan delay = (xeno.Comp.Enabled ? TimeSpan.Zero : xeno.Comp.DoAfter);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoZoomComponent>.op_Implicit(xeno), delay, ev, Entity<XenoZoomComponent>.op_Implicit(xeno))
		{
			BreakOnMove = true
		};
		_doAfter.TryStartDoAfter(doAfter);
	}

	private void OnXenoZoomDoAfter(Entity<XenoZoomComponent> xeno, ref XenoZoomDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		xeno.Comp.Enabled = !xeno.Comp.Enabled;
		if (xeno.Comp.Enabled)
		{
			_contentEye.SetMaxZoom(Entity<XenoZoomComponent>.op_Implicit(xeno), xeno.Comp.Zoom);
			_contentEye.SetZoom(Entity<XenoZoomComponent>.op_Implicit(xeno), xeno.Comp.Zoom);
			XenoZoomComponent comp = xeno.Comp;
			Angle localRotation = ((EntitySystem)this).Transform(args.User).LocalRotation;
			comp.Offset = DirectionExtensions.ToVec(((Angle)(ref localRotation)).GetCardinalDir()) * xeno.Comp.OffsetLength;
		}
		else
		{
			_contentEye.ResetZoom(Entity<XenoZoomComponent>.op_Implicit(xeno));
			xeno.Comp.Offset = Vector2.Zero;
		}
		((EntitySystem)this).Dirty<XenoZoomComponent>(xeno, (MetaDataComponent)null);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoZoomActionEvent>(Entity<XenoZoomComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), xeno.Comp.Enabled);
		}
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoZoomComponent>.op_Implicit(xeno));
		EyeComponent eye = default(EyeComponent);
		if (((EntitySystem)this).TryComp<EyeComponent>(Entity<XenoZoomComponent>.op_Implicit(xeno), ref eye))
		{
			_contentEye.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((xeno.Owner, eye)));
		}
	}

	private void OnXenoZoomGetEyeOffset(Entity<XenoZoomComponent> ent, ref GetEyeOffsetEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Offset += ent.Comp.Offset;
	}

	private void OnXenoZoomRefreshSpeed(Entity<XenoZoomComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled)
		{
			args.ModifySpeed(ent.Comp.Speed, ent.Comp.Speed);
		}
	}

	private void OnLeapAttempt(Entity<XenoZoomComponent> ent, ref XenoLeapAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled && ent.Comp.BlockLeaps)
		{
			args.Cancelled = true;
		}
	}

	private void OnRest(Entity<XenoZoomComponent> ent, ref XenoRestEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Enabled)
		{
			return;
		}
		ent.Comp.Enabled = false;
		_contentEye.ResetZoom(Entity<XenoZoomComponent>.op_Implicit(ent));
		ent.Comp.Offset = Vector2.Zero;
		((EntitySystem)this).Dirty<XenoZoomComponent>(ent, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoZoomComponent>.op_Implicit(ent));
		EyeComponent eye = default(EyeComponent);
		if (((EntitySystem)this).TryComp<EyeComponent>(Entity<XenoZoomComponent>.op_Implicit(ent), ref eye))
		{
			_contentEye.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((ent.Owner, eye)));
		}
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoZoomActionEvent>(Entity<XenoZoomComponent>.op_Implicit(ent)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), ent.Comp.Enabled);
		}
	}
}
