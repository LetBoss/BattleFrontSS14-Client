using System;
using System.Linq;
using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.UserInterface;
using Content.Shared.Administration;
using Content.Shared.IdentityManagement;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.ContextMenu.UI;

public sealed class EntityMenuElement : ContextMenuElement, IEntityControl
{
	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPlayerManager _playerManager;

	private AdminSystem _adminSystem;

	public EntityUid? Entity;

	public int Count { get; private set; }

	EntityUid? IEntityControl.UiEntity => Entity;

	public EntityMenuElement(EntityUid? entity = null)
	{
		IoCManager.InjectDependencies<EntityMenuElement>(this);
		_adminSystem = _entityManager.System<AdminSystem>();
		Entity = entity;
		if (Entity.HasValue)
		{
			Count = 1;
			UpdateEntity();
		}
	}

	[Obsolete]
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		Entity = null;
		Count = 0;
	}

	private string? SearchPlayerName(EntityUid entity)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netEntity = _entityManager.GetNetEntity(entity, (MetaDataComponent)null);
		return _adminSystem.PlayerList.FirstOrDefault(delegate(PlayerInfo player)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			NetEntity? netEntity2 = player.NetEntity;
			NetEntity val = netEntity;
			return netEntity2.HasValue && netEntity2.GetValueOrDefault() == val;
		})?.Username;
	}

	public unsafe void UpdateCount()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (base.SubMenu == null)
		{
			return;
		}
		Count = 0;
		Enumerator enumerator = ((Control)base.SubMenu.MenuBody).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is EntityMenuElement entityMenuElement)
				{
					Count += entityMenuElement.Count;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		((Control)base.IconLabel).Visible = Count > 1;
		if (((Control)base.IconLabel).Visible)
		{
			base.IconLabel.Text = Count.ToString();
		}
	}

	private string GetEntityDescriptionAdmin(EntityUid entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		EntityStringRepresentation val = _entityManager.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity));
		string name = ((EntityStringRepresentation)(ref val)).Name;
		string prototype = ((EntityStringRepresentation)(ref val)).Prototype;
		ICommonSession session = ((EntityStringRepresentation)(ref val)).Session;
		string text = ((session != null) ? session.Name : null) ?? SearchPlayerName(entity);
		bool deleted = ((EntityStringRepresentation)(ref val)).Deleted;
		return $"{name} ({((object)_entityManager.GetNetEntity(entity, (MetaDataComponent)null)/*cast due to constrained. prefix*/).ToString()}{((prototype != null) ? (", " + prototype) : "")}{((text != null) ? (", " + text) : "")}){(deleted ? "D" : "")}";
	}

	private string GetEntityDescription(EntityUid entity)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (_adminManager.HasFlag(AdminFlags.Admin | AdminFlags.Debug))
		{
			return GetEntityDescriptionAdmin(entity);
		}
		return Identity.Name(entity, _entityManager, ((ISharedPlayerManager)_playerManager).LocalEntity);
	}

	public void UpdateEntity(EntityUid? entity = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = entity;
		if (!val.HasValue)
		{
			entity = Entity;
		}
		if (_entityManager.Deleted(entity))
		{
			base.Icon.SetEntity((EntityUid?)null);
			Text = string.Empty;
		}
		else
		{
			base.Icon.SetEntity(entity);
			Text = GetEntityDescription(entity.Value);
		}
	}
}
