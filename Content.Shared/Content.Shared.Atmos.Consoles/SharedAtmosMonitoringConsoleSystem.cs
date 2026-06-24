using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Consoles;

public abstract class SharedAtmosMonitoringConsoleSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class AtmosMonitoringConsoleState(Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> chunks, Dictionary<NetEntity, AtmosDeviceNavMapData> atmosDevices) : ComponentState
	{
		public Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> Chunks = chunks;

		public Dictionary<NetEntity, AtmosDeviceNavMapData> AtmosDevices = atmosDevices;
	}

	[Serializable]
	[NetSerializable]
	protected sealed class AtmosMonitoringConsoleDeltaState(Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> modifiedChunks, Dictionary<NetEntity, AtmosDeviceNavMapData> atmosDevices, HashSet<Vector2i> allChunks) : ComponentState, IComponentDeltaState<AtmosMonitoringConsoleState>, IComponentDeltaState, IComponentState
	{
		public Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> ModifiedChunks = modifiedChunks;

		public Dictionary<NetEntity, AtmosDeviceNavMapData> AtmosDevices = atmosDevices;

		public HashSet<Vector2i> AllChunks = allChunks;

		public void ApplyToFullState(AtmosMonitoringConsoleState state)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			foreach (Vector2i key in state.Chunks.Keys)
			{
				if (!AllChunks.Contains(key))
				{
					state.Chunks.Remove(key);
				}
			}
			foreach (var (index, data) in ModifiedChunks)
			{
				state.Chunks[index] = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>(data);
			}
			state.AtmosDevices.Clear();
			foreach (var (nuid, atmosDevice) in AtmosDevices)
			{
				state.AtmosDevices.Add(nuid, atmosDevice);
			}
		}

		public AtmosMonitoringConsoleState CreateNewFullState(AtmosMonitoringConsoleState state)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> chunks = new Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>>(state.Chunks.Count);
			foreach (var (index, _) in state.Chunks)
			{
				if (AllChunks.Contains(index))
				{
					if (ModifiedChunks.ContainsKey(index))
					{
						chunks[index] = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>(ModifiedChunks[index]);
					}
					else
					{
						chunks[index] = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>(state.Chunks[index]);
					}
				}
			}
			return new AtmosMonitoringConsoleState(chunks, new Dictionary<NetEntity, AtmosDeviceNavMapData>(AtmosDevices));
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AtmosMonitoringConsoleComponent, ComponentGetState>((ComponentEventRefHandler<AtmosMonitoringConsoleComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, AtmosMonitoringConsoleComponent component, ref ComponentGetState args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key;
		AtmosPipeChunk value;
		Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> chunks;
		if (((ComponentGetState)(ref args)).FromTick <= ((Component)component).CreationTick || component.ForceFullUpdate)
		{
			component.ForceFullUpdate = false;
			chunks = new Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>>(component.AtmosPipeChunks.Count);
			foreach (KeyValuePair<Vector2i, AtmosPipeChunk> atmosPipeChunk in component.AtmosPipeChunks)
			{
				atmosPipeChunk.Deconstruct(out key, out value);
				Vector2i origin = key;
				AtmosPipeChunk chunk = value;
				chunks.Add(origin, chunk.AtmosPipeData);
			}
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new AtmosMonitoringConsoleState(chunks, component.AtmosDevices);
			return;
		}
		chunks = new Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>>();
		foreach (KeyValuePair<Vector2i, AtmosPipeChunk> atmosPipeChunk2 in component.AtmosPipeChunks)
		{
			atmosPipeChunk2.Deconstruct(out key, out value);
			Vector2i origin2 = key;
			AtmosPipeChunk chunk2 = value;
			if (!(chunk2.LastUpdate < ((ComponentGetState)(ref args)).FromTick))
			{
				chunks.Add(origin2, chunk2.AtmosPipeData);
			}
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new AtmosMonitoringConsoleDeltaState(chunks, component.AtmosDevices, new HashSet<Vector2i>(component.AtmosPipeChunks.Keys));
	}
}
