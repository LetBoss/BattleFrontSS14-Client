using System.Collections.Generic;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedDeviceNetworkJammerSystem : EntitySystem
{
	public void SetRange(Entity<DeviceNetworkJammerComponent> ent, float value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Range = value;
		((EntitySystem)this).Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent)null);
	}

	public bool TrySetRange(Entity<DeviceNetworkJammerComponent?> ent, float value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceNetworkJammerComponent>(Entity<DeviceNetworkJammerComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		SetRange(Entity<DeviceNetworkJammerComponent>.op_Implicit((Entity<DeviceNetworkJammerComponent>.op_Implicit(ent), ent.Comp)), value);
		return true;
	}

	public IReadOnlySet<string> GetJammableNetworks(Entity<DeviceNetworkJammerComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return ent.Comp.JammableNetworks;
	}

	public void AddJammableNetwork(Entity<DeviceNetworkJammerComponent> ent, string networkId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.JammableNetworks.Add(networkId))
		{
			((EntitySystem)this).Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void RemoveJammableNetwork(Entity<DeviceNetworkJammerComponent> ent, string networkId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.JammableNetworks.Remove(networkId))
		{
			((EntitySystem)this).Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void ClearJammableNetworks(Entity<DeviceNetworkJammerComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.JammableNetworks.Count != 0)
		{
			ent.Comp.JammableNetworks.Clear();
			((EntitySystem)this).Dirty<DeviceNetworkJammerComponent>(ent, (MetaDataComponent)null);
		}
	}
}
