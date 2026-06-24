using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.GordenLox;

[Serializable]
[NetSerializable]
public sealed class GordenLoxState(List<GordenLoxAlertEntry> alerts) : EuiStateBase
{
	public readonly List<GordenLoxAlertEntry> Alerts = alerts;
}
