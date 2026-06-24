using System;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public abstract class BaseXATSystem<T> : EntitySystem where T : Component
{
	protected delegate void XATEventHandler<TEvent>(Entity<XenoArtifactComponent> artifact, Entity<T, XenoArtifactNodeComponent> node, ref TEvent args) where TEvent : notnull;

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected SharedXenoArtifactSystem XenoArtifact;

	private EntityQuery<XenoArtifactUnlockingComponent> _unlockingQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_unlockingQuery = ((EntitySystem)this).GetEntityQuery<XenoArtifactUnlockingComponent>();
	}

	protected void XATSubscribeDirectEvent<TEvent>(XATEventHandler<TEvent> eventHandler) where TEvent : notnull
	{
		((EntitySystem)this).SubscribeLocalEvent<T, XenoArchNodeRelayedEvent<TEvent>>((ComponentEventHandler<T, XenoArchNodeRelayedEvent<TEvent>>)delegate(EntityUid uid, T component, XenoArchNodeRelayedEvent<TEvent> args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			XenoArtifactNodeComponent xenoArtifactNodeComponent = ((EntitySystem)this).Comp<XenoArtifactNodeComponent>(uid);
			if (CanTrigger(args.Artifact, Entity<XenoArtifactNodeComponent>.op_Implicit((uid, xenoArtifactNodeComponent))))
			{
				Entity<T, XenoArtifactNodeComponent> node = default(Entity<T, XenoArtifactNodeComponent>);
				node._002Ector(uid, component, xenoArtifactNodeComponent);
				eventHandler(args.Artifact, node, ref args.Args);
			}
		}, (Type[])null, (Type[])null);
	}

	protected bool CanTrigger(Entity<XenoArtifactComponent> artifact, Entity<XenoArtifactNodeComponent> node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.CurTime < artifact.Comp.NextUnlockTime)
		{
			return false;
		}
		XenoArtifactUnlockingComponent unlocking = default(XenoArtifactUnlockingComponent);
		if (_unlockingQuery.TryComp(Entity<XenoArtifactComponent>.op_Implicit(artifact), ref unlocking) && unlocking.TriggeredNodeIndexes.Contains(XenoArtifact.GetIndex(artifact, Entity<XenoArtifactNodeComponent>.op_Implicit(node))))
		{
			return false;
		}
		if (!XenoArtifact.CanUnlockNode(Entity<XenoArtifactNodeComponent>.op_Implicit((Entity<XenoArtifactNodeComponent>.op_Implicit(node), Entity<XenoArtifactNodeComponent>.op_Implicit(node)))))
		{
			return false;
		}
		return true;
	}

	protected void Trigger(Entity<XenoArtifactComponent> artifact, Entity<T, XenoArtifactNodeComponent> node)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.IsFirstTimePredicted)
		{
			((EntitySystem)this).Log.Debug($"Activated trigger {typeof(T).Name} on node {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<T, XenoArtifactNodeComponent>.op_Implicit(node), (MetaDataComponent)null)} for {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoArtifactComponent>.op_Implicit(artifact), (MetaDataComponent)null)}");
			XenoArtifact.TriggerXenoArtifact(artifact, Entity<XenoArtifactNodeComponent>.op_Implicit((node.Owner, node.Comp2)));
		}
	}
}
