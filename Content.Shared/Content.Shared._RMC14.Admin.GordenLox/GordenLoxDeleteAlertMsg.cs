using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.GordenLox;

[Serializable]
[NetSerializable]
public sealed class GordenLoxDeleteAlertMsg(int id) : EuiMessageBase
{
	public readonly int Id = id;
}
