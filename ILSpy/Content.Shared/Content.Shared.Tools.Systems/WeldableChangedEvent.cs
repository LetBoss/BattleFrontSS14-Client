using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Systems;

[ByRefEvent]
public readonly record struct WeldableChangedEvent(bool IsWelded)
{
	public readonly bool IsWelded = IsWelded;

	[CompilerGenerated]
	public void Deconstruct(out bool IsWelded)
	{
		IsWelded = this.IsWelded;
	}
}
