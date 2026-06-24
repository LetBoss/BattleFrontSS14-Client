// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingCategoriesConversion
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable disable
namespace Content.Shared.Humanoid.Markings;

public static class MarkingCategoriesConversion
{
  public static MarkingCategories FromHumanoidVisualLayers(HumanoidVisualLayers layer)
  {
    MarkingCategories markingCategories;
    switch (layer)
    {
      case HumanoidVisualLayers.Special:
        markingCategories = MarkingCategories.Special;
        break;
      case HumanoidVisualLayers.Tail:
        markingCategories = MarkingCategories.Tail;
        break;
      case HumanoidVisualLayers.Hair:
        markingCategories = MarkingCategories.Hair;
        break;
      case HumanoidVisualLayers.FacialHair:
        markingCategories = MarkingCategories.FacialHair;
        break;
      case HumanoidVisualLayers.UndergarmentTop:
        markingCategories = MarkingCategories.UndergarmentTop;
        break;
      case HumanoidVisualLayers.UndergarmentBottom:
        markingCategories = MarkingCategories.UndergarmentBottom;
        break;
      case HumanoidVisualLayers.Chest:
        markingCategories = MarkingCategories.Chest;
        break;
      case HumanoidVisualLayers.Head:
        markingCategories = MarkingCategories.Head;
        break;
      case HumanoidVisualLayers.Snout:
        markingCategories = MarkingCategories.Snout;
        break;
      case HumanoidVisualLayers.HeadSide:
        markingCategories = MarkingCategories.HeadSide;
        break;
      case HumanoidVisualLayers.HeadTop:
        markingCategories = MarkingCategories.HeadTop;
        break;
      case HumanoidVisualLayers.Eyes:
        markingCategories = MarkingCategories.Eyes;
        break;
      case HumanoidVisualLayers.RArm:
        markingCategories = MarkingCategories.Arms;
        break;
      case HumanoidVisualLayers.LArm:
        markingCategories = MarkingCategories.Arms;
        break;
      case HumanoidVisualLayers.RHand:
        markingCategories = MarkingCategories.Arms;
        break;
      case HumanoidVisualLayers.LHand:
        markingCategories = MarkingCategories.Arms;
        break;
      case HumanoidVisualLayers.RLeg:
        markingCategories = MarkingCategories.Legs;
        break;
      case HumanoidVisualLayers.LLeg:
        markingCategories = MarkingCategories.Legs;
        break;
      case HumanoidVisualLayers.RFoot:
        markingCategories = MarkingCategories.Legs;
        break;
      case HumanoidVisualLayers.LFoot:
        markingCategories = MarkingCategories.Legs;
        break;
      default:
        markingCategories = MarkingCategories.Overlay;
        break;
    }
    return markingCategories;
  }
}
