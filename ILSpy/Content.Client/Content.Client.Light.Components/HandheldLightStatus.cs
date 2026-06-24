using System.Numerics;
using Content.Shared.Light.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Light.Components;

public sealed class HandheldLightStatus : Control
{
	private const float TimerCycle = 1f;

	private readonly HandheldLightComponent _parent;

	private readonly PanelContainer[] _sections = (PanelContainer[])(object)new PanelContainer[5];

	private float _timer;

	private static readonly StyleBoxFlat StyleBoxLit = new StyleBoxFlat
	{
		BackgroundColor = Color.LimeGreen
	};

	private static readonly StyleBoxFlat StyleBoxUnlit = new StyleBoxFlat
	{
		BackgroundColor = Color.Black
	};

	public HandheldLightStatus(HandheldLightComponent parent)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		_parent = parent;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 4,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)this).AddChild((Control)(object)val);
		for (int i = 0; i < _sections.Length; i++)
		{
			PanelContainer val2 = new PanelContainer
			{
				MinSize = new Vector2(20f, 20f)
			};
			((Control)val).AddChild((Control)(object)val2);
			_sections[i] = val2;
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		_timer += ((FrameEventArgs)(ref args)).DeltaSeconds;
		_timer %= 1f;
		byte? level = _parent.Level;
		for (int i = 0; i < _sections.Length; i++)
		{
			if (i == 0)
			{
				if (level == 0 || !level.HasValue)
				{
					_sections[0].PanelOverride = (StyleBox)(object)StyleBoxUnlit;
				}
				else if (level == 1)
				{
					_sections[0].PanelOverride = (StyleBox)(object)((_timer > 0.5f) ? StyleBoxLit : StyleBoxUnlit);
				}
				else
				{
					_sections[0].PanelOverride = (StyleBox)(object)StyleBoxLit;
				}
			}
			else
			{
				_sections[i].PanelOverride = (StyleBox)(object)((level >= i + 2) ? StyleBoxLit : StyleBoxUnlit);
			}
		}
	}
}
