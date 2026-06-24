using System;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Crate;

public sealed class CrateOpenableSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CrateOpenableComponent, InteractUsingEvent>((EntityEventRefHandler<CrateOpenableComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<CrateOpenableComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (base.EntityManager.IsQueuedForDeletion(Entity<CrateOpenableComponent>.op_Implicit(ent)))
		{
			return;
		}
		if (!_tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.Tool)))
		{
			_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(ent.Comp.WrongToolPopup)), Entity<CrateOpenableComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_audio.PlayPredicted(ent.Comp.Sound, _transform.GetMoverCoordinates(Entity<CrateOpenableComponent>.op_Implicit(ent)), (EntityUid?)args.User, (AudioParams?)null);
		if (_net.IsClient)
		{
			return;
		}
		((EntitySystem)this).QueueDel((EntityUid?)Entity<CrateOpenableComponent>.op_Implicit(ent));
		EntityUid? val = default(EntityUid?);
		foreach (string spawn in EntitySpawnCollection.GetSpawns(ent.Comp.Spawn, _random))
		{
			((EntitySystem)this).TrySpawnNextTo(spawn, Entity<CrateOpenableComponent>.op_Implicit(ent), ref val, (TransformComponent)null, (ComponentRegistry)null);
		}
	}
}
