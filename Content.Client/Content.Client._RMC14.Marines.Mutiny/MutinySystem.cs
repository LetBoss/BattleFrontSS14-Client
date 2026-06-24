using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Mutiny;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Marines.Mutiny;

public sealed class MutinySystem : SharedMutinySystem
{
	protected override void MutineerAdded(Entity<MutineerComponent> ent, ref ComponentAdd args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		MarineComponent marineComponent = default(MarineComponent);
		if (((EntitySystem)this).TryComp<MarineComponent>(Entity<MutineerComponent>.op_Implicit(ent), ref marineComponent))
		{
			((EntitySystem)this).Dirty<MutineerComponent>(ent, (MetaDataComponent)null);
		}
	}

	protected override void MutineerRemoved(Entity<MutineerComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		MarineComponent marineComponent = default(MarineComponent);
		if (((EntitySystem)this).TryComp<MarineComponent>(Entity<MutineerComponent>.op_Implicit(ent), ref marineComponent))
		{
			((EntitySystem)this).Dirty<MutineerComponent>(ent, (MetaDataComponent)null);
		}
	}
}
