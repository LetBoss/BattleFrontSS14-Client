using System;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Item;

public sealed class HeldSpeedModifierSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HeldSpeedModifierComponent, GotEquippedHandEvent>((EntityEventRefHandler<HeldSpeedModifierComponent, GotEquippedHandEvent>)OnGotEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeldSpeedModifierComponent, GotUnequippedHandEvent>((EntityEventRefHandler<HeldSpeedModifierComponent, GotUnequippedHandEvent>)OnGotUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeldSpeedModifierComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>((ComponentEventHandler<HeldSpeedModifierComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>)OnRefreshMovementSpeedModifiers, (Type[])null, (Type[])null);
	}

	private void OnGotEquippedHand(Entity<HeldSpeedModifierComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
	}

	private void OnGotUnequippedHand(Entity<HeldSpeedModifierComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
	}

	public (float, float) GetHeldMovementSpeedModifiers(EntityUid uid, HeldSpeedModifierComponent component)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		float walkMod = component.WalkModifier;
		float sprintMod = component.SprintModifier;
		ClothingSpeedModifierComponent clothingSpeedModifier = default(ClothingSpeedModifierComponent);
		if (component.MirrorClothingModifier && ((EntitySystem)this).TryComp<ClothingSpeedModifierComponent>(uid, ref clothingSpeedModifier))
		{
			walkMod = clothingSpeedModifier.WalkModifier;
			sprintMod = clothingSpeedModifier.SprintModifier;
		}
		return (walkMod, sprintMod);
	}

	private void OnRefreshMovementSpeedModifiers(EntityUid uid, HeldSpeedModifierComponent component, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		var (walkMod, sprintMod) = GetHeldMovementSpeedModifiers(uid, component);
		args.Args.ModifySpeed(walkMod, sprintMod);
	}
}
