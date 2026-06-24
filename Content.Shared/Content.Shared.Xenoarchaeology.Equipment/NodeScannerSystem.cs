using System;
using Content.Shared.Interaction;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Equipment;

public sealed class NodeScannerSystem : EntitySystem
{
	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<NodeScannerComponent, BeforeRangedInteractEvent>((ComponentEventHandler<NodeScannerComponent, BeforeRangedInteractEvent>)OnBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NodeScannerComponent, GetVerbsEvent<UtilityVerb>>((ComponentEventHandler<NodeScannerComponent, GetVerbsEvent<UtilityVerb>>)AddScanVerb, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<NodeScannerConnectedComponent, NodeScannerComponent, TransformComponent> scannerQuery = ((EntitySystem)this).EntityQueryEnumerator<NodeScannerConnectedComponent, NodeScannerComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		NodeScannerConnectedComponent connected = default(NodeScannerConnectedComponent);
		NodeScannerComponent scanner = default(NodeScannerComponent);
		TransformComponent transform = default(TransformComponent);
		while (scannerQuery.MoveNext(ref uid, ref connected, ref scanner, ref transform))
		{
			if (!(connected.NextUpdate > _timing.CurTime))
			{
				connected.NextUpdate = _timing.CurTime + connected.LinkUpdateInterval;
				EntityUid attachedArtifact = connected.AttachedTo;
				EntityCoordinates artifactCoordinates = ((EntitySystem)this).Transform(attachedArtifact).Coordinates;
				if (!_transform.InRange(artifactCoordinates, transform.Coordinates, (float)scanner.MaxLinkedRange))
				{
					((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)connected);
				}
			}
		}
	}

	private void OnBeforeRangedInteract(EntityUid uid, NodeScannerComponent component, BeforeRangedInteractEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<XenoArtifactComponent>(target2))
			{
				XenoArtifactUnlockingComponent unlockingComponent = default(XenoArtifactUnlockingComponent);
				Entity<XenoArtifactUnlockingComponent> unlockingEnt = Entity<XenoArtifactUnlockingComponent>.op_Implicit(((EntitySystem)this).TryComp<XenoArtifactUnlockingComponent>(target2, ref unlockingComponent) ? (target2, unlockingComponent) : (target2, null));
				Attach(Entity<NodeScannerComponent>.op_Implicit((uid, component)), unlockingEnt, args.User);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void AddScanVerb(EntityUid uid, NodeScannerComponent component, GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactUnlockingComponent unlockingComponent = default(XenoArtifactUnlockingComponent);
		if (args.CanAccess && ((EntitySystem)this).TryComp<XenoArtifactUnlockingComponent>(args.Target, ref unlockingComponent))
		{
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					Attach(Entity<NodeScannerComponent>.op_Implicit((uid, component)), Entity<XenoArtifactUnlockingComponent>.op_Implicit((args.Target, unlockingComponent)), args.User);
				},
				Text = base.Loc.GetString("node-scan-tooltip")
			};
			args.Verbs.Add(verb);
		}
	}

	private void Attach(Entity<NodeScannerComponent> device, Entity<XenoArtifactUnlockingComponent?> unlockingEnt, EntityUid actor)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (_timing.IsFirstTimePredicted && (!((EntitySystem)this).TryComp<UseDelayComponent>(Entity<NodeScannerComponent>.op_Implicit(device), ref useDelay) || _useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<NodeScannerComponent>.op_Implicit(device), useDelay)), checkDelayed: true)))
		{
			NodeScannerConnectedComponent connected = ((EntitySystem)this).EnsureComp<NodeScannerConnectedComponent>(Entity<NodeScannerComponent>.op_Implicit(device));
			EntityUid artifact = Entity<XenoArtifactUnlockingComponent>.op_Implicit(unlockingEnt);
			if (connected.AttachedTo != artifact)
			{
				connected.AttachedTo = artifact;
				((EntitySystem)this).Dirty(Entity<NodeScannerComponent>.op_Implicit(device), (IComponent)(object)connected, (MetaDataComponent)null);
			}
			_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit((ValueTuple<EntityUid, UserInterfaceComponent>)(Entity<NodeScannerComponent>.op_Implicit(device), null)), (Enum)NodeScannerUiKey.Key, actor, true);
		}
	}
}
