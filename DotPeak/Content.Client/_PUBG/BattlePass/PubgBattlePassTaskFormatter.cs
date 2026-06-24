// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.BattlePass.PubgBattlePassTaskFormatter
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.BattlePass;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Client._PUBG.BattlePass;

public static class PubgBattlePassTaskFormatter
{
  public static string GetTaskDisplayText(BattlePassTaskInfo task)
  {
    string taskType = task.TaskType;
    string nameKey;
    if (taskType != null)
    {
      switch (taskType.Length)
      {
        case 5:
          if (taskType == "kills")
          {
            nameKey = Loc.GetString("pubg-bp-task-kills", new (string, object)[1]
            {
              ("value", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
        case 6:
          if (taskType == "damage")
          {
            nameKey = Loc.GetString("pubg-bp-task-damage", new (string, object)[1]
            {
              ("value", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
        case 8:
          if (taskType == "survival")
          {
            nameKey = Loc.GetString("pubg-bp-task-survival", new (string, object)[1]
            {
              ("value", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
        case 9:
          if (taskType == "placement")
          {
            nameKey = Loc.GetString("pubg-bp-task-placement", new (string, object)[2]
            {
              ("value", (object) PubgBattlePassTaskFormatter.ExtractPlacementTarget(task.NameKey)),
              ("count", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
        case 11:
          if (taskType == "revive_ally")
          {
            nameKey = Loc.GetString("pubg-bp-task-revive-ally", new (string, object)[1]
            {
              ("value", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
        case 12:
          switch (taskType[0])
          {
            case 'e':
              if (taskType == "energy_drink")
              {
                nameKey = Loc.GetString("pubg-bp-task-energy-drink", new (string, object)[1]
                {
                  ("value", (object) task.TargetValue)
                });
                goto label_24;
              }
              break;
            case 'h':
              if (taskType == "healing_full")
              {
                nameKey = Loc.GetString("pubg-bp-task-healing-full", new (string, object)[1]
                {
                  ("value", (object) task.TargetValue)
                });
                goto label_24;
              }
              break;
          }
          break;
        case 13:
          if (taskType == "damage_weapon")
          {
            nameKey = Loc.GetString("pubg-bp-task-damage-weapon", new (string, object)[2]
            {
              ("value", (object) task.TargetValue),
              ("weapon", (object) (task.WeaponId ?? "?"))
            });
            goto label_24;
          }
          break;
        case 14:
          if (taskType == "healing_medkit")
          {
            nameKey = Loc.GetString("pubg-bp-task-healing-medkit", new (string, object)[1]
            {
              ("value", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
        case 15:
          if (taskType == "healing_bandage")
          {
            nameKey = Loc.GetString("pubg-bp-task-healing-bandage", new (string, object)[1]
            {
              ("value", (object) task.TargetValue)
            });
            goto label_24;
          }
          break;
      }
    }
    nameKey = task.NameKey;
label_24:
    return nameKey;
  }

  private static int ExtractPlacementTarget(string nameKey)
  {
    int num = 0;
    bool flag = false;
    foreach (char ch in nameKey)
    {
      switch (ch)
      {
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          flag = true;
          num = num * 10 + ((int) ch - 48 /*0x30*/);
          break;
        default:
          if (flag)
            goto label_6;
          break;
      }
    }
label_6:
    return !flag ? 10 : num;
  }
}
