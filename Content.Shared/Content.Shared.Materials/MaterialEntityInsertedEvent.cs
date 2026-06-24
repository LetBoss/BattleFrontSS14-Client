using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Materials;

[ByRefEvent]
public readonly record struct MaterialEntityInsertedEvent(MaterialComponent MaterialComp)
{
	public readonly MaterialComponent MaterialComp = MaterialComp;

	[CompilerGenerated]
	public void Deconstruct(out MaterialComponent MaterialComp)
	{
		MaterialComp = this.MaterialComp;
	}
}
