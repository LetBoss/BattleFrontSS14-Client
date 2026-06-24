using System;
using System.Collections.Generic;
using Content.Client.Implants.UI;
using Content.Client.Items;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Implants;

public sealed class ImplanterSystem : SharedImplanterSystem
{
	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private IPrototypeManager _proto;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<ImplanterComponent, AfterAutoHandleStateEvent>)OnHandleImplanterState, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.ItemStatus<ImplanterComponent>((Func<Entity<ImplanterComponent>, Control?>)((Entity<ImplanterComponent> ent) => (Control?)(object)new ImplanterStatusControl(Entity<ImplanterComponent>.op_Implicit(ent))));
	}

	private void OnHandleImplanterState(EntityUid uid, ImplanterComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		DeimplantBoundUserInterface deimplantBoundUserInterface = default(DeimplantBoundUserInterface);
		if (_uiSystem.TryGetOpenUi<DeimplantBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)DeimplantUiKey.Key, ref deimplantBoundUserInterface))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			EntityPrototype val = default(EntityPrototype);
			foreach (EntProtoId item in component.DeimplantWhitelist)
			{
				if (_proto.TryIndex(item, ref val))
				{
					dictionary.Add(val.ID, val.Name);
				}
			}
			DeimplantBoundUserInterface deimplantBoundUserInterface2 = deimplantBoundUserInterface;
			EntProtoId? deimplantChosen = component.DeimplantChosen;
			deimplantBoundUserInterface2.UpdateState(dictionary, deimplantChosen.HasValue ? EntProtoId.op_Implicit(deimplantChosen.GetValueOrDefault()) : null);
		}
		component.UiUpdateNeeded = true;
	}
}
