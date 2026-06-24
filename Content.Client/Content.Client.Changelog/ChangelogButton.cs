using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Changelog;

public sealed class ChangelogButton : Button
{
	[Dependency]
	private ChangelogManager _changelogManager;

	public ChangelogButton()
	{
		IoCManager.InjectDependencies<ChangelogButton>(this);
		((Button)this).Text = " ";
	}

	protected override void EnteredTree()
	{
		((Control)this).EnteredTree();
		_changelogManager.NewChangelogEntriesChanged += UpdateStuff;
		UpdateStuff();
	}

	protected override void ExitedTree()
	{
		((Control)this).ExitedTree();
		_changelogManager.NewChangelogEntriesChanged -= UpdateStuff;
	}

	private void UpdateStuff()
	{
		if (_changelogManager.NewChangelogEntries)
		{
			((Button)this).Text = Loc.GetString("changelog-button-new-entries");
			((Control)this).StyleClasses.Add("Caution");
		}
		else
		{
			((Button)this).Text = Loc.GetString("changelog-button");
			((Control)this).StyleClasses.Remove("Caution");
		}
	}
}
