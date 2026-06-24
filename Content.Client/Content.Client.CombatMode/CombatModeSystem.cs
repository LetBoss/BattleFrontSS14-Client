using System;
using Content.Client.Hands.Systems;
using Content.Client.NPC.HTN;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.CombatMode;

public sealed class CombatModeSystem : SharedCombatModeSystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IEyeManager _eye;

	public event Action<bool>? LocalPlayerCombatModeUpdated;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CombatModeComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<CombatModeComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _cfg, CCVars.CombatModeIndicatorsPointShow, (Action<bool>)OnShowCombatIndicatorsChanged, true);
	}

	private void OnHandleState(EntityUid uid, CombatModeComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateHud(uid);
	}

	public override void Shutdown()
	{
		_overlayManager.RemoveOverlay<CombatModeIndicatorsOverlay>();
		((EntitySystem)this).Shutdown();
	}

	public bool IsInCombatMode()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return false;
		}
		return IsInCombatMode(localEntity.Value);
	}

	public override void SetInCombatMode(EntityUid entity, bool value, CombatModeComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.SetInCombatMode(entity, value, component);
		UpdateHud(entity);
	}

	protected override bool IsNpc(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).HasComp<HTNComponent>(uid);
	}

	private void UpdateHud(EntityUid entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(entity != localEntity.GetValueOrDefault()) && Timing.IsFirstTimePredicted)
		{
			bool obj = IsInCombatMode();
			this.LocalPlayerCombatModeUpdated?.Invoke(obj);
		}
	}

	private void OnShowCombatIndicatorsChanged(bool isShow)
	{
		if (isShow)
		{
			_overlayManager.AddOverlay((Overlay)(object)new CombatModeIndicatorsOverlay(_inputManager, (IEntityManager)(object)((EntitySystem)this).EntityManager, _eye, this, ((EntitySystem)this).EntityManager.System<HandsSystem>()));
		}
		else
		{
			_overlayManager.RemoveOverlay<CombatModeIndicatorsOverlay>();
		}
	}
}
