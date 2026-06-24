using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.VendingMachines;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Client.VendingMachines;

public sealed class VendingMachineSystem : SharedVendingMachineSystem
{
	[Dependency]
	private AnimationPlayerSystem _animationPlayer;

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineComponent, AppearanceChangeEvent>((ComponentEventRefHandler<VendingMachineComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineComponent, AnimationCompletedEvent>((ComponentEventHandler<VendingMachineComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineComponent, ComponentHandleState>((EntityEventRefHandler<VendingMachineComponent, ComponentHandleState>)OnVendingHandleState, (Type[])null, (Type[])null);
	}

	private void OnVendingHandleState(Entity<VendingMachineComponent> entity, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is VendingMachineComponentState vendingMachineComponentState))
		{
			return;
		}
		EntityUid owner = entity.Owner;
		VendingMachineComponent comp = entity.Comp;
		comp.Contraband = vendingMachineComponentState.Contraband;
		comp.EjectEnd = vendingMachineComponentState.EjectEnd;
		comp.DenyEnd = vendingMachineComponentState.DenyEnd;
		comp.DispenseOnHitEnd = vendingMachineComponentState.DispenseOnHitEnd;
		bool flag = !comp.Inventory.Keys.SequenceEqual(vendingMachineComponentState.Inventory.Keys) || !comp.EmaggedInventory.Keys.SequenceEqual(vendingMachineComponentState.EmaggedInventory.Keys) || !comp.ContrabandInventory.Keys.SequenceEqual(vendingMachineComponentState.ContrabandInventory.Keys);
		comp.Inventory.Clear();
		comp.EmaggedInventory.Clear();
		comp.ContrabandInventory.Clear();
		foreach (KeyValuePair<string, VendingMachineInventoryEntry> item in vendingMachineComponentState.Inventory)
		{
			comp.Inventory.Add(item.Key, new VendingMachineInventoryEntry(item.Value));
		}
		foreach (KeyValuePair<string, VendingMachineInventoryEntry> item2 in vendingMachineComponentState.EmaggedInventory)
		{
			comp.EmaggedInventory.Add(item2.Key, new VendingMachineInventoryEntry(item2.Value));
		}
		foreach (KeyValuePair<string, VendingMachineInventoryEntry> item3 in vendingMachineComponentState.ContrabandInventory)
		{
			comp.ContrabandInventory.Add(item3.Key, new VendingMachineInventoryEntry(item3.Value));
		}
		VendingMachineBoundUserInterface vendingMachineBoundUserInterface = default(VendingMachineBoundUserInterface);
		if (UISystem.TryGetOpenUi<VendingMachineBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(owner), (Enum)VendingMachineUiKey.Key, ref vendingMachineBoundUserInterface))
		{
			if (flag)
			{
				vendingMachineBoundUserInterface.Refresh();
			}
			else
			{
				vendingMachineBoundUserInterface.UpdateAmounts();
			}
		}
	}

	protected override void UpdateUI(Entity<VendingMachineComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		VendingMachineBoundUserInterface vendingMachineBoundUserInterface = default(VendingMachineBoundUserInterface);
		if (((EntitySystem)this).Resolve<VendingMachineComponent>(Entity<VendingMachineComponent>.op_Implicit(entity), ref entity.Comp, true) && UISystem.TryGetOpenUi<VendingMachineBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)VendingMachineUiKey.Key, ref vendingMachineBoundUserInterface))
		{
			vendingMachineBoundUserInterface.UpdateAmounts();
		}
	}

	private void OnAnimationCompleted(EntityUid uid, VendingMachineComponent component, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite))
		{
			AppearanceComponent val = default(AppearanceComponent);
			VendingMachineVisualState visualState = default(VendingMachineVisualState);
			if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val) || !_appearanceSystem.TryGetData<VendingMachineVisualState>(uid, (Enum)VendingMachineVisuals.VisualState, ref visualState, val))
			{
				visualState = VendingMachineVisualState.Normal;
			}
			UpdateAppearance(uid, visualState, component, sprite);
		}
	}

	private void OnAppearanceChange(EntityUid uid, VendingMachineComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			object value;
			VendingMachineVisualState visualState = ((args.AppearanceData.TryGetValue(VendingMachineVisuals.VisualState, out value) && value is VendingMachineVisualState) ? ((VendingMachineVisualState)value) : VendingMachineVisualState.Normal);
			UpdateAppearance(uid, visualState, component, args.Sprite);
		}
	}

	private void UpdateAppearance(EntityUid uid, VendingMachineVisualState visualState, VendingMachineComponent component, SpriteComponent sprite)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		SetLayerState(VendingMachineVisualLayers.Base, component.OffState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
		switch (visualState)
		{
		case VendingMachineVisualState.Normal:
			SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.NormalState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			break;
		case VendingMachineVisualState.Deny:
			if (component.LoopDenyAnimation)
			{
				SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.DenyState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			}
			else
			{
				PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.DenyState, (float)component.DenyDelay.TotalSeconds, sprite);
			}
			SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			break;
		case VendingMachineVisualState.Eject:
			PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.EjectState, (float)component.EjectDelay.TotalSeconds, sprite);
			SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			break;
		case VendingMachineVisualState.Broken:
			HideLayers(Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			SetLayerState(VendingMachineVisualLayers.Base, component.BrokenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			break;
		case VendingMachineVisualState.Off:
			HideLayers(Entity<SpriteComponent>.op_Implicit((uid, sprite)));
			break;
		}
	}

	private void SetLayerState(VendingMachineVisualLayers layer, string? state, Entity<SpriteComponent> sprite)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(state))
		{
			_sprite.LayerSetVisible(sprite.AsNullable(), (Enum)layer, true);
			_sprite.LayerSetAutoAnimated(sprite.AsNullable(), (Enum)layer, true);
			_sprite.LayerSetRsiState(sprite.AsNullable(), (Enum)layer, StateId.op_Implicit(state));
		}
	}

	private void PlayAnimation(EntityUid uid, VendingMachineVisualLayers layer, string? state, float animationTime, SpriteComponent sprite)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(state) && !_animationPlayer.HasRunningAnimation(uid, state))
		{
			Animation animation = GetAnimation(layer, state, animationTime);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)layer, true);
			_animationPlayer.Play(uid, animation, state);
		}
	}

	private static Animation GetAnimation(VendingMachineVisualLayers layer, string state, float animationTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Expected O, but got Unknown
		return new Animation
		{
			Length = TimeSpan.FromSeconds(animationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = layer,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(state), 0f)
				}
			} }
		};
	}

	private void HideLayers(Entity<SpriteComponent> sprite)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		HideLayer(VendingMachineVisualLayers.BaseUnshaded, sprite);
		HideLayer(VendingMachineVisualLayers.Screen, sprite);
	}

	private void HideLayer(VendingMachineVisualLayers layer, Entity<SpriteComponent> sprite)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (_sprite.LayerMapTryGet(sprite.AsNullable(), (Enum)layer, ref num, false))
		{
			_sprite.LayerSetVisible(sprite.AsNullable(), num, false);
		}
	}
}
