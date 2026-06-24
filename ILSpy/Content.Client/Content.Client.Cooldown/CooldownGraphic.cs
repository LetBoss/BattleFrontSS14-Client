using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Cooldown;

public sealed class CooldownGraphic : Control
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("CooldownAnimation");

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IPrototypeManager _protoMan;

	private readonly ShaderInstance _shader;

	public float Progress { get; set; }

	public CooldownGraphic()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<CooldownGraphic>(this);
		_shader = _protoMan.Index<ShaderPrototype>(Shader).InstanceUnique();
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		_ = (Span<float>)new float[10];
		float num = 1f - MathF.Abs(Progress);
		Color val = default(Color);
		if (Progress >= 0f)
		{
			val = Color.FromHsv(new Vector4(5f / 18f * num, 0.75f, 0.75f, 0.5f));
		}
		else
		{
			float num2 = MathHelper.Clamp(0.5f * num, 0f, 0.5f);
			((Color)(ref val))._002Ector(1f, 1f, 1f, num2);
		}
		_shader.SetParameter("progress", Progress);
		((DrawingHandleBase)handle).UseShader(_shader);
		handle.DrawRect(UIBox2i.op_Implicit(((Control)this).PixelSizeBox), val, true);
		((DrawingHandleBase)handle).UseShader((ShaderInstance)null);
	}

	public void FromTime(TimeSpan start, TimeSpan end)
	{
		TimeSpan timeSpan = end - start;
		TimeSpan curTime = _gameTiming.CurTime;
		double totalSeconds = timeSpan.TotalSeconds;
		double num = (curTime - start).TotalSeconds / totalSeconds;
		double num2 = ((num <= 1.0) ? (1.0 - num) : ((curTime - end).TotalSeconds * -5.0));
		Progress = MathHelper.Clamp((float)num2, -1f, 1f);
		((Control)this).Visible = num2 > -1.0;
	}
}
