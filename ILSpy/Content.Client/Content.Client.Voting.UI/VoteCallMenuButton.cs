using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Voting.UI;

public sealed class VoteCallMenuButton : Button
{
	[Dependency]
	private IVoteManager _voteManager;

	public VoteCallMenuButton()
	{
		IoCManager.InjectDependencies<VoteCallMenuButton>(this);
		((Button)this).Text = Loc.GetString("ui-vote-menu-button");
		((BaseButton)this).OnPressed += OnOnPressed;
	}

	private void OnOnPressed(ButtonEventArgs obj)
	{
		((BaseWindow)new VoteCallMenu()).OpenCentered();
	}

	protected override void EnteredTree()
	{
		((Control)this).EnteredTree();
		UpdateCanCall(_voteManager.CanCallVote);
		_voteManager.CanCallVoteChanged += UpdateCanCall;
	}

	protected override void ExitedTree()
	{
		((Control)this).ExitedTree();
		_voteManager.CanCallVoteChanged += UpdateCanCall;
	}

	private void UpdateCanCall(bool canCall)
	{
		((BaseButton)this).Disabled = !canCall;
	}
}
