using System;
using Content.Shared.Emag.Systems;
using Content.Shared.Popups;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Singularity.EntitySystems;

public abstract class SharedSingularityGeneratorSystem : EntitySystem
{
	[Dependency]
	protected SharedPopupSystem PopupSystem;

	[Dependency]
	private EmagSystem _emag;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SingularityGeneratorComponent, GotEmaggedEvent>((ComponentEventRefHandler<SingularityGeneratorComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
	}

	private void OnEmagged(EntityUid uid, SingularityGeneratorComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(uid, EmagType.Interaction) && !component.FailsafeDisabled)
		{
			component.FailsafeDisabled = true;
			args.Handled = true;
		}
	}
}
