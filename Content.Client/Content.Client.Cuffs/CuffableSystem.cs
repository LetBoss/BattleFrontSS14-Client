using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Cuffs;

public sealed class CuffableSystem : SharedCuffableSystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, ComponentShutdown>((ComponentEventHandler<CuffableComponent, ComponentShutdown>)OnCuffableShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, ComponentHandleState>((ComponentEventRefHandler<CuffableComponent, ComponentHandleState>)OnCuffableHandleState, (Type[])null, (Type[])null);
	}

	private void OnCuffableShutdown(EntityUid uid, CuffableComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)HumanoidVisualLayers.Handcuffs, false);
		}
	}

	private void OnCuffableHandleState(EntityUid uid, CuffableComponent component, ref ComponentHandleState args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is CuffableComponentState cuffableComponentState))
		{
			return;
		}
		component.CanStillInteract = cuffableComponentState.CanStillInteract;
		_actionBlocker.UpdateCanMove(uid);
		CuffedStateChangeEvent cuffedStateChangeEvent = default(CuffedStateChangeEvent);
		((EntitySystem)this).RaiseLocalEvent<CuffedStateChangeEvent>(uid, ref cuffedStateChangeEvent, false);
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			return;
		}
		bool flag = cuffableComponentState.NumHandsCuffed > 0;
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)HumanoidVisualLayers.Handcuffs, flag);
		if (flag)
		{
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)HumanoidVisualLayers.Handcuffs, cuffableComponentState.Color.Value);
			if (!object.Equals(component.CurrentRSI, cuffableComponentState.RSI) && cuffableComponentState.RSI != null)
			{
				component.CurrentRSI = cuffableComponentState.RSI;
				_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, item)), _sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)HumanoidVisualLayers.Handcuffs), new ResPath(component.CurrentRSI), (StateId?)StateId.op_Implicit(cuffableComponentState.IconState));
			}
			else
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)HumanoidVisualLayers.Handcuffs, StateId.op_Implicit(cuffableComponentState.IconState));
			}
		}
	}
}
