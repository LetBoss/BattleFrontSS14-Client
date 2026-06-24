using System.Collections.Generic;
using System.Numerics;
using Content.Shared.CrewManifest;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.CrewManifest.UI;

public sealed class CrewManifestSection : BoxContainer
{
	public CrewManifestSection(IPrototypeManager prototypeManager, SpriteSystem spriteSystem, string sectionName, List<CrewManifestEntry> entries)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((Control)this).HorizontalExpand = true;
		((Control)this).AddChild((Control)new Label
		{
			StyleClasses = { "LabelBig" },
			Text = Loc.GetString(sectionName)
		});
		GridContainer val = new GridContainer
		{
			HorizontalExpand = true,
			Columns = 2
		};
		((Control)this).AddChild((Control)(object)val);
		JobIconPrototype jobIconPrototype = default(JobIconPrototype);
		foreach (CrewManifestEntry entry in entries)
		{
			RichTextLabel val2 = new RichTextLabel
			{
				HorizontalExpand = true
			};
			val2.SetMessage(entry.Name, (Color?)null);
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalExpand = true
			};
			RichTextLabel val4 = new RichTextLabel();
			val4.SetMessage(entry.JobTitle, (Color?)null);
			if (prototypeManager.TryIndex<JobIconPrototype>(entry.JobIcon, ref jobIconPrototype))
			{
				TextureRect val5 = new TextureRect
				{
					TextureScale = new Vector2(2f, 2f),
					VerticalAlignment = (VAlignment)2,
					Texture = spriteSystem.Frame0(jobIconPrototype.Icon),
					Margin = new Thickness(0f, 0f, 4f, 0f)
				};
				((Control)val3).AddChild((Control)(object)val5);
				((Control)val3).AddChild((Control)(object)val4);
			}
			else
			{
				((Control)val3).AddChild((Control)(object)val4);
			}
			((Control)val).AddChild((Control)(object)val2);
			((Control)val).AddChild((Control)(object)val3);
		}
	}

	public CrewManifestSection(IPrototypeManager prototypeManager, SpriteSystem spriteSystem, DepartmentPrototype section, List<CrewManifestEntry> entries)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((Control)this).HorizontalExpand = true;
		((Control)this).AddChild((Control)new Label
		{
			StyleClasses = { "LabelBig" },
			Text = Loc.GetString(LocId.op_Implicit(section.Name))
		});
		GridContainer val = new GridContainer
		{
			HorizontalExpand = true,
			Columns = 2
		};
		((Control)this).AddChild((Control)(object)val);
		JobIconPrototype jobIconPrototype = default(JobIconPrototype);
		foreach (CrewManifestEntry entry in entries)
		{
			RichTextLabel val2 = new RichTextLabel
			{
				HorizontalExpand = true
			};
			val2.SetMessage(entry.Name, (Color?)null);
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalExpand = true
			};
			RichTextLabel val4 = new RichTextLabel();
			val4.SetMessage(entry.JobTitle, (Color?)null);
			if (prototypeManager.TryIndex<JobIconPrototype>(entry.JobIcon, ref jobIconPrototype))
			{
				TextureRect val5 = new TextureRect
				{
					TextureScale = new Vector2(2f, 2f),
					VerticalAlignment = (VAlignment)2,
					Texture = spriteSystem.Frame0(jobIconPrototype.Icon),
					Margin = new Thickness(0f, 0f, 4f, 0f)
				};
				((Control)val3).AddChild((Control)(object)val5);
				((Control)val3).AddChild((Control)(object)val4);
			}
			else
			{
				((Control)val3).AddChild((Control)(object)val4);
			}
			((Control)val).AddChild((Control)(object)val2);
			((Control)val).AddChild((Control)(object)val3);
		}
	}
}
