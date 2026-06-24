// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Roles.FindParasite.FindParasiteBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Roles.FindParasite;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Roles.FindParasite;

public sealed class FindParasiteBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private ItemList.Item? _selectedItem;
  private bool _impledDeselect;
  private ItemList? _spawnerList;
  [Robust.Shared.ViewVariables.ViewVariables]
  private FindParasiteWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<FindParasiteWindow>((BoundUserInterface) this);
    this._spawnerList = this._window.ParasiteSpawners;
    Button spawnButton = this._window.SpawnButton;
    this._spawnerList.OnItemSelected += new Action<ItemList.ItemListSelectedEventArgs>(this.OnItemSelect);
    this._spawnerList.OnItemDeselected += new Action<ItemList.ItemListDeselectedEventArgs>(this.OnItemDeselect);
    spawnButton.Text = Loc.GetString("xeno-ui-find-parasite-spawn-button");
    ((BaseButton) spawnButton).Disabled = true;
    ((BaseButton) spawnButton).OnButtonDown += (Action<BaseButton.ButtonEventArgs>) (args =>
    {
      if (this._selectedItem == null)
      {
        args.Button.Disabled = true;
      }
      else
      {
        this.TakeParasiteRole((NetEntity) this._selectedItem.Metadata);
        this.Close();
      }
    });
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is FindParasiteUIState findParasiteUiState) || this._spawnerList == null)
      return;
    List<SpawnerData> parasiteSpawners = findParasiteUiState.ActiveParasiteSpawners;
    this._spawnerList.Clear();
    foreach (SpawnerData spawnerData in parasiteSpawners)
      this._spawnerList.Add(new ItemList.Item(this._spawnerList)
      {
        Text = spawnerData.Name,
        Metadata = (object) spawnerData.Spawner
      });
  }

  private void OnItemSelect(ItemList.ItemListSelectedEventArgs args)
  {
    ((BaseButton) this._window.SpawnButton).Disabled = false;
    ItemList.Item obj = ((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex];
    NetEntity metadata1 = (NetEntity) obj.Metadata;
    if (this._selectedItem == null)
    {
      this.FollowParasiteSpawner(metadata1);
      this._selectedItem = obj;
    }
    else
    {
      NetEntity metadata2 = (NetEntity) this._selectedItem.Metadata;
      if (NetEntity.op_Equality(metadata1, metadata2))
      {
        this.TakeParasiteRole(metadata2);
        this.Close();
      }
      else
      {
        this._impledDeselect = true;
        this._selectedItem.Selected = false;
        this._selectedItem = obj;
        this.FollowParasiteSpawner(metadata1);
      }
    }
  }

  private void OnItemDeselect(ItemList.ItemListDeselectedEventArgs args)
  {
    NetEntity metadata1 = (NetEntity) ((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex].Metadata;
    if (this._selectedItem == null)
      return;
    if (this._impledDeselect)
    {
      this._impledDeselect = false;
    }
    else
    {
      NetEntity metadata2 = (NetEntity) this._selectedItem.Metadata;
      if (!NetEntity.op_Equality(metadata1, metadata2))
        return;
      this.TakeParasiteRole(metadata2);
      this.Close();
    }
  }

  public void FollowParasiteSpawner(NetEntity spawner)
  {
    this.SendMessage((BoundUserInterfaceMessage) new FollowParasiteSpawnerMessage(spawner));
  }

  public void TakeParasiteRole(NetEntity spawner)
  {
    this.SendMessage((BoundUserInterfaceMessage) new TakeParasiteRoleMessage(spawner));
  }
}
