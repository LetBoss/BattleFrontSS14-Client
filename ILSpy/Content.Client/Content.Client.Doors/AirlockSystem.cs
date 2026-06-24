using System;
using Content.Client.Wires.Visualizers;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Power;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Doors;

public sealed class AirlockSystem : SharedAirlockSystem
{
	[Dependency]
	private AppearanceSystem _appearanceSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, ComponentStartup>((ComponentEventHandler<AirlockComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, AppearanceChangeEvent>((ComponentEventRefHandler<AirlockComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnComponentStartup(EntityUid uid, AirlockComponent comp, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0127: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		DoorComponent doorComponent = default(DoorComponent);
		if (((EntitySystem)this).TryComp<DoorComponent>(uid, ref doorComponent))
		{
			if (comp.OpenUnlitVisible)
			{
				doorComponent.OpenSpriteStates.Add((DoorVisualLayers.BaseUnlit, comp.OpenSpriteState));
				doorComponent.ClosedSpriteStates.Add((DoorVisualLayers.BaseUnlit, comp.ClosedSpriteState));
			}
			((Animation)doorComponent.OpeningAnimation).AnimationTracks.Add((AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.BaseUnlit,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.OpeningSpriteState), 0f)
				}
			});
			((Animation)doorComponent.ClosingAnimation).AnimationTracks.Add((AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.BaseUnlit,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.ClosingSpriteState), 0f)
				}
			});
			doorComponent.DenyingAnimation = (object)new Animation
			{
				Length = TimeSpan.FromSeconds(comp.DenyAnimationTime),
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = DoorVisualLayers.BaseUnlit,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(comp.DenySpriteState), 0f)
					}
				} }
			};
			if (comp.AnimatePanel)
			{
				((Animation)doorComponent.OpeningAnimation).AnimationTracks.Add((AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = WiresVisualLayers.MaintenancePanel,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(comp.OpeningPanelSpriteState), 0f)
					}
				});
				((Animation)doorComponent.ClosingAnimation).AnimationTracks.Add((AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = WiresVisualLayers.MaintenancePanel,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(comp.ClosingPanelSpriteState), 0f)
					}
				});
			}
		}
	}

	private void OnAppearanceChange(EntityUid uid, AirlockComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			DoorState doorState = default(DoorState);
			if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<DoorState>(uid, (Enum)DoorVisuals.State, ref doorState, args.Component))
			{
				doorState = DoorState.Closed;
			}
			bool flag4 = default(bool);
			if (((SharedAppearanceSystem)_appearanceSystem).TryGetData<bool>(uid, (Enum)PowerDeviceVisuals.Powered, ref flag4, args.Component) && flag4)
			{
				bool flag5 = default(bool);
				flag = ((SharedAppearanceSystem)_appearanceSystem).TryGetData<bool>(uid, (Enum)DoorVisuals.BoltLights, ref flag5, args.Component) && flag5 && (doorState == DoorState.Closed || doorState == DoorState.Welded);
				bool flag6 = default(bool);
				flag2 = ((SharedAppearanceSystem)_appearanceSystem).TryGetData<bool>(uid, (Enum)DoorVisuals.EmergencyLights, ref flag6, args.Component) && flag6;
				bool flag7 = default(bool);
				flag3 = (doorState == DoorState.Closing || doorState == DoorState.Opening || doorState == DoorState.Denying || (doorState == DoorState.Open && comp.OpenUnlitVisible) || (((SharedAppearanceSystem)_appearanceSystem).TryGetData<bool>(uid, (Enum)DoorVisuals.ClosedLights, ref flag7, args.Component) && flag7)) && !flag && !flag2;
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseUnlit, flag3);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseBolted, flag);
			if (comp.EmergencyAccessLayer)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseEmergencyAccess, flag2 && doorState != DoorState.Open && doorState != DoorState.Opening && doorState != DoorState.Closing && !flag);
			}
			switch (doorState)
			{
			case DoorState.Open:
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseUnlit, StateId.op_Implicit(comp.ClosingSpriteState));
				_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseUnlit, 0f);
				break;
			case DoorState.Closed:
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseUnlit, StateId.op_Implicit(comp.OpeningSpriteState));
				_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseUnlit, 0f);
				break;
			}
		}
	}
}
