using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Consoles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Consoles;

public sealed class AtmosMonitoringConsoleSystem : SharedAtmosMonitoringConsoleSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AtmosMonitoringConsoleComponent, ComponentHandleState>((ComponentEventRefHandler<AtmosMonitoringConsoleComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, AtmosMonitoringConsoleComponent component, ref ComponentHandleState args)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		IComponentState current = ((ComponentHandleState)(ref args)).Current;
		Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> dictionary;
		Dictionary<NetEntity, AtmosDeviceNavMapData> atmosDevices;
		if (!(current is AtmosMonitoringConsoleDeltaState atmosMonitoringConsoleDeltaState))
		{
			if (!(current is AtmosMonitoringConsoleState atmosMonitoringConsoleState))
			{
				return;
			}
			dictionary = atmosMonitoringConsoleState.Chunks;
			atmosDevices = atmosMonitoringConsoleState.AtmosDevices;
			foreach (Vector2i key3 in component.AtmosPipeChunks.Keys)
			{
				if (!atmosMonitoringConsoleState.Chunks.ContainsKey(key3))
				{
					component.AtmosPipeChunks.Remove(key3);
				}
			}
		}
		else
		{
			dictionary = atmosMonitoringConsoleDeltaState.ModifiedChunks;
			atmosDevices = atmosMonitoringConsoleDeltaState.AtmosDevices;
			foreach (Vector2i key4 in component.AtmosPipeChunks.Keys)
			{
				if (!atmosMonitoringConsoleDeltaState.AllChunks.Contains(key4))
				{
					component.AtmosPipeChunks.Remove(key4);
				}
			}
		}
		foreach (KeyValuePair<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> item in dictionary)
		{
			item.Deconstruct(out var key, out var value);
			Vector2i val = key;
			Dictionary<AtmosMonitoringConsoleSubnet, ulong> dictionary2 = value;
			AtmosPipeChunk value2 = new AtmosPipeChunk(val);
			value2.AtmosPipeData = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>(dictionary2);
			component.AtmosPipeChunks[val] = value2;
		}
		component.AtmosDevices.Clear();
		foreach (var (key2, value3) in atmosDevices)
		{
			component.AtmosDevices[key2] = value3;
		}
	}
}
