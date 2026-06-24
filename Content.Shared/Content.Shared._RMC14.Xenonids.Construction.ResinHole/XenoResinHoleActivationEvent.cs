using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

[Serializable]
[NetSerializable]
public sealed class XenoResinHoleActivationEvent : EntityEventArgs
{
	public LocId message;

	public XenoResinHoleActivationEvent(LocId msg)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		message = msg;
	}
}
