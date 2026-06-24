// Decompiled with JetBrains decompiler
// Type: Content.Client.CrewManifest.UI.CrewManifestSection
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CrewManifest;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.CrewManifest.UI;

public sealed class CrewManifestSection : BoxContainer
{
  public CrewManifestSection(
    IPrototypeManager prototypeManager,
    SpriteSystem spriteSystem,
    string sectionName,
    List<CrewManifestEntry> entries)
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) this).HorizontalExpand = true;
    Label label = new Label();
    ((Control) label).StyleClasses.Add("LabelBig");
    label.Text = Loc.GetString(sectionName);
    ((Control) this).AddChild((Control) label);
    GridContainer gridContainer1 = new GridContainer();
    ((Control) gridContainer1).HorizontalExpand = true;
    gridContainer1.Columns = 2;
    GridContainer gridContainer2 = gridContainer1;
    ((Control) this).AddChild((Control) gridContainer2);
    foreach (CrewManifestEntry entry in entries)
    {
      RichTextLabel richTextLabel1 = new RichTextLabel();
      ((Control) richTextLabel1).HorizontalExpand = true;
      RichTextLabel richTextLabel2 = richTextLabel1;
      richTextLabel2.SetMessage(entry.Name, new Color?());
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer1).HorizontalExpand = true;
      BoxContainer boxContainer2 = boxContainer1;
      RichTextLabel richTextLabel3 = new RichTextLabel();
      richTextLabel3.SetMessage(entry.JobTitle, new Color?());
      JobIconPrototype jobIconPrototype;
      if (prototypeManager.TryIndex<JobIconPrototype>(entry.JobIcon, ref jobIconPrototype))
      {
        TextureRect textureRect1 = new TextureRect();
        textureRect1.TextureScale = new Vector2(2f, 2f);
        ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
        textureRect1.Texture = spriteSystem.Frame0(jobIconPrototype.Icon);
        ((Control) textureRect1).Margin = new Thickness(0.0f, 0.0f, 4f, 0.0f);
        TextureRect textureRect2 = textureRect1;
        ((Control) boxContainer2).AddChild((Control) textureRect2);
        ((Control) boxContainer2).AddChild((Control) richTextLabel3);
      }
      else
        ((Control) boxContainer2).AddChild((Control) richTextLabel3);
      ((Control) gridContainer2).AddChild((Control) richTextLabel2);
      ((Control) gridContainer2).AddChild((Control) boxContainer2);
    }
  }

  public CrewManifestSection(
    IPrototypeManager prototypeManager,
    SpriteSystem spriteSystem,
    DepartmentPrototype section,
    List<CrewManifestEntry> entries)
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) this).HorizontalExpand = true;
    Label label = new Label();
    ((Control) label).StyleClasses.Add("LabelBig");
    label.Text = Loc.GetString(LocId.op_Implicit(section.Name));
    ((Control) this).AddChild((Control) label);
    GridContainer gridContainer1 = new GridContainer();
    ((Control) gridContainer1).HorizontalExpand = true;
    gridContainer1.Columns = 2;
    GridContainer gridContainer2 = gridContainer1;
    ((Control) this).AddChild((Control) gridContainer2);
    foreach (CrewManifestEntry entry in entries)
    {
      RichTextLabel richTextLabel1 = new RichTextLabel();
      ((Control) richTextLabel1).HorizontalExpand = true;
      RichTextLabel richTextLabel2 = richTextLabel1;
      richTextLabel2.SetMessage(entry.Name, new Color?());
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer1).HorizontalExpand = true;
      BoxContainer boxContainer2 = boxContainer1;
      RichTextLabel richTextLabel3 = new RichTextLabel();
      richTextLabel3.SetMessage(entry.JobTitle, new Color?());
      JobIconPrototype jobIconPrototype;
      if (prototypeManager.TryIndex<JobIconPrototype>(entry.JobIcon, ref jobIconPrototype))
      {
        TextureRect textureRect1 = new TextureRect();
        textureRect1.TextureScale = new Vector2(2f, 2f);
        ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
        textureRect1.Texture = spriteSystem.Frame0(jobIconPrototype.Icon);
        ((Control) textureRect1).Margin = new Thickness(0.0f, 0.0f, 4f, 0.0f);
        TextureRect textureRect2 = textureRect1;
        ((Control) boxContainer2).AddChild((Control) textureRect2);
        ((Control) boxContainer2).AddChild((Control) richTextLabel3);
      }
      else
        ((Control) boxContainer2).AddChild((Control) richTextLabel3);
      ((Control) gridContainer2).AddChild((Control) richTextLabel2);
      ((Control) gridContainer2).AddChild((Control) boxContainer2);
    }
  }
}
