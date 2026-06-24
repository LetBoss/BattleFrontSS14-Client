using System;
using System.Collections.Generic;
using Content.Shared.Interaction;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Shared.Weapons.Melee.EnergySword;

public sealed class EnergySwordSystem : EntitySystem
{
	[Dependency]
	private SharedRgbLightControllerSystem _rgbSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedToolSystem _toolSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EnergySwordComponent, MapInitEvent>((EntityEventRefHandler<EnergySwordComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnergySwordComponent, InteractUsingEvent>((EntityEventRefHandler<EnergySwordComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<EnergySwordComponent> entity, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.ColorOptions.Count != 0)
		{
			entity.Comp.ActivatedColor = RandomExtensions.Pick<Color>(_random, (IReadOnlyList<Color>)entity.Comp.ColorOptions);
			((EntitySystem)this).Dirty<EnergySwordComponent>(entity, (MetaDataComponent)null);
		}
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<EnergySwordComponent>.op_Implicit(entity), ref appearanceComponent))
		{
			_appearance.SetData(Entity<EnergySwordComponent>.op_Implicit(entity), (Enum)ToggleableVisuals.Color, (object)entity.Comp.ActivatedColor, appearanceComponent);
		}
	}

	private void OnInteractUsing(Entity<EnergySwordComponent> entity, ref InteractUsingEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _toolSystem.HasQuality(args.Used, "Pulsing"))
		{
			((HandledEntityEventArgs)args).Handled = true;
			entity.Comp.Hacked = !entity.Comp.Hacked;
			if (entity.Comp.Hacked)
			{
				RgbLightControllerComponent rgb = ((EntitySystem)this).EnsureComp<RgbLightControllerComponent>(Entity<EnergySwordComponent>.op_Implicit(entity));
				_rgbSystem.SetCycleRate(Entity<EnergySwordComponent>.op_Implicit(entity), entity.Comp.CycleRate, rgb);
			}
			else
			{
				((EntitySystem)this).RemComp<RgbLightControllerComponent>(Entity<EnergySwordComponent>.op_Implicit(entity));
			}
			((EntitySystem)this).Dirty<EnergySwordComponent>(entity, (MetaDataComponent)null);
		}
	}
}
