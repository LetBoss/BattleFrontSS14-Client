using System.Collections.Generic;
using Content.Client.NetworkConfigurator.Systems;
using Content.Shared.DeviceNetwork.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.NetworkConfigurator;

public sealed class NetworkConfiguratorLinkOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IRobustRandom _random;

	private readonly DeviceListSystem _deviceListSystem;

	private readonly SharedTransformSystem _transformSystem;

	public Dictionary<EntityUid, Color> Colors = new Dictionary<EntityUid, Color>();

	public EntityUid? Action;

	public override OverlaySpace Space => (OverlaySpace)4;

	public NetworkConfiguratorLinkOverlay()
	{
		IoCManager.InjectDependencies<NetworkConfiguratorLinkOverlay>(this);
		_deviceListSystem = _entityManager.System<DeviceListSystem>();
		_transformSystem = _entityManager.System<SharedTransformSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent> val = _entityManager.EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent>();
		EntityUid val2 = default(EntityUid);
		NetworkConfiguratorActiveLinkOverlayComponent networkConfiguratorActiveLinkOverlayComponent = default(NetworkConfiguratorActiveLinkOverlayComponent);
		DeviceListComponent component = default(DeviceListComponent);
		while (val.MoveNext(ref val2, ref networkConfiguratorActiveLinkOverlayComponent))
		{
			if (_entityManager.Deleted(val2) || !_entityManager.TryGetComponent<DeviceListComponent>(val2, ref component))
			{
				_entityManager.RemoveComponentDeferred<NetworkConfiguratorActiveLinkOverlayComponent>(val2);
				continue;
			}
			if (!Colors.TryGetValue(val2, out var value))
			{
				((Color)(ref value))._002Ector((float)_random.Next(0, 255), (float)_random.Next(0, 255), (float)_random.Next(0, 255), 1f);
				Colors.Add(val2, value);
			}
			TransformComponent component2 = _entityManager.GetComponent<TransformComponent>(val2);
			if (component2.MapID == MapId.Nullspace)
			{
				continue;
			}
			foreach (EntityUid allDevice in _deviceListSystem.GetAllDevices(val2, component))
			{
				if (!_entityManager.Deleted(allDevice))
				{
					TransformComponent component3 = _entityManager.GetComponent<TransformComponent>(allDevice);
					if (!(component3.MapID == MapId.Nullspace))
					{
						((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawLine(_transformSystem.GetWorldPosition(component2), _transformSystem.GetWorldPosition(component3), Colors[val2]);
					}
				}
			}
		}
	}
}
