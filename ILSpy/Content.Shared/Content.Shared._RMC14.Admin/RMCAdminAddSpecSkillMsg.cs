using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin;

[Serializable]
[NetSerializable]
public sealed class RMCAdminAddSpecSkillMsg(string component) : EuiMessageBase
{
	public readonly string Component = component;
}
