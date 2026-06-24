using System;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;

public sealed class ResinWhispererSystem : EntitySystem
{
	[Dependency]
	private SharedDoorSystem _door;

	[Dependency]
	private ExamineSystemShared _examineSystem;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoWeedsSystem _weeds;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ResinDoorComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<ResinDoorComponent, GetVerbsEvent<AlternativeVerb>>)OnDoorAltVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinWhispererComponent, XenoSecreteStructureAdjustFields>((EntityEventRefHandler<ResinWhispererComponent, XenoSecreteStructureAdjustFields>)OnRemoteSecreteStructure, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinWhispererComponent, InRangeOverrideEvent>((EntityEventRefHandler<ResinWhispererComponent, InRangeOverrideEvent>)OnInRangeOverride, (Type[])null, (Type[])null);
	}

	private void OnDoorAltVerb(Entity<ResinDoorComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ResinWhispererComponent>(args.User))
		{
			return;
		}
		EntityUid target = args.Target;
		EntityUid user = args.User;
		args.Verbs.Add(new AlternativeVerb
		{
			Text = "Open Door",
			Impact = LogImpact.Low,
			Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				DoorComponent doorComponent = default(DoorComponent);
				if (CanRemoteOpenDoorPopup(Entity<ResinWhispererComponent>.op_Implicit(user), target) && ((EntitySystem)this).TryComp<DoorComponent>(target, ref doorComponent) && _door.TryToggleDoor(target, null, null, predicted: true))
				{
					if (doorComponent.State == DoorState.Opening)
					{
						_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-remote-open-door"), user, user);
					}
					if (doorComponent.State == DoorState.Closing)
					{
						_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-remote-close-door"), user, user);
					}
				}
			},
			Priority = 100
		});
	}

	private bool CanRemoteOpenDoorPopup(Entity<ResinWhispererComponent?> user, EntityUid target, bool doPopup = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ResinWhispererComponent>(Entity<ResinWhispererComponent>.op_Implicit(user), ref user.Comp, false))
		{
			return false;
		}
		if (!_weeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(user.Owner)))
		{
			if (doPopup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-remote-failed-need-on-weeds"), Entity<ResinWhispererComponent>.op_Implicit(user), Entity<ResinWhispererComponent>.op_Implicit(user));
			}
			return false;
		}
		if (!((EntitySystem)this).HasComp<DoorComponent>(target) || !((EntitySystem)this).HasComp<ResinDoorComponent>(target))
		{
			return false;
		}
		return true;
	}

	private void OnRemoteSecreteStructure(Entity<ResinWhispererComponent> ent, ref XenoSecreteStructureAdjustFields args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		XenoConstructionComponent constructComp = default(XenoConstructionComponent);
		if (!((EntitySystem)this).TryComp<XenoConstructionComponent>(Entity<ResinWhispererComponent>.op_Implicit(ent), ref constructComp))
		{
			return;
		}
		if (ent.Comp.StandardConstructDelay.HasValue)
		{
			constructComp.BuildDelay = ent.Comp.StandardConstructDelay.Value;
		}
		else
		{
			ent.Comp.StandardConstructDelay = constructComp.BuildDelay;
		}
		if (ent.Comp.MaxConstructDistance.HasValue)
		{
			constructComp.BuildRange = ent.Comp.MaxConstructDistance.Value;
		}
		else
		{
			ent.Comp.MaxConstructDistance = constructComp.BuildRange;
		}
		if (!_interaction.InRangeUnobstructed(Entity<ResinWhispererComponent>.op_Implicit(ent), args.TargetCoordinates, ent.Comp.MaxConstructDistance.Value.Float()))
		{
			if (!TileIsVisible(ent, args.TargetCoordinates))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-remote-failed-need-line-of-sight"), Entity<ResinWhispererComponent>.op_Implicit(ent), Entity<ResinWhispererComponent>.op_Implicit(ent));
				return;
			}
			if (!_weeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(ent.Owner)))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-remote-failed-need-on-weeds"), Entity<ResinWhispererComponent>.op_Implicit(ent), Entity<ResinWhispererComponent>.op_Implicit(ent));
				return;
			}
			constructComp.BuildDelay = ent.Comp.StandardConstructDelay.Value.Multiply(ent.Comp.RemoteConstructDelayMultiplier);
			constructComp.BuildRange = ent.Comp.MaxRemoteConstructDistance;
		}
	}

	private void OnInRangeOverride(Entity<ResinWhispererComponent> ent, ref InRangeOverrideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (CanRemoteOpenDoorPopup(Entity<ResinWhispererComponent>.op_Implicit(ent.Owner), args.Target, doPopup: false))
		{
			args.InRange = true;
			args.Handled = true;
		}
	}

	private bool TileIsVisible(Entity<ResinWhispererComponent> ent, EntityCoordinates targetCoordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates pointCoordinates = _transform.ToMapCoordinates(targetCoordinates, true);
		for (int i = 0; i < 9; i++)
		{
			switch (i)
			{
			case 1:
			case 7:
			case 8:
				pointCoordinates = ((MapCoordinates)(ref pointCoordinates)).Offset(0.499f, 0f);
				break;
			case 2:
				pointCoordinates = ((MapCoordinates)(ref pointCoordinates)).Offset(0f, -0.499f);
				break;
			case 3:
			case 4:
				pointCoordinates = ((MapCoordinates)(ref pointCoordinates)).Offset(-0.499f, 0f);
				break;
			case 5:
			case 6:
				pointCoordinates = ((MapCoordinates)(ref pointCoordinates)).Offset(0f, 0.499f);
				break;
			}
			if (_examineSystem.InRangeUnOccluded(Entity<ResinWhispererComponent>.op_Implicit(ent), pointCoordinates, ent.Comp.MaxRemoteConstructDistance))
			{
				return true;
			}
		}
		return false;
	}
}
