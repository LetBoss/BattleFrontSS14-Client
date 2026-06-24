using System.Collections.Generic;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Shared.DeviceNetwork;

public sealed class DeviceNet
{
	public readonly Dictionary<string, DeviceNetworkComponent> Devices = new Dictionary<string, DeviceNetworkComponent>();

	public readonly Dictionary<uint, HashSet<DeviceNetworkComponent>> ListeningDevices = new Dictionary<uint, HashSet<DeviceNetworkComponent>>();

	public readonly Dictionary<uint, HashSet<DeviceNetworkComponent>> ReceiveAllDevices = new Dictionary<uint, HashSet<DeviceNetworkComponent>>();

	private readonly IRobustRandom _random;

	public readonly int NetId;

	public DeviceNet(int netId, IRobustRandom random)
	{
		_random = random;
		NetId = netId;
	}

	public bool Add(DeviceNetworkComponent device)
	{
		if (device.CustomAddress)
		{
			if (!Devices.TryAdd(device.Address, device))
			{
				return false;
			}
		}
		else
		{
			if (string.IsNullOrWhiteSpace(device.Address) || Devices.ContainsKey(device.Address))
			{
				device.Address = GenerateValidAddress(device.Prefix);
			}
			Devices[device.Address] = device;
		}
		uint? receiveFrequency = device.ReceiveFrequency;
		if (receiveFrequency.HasValue)
		{
			uint freq = receiveFrequency.GetValueOrDefault();
			if (!ListeningDevices.TryGetValue(freq, out HashSet<DeviceNetworkComponent> devices))
			{
				devices = (ListeningDevices[freq] = new HashSet<DeviceNetworkComponent>());
			}
			devices.Add(device);
			if (!device.ReceiveAll)
			{
				return true;
			}
			if (!ReceiveAllDevices.TryGetValue(freq, out HashSet<DeviceNetworkComponent> receiveAlldevices))
			{
				receiveAlldevices = (ReceiveAllDevices[freq] = new HashSet<DeviceNetworkComponent>());
			}
			receiveAlldevices.Add(device);
			return true;
		}
		return true;
	}

	public bool Remove(DeviceNetworkComponent device)
	{
		if (device.Address == null || !Devices.Remove(device.Address))
		{
			return false;
		}
		uint? receiveFrequency = device.ReceiveFrequency;
		if (receiveFrequency.HasValue)
		{
			uint freq = receiveFrequency.GetValueOrDefault();
			if (ListeningDevices.TryGetValue(freq, out HashSet<DeviceNetworkComponent> listening))
			{
				listening.Remove(device);
				if (listening.Count == 0)
				{
					ListeningDevices.Remove(freq);
				}
			}
			if (device.ReceiveAll && ReceiveAllDevices.TryGetValue(freq, out HashSet<DeviceNetworkComponent> receiveAll))
			{
				receiveAll.Remove(device);
				if (receiveAll.Count == 0)
				{
					ListeningDevices.Remove(freq);
				}
			}
			return true;
		}
		return true;
	}

	public bool RandomizeAddress(string oldAddress, string? prefix = null)
	{
		if (!Devices.Remove(oldAddress, out DeviceNetworkComponent device))
		{
			return false;
		}
		device.Address = GenerateValidAddress(prefix ?? device.Prefix);
		device.CustomAddress = false;
		Devices[device.Address] = device;
		return true;
	}

	public bool UpdateAddress(string oldAddress, string newAddress)
	{
		if (Devices.ContainsKey(newAddress))
		{
			return false;
		}
		if (!Devices.Remove(oldAddress, out DeviceNetworkComponent device))
		{
			return false;
		}
		device.Address = newAddress;
		device.CustomAddress = true;
		Devices[newAddress] = device;
		return true;
	}

	public bool UpdateReceiveFrequency(string address, uint? newFrequency)
	{
		if (!Devices.TryGetValue(address, out DeviceNetworkComponent device))
		{
			return false;
		}
		if (device.ReceiveFrequency == newFrequency)
		{
			return true;
		}
		uint? receiveFrequency = device.ReceiveFrequency;
		if (receiveFrequency.HasValue)
		{
			uint freq = receiveFrequency.GetValueOrDefault();
			if (ListeningDevices.TryGetValue(freq, out HashSet<DeviceNetworkComponent> listening))
			{
				listening.Remove(device);
				if (listening.Count == 0)
				{
					ListeningDevices.Remove(freq);
				}
			}
			if (device.ReceiveAll && ReceiveAllDevices.TryGetValue(freq, out HashSet<DeviceNetworkComponent> receiveAll))
			{
				receiveAll.Remove(device);
				if (receiveAll.Count == 0)
				{
					ListeningDevices.Remove(freq);
				}
			}
		}
		device.ReceiveFrequency = newFrequency;
		if (!newFrequency.HasValue)
		{
			return true;
		}
		if (!ListeningDevices.TryGetValue(newFrequency.Value, out HashSet<DeviceNetworkComponent> devices))
		{
			devices = (ListeningDevices[newFrequency.Value] = new HashSet<DeviceNetworkComponent>());
		}
		devices.Add(device);
		if (!device.ReceiveAll)
		{
			return true;
		}
		if (!ReceiveAllDevices.TryGetValue(newFrequency.Value, out HashSet<DeviceNetworkComponent> receiveAlldevices))
		{
			receiveAlldevices = (ReceiveAllDevices[newFrequency.Value] = new HashSet<DeviceNetworkComponent>());
		}
		receiveAlldevices.Add(device);
		return true;
	}

	public bool UpdateReceiveAll(string address, bool receiveAll)
	{
		if (!Devices.TryGetValue(address, out DeviceNetworkComponent device))
		{
			return false;
		}
		if (device.ReceiveAll == receiveAll)
		{
			return true;
		}
		device.ReceiveAll = receiveAll;
		uint? receiveFrequency = device.ReceiveFrequency;
		if (receiveFrequency.HasValue)
		{
			uint freq = receiveFrequency.GetValueOrDefault();
			HashSet<DeviceNetworkComponent> devices;
			if (receiveAll)
			{
				if (!ReceiveAllDevices.TryGetValue(freq, out devices))
				{
					devices = (ReceiveAllDevices[freq] = new HashSet<DeviceNetworkComponent>());
				}
				devices.Add(device);
			}
			else if (ReceiveAllDevices.TryGetValue(freq, out devices))
			{
				devices.Remove(device);
				if (devices.Count == 0)
				{
					ReceiveAllDevices.Remove(freq);
				}
			}
			return true;
		}
		return true;
	}

	private string GenerateValidAddress(string? prefix)
	{
		prefix = (string.IsNullOrWhiteSpace(prefix) ? null : Loc.GetString(prefix));
		string address;
		do
		{
			int num = _random.Next();
			address = $"{prefix}{num >> 16:X4}-{num & 0xFFFF:X4}";
		}
		while (Devices.ContainsKey(address));
		return address;
	}
}
