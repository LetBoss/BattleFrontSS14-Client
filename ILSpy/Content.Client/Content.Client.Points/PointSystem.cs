using System;
using Content.Client.CharacterInfo;
using Content.Client.Message;
using Content.Shared.Points;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Points;

public sealed class PointSystem : SharedPointSystem
{
	[Dependency]
	private CharacterInfoSystem _characterInfo;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PointManagerComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<PointManagerComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CharacterInfoSystem.GetCharacterInfoControlsEvent>((EntityEventRefHandler<CharacterInfoSystem.GetCharacterInfoControlsEvent>)OnGetCharacterInfoControls, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, PointManagerComponent component, ref AfterAutoHandleStateEvent args)
	{
		_characterInfo.RequestCharacterInfo();
	}

	private void OnGetCharacterInfoControls(ref CharacterInfoSystem.GetCharacterInfoControlsEvent ev)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		foreach (PointManagerComponent item in ((EntitySystem)this).EntityQuery<PointManagerComponent>(false))
		{
			BoxContainer val = new BoxContainer
			{
				Margin = new Thickness(5f),
				Orientation = (LayoutOrientation)1
			};
			RichTextLabel val2 = new RichTextLabel
			{
				HorizontalAlignment = (HAlignment)2
			};
			val2.SetMarkup(((EntitySystem)this).Loc.GetString("point-scoreboard-header"));
			RichTextLabel val3 = new RichTextLabel();
			val3.SetMessage(item.Scoreboard, (Color?)null);
			((Control)val).AddChild((Control)(object)val2);
			((Control)val).AddChild((Control)(object)val3);
			ev.Controls.Add((Control)(object)val);
		}
	}
}
