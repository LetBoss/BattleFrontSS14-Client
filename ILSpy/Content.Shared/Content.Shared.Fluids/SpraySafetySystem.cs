using System;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Fluids.Components;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Fluids;

public sealed class SpraySafetySystem : EntitySystem
{
	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpraySafetyComponent, SolutionTransferAttemptEvent>((EntityEventRefHandler<SpraySafetyComponent, SolutionTransferAttemptEvent>)OnTransferAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpraySafetyComponent, SolutionTransferredEvent>((EntityEventRefHandler<SpraySafetyComponent, SolutionTransferredEvent>)OnTransferred, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpraySafetyComponent, SprayAttemptEvent>((EntityEventRefHandler<SpraySafetyComponent, SprayAttemptEvent>)OnSprayAttempt, (Type[])null, (Type[])null);
	}

	private void OnTransferAttempt(Entity<SpraySafetyComponent> ent, ref SolutionTransferAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Entity<SpraySafetyComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SpraySafetyComponent spraySafetyComponent = default(SpraySafetyComponent);
		val.Deconstruct(ref val2, ref spraySafetyComponent);
		EntityUid uid = val2;
		SpraySafetyComponent comp = spraySafetyComponent;
		if (uid == args.To && !_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(uid)))
		{
			args.Cancel(base.Loc.GetString(LocId.op_Implicit(comp.Popup)));
		}
	}

	private void OnTransferred(Entity<SpraySafetyComponent> ent, ref SolutionTransferredEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_audio.PlayPredicted(ent.Comp.RefillSound, Entity<SpraySafetyComponent>.op_Implicit(ent), args.User, (AudioParams?)null);
	}

	private void OnSprayAttempt(Entity<SpraySafetyComponent> ent, ref SprayAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)))
		{
			_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)), Entity<SpraySafetyComponent>.op_Implicit(ent), args.User);
			args.Cancel();
		}
	}
}
