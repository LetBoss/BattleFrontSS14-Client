// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Access.IdModificationConsoleBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines.Access;
using Content.Shared._RMC14.UserInterface;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Access;

public sealed class IdModificationConsoleBui : BoundUserInterface, IRefreshableBui
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly ContainerSystem _container;
  private IdModificationConsoleWindow? _window;
  private readonly HashSet<IdModificationConsoleAccessButton> _accessButtons = new HashSet<IdModificationConsoleAccessButton>();
  private readonly HashSet<IdModificationConsoleAccessGroupButton> _accessGroupButtons = new HashSet<IdModificationConsoleAccessGroupButton>();
  private string _currentAccessGroup = "";
  private readonly HashSet<IdModificationConsoleAccessGroupButton> _jobGroupButtons = new HashSet<IdModificationConsoleAccessGroupButton>();
  private readonly HashSet<IdModificationConsoleAccessButton> _jobButtons = new HashSet<IdModificationConsoleAccessButton>();
  private string _currentJobGroup = "";
  private readonly HashSet<IdModificationConsoleTabButton> tabs = new HashSet<IdModificationConsoleTabButton>();
  private string _currenttab = "";

  public IdModificationConsoleBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._container = this.EntMan.System<ContainerSystem>();
  }

  public void Refresh()
  {
    IdModificationConsoleWindow window = this._window;
    IdModificationConsoleComponent console;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<IdModificationConsoleComponent>(this.Owner, ref console))
      return;
    EntityUid? contained1;
    this.TryContainerEntity(this.Owner, console.TargetIdSlot, out contained1);
    EntityUid? contained2;
    this.TryContainerEntity(this.Owner, console.PrivilegedIdSlot, out contained2);
    MetaDataComponent metaDataComponent;
    this.EntMan.TryGetComponent<MetaDataComponent>(contained1, ref metaDataComponent);
    IdCardComponent idCardComponent;
    this.EntMan.TryGetComponent<IdCardComponent>(contained1, ref idCardComponent);
    AccessComponent accessComponent;
    this.EntMan.TryGetComponent<AccessComponent>(contained1, ref accessComponent);
    this._window.SignInButton.Text = !console.Authenticated ? (!contained2.HasValue ? "Sign In" : "Eject Card") : "Log Out";
    if (contained1.HasValue)
    {
      string str1 = metaDataComponent?.EntityName ?? "Unknown Name";
      string str2 = idCardComponent?.FullName ?? "Unknown Name";
      this._window.SignInTargetButton.Text = "Eject Card: " + str1;
      this._window.SignInTargetAccount.Text = str2 + "'s Account Number:";
      this._window.SignInTargetName.Text = str2;
    }
    else
    {
      this._window.SignInTargetButton.Text = "Insert Id To Modify";
      this._window.SignInTargetAccount.Text = "No Card Inserted";
      this._window.SignInTargetName.Text = string.Empty;
    }
    foreach (IdModificationConsoleTabButton tab in this.tabs)
    {
      if (tab.TabButton.Text != this._currenttab)
        ((BaseButton) tab.TabButton).Disabled = false;
    }
    if (console.Authenticated && idCardComponent != null && accessComponent != null)
    {
      ((Control) this._window.TabsContainer).Visible = true;
      ((Control) this._window.AccessContainer).Visible = false;
      ((Control) this._window.JobContainer).Visible = false;
      string currenttab = this._currenttab;
      switch (currenttab)
      {
        case "Access":
          ((Control) this._window.AccessContainer).Visible = true;
          this.AccessGroupRefresh(console, accessComponent);
          this.AccessButtonRefresh(accessComponent);
          this.RefreshIFFButton(console);
          break;
        case "Jobs":
          ((Control) this._window.JobContainer).Visible = true;
          this.jobGroupButtonRefresh();
          break;
        default:
          int num = currenttab == "Ranks" ? 1 : 0;
          break;
      }
    }
    else
    {
      ((Control) this._window.TabsContainer).Visible = false;
      ((Control) this._window.AccessContainer).Visible = false;
      ((Control) this._window.JobContainer).Visible = false;
    }
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<IdModificationConsoleWindow>((BoundUserInterface) this);
    IdModificationConsoleComponent console;
    if (!this.EntMan.TryGetComponent<IdModificationConsoleComponent>(this.Owner, ref console))
      return;
    EntityUid? contained;
    this.TryContainerEntity(this.Owner, console.TargetIdSlot, out contained);
    MetaDataComponent metaDataComponent;
    this.EntMan.TryGetComponent<MetaDataComponent>(contained, ref metaDataComponent);
    IdCardComponent idCardComponent;
    this.EntMan.TryGetComponent<IdCardComponent>(contained, ref idCardComponent);
    AccessComponent accessComponent;
    this.EntMan.TryGetComponent<AccessComponent>(contained, ref accessComponent);
    ((BaseButton) this._window.SignInButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleSignInBuiMsg()));
    ((BaseButton) this._window.SignInTargetButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleSignInTargetBuiMsg()));
    IdModificationConsoleTabButton tab1 = new IdModificationConsoleTabButton();
    tab1.TabButton.Text = "Access";
    ((BaseButton) tab1.TabButton).Disabled = true;
    ((BaseButton) tab1.TabButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      ((Control) this._window.AccessGroups).RemoveAllChildren();
      this._currentAccessGroup = "";
      ((Control) this._window.Accesses).RemoveAllChildren();
      this._currenttab = "Access";
      ((BaseButton) tab1.TabButton).Disabled = true;
      foreach (Control accessGroupButton in this._accessGroupButtons)
        ((Control) this._window.AccessGroups).AddChild(accessGroupButton);
      ((Control) this._window.GrantAllButton).Visible = false;
      ((Control) this._window.RevokeAllButton).Visible = false;
      this.Refresh();
    });
    this.tabs.Add(tab1);
    ((Control) this._window.Tabs).AddChild((Control) tab1);
    ((BaseButton) this._window.GrantAllButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleMultipleAccessChangeBuiMsg("GrantAll", this._currentAccessGroup));
      foreach (IdModificationConsoleAccessButton accessButton in this._accessButtons)
        ((Control) accessButton.AccessButton).ModulateSelfOverride = new Color?(Color.Green);
    });
    ((BaseButton) this._window.RevokeAllButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleMultipleAccessChangeBuiMsg("RevokeAll", this._currentAccessGroup));
      foreach (IdModificationConsoleAccessButton accessButton in this._accessButtons)
        ((Control) accessButton.AccessButton).ModulateSelfOverride = new Color?();
    });
    ((BaseButton) this._window.GrantAllGroupButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleMultipleAccessChangeBuiMsg("GrantAllGroup", this._currentAccessGroup));
      foreach (IdModificationConsoleAccessButton accessButton in this._accessButtons)
        ((Control) accessButton.AccessButton).ModulateSelfOverride = new Color?(Color.Green);
    });
    ((BaseButton) this._window.RevokeAllGroupButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleMultipleAccessChangeBuiMsg("RevokeAllGroup", this._currentAccessGroup));
      foreach (IdModificationConsoleAccessButton accessButton in this._accessButtons)
        ((Control) accessButton.AccessButton).ModulateSelfOverride = new Color?();
    });
    ((BaseButton) this._window.IFF).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleIFFChangeBuiMsg(console.HasIFF));
      this.Refresh();
    });
    this.DisplayAccessGroups(console);
    IdModificationConsoleTabButton tab2 = new IdModificationConsoleTabButton();
    tab2.TabButton.Text = "Jobs";
    ((BaseButton) tab2.TabButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._currenttab = "Jobs";
      this._currentJobGroup = "";
      ((BaseButton) tab2.TabButton).Disabled = true;
      this.Refresh();
    });
    this.tabs.Add(tab2);
    ((Control) this._window.Tabs).AddChild((Control) tab2);
    ((BaseButton) this._window.Terminate).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      ((Control) this._window.Terminate).Visible = false;
      ((Control) this._window.TerminateConfirm).Visible = true;
    });
    ((BaseButton) this._window.TerminateConfirm).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._window.TerminateConfirm.Text = "ID Terminated";
      ((BaseButton) this._window.TerminateConfirm).Disabled = true;
      this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleTerminateConfirmBuiMsg());
    });
    this.DisplayJobGroups(console);
    this.Refresh();
  }

  private void RefreshIFFButton(IdModificationConsoleComponent console)
  {
    IdModificationConsoleWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    if (console.HasIFF)
    {
      ((Control) this._window.IFF).ModulateSelfOverride = new Color?(Color.Maroon);
      this._window.IFF.Text = "Revoke IFF";
    }
    else
    {
      ((Control) this._window.IFF).ModulateSelfOverride = new Color?(Color.Green);
      this._window.IFF.Text = "Grant IFF";
    }
  }

  private void DisplayAccessGroups(IdModificationConsoleComponent console)
  {
    IdModificationConsoleWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    ((Control) this._window.AccessGroups).RemoveAllChildren();
    List<ProtoId<AccessLevelPrototype>> list = console.AccessGroups.ToList<ProtoId<AccessLevelPrototype>>();
    list.Sort();
    List<ProtoId<AccessLevelPrototype>> listAccess = console.AccessList.ToList<ProtoId<AccessLevelPrototype>>();
    listAccess.Sort();
    foreach (ProtoId<AccessLevelPrototype> protoId1 in list)
    {
      AccessLevelPrototype accessGroupPrototype;
      if (this._prototype.TryIndex<AccessLevelPrototype>(protoId1, ref accessGroupPrototype))
      {
        IdModificationConsoleAccessGroupButton button = new IdModificationConsoleAccessGroupButton();
        button.Tag = accessGroupPrototype.AccessGroup;
        ((Control) button.AccessButton).HorizontalExpand = true;
        ((Control) button.AccessButton).SetHeight = 30f;
        ((BaseButton) button.AccessButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_1 =>
        {
          this._currentAccessGroup = button.Tag;
          ((Control) button.AccessButton).ModulateSelfOverride = new Color?(Color.Green);
          ((Control) this._window.GrantAllButton).Visible = true;
          ((Control) this._window.RevokeAllButton).Visible = true;
          this._accessButtons.Clear();
          ((Control) this._window.Accesses).RemoveAllChildren();
          foreach (ProtoId<AccessLevelPrototype> protoId2 in listAccess)
          {
            ProtoId<AccessLevelPrototype> access = protoId2;
            AccessLevelPrototype accessLevelPrototype;
            if (this._prototype.TryIndex<AccessLevelPrototype>(access, ref accessLevelPrototype) && !(accessLevelPrototype.AccessGroup != accessGroupPrototype.AccessGroup))
            {
              IdModificationConsoleAccessButton accessButton = new IdModificationConsoleAccessButton();
              if (accessLevelPrototype.Name != null)
              {
                accessButton.AccessButton.Text = Loc.GetString(accessLevelPrototype.Name);
                accessButton.Tag = accessLevelPrototype.Name;
              }
              ((Control) accessButton.AccessButton).HorizontalExpand = true;
              ((Control) accessButton.AccessButton).SetHeight = 30f;
              ((BaseButton) accessButton.AccessButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_2 =>
              {
                this.ToggleAccessButtonColor(accessButton);
                this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleAccessChangeBuiMsg(access, ((Control) accessButton.AccessButton).ModulateSelfOverride.HasValue));
                this.Refresh();
              });
              this._accessButtons.Add(accessButton);
              ((Control) this._window.Accesses).AddChild((Control) accessButton);
              this.Refresh();
            }
          }
        });
        this._accessGroupButtons.Add(button);
      }
    }
  }

  private void DisplayJobGroups(IdModificationConsoleComponent console)
  {
    IdModificationConsoleWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    ((Control) this._window.AccessGroups).RemoveAllChildren();
    List<ProtoId<AccessGroupPrototype>> list = console.JobGroups.ToList<ProtoId<AccessGroupPrototype>>();
    list.Sort();
    List<ProtoId<AccessGroupPrototype>> listJob = console.JobList.ToList<ProtoId<AccessGroupPrototype>>();
    listJob.Sort();
    foreach (ProtoId<AccessGroupPrototype> protoId1 in list)
    {
      AccessGroupPrototype jobGroupPrototype;
      if (this._prototype.TryIndex<AccessGroupPrototype>(protoId1, ref jobGroupPrototype))
      {
        IdModificationConsoleAccessGroupButton button = new IdModificationConsoleAccessGroupButton();
        button.Tag = jobGroupPrototype.AccessGroup;
        button.AccessButton.Text = jobGroupPrototype.AccessGroup;
        ((Control) button.AccessButton).HorizontalExpand = true;
        ((Control) button.AccessButton).SetHeight = 30f;
        ((BaseButton) button.AccessButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_1 =>
        {
          this._currentJobGroup = button.Tag;
          ((Control) button.AccessButton).ModulateSelfOverride = new Color?(Color.Green);
          this._jobButtons.Clear();
          ((Control) this._window.Jobs).RemoveAllChildren();
          foreach (ProtoId<AccessGroupPrototype> protoId2 in listJob)
          {
            AccessGroupPrototype jobPrototype;
            if (this._prototype.TryIndex<AccessGroupPrototype>(protoId2, ref jobPrototype) && !(jobPrototype.AccessGroup != jobGroupPrototype.AccessGroup))
            {
              IdModificationConsoleAccessButton jobButton = new IdModificationConsoleAccessButton();
              if (jobPrototype.Name != null)
              {
                jobButton.AccessButton.Text = Loc.GetString(jobPrototype.Name);
                jobButton.Tag = jobPrototype.Name;
              }
              ((Control) jobButton.AccessButton).HorizontalExpand = true;
              ((Control) jobButton.AccessButton).SetHeight = 30f;
              ((BaseButton) jobButton.AccessButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_2 =>
              {
                foreach (IdModificationConsoleAccessButton jobButton1 in this._jobButtons)
                  ((BaseButton) jobButton1.AccessButton).Disabled = true;
                ((Control) jobButton.AccessButton).ModulateSelfOverride = new Color?(Color.Green);
                this.SendPredictedMessage((BoundUserInterfaceMessage) new IdModificationConsoleJobChangeBuiMsg(ProtoId<AccessGroupPrototype>.op_Implicit(jobPrototype)));
                this.Refresh();
              });
              this._jobButtons.Add(jobButton);
              ((Control) this._window.Jobs).AddChild((Control) jobButton);
              this.Refresh();
            }
          }
        });
        this._jobGroupButtons.Add(button);
        ((Control) this._window.JobGroups).AddChild((Control) button);
      }
    }
  }

  private void jobGroupButtonRefresh()
  {
    foreach (IdModificationConsoleAccessGroupButton jobGroupButton in this._jobGroupButtons)
    {
      if (jobGroupButton.Tag != this._currentJobGroup)
        ((Control) jobGroupButton.AccessButton).ModulateSelfOverride = new Color?();
    }
  }

  private void AccessButtonRefresh(AccessComponent access)
  {
    foreach (IdModificationConsoleAccessButton accessButton in this._accessButtons)
    {
      if (accessButton.AccessButton.Text == null)
        break;
      foreach (ProtoId<AccessLevelPrototype> tag in access.Tags)
      {
        AccessLevelPrototype accessLevelPrototype;
        if (this._prototype.TryIndex<AccessLevelPrototype>(tag, ref accessLevelPrototype) && accessLevelPrototype.Name != null && !(accessButton.Tag != accessLevelPrototype.Name))
        {
          ((Control) accessButton.AccessButton).ModulateSelfOverride = new Color?(Color.Green);
          break;
        }
      }
    }
  }

  private void ToggleAccessButtonColor(IdModificationConsoleAccessButton accessButton)
  {
    ((Control) accessButton.AccessButton).ModulateSelfOverride = !((Control) accessButton.AccessButton).ModulateSelfOverride.HasValue ? new Color?(Color.Green) : new Color?();
  }

  private void AccessGroupRefresh(
    IdModificationConsoleComponent console,
    AccessComponent? targetCardAccessComponent)
  {
    foreach (IdModificationConsoleAccessGroupButton accessGroupButton in this._accessGroupButtons)
    {
      if (targetCardAccessComponent != null)
      {
        string tag = accessGroupButton.Tag;
        int num1 = 0;
        int num2 = 0;
        foreach (ProtoId<AccessLevelPrototype> access in console.AccessList)
        {
          AccessLevelPrototype accessLevelPrototype;
          if (this._prototype.TryIndex<AccessLevelPrototype>(access, ref accessLevelPrototype) && !(accessLevelPrototype.AccessGroup != accessGroupButton.Tag))
          {
            ++num1;
            if (targetCardAccessComponent.Tags.Contains(access))
              ++num2;
          }
        }
        accessGroupButton.AccessButton.Text = num2 < num1 ? (num2 <= 0 ? "[ ◇ ] " + tag : "[ ◈ ] " + tag) : "[ ◆ ] " + tag;
      }
      else
        accessGroupButton.AccessButton.Text = accessGroupButton.Tag;
      if (!(accessGroupButton.Tag == this._currentAccessGroup))
      {
        ((Control) accessGroupButton.AccessButton).ModulateSelfOverride = new Color?();
        ((BaseButton) accessGroupButton.AccessButton).Disabled = false;
      }
    }
  }

  private bool TryContainerEntity(EntityUid ent, string containerType, out EntityUid? contained)
  {
    ContainerSlot containerSlot = ((SharedContainerSystem) this._container).EnsureContainer<ContainerSlot>(ent, containerType, (ContainerManagerComponent) null);
    contained = containerSlot.ContainedEntity;
    return contained.HasValue;
  }
}
