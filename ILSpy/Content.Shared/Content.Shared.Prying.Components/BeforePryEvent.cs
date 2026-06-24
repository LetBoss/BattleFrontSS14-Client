using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Prying.Components;

[ByRefEvent]
public record struct BeforePryEvent(EntityUid User, bool PryPowered, bool Force, bool StrongPry)
{
	public readonly EntityUid User = User;

	public readonly bool PryPowered = PryPowered;

	public readonly bool Force = Force;

	public readonly bool StrongPry = StrongPry;

	public string? Message = null;

	public bool Cancelled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid User, out bool PryPowered, out bool Force, out bool StrongPry)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		User = this.User;
		PryPowered = this.PryPowered;
		Force = this.Force;
		StrongPry = this.StrongPry;
	}
}
