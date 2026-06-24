using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Disposal;

public sealed class PressureBar : ProgressBar
{
	public bool UpdatePressure(TimeSpan fullTime)
	{
		TimeSpan curTime = IoCManager.Resolve<IGameTiming>().CurTime;
		float num = (float)Math.Min(1.0, 1.0 - (fullTime.TotalSeconds - curTime.TotalSeconds) * 0.05000000074505806);
		UpdatePressureBar(num);
		return num >= 1f;
	}

	private void UpdatePressureBar(float pressure)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0067: Expected O, but got Unknown
		((Range)this).Value = pressure;
		float num = pressure / ((Range)this).MaxValue;
		float x;
		if (num <= 0.5f)
		{
			num /= 0.5f;
			x = MathHelper.Lerp(0f, 0.066f, num);
		}
		else
		{
			num = (num - 0.5f) / 0.5f;
			x = MathHelper.Lerp(0.066f, 0.33f, num);
		}
		if (((ProgressBar)this).ForegroundStyleBoxOverride == null)
		{
			StyleBoxFlat val = new StyleBoxFlat();
			StyleBox val2 = (StyleBox)val;
			((ProgressBar)this).ForegroundStyleBoxOverride = (StyleBox)val;
		}
		((StyleBoxFlat)((ProgressBar)this).ForegroundStyleBoxOverride).BackgroundColor = Color.FromHsv(new Vector4(x, 1f, 0.8f, 1f));
	}
}
