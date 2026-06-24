using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Collections;

namespace Robust.Shared.Physics.Systems;

public record struct RayResult()
{
	public bool Hit => Results.Count > 0;

	public ValueList<RayHit> Results = default(ValueList<RayHit>);

	public static readonly RayResult Empty = new RayResult();

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Results = ");
		builder.Append(Results.ToString());
		builder.Append(", Hit = ");
		builder.Append(Hit.ToString());
		return true;
	}
}
