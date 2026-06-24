using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.Lobby.UI;

public sealed class PubgCharacterSetupGuiSavePanel : DefaultWindow
{
	public Button SaveButton { get; }

	public Button NoSaveButton { get; }

	public PubgCharacterSetupGuiSavePanel()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("character-setup-gui-save-panel-title");
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(10f)
		};
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("character-setup-gui-save-panel-text")
		});
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(0f, 10f, 0f, 0f)
		};
		SaveButton = new Button
		{
			Text = Loc.GetString("character-setup-gui-save-panel-save-button")
		};
		NoSaveButton = new Button
		{
			Text = Loc.GetString("character-setup-gui-save-panel-nosave-button"),
			Margin = new Thickness(10f, 0f, 0f, 0f)
		};
		((Control)val2).AddChild((Control)(object)SaveButton);
		((Control)val2).AddChild((Control)(object)NoSaveButton);
		((Control)val).AddChild((Control)(object)val2);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}
}
