using Content.Client.Guidebook.Components;
using Robust.Shared.GameObjects;

namespace Content.Client.UserInterface.Controls;

public static class FancyWindowExt
{
	public static void SetInfoFromEntity(this FancyWindow window, IEntityManager entityManager, EntityUid entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		window.SetTitleFromEntity(entityManager, entity);
		window.SetGuidebookFromEntity(entityManager, entity);
	}

	public static void SetTitleFromEntity(this FancyWindow window, IEntityManager entityManager, EntityUid entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		window.Title = entityManager.GetComponent<MetaDataComponent>(entity).EntityName;
	}

	public static void SetGuidebookFromEntity(this FancyWindow window, IEntityManager entityManager, EntityUid entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		window.HelpGuidebookIds = EntityManagerExt.GetComponentOrNull<GuideHelpComponent>(entityManager, entity)?.Guides;
	}
}
