using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.NodeContainer;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.Piping.Unary.Systems;

public abstract class SharedGasCanisterSystem : EntitySystem
{
	[Dependency]
	protected ISharedAdminLogManager AdminLogger;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	protected SharedUserInterfaceSystem UI;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<GasCanisterComponent, EntInsertedIntoContainerMessage>)OnCanisterContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<GasCanisterComponent, EntRemovedFromContainerMessage>)OnCanisterContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, ItemSlotInsertAttemptEvent>((ComponentEventRefHandler<GasCanisterComponent, ItemSlotInsertAttemptEvent>)OnCanisterInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, ComponentStartup>((EntityEventRefHandler<GasCanisterComponent, ComponentStartup>)OnCanisterStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, GasCanisterHoldingTankEjectMessage>((ComponentEventHandler<GasCanisterComponent, GasCanisterHoldingTankEjectMessage>)OnHoldingTankEjectMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, GasCanisterChangeReleasePressureMessage>((ComponentEventHandler<GasCanisterComponent, GasCanisterChangeReleasePressureMessage>)OnCanisterChangeReleasePressure, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, GasCanisterChangeReleaseValveMessage>((ComponentEventHandler<GasCanisterComponent, GasCanisterChangeReleaseValveMessage>)OnCanisterChangeReleaseValve, (Type[])null, (Type[])null);
	}

	private void OnCanisterStartup(Entity<GasCanisterComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_slots.AddItemSlot(ent.Owner, ent.Comp.ContainerName, ent.Comp.GasTankSlot);
	}

	private void OnCanisterContainerModified(EntityUid uid, GasCanisterComponent component, ContainerModifiedMessage args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Container.ID != component.ContainerName))
		{
			DirtyUI(uid, component);
			_appearance.SetData(uid, (Enum)GasCanisterVisuals.TankInserted, (object)(args is EntInsertedIntoContainerMessage), (AppearanceComponent)null);
		}
	}

	private static string GetContainedGasesString(Entity<GasCanisterComponent> canister)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return string.Join(", ", canister.Comp.Air);
	}

	private void OnHoldingTankEjectMessage(EntityUid uid, GasCanisterComponent canister, GasCanisterHoldingTankEjectMessage args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		if (canister.GasTankSlot.Item.HasValue)
		{
			EntityUid? item = canister.GasTankSlot.Item;
			_slots.TryEjectToHands(uid, canister.GasTankSlot, ((BaseBoundUserInterfaceEvent)args).Actor, excludeUserAudio: true);
			if (canister.ReleaseValve)
			{
				ISharedAdminLogManager adminLogger = AdminLogger;
				LogStringHandler handler = new LogStringHandler(80, 4);
				handler.AppendLiteral("Player ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
				handler.AppendLiteral(" ejected tank ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(item, (MetaDataComponent)null), "tank", "ToPrettyString(item)");
				handler.AppendLiteral(" from ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "canister", "ToPrettyString(uid)");
				handler.AppendLiteral(" while the valve was open, releasing [");
				handler.AppendFormatted(GetContainedGasesString(Entity<GasCanisterComponent>.op_Implicit((uid, canister))));
				handler.AppendLiteral("] to atmosphere");
				adminLogger.Add(LogType.CanisterTankEjected, LogImpact.High, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = AdminLogger;
				LogStringHandler handler2 = new LogStringHandler(27, 3);
				handler2.AppendLiteral("Player ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
				handler2.AppendLiteral(" ejected tank ");
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(item, (MetaDataComponent)null), "tank", "ToPrettyString(item)");
				handler2.AppendLiteral(" from ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "canister", "ToPrettyString(uid)");
				adminLogger2.Add(LogType.CanisterTankEjected, LogImpact.Medium, ref handler2);
			}
			GasCanisterBoundUserInterfaceState lastState = default(GasCanisterBoundUserInterfaceState);
			if (UI.TryGetUiState<GasCanisterBoundUserInterfaceState>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)GasCanisterUiKey.Key, ref lastState))
			{
				GasCanisterBoundUserInterfaceState newState = new GasCanisterBoundUserInterfaceState(lastState.CanisterPressure, lastState.PortStatus, 0f);
				UI.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)GasCanisterUiKey.Key, (BoundUserInterfaceState)(object)newState);
			}
			DirtyUI(uid, canister);
		}
	}

	private void OnCanisterChangeReleasePressure(EntityUid uid, GasCanisterComponent canister, GasCanisterChangeReleasePressureMessage args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		float pressure = Math.Clamp(args.Pressure, canister.MinReleasePressure, canister.MaxReleasePressure);
		ISharedAdminLogManager adminLogger = AdminLogger;
		LogStringHandler handler = new LogStringHandler(33, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the release pressure on ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "canister", "ToPrettyString(uid)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(args.Pressure, "args.Pressure");
		adminLogger.Add(LogType.CanisterPressure, LogImpact.Medium, ref handler);
		canister.ReleasePressure = pressure;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)canister, (MetaDataComponent)null);
		DirtyUI(uid, canister);
	}

	private void OnCanisterChangeReleaseValve(EntityUid uid, GasCanisterComponent canister, GasCanisterChangeReleaseValveMessage args)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		LogImpact impact = ((!canister.GasTankSlot.HasItem) ? LogImpact.High : LogImpact.Medium);
		Dictionary<Gas, float> containedGasDict = new Dictionary<Gas, float>();
		Array containedGasArray = Enum.GetValues(typeof(Gas));
		for (int i = 0; i < containedGasArray.Length; i++)
		{
			containedGasDict.Add((Gas)i, canister.Air[i]);
		}
		ISharedAdminLogManager adminLogger = AdminLogger;
		LogStringHandler handler = new LogStringHandler(44, 4);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the valve on ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "canister", "ToPrettyString(uid)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(args.Valve, "valveState", "args.Valve");
		handler.AppendLiteral(" while it contained [");
		handler.AppendFormatted(string.Join(", ", containedGasDict));
		handler.AppendLiteral("]");
		adminLogger.Add(LogType.CanisterValve, impact, ref handler);
		canister.ReleaseValve = args.Valve;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)canister, (MetaDataComponent)null);
		DirtyUI(uid, canister);
	}

	private void OnCanisterInsertAttempt(EntityUid uid, GasCanisterComponent component, ref ItemSlotInsertAttemptEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		GasTankComponent gasTank = default(GasTankComponent);
		if (!(args.Slot.ID != component.ContainerName) && args.User.HasValue && (!((EntitySystem)this).TryComp<GasTankComponent>(args.Item, ref gasTank) || gasTank.IsValveOpen))
		{
			args.Cancelled = true;
		}
	}

	protected abstract void DirtyUI(EntityUid uid, GasCanisterComponent? component = null, NodeContainerComponent? nodes = null);
}
