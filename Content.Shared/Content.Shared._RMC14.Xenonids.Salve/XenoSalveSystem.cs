using System;
using Content.Shared._RMC14.Xenonids.Heal;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Salve;

public sealed class XenoSalveSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RecentlySalvedComponent, ComponentStartup>((EntityEventRefHandler<RecentlySalvedComponent, ComponentStartup>)OnSalveAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RecentlySalvedComponent, ComponentShutdown>((EntityEventRefHandler<RecentlySalvedComponent, ComponentShutdown>)OnSalveRemoved, (Type[])null, (Type[])null);
	}

	private void OnSalveAdded(Entity<RecentlySalvedComponent> xeno, ref ComponentStartup args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<RecentlySalvedComponent>.op_Implicit(xeno), (Enum)XenoHealerVisuals.Gooped, (object)true, (AppearanceComponent)null);
		}
	}

	private void OnSalveRemoved(Entity<RecentlySalvedComponent> xeno, ref ComponentShutdown args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<RecentlySalvedComponent>.op_Implicit(xeno), (Enum)XenoHealerVisuals.Gooped, (object)false, (AppearanceComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RecentlySalvedComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RecentlySalvedComponent>();
		EntityUid uid = default(EntityUid);
		RecentlySalvedComponent salve = default(RecentlySalvedComponent);
		while (query.MoveNext(ref uid, ref salve))
		{
			if (!(time < salve.ExpiresAt))
			{
				((EntitySystem)this).RemCompDeferred<RecentlySalvedComponent>(uid);
			}
		}
	}
}
