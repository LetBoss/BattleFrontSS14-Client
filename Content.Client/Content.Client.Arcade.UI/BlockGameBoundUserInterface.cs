using System;
using Content.Shared.Arcade;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Arcade.UI;

public sealed class BlockGameBoundUserInterface : BoundUserInterface
{
	private BlockGameMenu? _menu;

	public BlockGameBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<BlockGameMenu>((BoundUserInterface)(object)this);
		_menu.OnAction += SendAction;
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (!(message is BlockGameMessages.BlockGameVisualUpdateMessage blockGameVisualUpdateMessage))
		{
			if (!(message is BlockGameMessages.BlockGameScoreUpdateMessage blockGameScoreUpdateMessage))
			{
				if (!(message is BlockGameMessages.BlockGameUserStatusMessage blockGameUserStatusMessage))
				{
					if (!(message is BlockGameMessages.BlockGameSetScreenMessage blockGameSetScreenMessage))
					{
						if (!(message is BlockGameMessages.BlockGameHighScoreUpdateMessage blockGameHighScoreUpdateMessage))
						{
							if (message is BlockGameMessages.BlockGameLevelUpdateMessage blockGameLevelUpdateMessage)
							{
								_menu?.UpdateLevel(blockGameLevelUpdateMessage.Level);
							}
						}
						else
						{
							_menu?.UpdateHighscores(blockGameHighScoreUpdateMessage.LocalHighscores, blockGameHighScoreUpdateMessage.GlobalHighscores);
						}
						return;
					}
					if (blockGameSetScreenMessage.IsStarted)
					{
						_menu?.SetStarted();
					}
					_menu?.SetScreen(blockGameSetScreenMessage.Screen);
					if (blockGameSetScreenMessage is BlockGameMessages.BlockGameGameOverScreenMessage blockGameGameOverScreenMessage)
					{
						_menu?.SetGameoverInfo(blockGameGameOverScreenMessage.FinalScore, blockGameGameOverScreenMessage.LocalPlacement, blockGameGameOverScreenMessage.GlobalPlacement);
					}
				}
				else
				{
					_menu?.SetUsability(blockGameUserStatusMessage.IsPlayer);
				}
			}
			else
			{
				_menu?.UpdatePoints(blockGameScoreUpdateMessage.Points);
			}
		}
		else
		{
			switch (blockGameVisualUpdateMessage.GameVisualType)
			{
			case BlockGameMessages.BlockGameVisualType.GameField:
				_menu?.UpdateBlocks(blockGameVisualUpdateMessage.Blocks);
				break;
			case BlockGameMessages.BlockGameVisualType.HoldBlock:
				_menu?.UpdateHeldBlock(blockGameVisualUpdateMessage.Blocks);
				break;
			case BlockGameMessages.BlockGameVisualType.NextBlock:
				_menu?.UpdateNextBlock(blockGameVisualUpdateMessage.Blocks);
				break;
			}
		}
	}

	public void SendAction(BlockGamePlayerAction action)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BlockGameMessages.BlockGamePlayerActionMessage(action));
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			BlockGameMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
		}
	}
}
