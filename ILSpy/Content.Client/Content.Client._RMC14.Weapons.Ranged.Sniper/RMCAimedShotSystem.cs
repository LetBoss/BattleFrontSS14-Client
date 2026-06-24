using System;
using Content.Client.Gameplay;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Weapons.Ranged.Sniper;

public sealed class RMCAimedShotSystem : SharedRMCAimedShotSystem
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
		((EntitySystem)this).SubscribeLocalEvent<AimedShotComponent, UniqueActionEvent>((EntityEventRefHandler<AimedShotComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequestAimedShotEvent>((EntitySessionEventHandler<RequestAimedShotEvent>)OnAimedShotRequest, (Type[])null, (Type[])null);
	}

	private void OnUniqueAction(Entity<AimedShotComponent> ent, ref UniqueActionEvent args)
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
				((EntitySystem)this).RaisePredictiveEvent<RequestAimedShotEvent>(new RequestAimedShotEvent
				{
					Target = val.Value,
					User = ((EntitySystem)this).GetNetEntity(args.UserUid, (MetaDataComponent)null),
					Gun = ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null)
				});
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnAimedShotRequest(RequestAimedShotEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		AimedShotRequested(ev.Gun, ev.User, ev.Target);
	}
}
