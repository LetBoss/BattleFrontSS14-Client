using System;
using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.IconSmoothing;

public sealed class ClientRandomIconSmoothSystem : SharedRandomIconSmoothSystem
{
	[Dependency]
	private IconSmoothSystem _iconSmooth;

	[Dependency]
	private AppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RandomIconSmoothComponent, AppearanceChangeEvent>((EntityEventRefHandler<RandomIconSmoothComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(Entity<RandomIconSmoothComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		IconSmoothComponent iconSmoothComponent = default(IconSmoothComponent);
		string text = default(string);
		if (((EntitySystem)this).TryComp<IconSmoothComponent>(Entity<RandomIconSmoothComponent>.op_Implicit(ent), ref iconSmoothComponent) && ((SharedAppearanceSystem)_appearance).TryGetData<string>(Entity<RandomIconSmoothComponent>.op_Implicit(ent), (Enum)RandomIconSmoothState.State, ref text, args.Component))
		{
			iconSmoothComponent.StateBase = text;
			_iconSmooth.SetStateBase(Entity<RandomIconSmoothComponent>.op_Implicit(ent), iconSmoothComponent, text);
		}
	}
}
