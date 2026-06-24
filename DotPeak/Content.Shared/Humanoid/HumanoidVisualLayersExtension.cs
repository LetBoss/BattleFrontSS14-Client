// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HumanoidVisualLayersExtension
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Part;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid;

public static class HumanoidVisualLayersExtension
{
  public static bool HasSexMorph(HumanoidVisualLayers layer)
  {
    bool flag;
    switch (layer)
    {
      case HumanoidVisualLayers.Chest:
        flag = true;
        break;
      case HumanoidVisualLayers.Head:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  public static string GetSexMorph(HumanoidVisualLayers layer, Sex sex, string id)
  {
    if (!HumanoidVisualLayersExtension.HasSexMorph(layer) || sex == Sex.Unsexed)
      return id;
    return $"{id}{sex}";
  }

  public static IEnumerable<HumanoidVisualLayers> Sublayers(HumanoidVisualLayers layer)
  {
    switch (layer)
    {
      case HumanoidVisualLayers.Chest:
        yield return HumanoidVisualLayers.Chest;
        yield return HumanoidVisualLayers.Tail;
        break;
      case HumanoidVisualLayers.Head:
        yield return HumanoidVisualLayers.Head;
        yield return HumanoidVisualLayers.Eyes;
        yield return HumanoidVisualLayers.HeadSide;
        yield return HumanoidVisualLayers.HeadTop;
        yield return HumanoidVisualLayers.Hair;
        yield return HumanoidVisualLayers.FacialHair;
        yield return HumanoidVisualLayers.Snout;
        break;
      case HumanoidVisualLayers.RArm:
        yield return HumanoidVisualLayers.RArm;
        yield return HumanoidVisualLayers.RHand;
        break;
      case HumanoidVisualLayers.LArm:
        yield return HumanoidVisualLayers.LArm;
        yield return HumanoidVisualLayers.LHand;
        break;
      case HumanoidVisualLayers.RLeg:
        yield return HumanoidVisualLayers.RLeg;
        yield return HumanoidVisualLayers.RFoot;
        break;
      case HumanoidVisualLayers.LLeg:
        yield return HumanoidVisualLayers.LLeg;
        yield return HumanoidVisualLayers.LFoot;
        break;
    }
  }

  public static HumanoidVisualLayers? ToHumanoidLayers(this BodyPartComponent part)
  {
    switch (part.PartType)
    {
      case BodyPartType.Torso:
        return new HumanoidVisualLayers?(HumanoidVisualLayers.Chest);
      case BodyPartType.Head:
        return new HumanoidVisualLayers?(HumanoidVisualLayers.Head);
      case BodyPartType.Arm:
        switch (part.Symmetry)
        {
          case BodyPartSymmetry.Left:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.LArm);
          case BodyPartSymmetry.Right:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.RArm);
        }
        break;
      case BodyPartType.Hand:
        switch (part.Symmetry)
        {
          case BodyPartSymmetry.Left:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.LHand);
          case BodyPartSymmetry.Right:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.RHand);
        }
        break;
      case BodyPartType.Leg:
        switch (part.Symmetry)
        {
          case BodyPartSymmetry.Left:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.LLeg);
          case BodyPartSymmetry.Right:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.RLeg);
        }
        break;
      case BodyPartType.Foot:
        switch (part.Symmetry)
        {
          case BodyPartSymmetry.Left:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.LFoot);
          case BodyPartSymmetry.Right:
            return new HumanoidVisualLayers?(HumanoidVisualLayers.RFoot);
        }
        break;
      case BodyPartType.Tail:
        return new HumanoidVisualLayers?(HumanoidVisualLayers.Tail);
    }
    return new HumanoidVisualLayers?();
  }
}
