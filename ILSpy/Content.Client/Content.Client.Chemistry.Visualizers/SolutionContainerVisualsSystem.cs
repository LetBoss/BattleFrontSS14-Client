using System;
using Content.Client.Items.Systems;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Hands;
using Content.Shared.Item;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Chemistry.Visualizers;

public sealed class SolutionContainerVisualsSystem : VisualizerSystem<SolutionContainerVisualsComponent>
{
	[Dependency]
	private RMCReagentSystem _reagent;

	[Dependency]
	private ItemSystem _itemSystem;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerVisualsComponent, MapInitEvent>((ComponentEventHandler<SolutionContainerVisualsComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerVisualsComponent, GetInhandVisualsEvent>((ComponentEventHandler<SolutionContainerVisualsComponent, GetInhandVisualsEvent>)OnGetHeldVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerVisualsComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<SolutionContainerVisualsComponent, GetEquipmentVisualsEvent>)OnGetClothingVisuals, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, SolutionContainerVisualsComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent val = ((EntitySystem)this).MetaData(uid);
		component.InitialDescription = val.EntityDescription;
	}

	protected override void OnAppearanceChange(EntityUid uid, SolutionContainerVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		float num = default(float);
		int num2 = default(int);
		if ((!string.IsNullOrEmpty(component.SolutionName) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)SolutionContainerVisuals.SolutionName, ref text, args.Component) && text != component.SolutionName) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(uid, (Enum)SolutionContainerVisuals.FillFraction, ref num, args.Component) || args.Sprite == null || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)component.Layer, ref num2, false))
		{
			return;
		}
		int num3 = component.MaxFillLevels;
		string text2 = component.FillBaseName;
		bool flag = component.ChangeColor;
		SpriteSpecifier val = component.MetamorphicDefaultSprite;
		if (num > 1f)
		{
			((EntitySystem)this).Log.Error("Attempted to set solution container visuals volume ratio on " + EntityStringRepresentation.op_Implicit(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))) + " to a value greater than 1. Volume should never be greater than max volume!");
			num = 1f;
		}
		if (component.Metamorphic)
		{
			int num4 = default(int);
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)component.BaseLayer, ref num4, false))
			{
				int num5 = default(int);
				bool flag2 = base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)component.OverlayLayer, ref num5, false);
				string text3 = default(string);
				if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)SolutionContainerVisuals.BaseOverride, ref text3, args.Component))
				{
					_reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(text3), out Reagent reagent);
					SpriteSpecifier val2 = reagent?.MetamorphicSprite;
					if (val2 != null)
					{
						base.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, val2);
						if (reagent.MetamorphicMaxFillLevels > 0)
						{
							base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
							num3 = reagent.MetamorphicMaxFillLevels;
							text2 = reagent.MetamorphicFillBaseName;
							flag = reagent.MetamorphicChangeColor;
							val = val2;
						}
						else
						{
							base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, false);
						}
						if (flag2)
						{
							base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num5, false);
						}
					}
					else
					{
						base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
						if (flag2)
						{
							base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num5, true);
						}
						if (component.MetamorphicDefaultSprite != null)
						{
							base.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, component.MetamorphicDefaultSprite);
						}
					}
				}
			}
		}
		else
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
		}
		int num6 = ContentHelpers.RoundToLevels(num, 1.0, num3 + 1);
		if (num6 > 0)
		{
			if (text2 == null)
			{
				return;
			}
			string text4 = text2 + num6;
			if (val != null)
			{
				base.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, val);
			}
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, StateId.op_Implicit(text4));
			Color val3 = default(Color);
			if (flag && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)SolutionContainerVisuals.Color, ref val3, args.Component))
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, val3);
			}
			else
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, Color.White);
			}
		}
		else if (component.EmptySpriteName == null)
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, false);
		}
		else
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, StateId.op_Implicit(component.EmptySpriteName));
			if (flag)
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, component.EmptySpriteColor);
			}
			else
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, Color.White);
			}
		}
		_itemSystem.VisualsChanged(uid);
	}

	private void OnGetHeldVisuals(EntityUid uid, SolutionContainerVisualsComponent component, GetInhandVisualsEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent val = default(AppearanceComponent);
		ItemComponent itemComponent = default(ItemComponent);
		float num = default(float);
		if (component.InHandsFillBaseName == null || !((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val) || !((EntitySystem)this).TryComp<ItemComponent>(uid, ref itemComponent) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(uid, (Enum)SolutionContainerVisuals.FillFraction, ref num, val))
		{
			return;
		}
		int num2 = ContentHelpers.RoundToLevels(num, 1.0, component.InHandsMaxFillLevels + 1);
		if (num2 > 0)
		{
			PrototypeLayerData val2 = new PrototypeLayerData();
			string item = (val2.State = ((itemComponent.HeldPrefix == null) ? "inhand-" : (itemComponent.HeldPrefix + "-inhand-")) + args.Location.ToString().ToLowerInvariant() + component.InHandsFillBaseName + num2);
			Color value = default(Color);
			if (component.ChangeColor && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)SolutionContainerVisuals.Color, ref value, val))
			{
				val2.Color = value;
			}
			args.Layers.Add((item, val2));
		}
	}

	private void OnGetClothingVisuals(Entity<SolutionContainerVisualsComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent val = default(AppearanceComponent);
		ClothingComponent clothingComponent = default(ClothingComponent);
		float num = default(float);
		if (ent.Comp.EquippedFillBaseName == null || !((EntitySystem)this).TryComp<AppearanceComponent>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), ref val) || !((EntitySystem)this).TryComp<ClothingComponent>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), ref clothingComponent) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), (Enum)SolutionContainerVisuals.FillFraction, ref num, val))
		{
			return;
		}
		int num2 = ContentHelpers.RoundToLevels(num, 1.0, ent.Comp.EquippedMaxFillLevels + 1);
		if (num2 <= 0)
		{
			return;
		}
		PrototypeLayerData val2 = new PrototypeLayerData();
		string text = ((clothingComponent.EquippedPrefix == null) ? ("equipped-" + args.Slot) : (" " + clothingComponent.EquippedPrefix + "-equipped-" + args.Slot)) + ent.Comp.EquippedFillBaseName + num2;
		SpriteComponent val3 = default(SpriteComponent);
		State val4 = default(State);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), ref val3) && val3.BaseRSI != null && val3.BaseRSI.TryGetState(StateId.op_Implicit(text), ref val4))
		{
			val2.State = text;
			Color value = default(Color);
			if (ent.Comp.ChangeColor && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), (Enum)SolutionContainerVisuals.Color, ref value, val))
			{
				val2.Color = value;
			}
			args.Layers.Add((text, val2));
		}
	}
}
