using System;
using System.Linq;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Crest;

public sealed class XenoCrestSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private CMArmorSystem _armor;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, XenoToggleCrestActionEvent>((EntityEventRefHandler<XenoCrestComponent, XenoToggleCrestActionEvent>)OnXenoCrestAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoCrestComponent, RefreshMovementSpeedModifiersEvent>)OnXenoCrestRefreshMovementSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, CMGetArmorEvent>((EntityEventRefHandler<XenoCrestComponent, CMGetArmorEvent>)OnXenoCrestGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, BeforeStatusEffectAddedEvent>((EntityEventRefHandler<XenoCrestComponent, BeforeStatusEffectAddedEvent>)OnXenoCrestBeforeStatusAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, XenoFortifyAttemptEvent>((EntityEventRefHandler<XenoCrestComponent, XenoFortifyAttemptEvent>)OnXenoCrestFortifyAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, XenoTailSweepAttemptEvent>((EntityEventRefHandler<XenoCrestComponent, XenoTailSweepAttemptEvent>)OnXenoCrestTailSweepAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCrestComponent, XenoRestAttemptEvent>((EntityEventRefHandler<XenoCrestComponent, XenoRestAttemptEvent>)OnXenoCrestRestAttempt, (Type[])null, (Type[])null);
	}

	private void OnXenoCrestAction(Entity<XenoCrestComponent> xeno, ref XenoToggleCrestActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoToggleCrestAttemptEvent attempt = default(XenoToggleCrestAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoToggleCrestAttemptEvent>(Entity<XenoCrestComponent>.op_Implicit(xeno), ref attempt, false);
		if (attempt.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		RMCSizeComponent size = default(RMCSizeComponent);
		if (((EntitySystem)this).TryComp<RMCSizeComponent>(Entity<XenoCrestComponent>.op_Implicit(xeno), ref size))
		{
			if (!xeno.Comp.Lowered)
			{
				xeno.Comp.OriginalSize = size.Size;
				size.Size = xeno.Comp.CrestSize;
			}
			else
			{
				size.Size = xeno.Comp.OriginalSize ?? RMCSizes.Xeno;
			}
			((EntitySystem)this).Dirty(xeno.Owner, (IComponent)(object)size, (MetaDataComponent)null);
		}
		xeno.Comp.Lowered = !xeno.Comp.Lowered;
		((EntitySystem)this).Dirty<XenoCrestComponent>(xeno, (MetaDataComponent)null);
		_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((ValueTuple<EntityUid, CMArmorComponent>)(Entity<XenoCrestComponent>.op_Implicit(xeno), null)));
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoCrestComponent>.op_Implicit(xeno));
		_appearance.SetData(Entity<XenoCrestComponent>.op_Implicit(xeno), (Enum)XenoVisualLayers.Crest, (object)xeno.Comp.Lowered, (AppearanceComponent)null);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoToggleCrestActionEvent>(Entity<XenoCrestComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), xeno.Comp.Lowered);
		}
	}

	private void OnXenoCrestRefreshMovementSpeed(Entity<XenoCrestComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Lowered)
		{
			args.ModifySpeed(xeno.Comp.SpeedMultiplier, xeno.Comp.SpeedMultiplier);
		}
	}

	private void OnXenoCrestGetArmor(Entity<XenoCrestComponent> xeno, ref CMGetArmorEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Lowered)
		{
			args.XenoArmor += xeno.Comp.Armor;
		}
	}

	private void OnXenoCrestBeforeStatusAdded(Entity<XenoCrestComponent> xeno, ref BeforeStatusEffectAddedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Lowered)
		{
			string[] immuneToStatuses = xeno.Comp.ImmuneToStatuses;
			EntProtoId effect = args.Effect;
			if (Enumerable.Contains(immuneToStatuses, ((EntProtoId)(ref effect)).Id))
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnXenoCrestFortifyAttempt(Entity<XenoCrestComponent> xeno, ref XenoFortifyAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Lowered)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-toggle-crest-cant-fortify"), Entity<XenoCrestComponent>.op_Implicit(xeno), Entity<XenoCrestComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}

	private void OnXenoCrestTailSweepAttempt(Entity<XenoCrestComponent> xeno, ref XenoTailSweepAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Lowered)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-toggle-crest-cant-tail-sweep"), Entity<XenoCrestComponent>.op_Implicit(xeno), Entity<XenoCrestComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}

	private void OnXenoCrestRestAttempt(Entity<XenoCrestComponent> xeno, ref XenoRestAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Lowered)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-toggle-crest-cant-rest"), Entity<XenoCrestComponent>.op_Implicit(xeno), Entity<XenoCrestComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}
}
