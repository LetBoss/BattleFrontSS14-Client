// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.FancyWindowExt
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Guidebook.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public static class FancyWindowExt
{
  public static void SetInfoFromEntity(
    this FancyWindow window,
    IEntityManager entityManager,
    EntityUid entity)
  {
    window.SetTitleFromEntity(entityManager, entity);
    window.SetGuidebookFromEntity(entityManager, entity);
  }

  public static void SetTitleFromEntity(
    this FancyWindow window,
    IEntityManager entityManager,
    EntityUid entity)
  {
    window.Title = entityManager.GetComponent<MetaDataComponent>(entity).EntityName;
  }

  public static void SetGuidebookFromEntity(
    this FancyWindow window,
    IEntityManager entityManager,
    EntityUid entity)
  {
    window.HelpGuidebookIds = EntityManagerExt.GetComponentOrNull<GuideHelpComponent>(entityManager, entity)?.Guides;
  }
}
