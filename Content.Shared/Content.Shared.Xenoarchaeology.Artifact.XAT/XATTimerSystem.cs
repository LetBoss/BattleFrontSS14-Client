using System;
using Content.Shared.Examine;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATTimerSystem : BaseQueryUpdateXATSystem<XATTimerComponent>
{
	[Dependency]
	private IRobustRandom _robustRandom;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XATTimerComponent, MapInitEvent>((EntityEventRefHandler<XATTimerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		XATSubscribeDirectEvent<ExaminedEvent>(OnExamine);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityQueryEnumerator<XATTimerComponent> timerQuery = ((EntitySystem)this).EntityQueryEnumerator<XATTimerComponent>();
		EntityUid uid = default(EntityUid);
		XATTimerComponent timer = default(XATTimerComponent);
		while (timerQuery.MoveNext(ref uid, ref timer))
		{
			if (!(Timing.CurTime < timer.NextActivation))
			{
				timer.NextActivation += GetNextDelay(timer);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)timer, (MetaDataComponent)null);
			}
		}
	}

	protected override void UpdateXAT(Entity<XenoArtifactComponent> artifact, Entity<XATTimerComponent, XenoArtifactNodeComponent> node, float frameTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.CurTime > node.Comp1.NextActivation)
		{
			Trigger(artifact, node);
		}
	}

	private void OnMapInit(Entity<XATTimerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan delay = GetNextDelay(Entity<XATTimerComponent>.op_Implicit(ent));
		ent.Comp.NextActivation = Timing.CurTime + delay;
		((EntitySystem)this).Dirty<XATTimerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnExamine(Entity<XenoArtifactComponent> artifact, Entity<XATTimerComponent, XenoArtifactNodeComponent> node, ref ExaminedEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(((EntitySystem)this).Loc.GetString("xenoarch-trigger-examine-timer", (ValueTuple<string, object>)("time", MathF.Ceiling((float)(node.Comp1.NextActivation - Timing.CurTime).TotalSeconds))));
		}
	}

	private TimeSpan GetNextDelay(XATTimerComponent comp)
	{
		return TimeSpan.FromSeconds(comp.PossibleDelayInSeconds.Next(_robustRandom));
	}
}
