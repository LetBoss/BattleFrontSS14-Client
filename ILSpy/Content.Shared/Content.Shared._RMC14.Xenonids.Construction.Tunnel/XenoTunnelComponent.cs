using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[RegisterComponent]
public sealed class XenoTunnelComponent : Component, ISerializationGenerated<XenoTunnelComponent>, ISerializationGenerated
{
	public const string ContainedMobsContainerId = "rmc_xeno_tunnel_mob_container";

	[DataField(null, false, 1, false, false, null)]
	public int MaxMobs = 3;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan SmallXenoEnterDelay = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan StandardXenoEnterDelay = TimeSpan.FromSeconds(4L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LargeXenoEnterDelay = TimeSpan.FromSeconds(12L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan SmallXenoMoveDelay = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan StandardXenoMoveDelay = TimeSpan.FromSeconds(2L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LargeXenoMoveDelay = TimeSpan.FromSeconds(6L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoTunnelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoTunnelComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoTunnelComponent>(this, ref target, hookCtx, false, context))
		{
			int MaxMobsTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxMobs, ref MaxMobsTemp, hookCtx, false, context))
			{
				MaxMobsTemp = MaxMobs;
			}
			target.MaxMobs = MaxMobsTemp;
			TimeSpan SmallXenoEnterDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(SmallXenoEnterDelay, ref SmallXenoEnterDelayTemp, hookCtx, false, context))
			{
				SmallXenoEnterDelayTemp = serialization.CreateCopy<TimeSpan>(SmallXenoEnterDelay, hookCtx, context, false);
			}
			target.SmallXenoEnterDelay = SmallXenoEnterDelayTemp;
			TimeSpan StandardXenoEnterDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StandardXenoEnterDelay, ref StandardXenoEnterDelayTemp, hookCtx, false, context))
			{
				StandardXenoEnterDelayTemp = serialization.CreateCopy<TimeSpan>(StandardXenoEnterDelay, hookCtx, context, false);
			}
			target.StandardXenoEnterDelay = StandardXenoEnterDelayTemp;
			TimeSpan LargeXenoEnterDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LargeXenoEnterDelay, ref LargeXenoEnterDelayTemp, hookCtx, false, context))
			{
				LargeXenoEnterDelayTemp = serialization.CreateCopy<TimeSpan>(LargeXenoEnterDelay, hookCtx, context, false);
			}
			target.LargeXenoEnterDelay = LargeXenoEnterDelayTemp;
			TimeSpan SmallXenoMoveDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(SmallXenoMoveDelay, ref SmallXenoMoveDelayTemp, hookCtx, false, context))
			{
				SmallXenoMoveDelayTemp = serialization.CreateCopy<TimeSpan>(SmallXenoMoveDelay, hookCtx, context, false);
			}
			target.SmallXenoMoveDelay = SmallXenoMoveDelayTemp;
			TimeSpan StandardXenoMoveDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StandardXenoMoveDelay, ref StandardXenoMoveDelayTemp, hookCtx, false, context))
			{
				StandardXenoMoveDelayTemp = serialization.CreateCopy<TimeSpan>(StandardXenoMoveDelay, hookCtx, context, false);
			}
			target.StandardXenoMoveDelay = StandardXenoMoveDelayTemp;
			TimeSpan LargeXenoMoveDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LargeXenoMoveDelay, ref LargeXenoMoveDelayTemp, hookCtx, false, context))
			{
				LargeXenoMoveDelayTemp = serialization.CreateCopy<TimeSpan>(LargeXenoMoveDelay, hookCtx, context, false);
			}
			target.LargeXenoMoveDelay = LargeXenoMoveDelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoTunnelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTunnelComponent cast = (XenoTunnelComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTunnelComponent cast = (XenoTunnelComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTunnelComponent def = (XenoTunnelComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoTunnelComponent Instantiate()
	{
		return new XenoTunnelComponent();
	}
}
