using System;
using System.Numerics;
using Content.Shared.Solar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Power;

public sealed class SolarControlNotARadar : Control
{
	[Dependency]
	private IGameTiming _gameTiming;

	private SolarControlConsoleBoundInterfaceState _lastState = new SolarControlConsoleBoundInterfaceState(Angle.op_Implicit(0f), Angle.op_Implicit(0f), 0f, Angle.op_Implicit(0f));

	private TimeSpan _lastStateTime = TimeSpan.Zero;

	public const int StandardSizeFull = 290;

	public const int StandardRadiusCircle = 140;

	public int SizeFull => (int)(290f * ((Control)this).UIScale);

	public int RadiusCircle => (int)(140f * ((Control)this).UIScale);

	public Angle PredictedPanelRotation => _lastState.Rotation + Angle.op_Implicit(Angle.op_Implicit(_lastState.AngularVelocity) * (_gameTiming.CurTime - _lastStateTime).TotalSeconds);

	public SolarControlNotARadar()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SolarControlNotARadar>(this);
		((Control)this).MinSize = new Vector2(SizeFull, SizeFull);
	}

	public void UpdateState(SolarControlConsoleBoundInterfaceState ls)
	{
		_lastState = ls;
		_lastStateTime = _gameTiming.CurTime;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		int num = SizeFull / 2;
		Color val = default(Color);
		((Color)(ref val))._002Ector(0.08f, 0.08f, 0.08f, 1f);
		Color val2 = default(Color);
		((Color)(ref val2))._002Ector(0.08f, 0.08f, 0.08f, 1f);
		int num2 = 4;
		int num3 = 8;
		int num4 = 8;
		((DrawingHandleBase)handle).DrawCircle(new Vector2(num, num), (float)(RadiusCircle + 1), val, true);
		((DrawingHandleBase)handle).DrawCircle(new Vector2(num, num), (float)RadiusCircle, Color.Black, true);
		for (int i = 0; i < num4; i++)
		{
			((DrawingHandleBase)handle).DrawCircle(new Vector2(num, num), (float)(RadiusCircle / num4 * i), val2, false);
		}
		for (int j = 0; j < num3; j++)
		{
			Angle val3 = Angle.op_Implicit(Math.PI / (double)num3 * (double)j);
			Vector2 vector = ((Angle)(ref val3)).ToVec() * RadiusCircle;
			((DrawingHandleBase)handle).DrawLine(new Vector2(num, num) - vector, new Vector2(num, num) + vector, val2);
		}
		Vector2 vector2 = new Vector2(1f, -1f);
		Angle val4 = default(Angle);
		((Angle)(ref val4))._002Ector(-Math.PI / 2.0);
		Angle val5 = PredictedPanelRotation + val4;
		Vector2 vector3 = ((Angle)(ref val5)).ToVec() * vector2 * RadiusCircle;
		Vector2 vector4 = new Vector2(vector3.Y, 0f - vector3.X);
		((DrawingHandleBase)handle).DrawLine(new Vector2(num, num) - vector4, new Vector2(num, num) + vector4, Color.White);
		((DrawingHandleBase)handle).DrawLine(new Vector2(num, num) + vector3 / num2, new Vector2(num, num) + vector3 - vector3 / num2, Color.DarkGray);
		val5 = _lastState.TowardsSun + val4;
		Vector2 vector5 = ((Angle)(ref val5)).ToVec() * vector2 * RadiusCircle;
		((DrawingHandleBase)handle).DrawLine(new Vector2(num, num) + vector5, new Vector2(num, num), Color.Yellow);
	}
}
