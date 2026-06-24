using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.UserInterface.Controls;

public sealed class ListContainerButton : ContainerButton, IEntityControl
{
	public readonly ListData Data;

	public readonly int Index;

	public EntityUid? UiEntity => (Data as EntityListData)?.Uid;

	public ListContainerButton(ListData data, int index)
	{
		((Control)this).AddStyleClass("button");
		Data = data;
		Index = index;
	}
}
