using System;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared.SubFloor;

public abstract class SharedTrayScannerSystem : EntitySystem
{
	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedEyeSystem _eye;

	public const float SubfloorRevealAlpha = 0.8f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, ComponentGetState>((ComponentEventRefHandler<TrayScannerComponent, ComponentGetState>)OnTrayScannerGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, ComponentHandleState>((ComponentEventRefHandler<TrayScannerComponent, ComponentHandleState>)OnTrayScannerHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, ActivateInWorldEvent>((ComponentEventHandler<TrayScannerComponent, ActivateInWorldEvent>)OnTrayScannerActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, GotEquippedHandEvent>((EntityEventRefHandler<TrayScannerComponent, GotEquippedHandEvent>)OnTrayHandEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, GotUnequippedHandEvent>((EntityEventRefHandler<TrayScannerComponent, GotUnequippedHandEvent>)OnTrayHandUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, GotEquippedEvent>((EntityEventRefHandler<TrayScannerComponent, GotEquippedEvent>)OnTrayEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerComponent, GotUnequippedEvent>((EntityEventRefHandler<TrayScannerComponent, GotUnequippedEvent>)OnTrayUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TrayScannerUserComponent, GetVisMaskEvent>((EntityEventRefHandler<TrayScannerUserComponent, GetVisMaskEvent>)OnUserGetVis, (Type[])null, (Type[])null);
	}

	private void OnUserGetVis(Entity<TrayScannerUserComponent> ent, ref GetVisMaskEvent args)
	{
		args.VisibilityMask |= 4;
	}

	private void OnEquip(EntityUid user)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!_netMan.IsClient)
		{
			TrayScannerUserComponent trayScannerUserComponent = ((EntitySystem)this).EnsureComp<TrayScannerUserComponent>(user);
			trayScannerUserComponent.Count++;
			if (trayScannerUserComponent.Count <= 1)
			{
				_eye.RefreshVisibilityMask(Entity<EyeComponent>.op_Implicit(user));
			}
		}
	}

	private void OnUnequip(EntityUid user)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TrayScannerUserComponent comp = default(TrayScannerUserComponent);
		if (!_netMan.IsClient && ((EntitySystem)this).TryComp<TrayScannerUserComponent>(user, ref comp))
		{
			comp.Count--;
			if (comp.Count <= 0)
			{
				((EntitySystem)this).RemComp<TrayScannerUserComponent>(user);
				_eye.RefreshVisibilityMask(Entity<EyeComponent>.op_Implicit(user));
			}
		}
	}

	private void OnTrayHandUnequipped(Entity<TrayScannerComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnUnequip(args.User);
	}

	private void OnTrayHandEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnEquip(args.User);
	}

	private void OnTrayUnequipped(Entity<TrayScannerComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnUnequip(args.Equipee);
	}

	private void OnTrayEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnEquip(args.Equipee);
	}

	private void OnTrayScannerActivate(EntityUid uid, TrayScannerComponent scanner, ActivateInWorldEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			SetScannerEnabled(uid, !scanner.Enabled, scanner);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void SetScannerEnabled(EntityUid uid, bool enabled, TrayScannerComponent? scanner = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TrayScannerComponent>(uid, ref scanner, true) && scanner.Enabled != enabled)
		{
			scanner.Enabled = enabled;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)scanner, (MetaDataComponent)null);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
			{
				_appearance.SetData(uid, (Enum)TrayScannerVisual.Visual, (object)(scanner.Enabled ? TrayScannerVisual.On : TrayScannerVisual.Off), appearance);
			}
		}
	}

	private void OnTrayScannerGetState(EntityUid uid, TrayScannerComponent scanner, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new TrayScannerState(scanner.Enabled, scanner.Range);
	}

	private void OnTrayScannerHandleState(EntityUid uid, TrayScannerComponent scanner, ref ComponentHandleState args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is TrayScannerState state)
		{
			scanner.Range = state.Range;
			SetScannerEnabled(uid, state.Enabled, scanner);
		}
	}
}
