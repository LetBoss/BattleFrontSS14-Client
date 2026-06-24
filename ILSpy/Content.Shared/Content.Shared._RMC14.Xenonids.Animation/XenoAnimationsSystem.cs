using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Xenonids.Animation;

public sealed class XenoAnimationsSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	public void PlayLungeAnimationEvent(EntityUid entityUid, Vector2 direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		PlayLungeAnimationEvent ev = new PlayLungeAnimationEvent(((EntitySystem)this).GetNetEntity(entityUid, (MetaDataComponent)null), Vector2Helpers.Normalized(direction), _net.IsClient);
		if (_net.IsServer)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev);
		}
		else
		{
			((EntitySystem)this).RaiseLocalEvent<PlayLungeAnimationEvent>(ev);
		}
	}
}
