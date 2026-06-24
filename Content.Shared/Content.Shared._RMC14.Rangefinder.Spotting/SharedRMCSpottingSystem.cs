using System;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Targeting;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Rangefinder.Spotting;

public abstract class SharedRMCSpottingSystem : EntitySystem
{
	[Dependency]
	private SharedRMCTargetingSystem _targeting;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	protected IGameTiming Timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SpottingComponent, GetItemActionsEvent>((EntityEventRefHandler<SpottingComponent, GetItemActionsEvent>)OnSpotterGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpottingComponent, SpotTargetActionEvent>((EntityEventRefHandler<SpottingComponent, SpotTargetActionEvent>)OnSpotTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpottingComponent, TargetingFinishedEvent>((EntityEventRefHandler<SpottingComponent, TargetingFinishedEvent>)OnTargetingFinished, (Type[])null, (Type[])null);
	}

	private void OnSpotterGetItemActions(Entity<SpottingComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	private void OnSpotTarget(Entity<SpottingComponent> ent, ref SpotTargetActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Activated = !ent.Comp.Activated;
		((EntitySystem)this).Dirty<SpottingComponent>(ent, (MetaDataComponent)null);
		SharedActionsSystem actions = _actions;
		EntityUid? action = ent.Comp.Action;
		actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), ent.Comp.Activated);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnTargetingFinished(Entity<SpottingComponent> ent, ref TargetingFinishedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		TargetingComponent targeting = default(TargetingComponent);
		if (((EntitySystem)this).TryComp<TargetingComponent>(Entity<SpottingComponent>.op_Implicit(ent), ref targeting))
		{
			_targeting.StopTargeting(Entity<TargetingComponent>.op_Implicit((Entity<SpottingComponent>.op_Implicit(ent), targeting)), args.Target);
		}
	}

	protected void SpotRequested(NetEntity netSpottingTool, NetEntity netUser, NetEntity netTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		EntityUid spottingTool = ((EntitySystem)this).GetEntity(netSpottingTool);
		EntityUid user = ((EntitySystem)this).GetEntity(netUser);
		EntityUid target = ((EntitySystem)this).GetEntity(netTarget);
		SpottingComponent spotting = default(SpottingComponent);
		if (((EntitySystem)this).TryComp<SpottingComponent>(spottingTool, ref spotting) && CanSpot(Entity<SpottingComponent>.op_Implicit((spottingTool, spotting)), target, user))
		{
			EntityTurnInvisibleComponent invisible = default(EntityTurnInvisibleComponent);
			TargetingLaserComponent targeting = default(TargetingLaserComponent);
			if (((EntitySystem)this).TryComp<EntityTurnInvisibleComponent>(user, ref invisible) && ((EntitySystem)this).TryComp<TargetingLaserComponent>(spottingTool, ref targeting))
			{
				targeting.ShowLaser = !invisible.Enabled;
				((EntitySystem)this).Dirty(spottingTool, (IComponent)(object)targeting, (MetaDataComponent)null);
			}
			spotting.NextSpot = Timing.CurTime + spotting.SpottingCooldown;
			((EntitySystem)this).Dirty(spottingTool, (IComponent)(object)spotting, (MetaDataComponent)null);
			SpottedComponent spotted = ((EntitySystem)this).EnsureComp<SpottedComponent>(target);
			((EntitySystem)this).Dirty(target, (IComponent)(object)spotted, (MetaDataComponent)null);
			_audio.PlayPredicted(spotting.SpottingSound, spottingTool, (EntityUid?)user, (AudioParams?)null);
			object layer = default(object);
			_appearance.TryGetData(spottingTool, (Enum)RangefinderLayers.Layer, ref layer, (AppearanceComponent)null);
			if (layer != null)
			{
				_appearance.SetData(spottingTool, (Enum)RangefinderLayers.Layer, (object)RangefinderMode.Spotter, (AppearanceComponent)null);
			}
			_targeting.Target(spottingTool, user, target, spotting.SpottingDuration, TargetedEffects.Spotted);
		}
	}

	private bool CanSpot(Entity<SpottingComponent> ent, EntityUid target, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<SpottableComponent>(target))
		{
			return false;
		}
		if (!_examine.InRangeUnOccluded(user, target, ent.Comp.SpottingRange))
		{
			return false;
		}
		if (ent.Comp.NextSpot > Timing.CurTime)
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<SpotterWhitelistComponent>(user))
		{
			string message = base.Loc.GetString("rmc-action-popup-spotting-user-no-skill", (ValueTuple<string, object>)("rangefinder", ent));
			_popup.PopupClient(message, user, user);
			return false;
		}
		if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var heldItem))
		{
			EntityUid? val = heldItem;
			EntityUid val2 = Entity<SpottingComponent>.op_Implicit(ent);
			if (val.HasValue && !(val.GetValueOrDefault() != val2))
			{
				return true;
			}
		}
		string message2 = base.Loc.GetString("rmc-action-popup-spotting-user-must-hold", (ValueTuple<string, object>)("rangefinder", ent));
		_popup.PopupClient(message2, user, user);
		return false;
	}
}
