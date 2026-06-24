using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public record struct ShotAttemptedEvent
{
	public bool Cancelled { get; private set; }

	public EntityUid User;

	public Entity<GunComponent> Used;

	public void Cancel()
	{
		Cancelled = true;
	}

	public void Uncancel()
	{
		Cancelled = false;
	}

	[CompilerGenerated]
	private readonly bool PrintMembers(StringBuilder builder)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		builder.Append("User = ");
		builder.Append(((object)User/*cast due to constrained. prefix*/).ToString());
		builder.Append(", Used = ");
		builder.Append(((object)Used/*cast due to constrained. prefix*/).ToString());
		builder.Append(", Cancelled = ");
		builder.Append(Cancelled.ToString());
		return true;
	}

	[CompilerGenerated]
	public override readonly int GetHashCode()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return (EqualityComparer<EntityUid>.Default.GetHashCode(User) * -1521134295 + EqualityComparer<Entity<GunComponent>>.Default.GetHashCode(Used)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(Cancelled);
	}

	[CompilerGenerated]
	public readonly bool Equals(ShotAttemptedEvent other)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (EqualityComparer<EntityUid>.Default.Equals(User, other.User) && EqualityComparer<Entity<GunComponent>>.Default.Equals(Used, other.Used))
		{
			return EqualityComparer<bool>.Default.Equals(Cancelled, other.Cancelled);
		}
		return false;
	}
}
