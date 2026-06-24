using System;
using System.Numerics;
using Content.Shared.Alert.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Alerts;

public sealed class GenericCounterAlertSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GenericCounterAlertComponent, UpdateAlertSpriteEvent>((EntityEventRefHandler<GenericCounterAlertComponent, UpdateAlertSpriteEvent>)OnUpdateAlertSprite, (Type[])null, (Type[])null);
	}

	private void OnUpdateAlertSprite(Entity<GenericCounterAlertComponent> ent, ref UpdateAlertSpriteEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent comp = args.SpriteViewEnt.Comp;
		GetGenericAlertCounterAmountEvent getGenericAlertCounterAmountEvent = new GetGenericAlertCounterAmountEvent(args.Alert);
		((EntitySystem)this).RaiseLocalEvent<GetGenericAlertCounterAmountEvent>(args.ViewerEnt, ref getGenericAlertCounterAmountEvent, false);
		if (!getGenericAlertCounterAmountEvent.Handled || !getGenericAlertCounterAmountEvent.Amount.HasValue)
		{
			return;
		}
		int maxDigitCount = GetMaxDigitCount(Entity<GenericCounterAlertComponent, SpriteComponent>.op_Implicit((Entity<GenericCounterAlertComponent>.op_Implicit(ent), Entity<GenericCounterAlertComponent>.op_Implicit(ent), comp)));
		int num = (int)Math.Clamp(getGenericAlertCounterAmountEvent.Amount.Value, 0.0, Math.Pow(10.0, maxDigitCount) - 1.0);
		int num2 = (ent.Comp.HideLeadingZeroes ? num.ToString().Length : maxDigitCount);
		if (ent.Comp.HideLeadingZeroes)
		{
			int num3 = default(int);
			for (int i = 0; i < ent.Comp.DigitKeys.Count; i++)
			{
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(ent.Owner), ent.Comp.DigitKeys[i], ref num3, false))
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(ent.Owner), num3, i <= num2 - 1);
				}
			}
		}
		float num4 = (float)((ent.Comp.AlertSize.X - num2 * ent.Comp.GlyphWidth) / 2) * (1f / 32f);
		int num5 = default(int);
		for (int j = 0; j < ent.Comp.DigitKeys.Count; j++)
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(ent.Owner), ent.Comp.DigitKeys[j], ref num5, false))
			{
				int num6 = num / (int)Math.Pow(10.0, j) % 10;
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit(ent.Owner), num5, StateId.op_Implicit(num6.ToString()));
				if (ent.Comp.CenterGlyph)
				{
					float x = num4 + (float)((num2 - 1 - j) * ent.Comp.GlyphWidth) * (1f / 32f);
					_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit(ent.Owner), num5, new Vector2(x, 0f));
				}
			}
		}
	}

	private int GetMaxDigitCount(Entity<GenericCounterAlertComponent, SpriteComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		for (int num = ent.Comp1.DigitKeys.Count - 1; num >= 0; num--)
		{
			if (_sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), ent.Comp1.DigitKeys[num]))
			{
				return num + 1;
			}
		}
		return 0;
	}
}
