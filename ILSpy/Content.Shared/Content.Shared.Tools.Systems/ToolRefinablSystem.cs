using System;
using Content.Shared.Construction;
using Content.Shared.Interaction;
using Content.Shared.Storage;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Tools.Systems;

public sealed class ToolRefinablSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedToolSystem _toolSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToolRefinableComponent, InteractUsingEvent>((ComponentEventHandler<ToolRefinableComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolRefinableComponent, WelderRefineDoAfterEvent>((ComponentEventHandler<ToolRefinableComponent, WelderRefineDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(EntityUid uid, ToolRefinableComponent component, InteractUsingEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = _toolSystem.UseTool(args.Used, args.User, uid, component.RefineTime, ProtoId<ToolQualityPrototype>.op_Implicit(component.QualityNeeded), new WelderRefineDoAfterEvent(), component.RefineFuel);
		}
	}

	private void OnDoAfter(EntityUid uid, ToolRefinableComponent component, WelderRefineDoAfterEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || _net.IsClient)
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		foreach (string spawn in EntitySpawnCollection.GetSpawns(component.RefineResult, _random))
		{
			((EntitySystem)this).SpawnNextToOrDrop(spawn, uid, xform, (ComponentRegistry)null);
		}
		((EntitySystem)this).Del((EntityUid?)uid);
	}
}
