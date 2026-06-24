using System;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.AlertLevel;
using Content.Shared.Clock;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.AlertLevel;

public sealed class RMCAlertLevelDisplayVisualizerSystem : EntitySystem
{
	[Dependency]
	private SharedGameTicker _ticker;

	[Dependency]
	private RMCAlertLevelSystem _alertLevel;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Update(float frameTime)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_alertLevel.Get() > RMCAlertLevels.Green)
		{
			return;
		}
		EntityQueryEnumerator<RMCAlertLevelDisplayComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RMCAlertLevelDisplayComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		RMCAlertLevelDisplayComponent rMCAlertLevelDisplayComponent = default(RMCAlertLevelDisplayComponent);
		SpriteComponent item2 = default(SpriteComponent);
		int num = default(int);
		int num2 = default(int);
		int num3 = default(int);
		int num4 = default(int);
		int num5 = default(int);
		while (val.MoveNext(ref item, ref rMCAlertLevelDisplayComponent, ref item2))
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)RMCAlertLevelDisplayVisualLayers.HourTens, ref num, false) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)RMCAlertLevelDisplayVisualLayers.HourOnes, ref num2, false) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)RMCAlertLevelDisplayVisualLayers.Separator, ref num3, false) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)RMCAlertLevelDisplayVisualLayers.MinuteTens, ref num4, false) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)RMCAlertLevelDisplayVisualLayers.MinuteOnes, ref num5, false))
			{
				string text = ((((EntitySystem)this).EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault()?.TimeOffset ?? TimeSpan.Zero) + _ticker.RoundDuration()).ToString("hh\\:mm");
				string text2 = $"{text[0]}";
				string text3 = $"{text[1]}";
				string text4 = "~";
				string text5 = $"{text[3]}";
				string text6 = $"{text[4]}";
				_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((item, item2)), num, new Vector2(0.11f, -0.4375f));
				_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((item, item2)), num2, new Vector2(0.28f, -0.4375f));
				_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((item, item2)), num3, new Vector2(0.406f, -0.4375f));
				_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((item, item2)), num4, new Vector2(0.56f, -0.4375f));
				_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((item, item2)), num5, new Vector2(0.73f, -0.4375f));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num, StateId.op_Implicit(text2));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num2, StateId.op_Implicit(text3));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num3, StateId.op_Implicit(text4));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num4, StateId.op_Implicit(text5));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num5, StateId.op_Implicit(text6));
			}
		}
	}
}
