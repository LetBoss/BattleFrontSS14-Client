using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Commendations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client._RMC14.Commendations;

public sealed class CommendationsManager : IPostInjectInit
{
	[Dependency]
	private INetManager _net;

	private CommendationsWindow? _receivedWindow;

	private CommendationsWindow? _givenWindow;

	private readonly List<Commendation> _commendationsReceived = new List<Commendation>();

	private readonly List<Commendation> _commendationsGiven = new List<Commendation>();

	public void PostInject()
	{
		_net.RegisterNetMessage<CommendationsMsg>((ProcessMessage<CommendationsMsg>)OnCommendations, (NetMessageAccept)3);
	}

	private void OnCommendations(CommendationsMsg message)
	{
		_commendationsReceived.Clear();
		_commendationsReceived.AddRange(message.CommendationsReceived.OrderByDescending((Commendation c) => c.Round));
		_commendationsGiven.Clear();
		_commendationsGiven.AddRange(message.CommendationsGiven.OrderByDescending((Commendation c) => c.Round));
	}

	private void OpenWindow(ref CommendationsWindow? window, Action onClose, List<Commendation> commendations)
	{
		if (window != null)
		{
			((BaseWindow)window).MoveToFront();
			return;
		}
		window = new CommendationsWindow();
		((BaseWindow)window).OnClose += onClose;
		((BaseWindow)window).OpenCentered();
		foreach (Commendation commendation in commendations)
		{
			CommendationContainer commendationContainer = new CommendationContainer();
			commendationContainer.Title.Text = $"[bold]Round {commendation.Round} - {commendation.Name}[/bold]";
			commendationContainer.Description.Text = $"Issued to [bold]{commendation.Receiver}[/bold] by [bold]{commendation.Giver}[/bold] for:\n{commendation.Text}";
			((Control)window.Commendations).AddChild((Control)(object)commendationContainer);
		}
	}

	public void OpenReceivedWindow()
	{
		OpenWindow(ref _receivedWindow, delegate
		{
			_receivedWindow = null;
		}, _commendationsReceived);
	}

	public void OpenGivenWindow()
	{
		OpenWindow(ref _givenWindow, delegate
		{
			_givenWindow = null;
		}, _commendationsGiven);
	}
}
