using System;
using Content.Shared._RMC14.Medical.HUD;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Medical.HUD.Holocard;

public sealed class HolocardChangeBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IEntityManager _entities;

	private HolocardChangeWindow? _window;

	public HolocardChangeBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<HolocardChangeBoundUserInterface>(this);
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<HolocardChangeWindow>((BoundUserInterface)(object)this);
		_window.HolocardStateList.OnItemSelected += delegate(ItemListSelectedEventArgs obj)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			HolocardStatus? holocardStatus = (HolocardStatus?)((ItemListEventArgs)obj).ItemList[obj.ItemIndex].Metadata;
			if (holocardStatus.HasValue)
			{
				HolocardStatus valueOrDefault = holocardStatus.GetValueOrDefault();
				NetEntity? netEntity = _entities.GetNetEntity(((ISharedPlayerManager)_player).LocalEntity, (MetaDataComponent)null);
				if (netEntity.HasValue)
				{
					NetEntity valueOrDefault2 = netEntity.GetValueOrDefault();
					((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HolocardChangeEvent(valueOrDefault2, valueOrDefault));
				}
				((BoundUserInterface)this).Close();
			}
		};
	}
}
