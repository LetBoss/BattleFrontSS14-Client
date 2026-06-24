using System;
using Content.Shared.Clock;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Clock;

public sealed class ClockSystem : SharedClockSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ClockComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ClockComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		ClockComponent clockComponent = default(ClockComponent);
		SpriteComponent item2 = default(SpriteComponent);
		int num = default(int);
		int num2 = default(int);
		while (val.MoveNext(ref item, ref clockComponent, ref item2))
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)ClockVisualLayers.HourHand, ref num, false) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)ClockVisualLayers.MinuteHand, ref num2, false))
			{
				TimeSpan clockTime = GetClockTime(Entity<ClockComponent>.op_Implicit((item, clockComponent)));
				string text = $"{clockComponent.HoursBase}{clockTime.Hours % 12}";
				string text2 = $"{clockComponent.MinutesBase}{clockTime.Minutes / 5}";
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num, StateId.op_Implicit(text));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((item, item2)), num2, StateId.op_Implicit(text2));
			}
		}
	}
}
