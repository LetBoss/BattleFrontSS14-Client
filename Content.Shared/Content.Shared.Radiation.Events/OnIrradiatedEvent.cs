using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radiation.Events;

public readonly record struct OnIrradiatedEvent(float FrameTime, float RadsPerSecond, EntityUid Origin)
{
	public float TotalRads => RadsPerSecond * FrameTime;

	public readonly float FrameTime = FrameTime;

	public readonly float RadsPerSecond = RadsPerSecond;

	public readonly EntityUid Origin = Origin;

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		builder.Append("FrameTime = ");
		builder.Append(FrameTime.ToString());
		builder.Append(", RadsPerSecond = ");
		builder.Append(RadsPerSecond.ToString());
		builder.Append(", Origin = ");
		builder.Append(((object)Origin/*cast due to constrained. prefix*/).ToString());
		builder.Append(", TotalRads = ");
		builder.Append(TotalRads.ToString());
		return true;
	}

	[CompilerGenerated]
	public void Deconstruct(out float FrameTime, out float RadsPerSecond, out EntityUid Origin)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		FrameTime = this.FrameTime;
		RadsPerSecond = this.RadsPerSecond;
		Origin = this.Origin;
	}
}
