using System;
using Content.Client.Gameplay;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared._RMC14.Weapons.Common;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Weapons.Ranged.Sniper;

public sealed class RMCSpottingSystem : SharedRMCSpottingSystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IStateManager _state;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpottingComponent, UniqueActionEvent>((EntityEventRefHandler<SpottingComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequestSpotEvent>((EntitySessionEventHandler<RequestSpotEvent>)OnSpotRequest, (Type[])null, (Type[])null);
	}

	private void OnUniqueAction(Entity<SpottingComponent> ent, ref UniqueActionEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.IsFirstTimePredicted && !((HandledEntityEventArgs)args).Handled && ((ISharedPlayerManager)_player).LocalEntity.HasValue && ent.Comp.Activated)
		{
			MapCoordinates coordinates = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);
			NetEntity? val = null;
			if (_state.CurrentState is GameplayStateBase gameplayStateBase)
			{
				val = ((EntitySystem)this).GetNetEntity(gameplayStateBase.GetClickedEntity(coordinates), (MetaDataComponent)null);
			}
			if (((ISharedPlayerManager)_player).LocalSession != null && val.HasValue)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestSpotEvent>(new RequestSpotEvent
				{
					Target = val.Value,
					User = ((EntitySystem)this).GetNetEntity(args.UserUid, (MetaDataComponent)null),
					SpottingTool = ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null)
				});
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnSpotRequest(RequestSpotEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		SpotRequested(ev.SpottingTool, ev.User, ev.Target);
	}
}
