using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Intel.Tech;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct TechOption(string Name, string Description, int Cost, int Increase, bool Repurchasable, int CurrentCost, bool Purchased, List<object> Events, Rsi Icon, TimeSpan TimeLock);
