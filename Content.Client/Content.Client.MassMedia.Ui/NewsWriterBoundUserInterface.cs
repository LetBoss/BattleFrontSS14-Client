using System;
using Content.Shared.MassMedia.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client.MassMedia.Ui;

public sealed class NewsWriterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private NewsWriterMenu? _menu;

	public NewsWriterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<NewsWriterMenu>((BoundUserInterface)(object)this);
		_menu.ArticleEditorPanel.PublishButtonPressed += OnPublishButtonPressed;
		_menu.DeleteButtonPressed += OnDeleteButtonPressed;
		_menu.CreateButtonPressed += OnCreateButtonPressed;
		_menu.ArticleEditorPanel.ArticleDraftUpdated += OnArticleDraftUpdated;
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NewsWriterArticlesRequestMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is NewsWriterBoundUserInterfaceState newsWriterBoundUserInterfaceState)
		{
			_menu?.UpdateUI(newsWriterBoundUserInterfaceState.Articles, newsWriterBoundUserInterfaceState.PublishEnabled, newsWriterBoundUserInterfaceState.NextPublish, newsWriterBoundUserInterfaceState.DraftTitle, newsWriterBoundUserInterfaceState.DraftContent);
		}
	}

	private void OnPublishButtonPressed()
	{
		NewsWriterMenu? menu = _menu;
		string text = ((menu != null) ? menu.ArticleEditorPanel.TitleField.Text.Trim() : null) ?? "";
		if (_menu != null && text.Length != 0)
		{
			string text2 = Rope.Collapse(_menu.ArticleEditorPanel.ContentField.TextRope).Trim();
			if (text2.Length != 0)
			{
				string title = ((text.Length <= 25) ? text : (text.Substring(0, 22) + "..."));
				string content = ((text2.Length <= 2048) ? text2 : (text2.Substring(0, 2045) + "..."));
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NewsWriterPublishMessage(title, content));
			}
		}
	}

	private void OnDeleteButtonPressed(int articleNum)
	{
		if (_menu != null)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NewsWriterDeleteMessage(articleNum));
		}
	}

	private void OnCreateButtonPressed()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NewsWriterRequestDraftMessage());
	}

	private void OnArticleDraftUpdated(string title, string content)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NewsWriterSaveDraftMessage(title, content));
	}
}
