using System.Numerics;

namespace Content.Client._RMC14.TacticalMap;

public struct TacticalMapSettings
{
	public float ZoomFactor = 0f;

	public Vector2 PanOffset = default(Vector2);

	public float BlipSizeMultiplier = 0f;

	public float LineThickness = 0f;

	public int SelectedColorIndex = 0;

	public bool SettingsVisible = false;

	public TacticalMapControl.LabelMode LabelMode = TacticalMapControl.LabelMode.Area;

	public Vector2 WindowSize = default(Vector2);

	public Vector2 WindowPosition = default(Vector2);

	public TacticalMapSettings()
	{
	}
}
