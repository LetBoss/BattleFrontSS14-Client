using System;
using Content.Shared.Access.Systems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class BatteryWeaponFireModesSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private AccessReaderSystem _accessReaderSystem;

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BatteryWeaponFireModesComponent, UseInHandEvent>((ComponentEventHandler<BatteryWeaponFireModesComponent, UseInHandEvent>)OnUseInHandEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BatteryWeaponFireModesComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<BatteryWeaponFireModesComponent, GetVerbsEvent<Verb>>)OnGetVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BatteryWeaponFireModesComponent, ExaminedEvent>((ComponentEventHandler<BatteryWeaponFireModesComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(EntityUid uid, BatteryWeaponFireModesComponent component, ExaminedEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (component.FireModes.Count >= 2)
		{
			BatteryWeaponFireMode fireMode = GetMode(component);
			EntityPrototype proto = default(EntityPrototype);
			if (_prototypeManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(fireMode.Prototype), ref proto))
			{
				args.PushMarkup(base.Loc.GetString("gun-set-fire-mode", (ValueTuple<string, object>)("mode", proto.Name)));
			}
		}
	}

	private BatteryWeaponFireMode GetMode(BatteryWeaponFireModesComponent component)
	{
		return component.FireModes[component.CurrentFireMode];
	}

	private void OnGetVerb(EntityUid uid, BatteryWeaponFireModesComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || component.FireModes.Count < 2 || !_accessReaderSystem.IsAllowed(args.User, uid))
		{
			return;
		}
		for (int i = 0; i < component.FireModes.Count; i++)
		{
			BatteryWeaponFireMode fireMode = component.FireModes[i];
			EntityPrototype entProto = _prototypeManager.Index<EntityPrototype>(EntProtoId.op_Implicit(fireMode.Prototype));
			int index = i;
			Verb v = new Verb
			{
				Priority = 1,
				Category = VerbCategory.SelectType,
				Text = entProto.Name,
				Disabled = (i == component.CurrentFireMode),
				Impact = LogImpact.Medium,
				DoContactInteraction = true,
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					TrySetFireMode(uid, component, index, args.User);
				}
			};
			args.Verbs.Add(v);
		}
	}

	private void OnUseInHandEvent(EntityUid uid, BatteryWeaponFireModesComponent component, UseInHandEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			TryCycleFireMode(uid, component, args.User);
		}
	}

	public void TryCycleFireMode(EntityUid uid, BatteryWeaponFireModesComponent component, EntityUid? user = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (component.FireModes.Count >= 2)
		{
			int index = (component.CurrentFireMode + 1) % component.FireModes.Count;
			TrySetFireMode(uid, component, index, user);
		}
	}

	public bool TrySetFireMode(EntityUid uid, BatteryWeaponFireModesComponent component, int index, EntityUid? user = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index >= component.FireModes.Count)
		{
			return false;
		}
		if (user.HasValue && !_accessReaderSystem.IsAllowed(user.Value, uid))
		{
			return false;
		}
		SetFireMode(uid, component, index, user);
		return true;
	}

	private void SetFireMode(EntityUid uid, BatteryWeaponFireModesComponent component, int index, EntityUid? user = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		BatteryWeaponFireMode fireMode = component.FireModes[index];
		component.CurrentFireMode = index;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		EntityPrototype prototype = default(EntityPrototype);
		if (_prototypeManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(fireMode.Prototype), ref prototype))
		{
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
			{
				_appearanceSystem.SetData(uid, (Enum)BatteryWeaponFireModeVisuals.State, (object)prototype.ID, appearance);
			}
			if (user.HasValue)
			{
				_popupSystem.PopupClient(base.Loc.GetString("gun-set-fire-mode", (ValueTuple<string, object>)("mode", prototype.Name)), uid, user.Value);
			}
		}
		ProjectileBatteryAmmoProviderComponent projectileBatteryAmmoProviderComponent = default(ProjectileBatteryAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<ProjectileBatteryAmmoProviderComponent>(uid, ref projectileBatteryAmmoProviderComponent))
		{
			float OldFireCost = projectileBatteryAmmoProviderComponent.FireCost;
			projectileBatteryAmmoProviderComponent.Prototype = EntProtoId.op_Implicit(fireMode.Prototype);
			projectileBatteryAmmoProviderComponent.FireCost = fireMode.FireCost;
			float FireCostDiff = fireMode.FireCost / OldFireCost;
			projectileBatteryAmmoProviderComponent.Shots = (int)Math.Round((float)projectileBatteryAmmoProviderComponent.Shots / FireCostDiff);
			projectileBatteryAmmoProviderComponent.Capacity = (int)Math.Round((float)projectileBatteryAmmoProviderComponent.Capacity / FireCostDiff);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)projectileBatteryAmmoProviderComponent, (MetaDataComponent)null);
			UpdateClientAmmoEvent updateClientAmmoEvent = default(UpdateClientAmmoEvent);
			((EntitySystem)this).RaiseLocalEvent<UpdateClientAmmoEvent>(uid, ref updateClientAmmoEvent, false);
		}
	}
}
