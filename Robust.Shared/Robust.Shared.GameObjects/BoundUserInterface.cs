using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Robust.Shared.GameObjects;

public abstract class BoundUserInterface : IDisposable
{
	[Dependency]
	protected internal readonly IEntityManager EntMan;

	[Dependency]
	protected readonly ISharedPlayerManager PlayerManager;

	protected readonly SharedUserInterfaceSystem UiSystem;

	public readonly Enum UiKey;

	internal List<IDisposable>? Disposals;

	public bool IsOpened { get; protected set; }

	public EntityUid Owner { get; }

	protected internal BoundUserInterfaceState? State { get; internal set; }

	protected BoundUserInterface(EntityUid owner, Enum uiKey)
	{
		IoCManager.InjectDependencies(this);
		UiSystem = EntMan.System<SharedUserInterfaceSystem>();
		Owner = owner;
		UiKey = uiKey;
	}

	[MustCallBase(false)]
	protected internal virtual void Open()
	{
		if (!IsOpened)
		{
			IsOpened = true;
		}
	}

	protected internal virtual void UpdateState(BoundUserInterfaceState state)
	{
	}

	public void Update<T>() where T : BoundUserInterfaceState
	{
		if (UiSystem.TryGetUiState<T>(Owner, UiKey, out T state))
		{
			UpdateState(state);
		}
		Update();
	}

	public virtual void Update()
	{
	}

	public virtual void OnProtoReload(PrototypesReloadedEventArgs args)
	{
	}

	protected internal virtual void ReceiveMessage(BoundUserInterfaceMessage message)
	{
	}

	public void Close()
	{
		if (IsOpened)
		{
			IsOpened = false;
			UiSystem.CloseUi(Owner, UiKey, PlayerManager.LocalEntity, predicted: true);
		}
	}

	public void SendMessage(BoundUserInterfaceMessage message)
	{
		UiSystem.ClientSendUiMessage(Owner, UiKey, message);
	}

	public void SendPredictedMessage(BoundUserInterfaceMessage message)
	{
		UiSystem.SendPredictedUiMessage(this, message);
	}

	~BoundUserInterface()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing || Disposals == null)
		{
			return;
		}
		foreach (IDisposable disposal in Disposals)
		{
			disposal.Dispose();
		}
		Disposals = null;
	}
}
