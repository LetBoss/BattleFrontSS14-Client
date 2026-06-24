using System;
using Content.Shared.Input;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Shuttles.Systems;

public sealed class ShuttleConsoleSystem : SharedShuttleConsoleSystem
{
	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IPlayerManager _playerManager;

	public override void Initialize()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PilotComponent, ComponentHandleState>((ComponentEventRefHandler<PilotComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		IInputCmdContext obj = _input.Contexts.New("shuttle", "common");
		obj.AddFunction(ContentKeyFunctions.ShuttleStrafeUp);
		obj.AddFunction(ContentKeyFunctions.ShuttleStrafeDown);
		obj.AddFunction(ContentKeyFunctions.ShuttleStrafeLeft);
		obj.AddFunction(ContentKeyFunctions.ShuttleStrafeRight);
		obj.AddFunction(ContentKeyFunctions.ShuttleRotateLeft);
		obj.AddFunction(ContentKeyFunctions.ShuttleRotateRight);
		obj.AddFunction(ContentKeyFunctions.ShuttleBrake);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_input.Contexts.Remove("shuttle");
	}

	protected override void HandlePilotShutdown(EntityUid uid, PilotComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.HandlePilotShutdown(uid, component, args);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != uid))
		{
			_input.Contexts.SetActiveContext("human");
		}
	}

	private void OnHandleState(EntityUid uid, PilotComponent component, ref ComponentHandleState args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is PilotComponentState pilotComponentState)
		{
			EntityUid? val = ((EntitySystem)this).EnsureEntity<PilotComponent>(pilotComponentState.Console, uid);
			if (!val.HasValue)
			{
				component.Console = null;
				_input.Contexts.SetActiveContext("human");
			}
			else if (!((EntitySystem)this).HasComp<ShuttleConsoleComponent>(val))
			{
				((EntitySystem)this).Log.Warning($"Unable to set Helmsman console to {val}");
			}
			else
			{
				component.Console = val;
				ActionBlockerSystem.UpdateCanMove(uid);
				_input.Contexts.SetActiveContext("shuttle");
			}
		}
	}
}
