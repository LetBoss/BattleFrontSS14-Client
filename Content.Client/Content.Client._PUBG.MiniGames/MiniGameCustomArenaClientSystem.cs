using System;
using System.Collections.Generic;
using Content.Shared._PUBG.MiniGames;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.MiniGames;

public sealed class MiniGameCustomArenaClientSystem : EntitySystem
{
	private readonly List<MiniGameArenaInfo> _cachedArenas = new List<MiniGameArenaInfo>();

	private int _cachedMaxArenas;

	public IReadOnlyList<MiniGameArenaInfo> CachedArenas => _cachedArenas;

	public int CachedMaxArenas => _cachedMaxArenas;

	public event Action<List<MiniGameArenaInfo>, int>? OnArenaListUpdated;

	public event Action<string>? OnError;

	public event Action? OnUIOpen;

	public event Action? OnUIClose;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameArenaListResponseMessage>((EntityEventHandler<MiniGameArenaListResponseMessage>)OnArenaListResponse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameArenaErrorMessage>((EntityEventHandler<MiniGameArenaErrorMessage>)OnErrorMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameArenaUIOpenMessage>((EntityEventHandler<MiniGameArenaUIOpenMessage>)OnUIOpenMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameArenaUICloseMessage>((EntityEventHandler<MiniGameArenaUICloseMessage>)OnUICloseMessage, (Type[])null, (Type[])null);
	}

	public void RequestEnterCustomization(string? arenaName = null)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaEnterCustomizationMessage(arenaName));
	}

	public void RequestExitCustomization()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaExitCustomizationMessage());
	}

	public void RequestSaveArena(string displayName, bool overwrite = false, string? existingArenaName = null)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaSaveMessage(displayName, overwrite, existingArenaName));
	}

	public void RequestLoadArena(string arenaName)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaLoadMessage(arenaName));
	}

	public void RequestDeleteArena(string arenaName)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaDeleteMessage(arenaName));
	}

	public void RequestArenaList()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaListRequestMessage());
	}

	public void SetSpawnVisibility(bool showSpawns)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameArenaSpawnVisibilityMessage(showSpawns));
	}

	private void OnArenaListResponse(MiniGameArenaListResponseMessage msg)
	{
		_cachedArenas.Clear();
		_cachedArenas.AddRange(msg.Arenas);
		_cachedMaxArenas = msg.MaxArenas;
		this.OnArenaListUpdated?.Invoke(msg.Arenas, msg.MaxArenas);
	}

	private void OnErrorMessage(MiniGameArenaErrorMessage msg)
	{
		this.OnError?.Invoke(msg.ErrorLocKey);
	}

	private void OnUIOpenMessage(MiniGameArenaUIOpenMessage msg)
	{
		this.OnUIOpen?.Invoke();
	}

	private void OnUICloseMessage(MiniGameArenaUICloseMessage msg)
	{
		this.OnUIClose?.Invoke();
	}
}
