using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction;

[ByRefEvent]
public record struct GetUsedEntityEvent(EntityUid User)
{
	public bool Handled => Used.HasValue;

	public EntityUid User = User;

	public EntityUid? Used = null;

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("User = ");
		builder.Append(((object)Unsafe.As<EntityUid, EntityUid>(ref User)/*cast due to constrained. prefix*/).ToString());
		builder.Append(", Used = ");
		builder.Append(Used.ToString());
		builder.Append(", Handled = ");
		builder.Append(Handled.ToString());
		return true;
	}

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid User)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		User = this.User;
	}
}
