using System;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Dialog;

public sealed class DialogUISystem : EntitySystem
{
	[Dependency]
	private RMCUserInterfaceSystem _rmcUI;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DialogComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<DialogComponent, AfterAutoHandleStateEvent>)OnDialogState, (Type[])null, (Type[])null);
	}

	private void OnDialogState(Entity<DialogComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_rmcUI.TryBui(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), delegate(DialogBui bui)
		{
			bui.Refresh();
		});
	}
}
