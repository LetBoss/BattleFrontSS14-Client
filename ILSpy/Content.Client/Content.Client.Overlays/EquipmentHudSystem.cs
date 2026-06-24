using System;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.ViewVariables;

namespace Content.Client.Overlays;

public abstract class EquipmentHudSystem<T> : EntitySystem where T : IComponent
{
	[Dependency]
	private IPlayerManager _player;

	[ViewVariables]
	protected bool IsActive;

	protected virtual SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<T, ComponentStartup>((EntityEventRefHandler<T, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<T, ComponentRemove>((EntityEventRefHandler<T, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerAttachedEvent>((EntityEventHandler<LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerDetachedEvent>((EntityEventHandler<LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<T, GotEquippedEvent>((EntityEventRefHandler<T, GotEquippedEvent>)OnCompEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<T, GotUnequippedEvent>((EntityEventRefHandler<T, GotUnequippedEvent>)OnCompUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<T, RefreshEquipmentHudEvent<T>>((EntityEventRefHandler<T, RefreshEquipmentHudEvent<T>>)OnRefreshComponentHud, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<T, InventoryRelayedEvent<RefreshEquipmentHudEvent<T>>>((EntityEventRefHandler<T, InventoryRelayedEvent<RefreshEquipmentHudEvent<T>>>)OnRefreshEquipmentHud, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
	}

	private void Update(RefreshEquipmentHudEvent<T> ev)
	{
		IsActive = true;
		UpdateInternal(ev);
	}

	public void Deactivate()
	{
		if (IsActive)
		{
			IsActive = false;
			DeactivateInternal();
		}
	}

	protected virtual void UpdateInternal(RefreshEquipmentHudEvent<T> args)
	{
	}

	protected virtual void DeactivateInternal()
	{
	}

	private void OnStartup(Entity<T> ent, ref ComponentStartup args)
	{
		RefreshOverlay();
	}

	private void OnRemove(Entity<T> ent, ref ComponentRemove args)
	{
		RefreshOverlay();
	}

	private void OnPlayerAttached(LocalPlayerAttachedEvent args)
	{
		RefreshOverlay();
	}

	private void OnPlayerDetached(LocalPlayerDetachedEvent args)
	{
		ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
		if (!((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null)).HasValue)
		{
			Deactivate();
		}
	}

	private void OnCompEquip(Entity<T> ent, ref GotEquippedEvent args)
	{
		RefreshOverlay();
	}

	private void OnCompUnequip(Entity<T> ent, ref GotUnequippedEvent args)
	{
		RefreshOverlay();
	}

	private void OnRoundRestart(RoundRestartCleanupEvent args)
	{
		Deactivate();
	}

	protected virtual void OnRefreshEquipmentHud(Entity<T> ent, ref InventoryRelayedEvent<RefreshEquipmentHudEvent<T>> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnRefreshComponentHud(ent, ref args.Args);
	}

	protected virtual void OnRefreshComponentHud(Entity<T> ent, ref RefreshEquipmentHudEvent<T> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.Active = true;
		args.Components.Add(ent.Comp);
	}

	protected void RefreshOverlay()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid valueOrDefault = val.GetValueOrDefault();
			RefreshEquipmentHudEvent<T> ev = new RefreshEquipmentHudEvent<T>(TargetSlots);
			((EntitySystem)this).RaiseLocalEvent<RefreshEquipmentHudEvent<T>>(valueOrDefault, ref ev, false);
			if (ev.Active)
			{
				Update(ev);
			}
			else
			{
				Deactivate();
			}
		}
	}
}
