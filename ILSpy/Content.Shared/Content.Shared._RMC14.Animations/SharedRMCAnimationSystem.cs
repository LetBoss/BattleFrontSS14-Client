using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Animations;

public abstract class SharedRMCAnimationSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	public void Play(Entity<RMCAnimationComponent?> ent, string key)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		NetEntity? netEnt = default(NetEntity?);
		if (!_net.IsClient && ((EntitySystem)this).TryGetNetEntity(Entity<RMCAnimationComponent>.op_Implicit(ent), ref netEnt, (MetaDataComponent)null))
		{
			RMCPlayAnimationEvent ev = new RMCPlayAnimationEvent(netEnt.Value, new RMCAnimationId(key));
			Filter filter = Filter.Pvs(Entity<RMCAnimationComponent>.op_Implicit(ent), 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, filter, true);
		}
	}

	public void Flick(Entity<RMCAnimationComponent?> ent, Rsi animationRsi, Rsi defaultRsi, string? layer = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		NetEntity? netEnt = default(NetEntity?);
		if (!_net.IsClient && ((EntitySystem)this).TryGetNetEntity(Entity<RMCAnimationComponent>.op_Implicit(ent), ref netEnt, (MetaDataComponent)null))
		{
			RMCFlickEvent ev = new RMCFlickEvent(netEnt.Value, animationRsi, defaultRsi, layer);
			Filter filter = Filter.Pvs(Entity<RMCAnimationComponent>.op_Implicit(ent), 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, filter, true);
		}
	}

	public void TryFlick(Entity<RMCAnimationComponent?> ent, Rsi? animationRsi, Rsi? defaultRsi, string? layer = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (animationRsi != null && defaultRsi != null)
		{
			Flick(ent, animationRsi, defaultRsi, layer);
		}
	}
}
