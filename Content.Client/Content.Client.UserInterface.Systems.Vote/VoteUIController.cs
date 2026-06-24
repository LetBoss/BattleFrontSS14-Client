using System;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.Voting;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Vote;

public sealed class VoteUIController : UIController
{
	[Dependency]
	private IVoteManager _votes;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
	}

	private void OnScreenLoad()
	{
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (!(activeScreen is DefaultGameScreen defaultGameScreen))
		{
			if (!(activeScreen is SeparatedChatGameScreen separatedChatGameScreen))
			{
				if (activeScreen is BattlefrontGameScreen battlefrontGameScreen)
				{
					_votes.SetPopupContainer((Control)(object)battlefrontGameScreen.VoteMenu);
				}
			}
			else
			{
				_votes.SetPopupContainer((Control)(object)separatedChatGameScreen.VoteMenu);
			}
		}
		else
		{
			_votes.SetPopupContainer((Control)(object)defaultGameScreen.VoteMenu);
		}
	}

	private void OnScreenUnload()
	{
		_votes.ClearPopupContainer();
	}
}
