using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileHelloEvent : EntityEventArgs
{
	public bool Enabled { get; }

	public int Nonce { get; }

	public float HeartbeatIntervalSeconds { get; }

	public int MaxModulesPerList { get; }

	public int MaxModuleNameLength { get; }

	public float FocusDistanceThreshold { get; }

	public int MaxProfileFrameBytes { get; }

	public string DecoyCommandName { get; }

	public string DecoyCVarName { get; }

	public int RuleSalt { get; }

	public List<int> StrictCommandRuleIds { get; }

	public List<int> SuspiciousCommandRuleIds { get; }

	public List<int> DiscoverableRootRuleIds { get; }

	public List<int> DiscoverableTypeRuleIds { get; }

	public List<string> DynamicDecoyCommands { get; }

	public RMCWeaponProfileHelloEvent(bool enabled, int nonce, float heartbeatIntervalSeconds, int maxModulesPerList, int maxModuleNameLength, float focusDistanceThreshold, int maxProfileFrameBytes, string decoyCommandName, string decoyCVarName, int ruleSalt, List<int>? strictCommandRuleIds, List<int>? suspiciousCommandRuleIds, List<int>? discoverableRootRuleIds, List<int>? discoverableTypeRuleIds, List<string>? dynamicDecoyCommands)
	{
		Enabled = enabled;
		Nonce = nonce;
		HeartbeatIntervalSeconds = heartbeatIntervalSeconds;
		MaxModulesPerList = maxModulesPerList;
		MaxModuleNameLength = maxModuleNameLength;
		FocusDistanceThreshold = focusDistanceThreshold;
		MaxProfileFrameBytes = maxProfileFrameBytes;
		DecoyCommandName = decoyCommandName;
		DecoyCVarName = decoyCVarName;
		RuleSalt = ruleSalt;
		StrictCommandRuleIds = strictCommandRuleIds ?? new List<int>();
		SuspiciousCommandRuleIds = suspiciousCommandRuleIds ?? new List<int>();
		DiscoverableRootRuleIds = discoverableRootRuleIds ?? new List<int>();
		DiscoverableTypeRuleIds = discoverableTypeRuleIds ?? new List<int>();
		DynamicDecoyCommands = dynamicDecoyCommands ?? new List<string>();
	}
}
