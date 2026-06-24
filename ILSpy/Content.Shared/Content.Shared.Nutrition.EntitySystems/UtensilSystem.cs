using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Nutrition.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tools.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class UtensilSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private FoodSystem _foodSystem;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private IRobustRandom _robustRandom;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UtensilComponent, AfterInteractEvent>((EntityEventRefHandler<UtensilComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, new Type[2]
		{
			typeof(ItemSlotsSystem),
			typeof(ToolOpenableSystem)
		});
	}

	private void OnAfterInteract(Entity<UtensilComponent> entity, ref AfterInteractEvent ev)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled && ev.Target.HasValue && ev.CanReach)
		{
			(bool, bool) result = TryUseUtensil(ev.User, ev.Target.Value, entity);
			((HandledEntityEventArgs)ev).Handled = result.Item2;
		}
	}

	public (bool Success, bool Handled) TryUseUtensil(EntityUid user, EntityUid target, Entity<UtensilComponent> utensil)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		FoodComponent food = default(FoodComponent);
		if (!((EntitySystem)this).TryComp<FoodComponent>(target, ref food))
		{
			return (Success: false, Handled: false);
		}
		if ((food.Utensil & utensil.Comp.Types) == 0)
		{
			_popupSystem.PopupClient(base.Loc.GetString("food-system-wrong-utensil", (ValueTuple<string, object>)("food", target), (ValueTuple<string, object>)("utensil", utensil.Owner)), user, user);
			return (Success: false, Handled: true);
		}
		if (!_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return (Success: false, Handled: true);
		}
		return _foodSystem.TryFeed(user, user, target, food);
	}

	public void TryBreak(EntityUid uid, EntityUid userUid, UtensilComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<UtensilComponent>(uid, ref component, true) && RandomExtensions.Prob(_robustRandom, component.BreakChance))
		{
			_audio.PlayPredicted(component.BreakSound, userUid, (EntityUid?)userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-2f));
			((EntitySystem)this).Del((EntityUid?)uid);
		}
	}
}
