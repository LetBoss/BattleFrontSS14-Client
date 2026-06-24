namespace Content.Shared.APC;

public enum ApcChannelState : sbyte
{
	None = 0,
	Control = 1,
	Auto = 0,
	Manual = 1,
	Power = 2,
	Off = 0,
	On = 2,
	All = 3,
	AutoOff = 0,
	AutoOn = 2,
	ManualOff = 1,
	ManualOn = 3,
	LogWidth = 1
}
