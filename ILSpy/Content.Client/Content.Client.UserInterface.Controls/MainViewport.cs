using System.Numerics;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class MainViewport : UIWidget
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private ViewportManager _vpManager;

	public ScalingViewport Viewport { get; }

	public MainViewport()
	{
		IoCManager.InjectDependencies<MainViewport>(this);
		ScalingViewport scalingViewport = new ScalingViewport();
		((Control)scalingViewport).AlwaysRender = true;
		scalingViewport.RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt;
		((Control)scalingViewport).MouseFilter = (MouseFilterMode)0;
		Viewport = scalingViewport;
		((Control)this).AddChild((Control)(object)Viewport);
	}

	protected override void EnteredTree()
	{
		((Control)this).EnteredTree();
		_vpManager.AddViewport(this);
	}

	protected override void ExitedTree()
	{
		((Control)this).ExitedTree();
		_vpManager.RemoveViewport(this);
	}

	public void UpdateCfg()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		bool cVar = _cfg.GetCVar<bool>(CCVars.ViewportStretch);
		bool cVar2 = _cfg.GetCVar<bool>(CCVars.ViewportScaleRender);
		int num = _cfg.GetCVar<int>(CCVars.ViewportFixedScaleFactor);
		bool cVar3 = _cfg.GetCVar<bool>(CCVars.ViewportVerticalFit);
		if (cVar)
		{
			int? num2 = CalcSnappingFactor();
			if (!num2.HasValue)
			{
				Viewport.FixedStretchSize = null;
				Viewport.StretchMode = ScalingViewportStretchMode.Bilinear;
				Viewport.IgnoreDimension = (cVar3 ? ScalingViewportIgnoreDimension.Horizontal : ScalingViewportIgnoreDimension.None);
				if (cVar2)
				{
					Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt;
					return;
				}
				Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
				Viewport.FixedRenderScale = 1;
				return;
			}
			num = num2.Value;
		}
		Viewport.FixedStretchSize = Viewport.ViewportSize * num;
		Viewport.StretchMode = ScalingViewportStretchMode.Nearest;
		if (cVar2)
		{
			Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
			Viewport.FixedRenderScale = num;
		}
		else
		{
			Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
			Viewport.FixedRenderScale = 1;
		}
	}

	private int? CalcSnappingFactor()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int cVar = _cfg.GetCVar<int>(CCVars.ViewportSnapToleranceMargin);
		int cVar2 = _cfg.GetCVar<int>(CCVars.ViewportSnapToleranceClip);
		bool cVar3 = _cfg.GetCVar<bool>(CCVars.ViewportVerticalFit);
		float num = default(float);
		float num2 = default(float);
		for (int i = 1; i <= 10; i++)
		{
			int toleranceMargin = i * cVar;
			int toleranceClip = i * cVar2;
			Vector2 vector = Vector2i.op_Implicit(Viewport.ViewportSize) * i;
			Vector2Helpers.Deconstruct(Vector2i.op_Implicit(((Control)this).PixelSize) - vector, ref num, ref num2);
			float a = num;
			float a2 = num2;
			if (((Fits(a) || cVar3) && Fits(a2)) || (!cVar3 && Fits(a) && Larger(a2)) || (Larger(a) && Fits(a2)))
			{
				return i;
			}
			bool Fits(float num3)
			{
				if (num3 <= (float)toleranceMargin)
				{
					return num3 >= (float)(-toleranceClip);
				}
				return false;
			}
			bool Larger(float num3)
			{
				return num3 > (float)toleranceMargin;
			}
		}
		return null;
	}

	protected override void Resized()
	{
		((Control)this).Resized();
		UpdateCfg();
	}
}
