using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.StatusEffect;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class EyeProtectionSystem : EntitySystem
{
	[Dependency]
	private StatusEffectsSystem _statusEffectsSystem;

	[Dependency]
	private BlindableSystem _blindingSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RequiresEyeProtectionComponent, ToolUseAttemptEvent>((ComponentEventHandler<RequiresEyeProtectionComponent, ToolUseAttemptEvent>)OnUseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequiresEyeProtectionComponent, ItemToggledEvent>((ComponentEventHandler<RequiresEyeProtectionComponent, ItemToggledEvent>)OnWelderToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeProtectionComponent, GetEyeProtectionEvent>((ComponentEventHandler<EyeProtectionComponent, GetEyeProtectionEvent>)OnGetProtection, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeProtectionComponent, InventoryRelayedEvent<GetEyeProtectionEvent>>((ComponentEventHandler<EyeProtectionComponent, InventoryRelayedEvent<GetEyeProtectionEvent>>)OnGetRelayedProtection, (Type[])null, (Type[])null);
	}

	private void OnGetRelayedProtection(EntityUid uid, EyeProtectionComponent component, InventoryRelayedEvent<GetEyeProtectionEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnGetProtection(uid, component, args.Args);
	}

	private void OnGetProtection(EntityUid uid, EyeProtectionComponent component, GetEyeProtectionEvent args)
	{
		args.Protection += component.ProtectionTime;
	}

	private void OnUseAttempt(EntityUid uid, RequiresEyeProtectionComponent component, ToolUseAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		BlindableComponent blindable = default(BlindableComponent);
		if (component.Toggled && ((EntitySystem)this).TryComp<BlindableComponent>(args.User, ref blindable) && !blindable.IsBlind)
		{
			GetEyeProtectionEvent ev = new GetEyeProtectionEvent();
			((EntitySystem)this).RaiseLocalEvent<GetEyeProtectionEvent>(args.User, ev, false);
			float time = (float)(component.StatusEffectTime - ev.Protection).TotalSeconds;
			if (!(time <= 0f))
			{
				_blindingSystem.AdjustEyeDamage(Entity<BlindableComponent>.op_Implicit((args.User, blindable)), 1);
				TimeSpan statusTimeSpan = TimeSpan.FromSeconds(time * MathF.Sqrt(blindable.EyeDamage));
				_statusEffectsSystem.TryAddStatusEffect(args.User, ProtoId<StatusEffectPrototype>.op_Implicit(TemporaryBlindnessSystem.BlindingStatusEffect), statusTimeSpan, refresh: false, ProtoId<StatusEffectPrototype>.op_Implicit(TemporaryBlindnessSystem.BlindingStatusEffect));
			}
		}
	}

	private void OnWelderToggled(EntityUid uid, RequiresEyeProtectionComponent component, ItemToggledEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		component.Toggled = args.Activated;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
