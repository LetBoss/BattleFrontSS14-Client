using Content.Client.Message;
using Content.Shared.Remotes.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Remote.UI;

public sealed class DoorRemoteStatusControl : Control
{
	private readonly Entity<DoorRemoteComponent> _entity;

	private readonly RichTextLabel _label;

	private OperatingMode PrevOperatingMode = OperatingMode.placeholderForUiUpdates;

	public DoorRemoteStatusControl(Entity<DoorRemoteComponent> entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		_entity = entity;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (PrevOperatingMode != _entity.Comp.Mode)
		{
			PrevOperatingMode = _entity.Comp.Mode;
			string item = Loc.GetString(_entity.Comp.Mode switch
			{
				OperatingMode.OpenClose => "door-remote-open-close-text", 
				OperatingMode.ToggleBolts => "door-remote-toggle-bolt-text", 
				OperatingMode.ToggleEmergencyAccess => "door-remote-emergency-access-text", 
				_ => "door-remote-invalid-text", 
			});
			_label.SetMarkup(Loc.GetString("door-remote-mode-label", new(string, object)[1] { ("modeString", item) }));
		}
	}
}
