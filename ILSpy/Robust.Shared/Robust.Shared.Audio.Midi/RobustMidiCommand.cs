namespace Robust.Shared.Audio.Midi;

public enum RobustMidiCommand : byte
{
	NoteOff = 128,
	NoteOn = 144,
	AfterTouch = 160,
	ControlChange = 176,
	ProgramChange = 192,
	ChannelPressure = 208,
	PitchBend = 224,
	SystemMessage = 240
}
