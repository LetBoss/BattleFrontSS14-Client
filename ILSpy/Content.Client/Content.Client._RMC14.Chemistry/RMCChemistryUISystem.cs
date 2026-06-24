using System;
using Content.Shared._RMC14.Chemistry;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Chemistry;

public sealed class RMCChemistryUISystem : SharedRMCChemistrySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private UserInterfaceSystem _ui;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCChemicalDispenserComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RMCChemicalDispenserComponent, AfterAutoHandleStateEvent>)OnDispenserAfterState, (Type[])null, (Type[])null);
	}

	private void OnDispenserAfterState(Entity<RMCChemicalDispenserComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateDispenserUI(ent);
	}

	private void UpdateDispenserUI(Entity<RMCChemicalDispenserComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is RMCChemicalDispenserBui rMCChemicalDispenserBui)
				{
					rMCChemicalDispenserBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"RMCChemicalDispenserBui"}\n{value}");
		}
	}

	protected override void DispenserUpdated(Entity<RMCChemicalDispenserComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.DispenserUpdated(ent);
		UpdateDispenserUI(ent);
	}

	public override void Update(float frameTime)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		UserInterfaceComponent val = default(UserInterfaceComponent);
		foreach (var actorUi in ((SharedUserInterfaceSystem)_ui).GetActorUis(Entity<UserInterfaceUserComponent>.op_Implicit(valueOrDefault)))
		{
			Enum item = actorUi.Item2;
			if (!(item is RMCChemicalDispenserUi) || (RMCChemicalDispenserUi)(object)item != RMCChemicalDispenserUi.Key)
			{
				continue;
			}
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(actorUi.Item1, ref val))
			{
				break;
			}
			foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
			{
				if (value is RMCChemicalDispenserBui rMCChemicalDispenserBui)
				{
					rMCChemicalDispenserBui.Refresh();
				}
			}
		}
	}
}
