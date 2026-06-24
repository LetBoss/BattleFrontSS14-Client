using System;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Fruit.Events;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Fruit;

public sealed class XenoFruitPlanterVisualsSystem : EntitySystem
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoFruitPlanterVisualsComponent, MobStateChangedEvent>)OnVisualsMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, XenoRestEvent>((EntityEventRefHandler<XenoFruitPlanterVisualsComponent, XenoRestEvent>)OnVisualsRest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, XenoFruitPlanterVisualsChangedEvent>((EntityEventRefHandler<XenoFruitPlanterVisualsComponent, XenoFruitPlanterVisualsChangedEvent>)OnVisualsFruitChanged, (Type[])null, (Type[])null);
	}

	private void OnVisualsMobStateChanged(Entity<XenoFruitPlanterVisualsComponent> xeno, ref MobStateChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<XenoFruitPlanterVisualsComponent>.op_Implicit(xeno), (Enum)XenoFruitPlanterVisuals.Downed, (object)(args.NewMobState != MobState.Alive), (AppearanceComponent)null);
	}

	private void OnVisualsRest(Entity<XenoFruitPlanterVisualsComponent> xeno, ref XenoRestEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<XenoFruitPlanterVisualsComponent>.op_Implicit(xeno), (Enum)XenoFruitPlanterVisuals.Resting, (object)args.Resting, (AppearanceComponent)null);
	}

	private void OnVisualsFruitChanged(Entity<XenoFruitPlanterVisualsComponent> xeno, ref XenoFruitPlanterVisualsChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitComponent comp = default(XenoFruitComponent);
		if (args.Choice.TryGet(ref comp, _prototype, _compFactory))
		{
			Color? color = comp.Color;
			if (color.HasValue)
			{
				Color color2 = color.GetValueOrDefault();
				_appearance.SetData(Entity<XenoFruitPlanterVisualsComponent>.op_Implicit(xeno), (Enum)XenoFruitPlanterVisuals.Color, (object)color2, (AppearanceComponent)null);
			}
		}
	}
}
