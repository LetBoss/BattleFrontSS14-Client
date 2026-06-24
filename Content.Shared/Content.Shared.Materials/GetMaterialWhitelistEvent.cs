using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Materials;

[ByRefEvent]
public record struct GetMaterialWhitelistEvent(EntityUid Storage)
{
	public readonly EntityUid Storage = Storage;

	public List<ProtoId<MaterialPrototype>> Whitelist = new List<ProtoId<MaterialPrototype>>();

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid Storage)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Storage = this.Storage;
	}
}
