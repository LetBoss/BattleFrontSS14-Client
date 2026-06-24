using System;
using Content.Shared._RMC14.Movement;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared.Clothing;

public sealed class ClothingSpeedModifierSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private ItemToggleSystem _toggle;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ClothingSpeedModifierComponent, ComponentGetState>((ComponentEventRefHandler<ClothingSpeedModifierComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSpeedModifierComponent, ComponentHandleState>((ComponentEventRefHandler<ClothingSpeedModifierComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSpeedModifierComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>((ComponentEventHandler<ClothingSpeedModifierComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>)OnRefreshMoveSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSpeedModifierComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<ClothingSpeedModifierComponent, GetVerbsEvent<ExamineVerb>>)OnClothingVerbExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSpeedModifierComponent, ItemToggledEvent>((EntityEventRefHandler<ClothingSpeedModifierComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, ClothingSpeedModifierComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new ClothingSpeedModifierComponentState(component.WalkModifier, component.SprintModifier);
	}

	private void OnHandleState(EntityUid uid, ClothingSpeedModifierComponent component, ref ComponentHandleState args)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is ClothingSpeedModifierComponentState state)
		{
			bool num = !MathHelper.CloseTo(component.SprintModifier, state.SprintModifier, 1E-07f) || !MathHelper.CloseTo(component.WalkModifier, state.WalkModifier, 1E-07f);
			component.WalkModifier = state.WalkModifier;
			component.SprintModifier = state.SprintModifier;
			BaseContainer container = default(BaseContainer);
			if (num && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container))
			{
				_movementSpeed.RefreshMovementSpeedModifiers(container.Owner);
			}
		}
	}

	private void OnRefreshMoveSpeed(EntityUid uid, ClothingSpeedModifierComponent component, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		RMCMovementSpeedRefreshedEvent ev = new RMCMovementSpeedRefreshedEvent(component.WalkModifier, component.SprintModifier);
		((EntitySystem)this).RaiseLocalEvent<RMCMovementSpeedRefreshedEvent>(uid, ref ev, false);
		float walkModifier = ev.WalkModifier;
		float sprintModifier = ev.SprintModifier;
		if (_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(uid)))
		{
			args.Args.ModifySpeed(walkModifier, sprintModifier);
		}
	}

	private void OnClothingVerbExamine(EntityUid uid, ClothingSpeedModifierComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		if (!args.CanInteract || !args.CanAccess)
		{
			return;
		}
		float walkModifierPercentage = MathF.Round((1f - component.WalkModifier) * 100f, 1);
		float sprintModifierPercentage = MathF.Round((1f - component.SprintModifier) * 100f, 1);
		if (walkModifierPercentage == 0f && sprintModifierPercentage == 0f)
		{
			return;
		}
		FormattedMessage msg = new FormattedMessage();
		if (MathHelper.CloseTo(walkModifierPercentage, sprintModifierPercentage, 0.5f))
		{
			if (walkModifierPercentage < 0f)
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("clothing-speed-increase-equal-examine", (ValueTuple<string, object>)("walkSpeed", (int)MathF.Abs(walkModifierPercentage)), (ValueTuple<string, object>)("runSpeed", (int)MathF.Abs(sprintModifierPercentage))));
			}
			else
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("clothing-speed-decrease-equal-examine", (ValueTuple<string, object>)("walkSpeed", (int)walkModifierPercentage), (ValueTuple<string, object>)("runSpeed", (int)sprintModifierPercentage)));
			}
		}
		else
		{
			if (sprintModifierPercentage < 0f)
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("clothing-speed-increase-run-examine", (ValueTuple<string, object>)("runSpeed", (int)MathF.Abs(sprintModifierPercentage))));
			}
			else if (sprintModifierPercentage > 0f)
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("clothing-speed-decrease-run-examine", (ValueTuple<string, object>)("runSpeed", (int)sprintModifierPercentage)));
			}
			if (walkModifierPercentage != 0f && sprintModifierPercentage != 0f)
			{
				msg.PushNewline();
			}
			if (walkModifierPercentage < 0f)
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("clothing-speed-increase-walk-examine", (ValueTuple<string, object>)("walkSpeed", (int)MathF.Abs(walkModifierPercentage))));
			}
			else if (walkModifierPercentage > 0f)
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("clothing-speed-decrease-walk-examine", (ValueTuple<string, object>)("walkSpeed", (int)walkModifierPercentage)));
			}
		}
		_examine.AddDetailedExamineVerb(args, (Component)(object)component, msg, base.Loc.GetString("clothing-speed-examinable-verb-text"), "/Textures/Interface/VerbIcons/outfit.svg.192dpi.png", base.Loc.GetString("clothing-speed-examinable-verb-message"));
	}

	private void OnToggled(Entity<ClothingSpeedModifierComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ClothingSpeedModifierComponent>.op_Implicit(ent));
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(ent.Owner, null, null)), ref container))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(container.Owner);
		}
	}
}
