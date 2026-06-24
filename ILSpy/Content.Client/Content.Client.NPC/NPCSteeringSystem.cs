using System;
using Content.Shared.NPC;
using Content.Shared.NPC.Events;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NPC;

public sealed class NPCSteeringSystem : SharedNPCSteeringSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	private bool _debugEnabled;

	public bool DebugEnabled
	{
		get
		{
			return _debugEnabled;
		}
		set
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (_debugEnabled == value)
			{
				return;
			}
			_debugEnabled = value;
			if (_debugEnabled)
			{
				_overlay.AddOverlay((Overlay)(object)new NPCSteeringOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager));
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestNPCSteeringDebugEvent
				{
					Enabled = true
				});
				return;
			}
			_overlay.RemoveOverlay<NPCSteeringOverlay>();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestNPCSteeringDebugEvent
			{
				Enabled = false
			});
			AllEntityQueryEnumerator<NPCSteeringComponent> val = ((EntitySystem)this).AllEntityQuery<NPCSteeringComponent>();
			EntityUid val2 = default(EntityUid);
			NPCSteeringComponent nPCSteeringComponent = default(NPCSteeringComponent);
			while (val.MoveNext(ref val2, ref nPCSteeringComponent))
			{
				((EntitySystem)this).RemCompDeferred<NPCSteeringComponent>(val2);
			}
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<NPCSteeringDebugEvent>((EntityEventHandler<NPCSteeringDebugEvent>)OnDebugEvent, (Type[])null, (Type[])null);
	}

	private void OnDebugEvent(NPCSteeringDebugEvent ev)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!DebugEnabled)
		{
			return;
		}
		foreach (NPCSteeringDebugData datum in ev.Data)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(datum.EntityUid);
			if (((EntitySystem)this).Exists(entity))
			{
				NPCSteeringComponent nPCSteeringComponent = ((EntitySystem)this).EnsureComp<NPCSteeringComponent>(entity);
				nPCSteeringComponent.Direction = datum.Direction;
				nPCSteeringComponent.DangerMap = datum.Danger;
				nPCSteeringComponent.InterestMap = datum.Interest;
				nPCSteeringComponent.DangerPoints = datum.DangerPoints;
			}
		}
	}
}
