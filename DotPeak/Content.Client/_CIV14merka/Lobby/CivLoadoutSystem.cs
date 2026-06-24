// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.CivLoadoutSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Loadout;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Lobby;

public sealed class CivLoadoutSystem : EntitySystem
{
  private CivLoadoutStateEvent _state = new CivLoadoutStateEvent(new Dictionary<string, List<string>>(), new Dictionary<string, List<string>>(), new List<string>());

  public event Action<CivLoadoutStateEvent>? StateUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivLoadoutStateEvent>(new EntitySessionEventHandler<CivLoadoutStateEvent>(this.OnState), (Type[]) null, (Type[]) null);
  }

  public CivLoadoutStateEvent GetState() => this._state;

  public void RequestState()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivLoadoutRequestStateEvent());
  }

  public void SetItem(string faction, CivTdmClass cls, string itemKey, bool disabled)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivLoadoutSetItemRequestEvent(faction, cls, itemKey, disabled));
    string key = CivLoadoutKeys.Combo(faction, cls);
    List<string> stringList;
    if (!this._state.Disabled.TryGetValue(key, out stringList))
    {
      if (!disabled)
        return;
      stringList = new List<string>();
      this._state.Disabled[key] = stringList;
    }
    if (disabled)
    {
      if (stringList.Contains(itemKey))
        return;
      stringList.Add(itemKey);
    }
    else
      stringList.Remove(itemKey);
  }

  public void SetSlotChoice(string faction, CivTdmClass cls, string slot, string proto)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivLoadoutSetSlotChoiceRequestEvent(faction, cls, slot, proto));
    string key = CivLoadoutKeys.Combo(faction, cls);
    List<string> stringList;
    if (!this._state.Selections.TryGetValue(key, out stringList))
    {
      stringList = new List<string>();
      this._state.Selections[key] = stringList;
    }
    string slot1;
    stringList.RemoveAll((Predicate<string>) (entry => CivLoadoutKeys.TryParseSlotChoice(entry, out slot1, out string _) && slot1 == slot));
    stringList.Add(CivLoadoutKeys.SlotChoice(slot, proto));
  }

  private void OnState(CivLoadoutStateEvent msg, EntitySessionEventArgs args)
  {
    this._state = msg;
    Action<CivLoadoutStateEvent> stateUpdated = this.StateUpdated;
    if (stateUpdated == null)
      return;
    stateUpdated(msg);
  }
}
