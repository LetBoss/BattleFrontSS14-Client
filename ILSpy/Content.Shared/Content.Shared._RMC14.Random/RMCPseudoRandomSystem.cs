using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Random;

public sealed class RMCPseudoRandomSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public Xoroshiro64S GetXoroshiro64S(EntityUid ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return new Xoroshiro64S((long)(((ulong)_timing.CurTick.Value << 32) | (uint)((EntitySystem)this).GetNetEntity(ent, (MetaDataComponent)null).Id));
	}

	public float NextFloat(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetXoroshiro64S(ent).NextFloat();
	}

	public float NextFloat(ref Xoroshiro64S xoroshiro)
	{
		return xoroshiro.NextFloat();
	}

	public Angle NextAngle(EntityUid ent, Angle minValue, Angle maxValue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return Angle.op_Implicit((double)NextFloat(ent) * Angle.op_Implicit(maxValue - minValue)) + minValue;
	}

	public Angle NextAngle(ref Xoroshiro64S xoroshiro, Angle minValue, Angle maxValue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return Angle.op_Implicit((double)NextFloat(ref xoroshiro) * Angle.op_Implicit(maxValue - minValue)) + minValue;
	}
}
