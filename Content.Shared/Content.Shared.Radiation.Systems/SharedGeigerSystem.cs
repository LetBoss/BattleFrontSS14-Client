using System;
using Content.Shared.Examine;
using Content.Shared.Radiation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Radiation.Systems;

public abstract class SharedGeigerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GeigerComponent, ExaminedEvent>((ComponentEventHandler<GeigerComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnExamine(EntityUid uid, GeigerComponent component, ExaminedEvent args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (component.ShowExamine && component.IsEnabled && args.IsInDetailsRange)
		{
			float currentRads = component.CurrentRadiation;
			string rads = currentRads.ToString("N1");
			Color color = LevelToColor(component.DangerLevel);
			string msg = base.Loc.GetString("geiger-component-examine", (ValueTuple<string, object>)("rads", rads), (ValueTuple<string, object>)("color", color));
			args.PushMarkup(msg);
		}
	}

	public static Color LevelToColor(GeigerDangerLevel level)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		switch (level)
		{
		case GeigerDangerLevel.None:
			return Color.Green;
		case GeigerDangerLevel.Low:
			return Color.Yellow;
		case GeigerDangerLevel.Med:
			return Color.DarkOrange;
		case GeigerDangerLevel.High:
		case GeigerDangerLevel.Extreme:
			return Color.Red;
		default:
			return Color.White;
		}
	}
}
