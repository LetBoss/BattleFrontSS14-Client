using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Localization;

public sealed record LocValueEntity : LocValue<EntityUid>
{
	public LocValueEntity(EntityUid Value)
		: base(Value)
	{
	}

	public override string Format(LocContext ctx)
	{
		return IoCManager.Resolve<IEntityManager>().GetComponent<MetaDataComponent>(base.Value).EntityName;
	}

	[CompilerGenerated]
	public void Deconstruct(out EntityUid Value)
	{
		Value = base.Value;
	}
}
