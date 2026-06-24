using Content.Client.UserInterface.Controls;
using Content.Shared.Hands.Components;
using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Systems.Hands.Controls;

public sealed class HandButton : SlotControl
{
	public HandLocation HandLocation { get; }

	public HandButton(string handName, HandLocation handLocation)
	{
		HandLocation = handLocation;
		((Control)this).Name = "hand_" + handName;
		base.SlotName = handName;
		SetBackground(handLocation);
	}

	private void SetBackground(HandLocation handLoc)
	{
		base.ButtonTexturePath = handLoc switch
		{
			HandLocation.Left => "Slots/hand_l", 
			HandLocation.Middle => "Slots/hand_m", 
			HandLocation.Right => "Slots/hand_r", 
			_ => base.ButtonTexturePath, 
		};
	}
}
