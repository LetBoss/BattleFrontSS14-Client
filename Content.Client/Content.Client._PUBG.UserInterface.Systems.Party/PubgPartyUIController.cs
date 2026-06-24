using System;
using System.Numerics;
using Content.Client._PUBG.Party;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private const float PanelWidth = 220f;

	private const float PanelTopMargin = 320f;

	private const float PanelLeftMargin = 20f;

	[Dependency]
	private IConfigurationManager _cfg;

	private LayoutContainer? _viewport;

	private PubgPartyHud? _hud;

	private bool _subscribed;

	private bool _hudEnabled;

	private int _offsetY;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(EnsureHud));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(ClearHud));
		_cfg.OnValueChanged<bool>(CCVars.PubgPartyHudEnabled, (Action<bool>)OnHudEnabledChanged, true);
		_cfg.OnValueChanged<int>(CCVars.PubgPartyHudOffsetY, (Action<int>)OnOffsetChanged, true);
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureHud();
		SubscribePartySystem();
	}

	public void OnStateExited(GameplayState state)
	{
		ClearHud();
		UnsubscribePartySystem();
	}

	private void SubscribePartySystem()
	{
		if (!_subscribed)
		{
			base.EntityManager.System<PubgPartyClientSystem>().PartyStateUpdated += OnPartyStateUpdated;
			_subscribed = true;
		}
	}

	private void UnsubscribePartySystem()
	{
		if (_subscribed)
		{
			PubgPartyClientSystem pubgPartyClientSystem = base.EntityManager.SystemOrNull<PubgPartyClientSystem>();
			if (pubgPartyClientSystem != null)
			{
				pubgPartyClientSystem.PartyStateUpdated -= OnPartyStateUpdated;
			}
			_subscribed = false;
		}
	}

	private void OnPartyStateUpdated()
	{
		EnsureHud();
		UpdateHud();
	}

	private void OnHudEnabledChanged(bool enabled)
	{
		_hudEnabled = enabled;
		if (!enabled)
		{
			PubgPartyHud hud = _hud;
			if (hud != null && !((Control)hud).Disposed)
			{
				((Control)hud).Orphan();
			}
			_hud = null;
		}
		else
		{
			EnsureHud();
			UpdateHud();
		}
	}

	private void EnsureHud()
	{
		if (!_hudEnabled)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null && activeScreen.GetWidget<MainViewport>() != null)
		{
			LayoutContainer val;
			try
			{
				val = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
			}
			catch (ArgumentException)
			{
				return;
			}
			_viewport = val;
			if (_hud == null)
			{
				PubgPartyHud pubgPartyHud = new PubgPartyHud();
				((Control)pubgPartyHud).MinSize = new Vector2(220f, 0f);
				_hud = pubgPartyHud;
				_hud.VoicePressed += OnVoicePressed;
				((Control)val).AddChild((Control)(object)_hud);
				ApplyHudLayout((Control)(object)_hud);
				UpdateHud();
			}
		}
	}

	private void ApplyHudLayout(Control hud)
	{
		if (GetScreenType() == ScreenType.Default)
		{
			LayoutContainer.SetAnchorAndMarginPreset(hud, (LayoutPreset)0, (LayoutPresetMode)0, 0);
			LayoutContainer.SetMarginLeft(hud, 20f);
			LayoutContainer.SetMarginTop(hud, 320f - (float)_offsetY);
		}
		else
		{
			LayoutContainer.SetAnchorAndMarginPreset(hud, (LayoutPreset)0, (LayoutPresetMode)0, 0);
			LayoutContainer.SetMarginLeft(hud, 20f);
			LayoutContainer.SetMarginTop(hud, 320f - (float)_offsetY);
		}
	}

	private void UpdateHud()
	{
		if (_hud != null && _hudEnabled)
		{
			PubgPartyClientSystem pubgPartyClientSystem = base.EntityManager.System<PubgPartyClientSystem>();
			bool isFiftyFiftyMode = pubgPartyClientSystem.IsFiftyFiftyMode;
			if (!isFiftyFiftyMode && pubgPartyClientSystem.Members.Count <= 1)
			{
				((Control)_hud).Visible = false;
				return;
			}
			((Control)_hud).Visible = true;
			_hud.UpdateMembers(pubgPartyClientSystem.Members, isFiftyFiftyMode, pubgPartyClientSystem.LocalTeamTag);
			UpdateHudSize(Math.Max(1, pubgPartyClientSystem.Members.Count), isFiftyFiftyMode);
		}
	}

	private void UpdateHudSize(int memberCount, bool compactMode)
	{
		if (compactMode)
		{
			((Control)_hud).MinSize = new Vector2(220f, 112f);
			((Control)_hud).MaxSize = new Vector2(220f, 112f);
			return;
		}
		float num = 54f;
		float num2 = 90f + num * (float)Math.Max(1, memberCount);
		if (memberCount > 2)
		{
			num2 += 20f;
		}
		((Control)_hud).MinSize = new Vector2(220f, num2);
		((Control)_hud).MaxSize = new Vector2(220f, num2);
	}

	private void ClearHud()
	{
		if (_hud != null)
		{
			_hud.VoicePressed -= OnVoicePressed;
		}
		PubgPartyHud hud = _hud;
		if (hud != null && !((Control)hud).Disposed)
		{
			((Control)hud).Orphan();
		}
		_hud = null;
		_viewport = null;
	}

	private void OnOffsetChanged(int offsetY)
	{
		_offsetY = offsetY;
		if (_hud != null)
		{
			ApplyHudLayout((Control)(object)_hud);
		}
	}

	private void OnVoicePressed()
	{
		base.EntityManager.System<PubgPartyClientSystem>().RequestVoice();
	}

	private ScreenType GetScreenType()
	{
		if (!Enum.TryParse<ScreenType>(_cfg.GetCVar<string>(CCVars.UILayout), out var result))
		{
			return ScreenType.Default;
		}
		return result;
	}
}
