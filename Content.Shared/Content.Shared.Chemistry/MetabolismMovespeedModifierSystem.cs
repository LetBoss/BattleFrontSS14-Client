using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Chemistry;

public sealed class MetabolismMovespeedModifierSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private MovementSpeedModifierSystem _movespeed;

	private readonly List<Entity<MovespeedModifierMetabolismComponent>> _components = new List<Entity<MovespeedModifierMetabolismComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<MovespeedModifierMetabolismComponent, ComponentStartup>((EntityEventRefHandler<MovespeedModifierMetabolismComponent, ComponentStartup>)AddComponent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovespeedModifierMetabolismComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<MovespeedModifierMetabolismComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovespeed, (Type[])null, (Type[])null);
	}

	private void OnRefreshMovespeed(EntityUid uid, MovespeedModifierMetabolismComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
	}

	private void AddComponent(Entity<MovespeedModifierMetabolismComponent> metabolism, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_components.Add(metabolism);
	}

	public override void Update(float frameTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan currentTime = _gameTiming.CurTime;
		for (int i = _components.Count - 1; i >= 0; i--)
		{
			Entity<MovespeedModifierMetabolismComponent> metabolism = _components[i];
			if (((Component)metabolism.Comp).Deleted)
			{
				_components.RemoveAt(i);
			}
			else if (!(metabolism.Comp.ModifierTimer > currentTime))
			{
				_components.RemoveAt(i);
				((EntitySystem)this).RemComp<MovespeedModifierMetabolismComponent>(Entity<MovespeedModifierMetabolismComponent>.op_Implicit(metabolism));
				_movespeed.RefreshMovementSpeedModifiers(Entity<MovespeedModifierMetabolismComponent>.op_Implicit(metabolism));
			}
		}
	}
}
