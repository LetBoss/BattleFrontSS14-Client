// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Controls.GuideMicrowaveGroupEmbed
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Guidebook.Richtext;
using Content.Shared.Kitchen;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Client.Guidebook.Controls;

public sealed class GuideMicrowaveGroupEmbed : BoxContainer, IDocumentTag
{
  [Dependency]
  private ILogManager _logManager;
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly ISawmill _sawmill;

  public GuideMicrowaveGroupEmbed()
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    IoCManager.InjectDependencies<GuideMicrowaveGroupEmbed>(this);
    this._sawmill = this._logManager.GetSawmill("guidebook.microwave_group");
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
  }

  public GuideMicrowaveGroupEmbed(string group)
    : this()
  {
    this.CreateEntries(group);
  }

  public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
  {
    control = (Control) null;
    string group;
    if (!args.TryGetValue("Group", out group))
    {
      this._sawmill.Error("Microwave group embed tag is missing group argument");
      return false;
    }
    this.CreateEntries(group);
    control = (Control) this;
    return true;
  }

  private void CreateEntries(string group)
  {
    foreach (FoodRecipePrototype recipe in (IEnumerable<FoodRecipePrototype>) this._prototype.EnumeratePrototypes<FoodRecipePrototype>().Where<FoodRecipePrototype>((Func<FoodRecipePrototype, bool>) (p => p.Group.Equals(group))).OrderBy<FoodRecipePrototype, string>((Func<FoodRecipePrototype, string>) (p => p.Name)))
      ((Control) this).AddChild((Control) new GuideMicrowaveEmbed(recipe));
  }
}
