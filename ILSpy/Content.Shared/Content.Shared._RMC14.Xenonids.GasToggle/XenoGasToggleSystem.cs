using System;
using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.GasToggle;

public sealed class XenoGasToggleSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoGasToggleComponent, XenoGasToggleActionEvent>((EntityEventRefHandler<XenoGasToggleComponent, XenoGasToggleActionEvent>)OnToggleType, (Type[])null, (Type[])null);
	}

	private void OnToggleType(Entity<XenoGasToggleComponent> xeno, ref XenoGasToggleActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			xeno.Comp.IsNeurotoxin = !xeno.Comp.IsNeurotoxin;
			_actions.SetToggled(args.Action.AsNullable(), xeno.Comp.IsNeurotoxin);
			((EntitySystem)this).Dirty<XenoGasToggleComponent>(xeno, (MetaDataComponent)null);
		}
	}
}
