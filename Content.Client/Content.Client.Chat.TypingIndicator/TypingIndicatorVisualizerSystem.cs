using System;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Chat.TypingIndicator;

public sealed class TypingIndicatorVisualizerSystem : VisualizerSystem<TypingIndicatorComponent>
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private InventorySystem _inventory;

	protected override void OnAppearanceChange(EntityUid uid, TypingIndicatorComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		ProtoId<TypingIndicatorPrototype> val = component.TypingIndicatorPrototype;
		BeforeShowTypingIndicatorEvent args2 = new BeforeShowTypingIndicatorEvent();
		InventoryComponent item = default(InventoryComponent);
		if (((EntitySystem)this).TryComp<InventoryComponent>(uid, ref item))
		{
			_inventory.RelayEvent(Entity<InventoryComponent>.op_Implicit((uid, item)), ref args2);
		}
		ProtoId<TypingIndicatorPrototype>? mostRecentIndicator = args2.GetMostRecentIndicator();
		if (mostRecentIndicator.HasValue)
		{
			val = mostRecentIndicator.Value;
		}
		TypingIndicatorPrototype typingIndicatorPrototype = default(TypingIndicatorPrototype);
		if (!_prototypeManager.TryIndex<TypingIndicatorPrototype>(val, ref typingIndicatorPrototype))
		{
			((EntitySystem)this).Log.Error($"Unknown typing indicator id: {component.TypingIndicatorPrototype}");
			return;
		}
		int num = default(int);
		if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)TypingIndicatorLayers.Base, ref num, false))
		{
			num = base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)TypingIndicatorLayers.Base);
		}
		base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, typingIndicatorPrototype.SpritePath, (StateId?)null);
		base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(typingIndicatorPrototype.TypingState));
		args.Sprite.LayerSetShader(num, typingIndicatorPrototype.Shader);
		base.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, typingIndicatorPrototype.Offset);
		TypingIndicatorState typingIndicatorState = default(TypingIndicatorState);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<TypingIndicatorState>(uid, (Enum)TypingIndicatorVisuals.State, ref typingIndicatorState, (AppearanceComponent)null);
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, typingIndicatorState != TypingIndicatorState.None);
		switch (typingIndicatorState)
		{
		case TypingIndicatorState.Idle:
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(typingIndicatorPrototype.IdleState));
			break;
		case TypingIndicatorState.Typing:
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(typingIndicatorPrototype.TypingState));
			break;
		}
	}
}
