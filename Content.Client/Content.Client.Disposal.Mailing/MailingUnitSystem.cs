using System;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Mailing;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Disposal.Mailing;

public sealed class MailingUnitSystem : SharedMailingUnitSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MailingUnitComponent, AfterAutoHandleStateEvent>)OnMailingState, (Type[])null, (Type[])null);
	}

	private void OnMailingState(Entity<MailingUnitComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MailingUnitBoundUserInterface mailingUnitBoundUserInterface = default(MailingUnitBoundUserInterface);
		if (UserInterfaceSystem.TryGetOpenUi<MailingUnitBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)MailingUnitUiKey.Key, ref mailingUnitBoundUserInterface))
		{
			mailingUnitBoundUserInterface.Refresh(ent);
		}
	}
}
