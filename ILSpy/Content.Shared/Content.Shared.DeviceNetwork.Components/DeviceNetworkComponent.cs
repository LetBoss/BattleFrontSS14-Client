using System;
using System.Collections.Generic;
using Content.Shared.DeviceNetwork.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.DeviceNetwork.Components;

[RegisterComponent]
[Access(new Type[]
{
	typeof(SharedDeviceNetworkSystem),
	typeof(DeviceNet)
})]
public sealed class DeviceNetworkComponent : Component, ISerializationGenerated<DeviceNetworkComponent>, ISerializationGenerated
{
	public enum DeviceNetIdDefaults
	{
		Private = 0,
		Wired = 1,
		Wireless = 2,
		Apc = 3,
		AtmosDevices = 4,
		Reserved = 100
	}

	[DataField("receiveFrequency", false, 1, false, false, null)]
	public uint? ReceiveFrequency;

	[DataField("receiveFrequencyId", false, 1, false, false, typeof(PrototypeIdSerializer<DeviceFrequencyPrototype>))]
	public string? ReceiveFrequencyId;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("transmitFrequency", false, 1, false, false, null)]
	public uint? TransmitFrequency;

	[DataField("transmitFrequencyId", false, 1, false, false, typeof(PrototypeIdSerializer<DeviceFrequencyPrototype>))]
	public string? TransmitFrequencyId;

	[DataField("address", false, 1, false, false, null)]
	public string Address = string.Empty;

	[DataField("customAddress", false, 1, false, false, null)]
	public bool CustomAddress;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("prefix", false, 1, false, false, null)]
	public string? Prefix;

	[DataField("receiveAll", false, 1, false, false, null)]
	public bool ReceiveAll;

	[DataField("examinableAddress", false, 1, false, false, null)]
	public bool ExaminableAddress;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("autoConnect", false, 1, false, false, null)]
	public bool AutoConnect = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sendBroadcastAttemptEvent", false, 1, false, false, null)]
	public bool SendBroadcastAttemptEvent;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("savableAddress", false, 1, false, false, null)]
	public bool SavableAddress = true;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedDeviceListSystem) })]
	public HashSet<EntityUid> DeviceLists = new HashSet<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedNetworkConfiguratorSystem) })]
	public HashSet<EntityUid> Configurators = new HashSet<EntityUid>();

	[DataField("deviceNetId", false, 1, false, false, null)]
	public DeviceNetIdDefaults NetIdEnum { get; set; }

	public int DeviceNetId => (int)NetIdEnum;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DeviceNetworkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DeviceNetworkComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DeviceNetworkComponent>(this, ref target, hookCtx, false, context))
		{
			DeviceNetIdDefaults NetIdEnumTemp = DeviceNetIdDefaults.Private;
			if (!serialization.TryCustomCopy<DeviceNetIdDefaults>(NetIdEnum, ref NetIdEnumTemp, hookCtx, false, context))
			{
				NetIdEnumTemp = NetIdEnum;
			}
			target.NetIdEnum = NetIdEnumTemp;
			uint? ReceiveFrequencyTemp = null;
			if (!serialization.TryCustomCopy<uint?>(ReceiveFrequency, ref ReceiveFrequencyTemp, hookCtx, false, context))
			{
				ReceiveFrequencyTemp = ReceiveFrequency;
			}
			target.ReceiveFrequency = ReceiveFrequencyTemp;
			string ReceiveFrequencyIdTemp = null;
			if (!serialization.TryCustomCopy<string>(ReceiveFrequencyId, ref ReceiveFrequencyIdTemp, hookCtx, false, context))
			{
				ReceiveFrequencyIdTemp = ReceiveFrequencyId;
			}
			target.ReceiveFrequencyId = ReceiveFrequencyIdTemp;
			uint? TransmitFrequencyTemp = null;
			if (!serialization.TryCustomCopy<uint?>(TransmitFrequency, ref TransmitFrequencyTemp, hookCtx, false, context))
			{
				TransmitFrequencyTemp = TransmitFrequency;
			}
			target.TransmitFrequency = TransmitFrequencyTemp;
			string TransmitFrequencyIdTemp = null;
			if (!serialization.TryCustomCopy<string>(TransmitFrequencyId, ref TransmitFrequencyIdTemp, hookCtx, false, context))
			{
				TransmitFrequencyIdTemp = TransmitFrequencyId;
			}
			target.TransmitFrequencyId = TransmitFrequencyIdTemp;
			string AddressTemp = null;
			if (Address == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Address, ref AddressTemp, hookCtx, false, context))
			{
				AddressTemp = Address;
			}
			target.Address = AddressTemp;
			bool CustomAddressTemp = false;
			if (!serialization.TryCustomCopy<bool>(CustomAddress, ref CustomAddressTemp, hookCtx, false, context))
			{
				CustomAddressTemp = CustomAddress;
			}
			target.CustomAddress = CustomAddressTemp;
			string PrefixTemp = null;
			if (!serialization.TryCustomCopy<string>(Prefix, ref PrefixTemp, hookCtx, false, context))
			{
				PrefixTemp = Prefix;
			}
			target.Prefix = PrefixTemp;
			bool ReceiveAllTemp = false;
			if (!serialization.TryCustomCopy<bool>(ReceiveAll, ref ReceiveAllTemp, hookCtx, false, context))
			{
				ReceiveAllTemp = ReceiveAll;
			}
			target.ReceiveAll = ReceiveAllTemp;
			bool ExaminableAddressTemp = false;
			if (!serialization.TryCustomCopy<bool>(ExaminableAddress, ref ExaminableAddressTemp, hookCtx, false, context))
			{
				ExaminableAddressTemp = ExaminableAddress;
			}
			target.ExaminableAddress = ExaminableAddressTemp;
			bool AutoConnectTemp = false;
			if (!serialization.TryCustomCopy<bool>(AutoConnect, ref AutoConnectTemp, hookCtx, false, context))
			{
				AutoConnectTemp = AutoConnect;
			}
			target.AutoConnect = AutoConnectTemp;
			bool SendBroadcastAttemptEventTemp = false;
			if (!serialization.TryCustomCopy<bool>(SendBroadcastAttemptEvent, ref SendBroadcastAttemptEventTemp, hookCtx, false, context))
			{
				SendBroadcastAttemptEventTemp = SendBroadcastAttemptEvent;
			}
			target.SendBroadcastAttemptEvent = SendBroadcastAttemptEventTemp;
			bool SavableAddressTemp = false;
			if (!serialization.TryCustomCopy<bool>(SavableAddress, ref SavableAddressTemp, hookCtx, false, context))
			{
				SavableAddressTemp = SavableAddress;
			}
			target.SavableAddress = SavableAddressTemp;
			HashSet<EntityUid> DeviceListsTemp = null;
			if (DeviceLists == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(DeviceLists, ref DeviceListsTemp, hookCtx, true, context))
			{
				DeviceListsTemp = serialization.CreateCopy<HashSet<EntityUid>>(DeviceLists, hookCtx, context, false);
			}
			target.DeviceLists = DeviceListsTemp;
			HashSet<EntityUid> ConfiguratorsTemp = null;
			if (Configurators == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(Configurators, ref ConfiguratorsTemp, hookCtx, true, context))
			{
				ConfiguratorsTemp = serialization.CreateCopy<HashSet<EntityUid>>(Configurators, hookCtx, context, false);
			}
			target.Configurators = ConfiguratorsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DeviceNetworkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceNetworkComponent cast = (DeviceNetworkComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceNetworkComponent cast = (DeviceNetworkComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceNetworkComponent def = (DeviceNetworkComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DeviceNetworkComponent Instantiate()
	{
		return new DeviceNetworkComponent();
	}
}
