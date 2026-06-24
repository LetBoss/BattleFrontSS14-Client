using System;
using Content.Shared.NPC;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NPC.HTN;

public sealed class HTNSystem : EntitySystem
{
	private bool _enableOverlay;

	public bool EnableOverlay
	{
		get
		{
			return _enableOverlay;
		}
		set
		{
			IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
			_enableOverlay = value;
			if (_enableOverlay)
			{
				val.AddOverlay((Overlay)(object)new HTNOverlay((IEntityManager)(object)base.EntityManager, IoCManager.Resolve<IResourceCache>()));
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestHTNMessage
				{
					Enabled = true
				});
			}
			else
			{
				val.RemoveOverlay<HTNOverlay>();
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestHTNMessage
				{
					Enabled = false
				});
			}
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<HTNMessage>((EntityEventHandler<HTNMessage>)OnHTNMessage, (Type[])null, (Type[])null);
	}

	private void OnHTNMessage(HTNMessage ev)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		HTNComponent hTNComponent = default(HTNComponent);
		if (((EntitySystem)this).TryComp<HTNComponent>(((EntitySystem)this).GetEntity(ev.Uid), ref hTNComponent))
		{
			hTNComponent.DebugText = ev.Text;
		}
	}
}
