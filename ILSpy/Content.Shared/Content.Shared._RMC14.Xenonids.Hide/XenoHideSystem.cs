using System;
using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.Hide;

public sealed class XenoHideSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoHideComponent, XenoHideActionEvent>((EntityEventRefHandler<XenoHideComponent, XenoHideActionEvent>)OnXenoHideAction, (Type[])null, (Type[])null);
	}

	private void OnXenoHideAction(Entity<XenoHideComponent> xeno, ref XenoHideActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		xeno.Comp.Hiding = !xeno.Comp.Hiding;
		((EntitySystem)this).Dirty<XenoHideComponent>(xeno, (MetaDataComponent)null);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoHideActionEvent>(Entity<XenoHideComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), xeno.Comp.Hiding);
		}
		_appearance.SetData(Entity<XenoHideComponent>.op_Implicit(xeno), (Enum)XenoVisualLayers.Hide, (object)xeno.Comp.Hiding, (AppearanceComponent)null);
	}
}
