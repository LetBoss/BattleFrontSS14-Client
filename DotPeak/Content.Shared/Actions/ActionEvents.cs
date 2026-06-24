// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.GetItemActionsEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Actions;

public sealed class GetItemActionsEvent : EntityEventArgs
{
  private readonly ActionContainerSystem _system;
  public readonly SortedSet<EntityUid> Actions = new SortedSet<EntityUid>();
  public EntityUid User;
  public EntityUid Provider;
  public Content.Shared.Inventory.SlotFlags? SlotFlags;

  public bool InHands => !this.SlotFlags.HasValue;

  public GetItemActionsEvent(
    ActionContainerSystem system,
    EntityUid user,
    EntityUid provider,
    Content.Shared.Inventory.SlotFlags? slotFlags = null)
  {
    this._system = system;
    this.User = user;
    this.Provider = provider;
    this.SlotFlags = slotFlags;
  }

  public void AddAction(ref EntityUid? actionId, string prototypeId, EntityUid container)
  {
    if (!this._system.EnsureAction(container, ref actionId, prototypeId))
      return;
    this.Actions.Add(actionId.Value);
  }

  public void AddAction(ref EntityUid? actionId, string prototypeId)
  {
    this.AddAction(ref actionId, prototypeId, this.Provider);
  }

  public void AddAction(EntityUid? actionId)
  {
    if (!actionId.HasValue)
      return;
    this.Actions.Add(actionId.Value);
  }
}
