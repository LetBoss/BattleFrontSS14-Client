// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.BoundUserInterface
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
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
    IoCManager.InjectDependencies<BoundUserInterface>(this);
    this.UiSystem = this.EntMan.System<SharedUserInterfaceSystem>();
    this.Owner = owner;
    this.UiKey = uiKey;
  }

  [MustCallBase(false)]
  protected internal virtual void Open()
  {
    if (this.IsOpened)
      return;
    this.IsOpened = true;
  }

  protected internal virtual void UpdateState(BoundUserInterfaceState state)
  {
  }

  public void Update<T>() where T : BoundUserInterfaceState
  {
    T state;
    if (this.UiSystem.TryGetUiState<T>((Entity<UserInterfaceComponent>) this.Owner, this.UiKey, out state))
      this.UpdateState((BoundUserInterfaceState) state);
    this.Update();
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
    if (!this.IsOpened)
      return;
    this.IsOpened = false;
    this.UiSystem.CloseUi((Entity<UserInterfaceComponent>) this.Owner, this.UiKey, this.PlayerManager.LocalEntity, true);
  }

  public void SendMessage(BoundUserInterfaceMessage message)
  {
    this.UiSystem.ClientSendUiMessage((Entity<UserInterfaceComponent>) this.Owner, this.UiKey, message);
  }

  public void SendPredictedMessage(BoundUserInterfaceMessage message)
  {
    this.UiSystem.SendPredictedUiMessage(this, message);
  }

  ~BoundUserInterface() => this.Dispose(false);

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing || this.Disposals == null)
      return;
    foreach (IDisposable disposal in this.Disposals)
      disposal.Dispose();
    this.Disposals = (List<IDisposable>) null;
  }
}
