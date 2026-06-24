using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.CombatMode;

[ByRefEvent]
public record struct DisarmedEvent(EntityUid Target, EntityUid Source, float PushProb)
{
	public readonly EntityUid Target = Target;

	public readonly EntityUid Source = Source;

	public readonly float PushProbability = PushProb;

	public string PopupPrefix = "";

	public bool IsStunned = false;

	public bool Handled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid Target, out EntityUid Source, out float PushProb)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Target = this.Target;
		Source = this.Source;
		PushProb = this.PushProb;
	}
}
