using System;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems;

public sealed class AutoOrientSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedMoverController _mover;

	private TimeSpan _delay = TimeSpan.Zero;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AutoOrientComponent, EntParentChangedMessage>((EntityEventRefHandler<AutoOrientComponent, EntParentChangedMessage>)OnEntParentChanged, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<double>(((EntitySystem)this).Subs, _cfgManager, CCVars.AutoOrientDelay, (Action<double>)OnAutoOrient, true);
	}

	private void OnAutoOrient(double obj)
	{
		_delay = TimeSpan.FromSeconds(obj);
	}

	private void OnEntParentChanged(Entity<AutoOrientComponent> ent, ref EntParentChangedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextChange = _timing.CurTime + _delay;
		((EntitySystem)this).Dirty<AutoOrientComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<AutoOrientComponent> query = ((EntitySystem)this).EntityQueryEnumerator<AutoOrientComponent>();
		EntityUid uid = default(EntityUid);
		AutoOrientComponent comp = default(AutoOrientComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.NextChange <= _timing.CurTime)
			{
				comp.NextChange = null;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				_mover.ResetCamera(uid);
			}
		}
	}
}
