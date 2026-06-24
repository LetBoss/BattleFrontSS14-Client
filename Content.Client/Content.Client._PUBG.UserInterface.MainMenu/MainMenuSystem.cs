using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Cases;
using Content.Shared._PUBG.Skin;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.UserInterface.MainMenu;

public sealed class MainMenuSystem : EntitySystem
{
	private static readonly TimeSpan SkinStateRequestDebounce = TimeSpan.FromMilliseconds(500L);

	private static readonly TimeSpan SkinStateRequestTimeout = TimeSpan.FromSeconds(8L);

	private Dictionary<string, string> _cachedCurrentOutfit = new Dictionary<string, string>();

	private int _cachedCoins;

	private int _cachedScrap;

	private int _cachedPremiumCoins;

	private bool _hasCachedBalance;

	private int _cachedPlayerLevel = 1;

	private bool _hasCachedPlayerLevel;

	private DateTime _lastSkinStateRequestAt = DateTime.MinValue;

	private bool _skinStateRequestInFlight;

	public event Action<SkinOpenMessage>? OnSkinOpenReceived;

	public event Action<SkinStateMessage>? OnSkinStateReceived;

	public event Action<BalanceUpdateMessage>? OnBalanceUpdateReceived;

	public event Action<SkinProfileStateMessage>? OnSkinProfileStateReceived;

	public event Action<CaseOpenResultMessage>? OnCaseOpenResultReceived;

	public event Action<CaseOpenErrorMessage>? OnCaseOpenErrorReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<SkinOpenMessage>((EntityEventHandler<SkinOpenMessage>)OnSkinOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<SkinStateMessage>((EntityEventHandler<SkinStateMessage>)OnSkinState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<BalanceUpdateMessage>((EntityEventHandler<BalanceUpdateMessage>)OnBalanceUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<SkinProfileStateMessage>((EntityEventHandler<SkinProfileStateMessage>)OnSkinProfileState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CaseOpenResultMessage>((EntityEventHandler<CaseOpenResultMessage>)OnCaseOpenResult, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CaseOpenErrorMessage>((EntityEventHandler<CaseOpenErrorMessage>)OnCaseOpenError, (Type[])null, (Type[])null);
	}

	private void OnSkinOpen(SkinOpenMessage msg)
	{
		this.OnSkinOpenReceived?.Invoke(msg);
	}

	private void OnSkinState(SkinStateMessage msg)
	{
		_skinStateRequestInFlight = false;
		_cachedCurrentOutfit = new Dictionary<string, string>(msg.CurrentOutfit);
		_cachedCoins = msg.PlayerCoins;
		_cachedScrap = msg.PlayerScrap;
		_cachedPremiumCoins = msg.PlayerPremiumCoins;
		_hasCachedBalance = true;
		_cachedPlayerLevel = msg.PlayerLevel;
		_hasCachedPlayerLevel = true;
		this.OnSkinStateReceived?.Invoke(msg);
	}

	private void OnBalanceUpdate(BalanceUpdateMessage msg)
	{
		_cachedCoins = msg.Coins;
		_cachedScrap = msg.Scrap;
		_cachedPremiumCoins = msg.PremiumCoins;
		_hasCachedBalance = true;
		this.OnBalanceUpdateReceived?.Invoke(msg);
	}

	private void OnSkinProfileState(SkinProfileStateMessage msg)
	{
		this.OnSkinProfileStateReceived?.Invoke(msg);
	}

	private void OnCaseOpenResult(CaseOpenResultMessage msg)
	{
		this.OnCaseOpenResultReceived?.Invoke(msg);
	}

	private void OnCaseOpenError(CaseOpenErrorMessage msg)
	{
		this.OnCaseOpenErrorReceived?.Invoke(msg);
	}

	public Dictionary<string, string> GetCachedCurrentOutfit()
	{
		return new Dictionary<string, string>(_cachedCurrentOutfit);
	}

	public bool TryGetCachedBalance(out int coins, out int scrap, out int premium)
	{
		coins = _cachedCoins;
		scrap = _cachedScrap;
		premium = _cachedPremiumCoins;
		return _hasCachedBalance;
	}

	public bool TryGetCachedPlayerLevel(out int level)
	{
		level = _cachedPlayerLevel;
		return _hasCachedPlayerLevel;
	}

	public void RequestSkinState(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_skinStateRequestInFlight || !(utcNow - _lastSkinStateRequestAt < SkinStateRequestTimeout)) && (force || !(utcNow - _lastSkinStateRequestAt < SkinStateRequestDebounce)))
		{
			_skinStateRequestInFlight = true;
			_lastSkinStateRequestAt = utcNow;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new SkinOpenMessage());
		}
	}
}
