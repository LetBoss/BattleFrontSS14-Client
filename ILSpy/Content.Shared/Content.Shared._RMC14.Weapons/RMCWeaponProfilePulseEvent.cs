using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfilePulseEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Sequence { get; }

	public string BuildVersion { get; }

	public bool CompactPayload { get; }

	public bool DebuggerAttached { get; }

	public int DynamicAssemblyCount { get; }

	public int ManagedModuleCountTotal { get; }

	public int NativeModuleCountTotal { get; }

	public int SideMarkerCountTotal { get; }

	public List<string> ManagedModules { get; }

	public List<string> NativeModules { get; }

	public List<string> SideMarkers { get; }

	public RMCWeaponProfilePulseEvent(int nonce, int sequence, string buildVersion, bool compactPayload, bool debuggerAttached, int dynamicAssemblyCount, int managedModuleCountTotal, int nativeModuleCountTotal, int sideMarkerCountTotal, List<string>? managedModules, List<string>? nativeModules, List<string>? sideMarkers)
	{
		Nonce = nonce;
		Sequence = sequence;
		BuildVersion = buildVersion;
		CompactPayload = compactPayload;
		DebuggerAttached = debuggerAttached;
		DynamicAssemblyCount = dynamicAssemblyCount;
		ManagedModuleCountTotal = managedModuleCountTotal;
		NativeModuleCountTotal = nativeModuleCountTotal;
		SideMarkerCountTotal = sideMarkerCountTotal;
		ManagedModules = managedModules ?? new List<string>();
		NativeModules = nativeModules ?? new List<string>();
		SideMarkers = sideMarkers ?? new List<string>();
	}
}
