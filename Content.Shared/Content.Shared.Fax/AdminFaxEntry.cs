using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public sealed class AdminFaxEntry
{
	public NetEntity Uid { get; }

	public string Name { get; }

	public string Address { get; }

	public AdminFaxEntry(NetEntity uid, string name, string address)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Name = name;
		Address = address;
	}
}
