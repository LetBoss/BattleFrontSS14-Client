// Decompiled with JetBrains decompiler
// Type: Content.Client.Construction.UI.IConstructionMenuView
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared.Construction.Prototypes;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Construction.UI;

public interface IConstructionMenuView : IDisposable
{
  string[] Categories { get; set; }

  OptionButton OptionCategories { get; }

  bool EraseButtonPressed { get; set; }

  bool GridViewButtonPressed { get; set; }

  bool BuildButtonPressed { get; set; }

  ListContainer Recipes { get; }

  ItemList RecipeStepList { get; }

  ScrollContainer RecipesGridScrollContainer { get; }

  GridContainer RecipesGrid { get; }

  event EventHandler<(string search, string catagory)> PopulateRecipes;

  event EventHandler<ConstructionMenu.ConstructionMenuListData?> RecipeSelected;

  event EventHandler RecipeFavorited;

  event EventHandler<bool> BuildButtonToggled;

  event EventHandler<bool> EraseButtonToggled;

  event EventHandler ClearAllGhosts;

  void ClearRecipeInfo();

  void SetRecipeInfo(
    string name,
    string description,
    EntityPrototype? targetPrototype,
    Color iconColor,
    bool isItem,
    bool isFavorite,
    ConstructionPrototype prototype);

  void ResetPlacement();

  event Action? OnClose;

  bool IsOpen { get; }

  void OpenCentered();

  void MoveToFront();

  bool IsAtFront();

  void Close();
}
