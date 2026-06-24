// Decompiled with JetBrains decompiler
// Type: Content.Client.MassMedia.Ui.NewsWriterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.MassMedia.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.MassMedia.Ui;

public sealed class NewsWriterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private NewsWriterMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<NewsWriterMenu>((BoundUserInterface) this);
    this._menu.ArticleEditorPanel.PublishButtonPressed += new Action(this.OnPublishButtonPressed);
    this._menu.DeleteButtonPressed += new Action<int>(this.OnDeleteButtonPressed);
    this._menu.CreateButtonPressed += new Action(this.OnCreateButtonPressed);
    this._menu.ArticleEditorPanel.ArticleDraftUpdated += new Action<string, string>(this.OnArticleDraftUpdated);
    this.SendMessage((BoundUserInterfaceMessage) new NewsWriterArticlesRequestMessage());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is NewsWriterBoundUserInterfaceState userInterfaceState))
      return;
    this._menu?.UpdateUI(userInterfaceState.Articles, userInterfaceState.PublishEnabled, userInterfaceState.NextPublish, userInterfaceState.DraftTitle, userInterfaceState.DraftContent);
  }

  private void OnPublishButtonPressed()
  {
    string str1 = this._menu?.ArticleEditorPanel.TitleField.Text.Trim() ?? "";
    if (this._menu == null || str1.Length == 0)
      return;
    string str2 = Rope.Collapse(this._menu.ArticleEditorPanel.ContentField.TextRope).Trim();
    if (str2.Length == 0)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new NewsWriterPublishMessage(str1.Length <= 25 ? str1 : str1.Substring(0, 22) + "...", str2.Length <= 2048 /*0x0800*/ ? str2 : str2.Substring(0, 2045) + "..."));
  }

  private void OnDeleteButtonPressed(int articleNum)
  {
    if (this._menu == null)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new NewsWriterDeleteMessage(articleNum));
  }

  private void OnCreateButtonPressed()
  {
    this.SendMessage((BoundUserInterfaceMessage) new NewsWriterRequestDraftMessage());
  }

  private void OnArticleDraftUpdated(string title, string content)
  {
    this.SendMessage((BoundUserInterfaceMessage) new NewsWriterSaveDraftMessage(title, content));
  }
}
