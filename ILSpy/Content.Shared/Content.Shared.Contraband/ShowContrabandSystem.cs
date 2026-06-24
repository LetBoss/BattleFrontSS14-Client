using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Contraband;

public sealed class ShowContrabandSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).Subs.SubscribeWithRelay<ShowContrabandDetailsComponent, GetContrabandDetailsEvent>((EntityEventRefHandler<ShowContrabandDetailsComponent, GetContrabandDetailsEvent>)OnGetContrabandDetails, true, true, true);
	}

	private void OnGetContrabandDetails(Entity<ShowContrabandDetailsComponent> ent, ref GetContrabandDetailsEvent args)
	{
		args.CanShowContraband = true;
	}
}
