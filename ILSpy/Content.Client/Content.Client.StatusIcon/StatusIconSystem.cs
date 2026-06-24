using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Stealth;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Stealth.Components;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.StatusIcon;

public sealed class StatusIconSystem : SharedStatusIconSystem
{
	[Dependency]
	private IConfigurationManager _configuration;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	private bool _globalEnabled;

	private bool _localEnabled;

	public override void Initialize()
	{
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, CCVars.LocalStatusIconsEnabled, (Action<bool>)OnLocalStatusIconChanged, true);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, CCVars.GlobalStatusIconsEnabled, (Action<bool>)OnGlobalStatusIconChanged, true);
	}

	private void OnLocalStatusIconChanged(bool obj)
	{
		_localEnabled = obj;
		UpdateOverlayVisible();
	}

	private void OnGlobalStatusIconChanged(bool obj)
	{
		_globalEnabled = obj;
		UpdateOverlayVisible();
	}

	private void UpdateOverlayVisible()
	{
		if (!_overlay.RemoveOverlay<StatusIconOverlay>() && _globalEnabled && _localEnabled)
		{
			_overlay.AddOverlay((Overlay)(object)new StatusIconOverlay());
		}
	}

	public List<StatusIconData> GetStatusIcons(EntityUid uid, MetaDataComponent? meta = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		List<StatusIconData> list = new List<StatusIconData>();
		if (!((EntitySystem)this).Resolve(uid, ref meta, true))
		{
			return list;
		}
		if ((int)meta.EntityLifeStage >= 4)
		{
			return list;
		}
		GetStatusIconsEvent getStatusIconsEvent = new GetStatusIconsEvent(list);
		((EntitySystem)this).RaiseLocalEvent<GetStatusIconsEvent>(uid, ref getStatusIconsEvent, false);
		return getStatusIconsEvent.StatusIcons;
	}

	public bool IsVisible(Entity<MetaDataComponent> ent, StatusIconData data)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		EntityUid? val2 = val;
		EntityUid owner = ent.Owner;
		if (val2.HasValue && val2.GetValueOrDefault() == owner)
		{
			return true;
		}
		if (data.VisibleToGhosts && ((EntitySystem)this).HasComp<GhostComponent>(val))
		{
			return true;
		}
		if (data.HideInContainer && (ent.Comp.Flags & 2) != 0)
		{
			return false;
		}
		StealthComponent stealthComponent = default(StealthComponent);
		if (data.HideOnStealth && ((EntitySystem)this).TryComp<StealthComponent>(Entity<MetaDataComponent>.op_Implicit(ent), ref stealthComponent) && stealthComponent.Enabled)
		{
			return false;
		}
		SpriteComponent val3 = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<MetaDataComponent>.op_Implicit(ent), ref val3) && !val3.Visible)
		{
			return false;
		}
		if (data.HideOnStealth && ((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(Entity<MetaDataComponent>.op_Implicit(ent)))
		{
			return false;
		}
		if (data.ShowTo != null && !_entityWhitelist.IsValid(data.ShowTo, val))
		{
			return false;
		}
		return true;
	}
}
