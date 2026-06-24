using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.Laser;

public sealed class GunToggleableLaserSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableLaserComponent, GetItemActionsEvent>((EntityEventRefHandler<GunToggleableLaserComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableLaserComponent, GunToggleLaserActionEvent>((EntityEventRefHandler<GunToggleableLaserComponent, GunToggleLaserActionEvent>)OnToggleLaser, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<GunToggleableLaserComponent> ent, ref GetItemActionsEvent args)
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

	private void OnToggleLaser(Entity<GunToggleableLaserComponent> ent, ref GunToggleLaserActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ToggleLaser(ent, args.Performer))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool ToggleLaser(Entity<GunToggleableLaserComponent> ent, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<GunToggleableLaserComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		ent.Comp.Active = !ent.Comp.Active;
		if (ent.Comp.Settings.Count == 0)
		{
			return false;
		}
		ref int settingIndex = ref ent.Comp.Setting;
		settingIndex++;
		if (settingIndex >= ent.Comp.Settings.Count)
		{
			settingIndex = 0;
		}
		GunToggleableLaserSetting setting = ent.Comp.Settings[settingIndex];
		EntityUid? action = ent.Comp.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)(object)setting.Icon);
		}
		((EntitySystem)this).Dirty<GunToggleableLaserComponent>(ent, (MetaDataComponent)null);
		return true;
	}
}
