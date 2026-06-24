using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NewsReaderUi : UIFragment, ISerializationGenerated<NewsReaderUi>, ISerializationGenerated
{
	private NewsReaderUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		_fragment = new NewsReaderUiFragment();
		_fragment.OnNextButtonPressed += delegate
		{
			SendNewsReaderMessage(NewsReaderUiAction.Next, userInterface);
		};
		_fragment.OnPrevButtonPressed += delegate
		{
			SendNewsReaderMessage(NewsReaderUiAction.Prev, userInterface);
		};
		_fragment.OnNotificationSwithPressed += delegate
		{
			SendNewsReaderMessage(NewsReaderUiAction.NotificationSwitch, userInterface);
		};
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (!(state is NewsReaderBoundUserInterfaceState newsReaderBoundUserInterfaceState))
		{
			if (state is NewsReaderEmptyBoundUserInterfaceState newsReaderEmptyBoundUserInterfaceState)
			{
				_fragment?.UpdateEmptyState(newsReaderEmptyBoundUserInterfaceState.NotificationOn);
			}
		}
		else
		{
			_fragment?.UpdateState(newsReaderBoundUserInterfaceState.Article, newsReaderBoundUserInterfaceState.TargetNum, newsReaderBoundUserInterfaceState.TotalNum, newsReaderBoundUserInterfaceState.NotificationOn);
		}
	}

	private void SendNewsReaderMessage(NewsReaderUiAction action, BoundUserInterface userInterface)
	{
		CartridgeUiMessage cartridgeUiMessage = new CartridgeUiMessage(new NewsReaderUiMessageEvent(action));
		userInterface.SendMessage((BoundUserInterfaceMessage)(object)cartridgeUiMessage);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NewsReaderUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (NewsReaderUi)target2;
		serialization.TryCustomCopy<NewsReaderUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NewsReaderUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NewsReaderUi target2 = (NewsReaderUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NewsReaderUi target2 = (NewsReaderUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NewsReaderUi Instantiate()
	{
		return new NewsReaderUi();
	}
}
