using System;
using System.Collections.Generic;
using Content.Client.UserInterface.Controls;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Client.Viewport;

public sealed class ViewportManager
{
	[Dependency]
	private IConfigurationManager _cfg;

	private readonly List<MainViewport> _viewports = new List<MainViewport>();

	public void Initialize()
	{
		_cfg.OnValueChanged<bool>(CCVars.ViewportStretch, (Action<bool>)delegate
		{
			UpdateCfg();
		}, false);
		_cfg.OnValueChanged<int>(CCVars.ViewportSnapToleranceClip, (Action<int>)delegate
		{
			UpdateCfg();
		}, false);
		_cfg.OnValueChanged<int>(CCVars.ViewportSnapToleranceMargin, (Action<int>)delegate
		{
			UpdateCfg();
		}, false);
		_cfg.OnValueChanged<bool>(CCVars.ViewportScaleRender, (Action<bool>)delegate
		{
			UpdateCfg();
		}, false);
		_cfg.OnValueChanged<int>(CCVars.ViewportFixedScaleFactor, (Action<int>)delegate
		{
			UpdateCfg();
		}, false);
	}

	private void UpdateCfg()
	{
		_viewports.ForEach(delegate(MainViewport v)
		{
			v.UpdateCfg();
		});
	}

	public void AddViewport(MainViewport vp)
	{
		_viewports.Add(vp);
	}

	public void RemoveViewport(MainViewport vp)
	{
		_viewports.Remove(vp);
	}
}
