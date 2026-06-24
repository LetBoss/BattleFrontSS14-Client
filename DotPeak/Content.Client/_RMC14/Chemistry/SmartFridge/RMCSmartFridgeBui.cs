// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Chemistry.SmartFridge.RMCSmartFridgeBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Shared._RMC14.Chemistry.SmartFridge;
using Content.Shared._RMC14.UserInterface;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Chemistry.SmartFridge;

public sealed class RMCSmartFridgeBui : BoundUserInterface, IRefreshableBui
{
  [Dependency]
  private ILocalizationManager _loc;
  private readonly ContainerSystem _container;
  private readonly EntityQuery<RMCSmartFridgeInsertableComponent> _insertableQuery;
  private readonly EntityQuery<MetaDataComponent> _metaDataQuery;
  private RMCSmartFridgeWindow? _window;
  private readonly SortedDictionary<string, SortedDictionary<string, int>> _contents = new SortedDictionary<string, SortedDictionary<string, int>>();
  private readonly Dictionary<string, EntityUid> _first = new Dictionary<string, EntityUid>();

  public RMCSmartFridgeBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._container = this.EntMan.System<ContainerSystem>();
    this._insertableQuery = this.EntMan.GetEntityQuery<RMCSmartFridgeInsertableComponent>();
    this._metaDataQuery = this.EntMan.GetEntityQuery<MetaDataComponent>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCSmartFridgeWindow>((BoundUserInterface) this);
    MetaDataComponent metaDataComponent;
    if (this.EntMan.TryGetComponent<MetaDataComponent>(this.Owner, ref metaDataComponent))
      this._window.Title = metaDataComponent.EntityName;
    this.Refresh();
  }

  public void Refresh()
  {
    if (this._window == null)
      return;
    TabContainer contentsTabs = this._window.ContentsTabs;
    RMCSmartFridgeComponent smartFridgeComponent;
    BaseContainer baseContainer;
    if (!this.EntMan.TryGetComponent<RMCSmartFridgeComponent>(this.Owner, ref smartFridgeComponent) || !((SharedContainerSystem) this._container).TryGetContainer(this.Owner, smartFridgeComponent.ContainerId, ref baseContainer, (ContainerManagerComponent) null) || baseContainer.ContainedEntities.Count == 0)
    {
      ((Control) contentsTabs).RemoveAllChildren();
      ((Control) this._window.ContentsEmptyLabel).Visible = true;
      ((Control) contentsTabs).Visible = false;
    }
    else
    {
      ((Control) this._window.ContentsEmptyLabel).Visible = false;
      ((Control) contentsTabs).Visible = true;
      foreach (SortedDictionary<string, int> sortedDictionary in this._contents.Values)
        sortedDictionary.Clear();
      this._first.Clear();
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
      {
        RMCSmartFridgeInsertableComponent insertableComponent;
        MetaDataComponent metaDataComponent;
        if (this._insertableQuery.TryComp(containedEntity, ref insertableComponent) && this._metaDataQuery.TryComp(containedEntity, ref metaDataComponent))
        {
          string str1 = insertableComponent.Category;
          string str2;
          if (this._loc.TryGetString(insertableComponent.Category, ref str2))
            str1 = str2;
          string entityName = metaDataComponent.EntityName;
          SortedDictionary<string, int> orNew = Extensions.GetOrNew<string, SortedDictionary<string, int>>((IDictionary<string, SortedDictionary<string, int>>) this._contents, str1);
          orNew[entityName] = orNew.GetValueOrDefault<string, int>(entityName) + 1;
          this._first.TryAdd(entityName, containedEntity);
        }
      }
      int after1 = 0;
      foreach ((string key2, SortedDictionary<string, int> sortedDictionary1) in this._contents)
      {
        string str = key2;
        SortedDictionary<string, int> sortedDictionary2 = sortedDictionary1;
        if (sortedDictionary2.Count != 0)
        {
          RMCSmartFridgeSection smartFridgeSection;
          if (after1 < ((Control) contentsTabs).ChildCount)
          {
            smartFridgeSection = (RMCSmartFridgeSection) ((Control) contentsTabs).GetChild(after1);
          }
          else
          {
            smartFridgeSection = new RMCSmartFridgeSection();
            ((Control) contentsTabs).AddChild((Control) smartFridgeSection);
          }
          TabContainer.SetTabTitle((Control) smartFridgeSection, str);
          TabContainer.SetTabVisible((Control) smartFridgeSection, true);
          int after2 = 0;
          int num2;
          foreach ((key2, num2) in sortedDictionary2)
          {
            string key3 = key2;
            int num3 = num2;
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            RMCSmartFridgeBui.\u003C\u003Ec__DisplayClass9_0 cDisplayClass90 = new RMCSmartFridgeBui.\u003C\u003Ec__DisplayClass9_0();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass90.\u003C\u003E4__this = this;
            EntityUid entityUid;
            if (!this._first.TryGetValue(key3, out entityUid))
            {
              ++after2;
            }
            else
            {
              // ISSUE: reference to a compiler-generated field
              cDisplayClass90.netFirst = this.EntMan.GetNetEntity(entityUid, (MetaDataComponent) null);
              RMCSmartFridgeRow rmcSmartFridgeRow;
              if (after2 < ((Control) smartFridgeSection.Container).ChildCount)
              {
                rmcSmartFridgeRow = (RMCSmartFridgeRow) ((Control) smartFridgeSection.Container).GetChild(after2);
              }
              else
              {
                rmcSmartFridgeRow = new RMCSmartFridgeRow();
                ((Control) smartFridgeSection.Container).AddChild((Control) rmcSmartFridgeRow);
              }
              rmcSmartFridgeRow.SpriteView.SetEntity(new EntityUid?(entityUid));
              rmcSmartFridgeRow.AmountLabel.Text = $"{num3}";
              rmcSmartFridgeRow.NameButton.Text = key3;
              rmcSmartFridgeRow.NameButton.ClearOnPressed();
              // ISSUE: reference to a compiler-generated method
              rmcSmartFridgeRow.NameButton.OnPressed += new Action<BaseButton.ButtonEventArgs>(cDisplayClass90.\u003CRefresh\u003Eb__0);
              MetaDataComponent metaDataComponent;
              if (this.EntMan.TryGetComponent<MetaDataComponent>(entityUid, ref metaDataComponent))
              {
                // ISSUE: object of a compiler-generated type is created
                // ISSUE: variable of a compiler-generated type
                RMCSmartFridgeBui.\u003C\u003Ec__DisplayClass9_1 cDisplayClass91 = new RMCSmartFridgeBui.\u003C\u003Ec__DisplayClass9_1();
                ((Control) rmcSmartFridgeRow.TooltipLabel).Visible = true;
                FormattedMessage formattedMessage = new FormattedMessage();
                formattedMessage.AddText(key3);
                formattedMessage.PushNewline();
                if (!string.IsNullOrWhiteSpace(metaDataComponent.EntityDescription))
                  formattedMessage.AddText(metaDataComponent.EntityDescription);
                // ISSUE: reference to a compiler-generated field
                cDisplayClass91.tooltip = new Tooltip();
                // ISSUE: reference to a compiler-generated field
                cDisplayClass91.tooltip.SetMessage(formattedMessage);
                ((Control) rmcSmartFridgeRow.TooltipLabel).ToolTip = metaDataComponent.EntityDescription;
                ((Control) rmcSmartFridgeRow.TooltipLabel).TooltipDelay = new float?(0.0f);
                // ISSUE: method pointer
                ((Control) rmcSmartFridgeRow.TooltipLabel).TooltipSupplier = new TooltipSupplier((object) cDisplayClass91, __methodptr(\u003CRefresh\u003Eb__1));
              }
              else
                ((Control) rmcSmartFridgeRow.TooltipLabel).Visible = false;
              ++after2;
            }
          }
          ((Control) smartFridgeSection.Container).RemoveChildrenAfter(after2);
          ++after1;
        }
      }
      ((Control) contentsTabs).SetTabVisibleAfter(after1, false);
      ((Control) contentsTabs).SetVisibleAfter(after1, false);
      int currentTab = contentsTabs.CurrentTab;
      if (currentTab >= ((Control) contentsTabs).ChildCount || ((Control) contentsTabs).GetChild(currentTab).Visible)
        return;
      contentsTabs.CurrentTab = 0;
    }
  }
}
