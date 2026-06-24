using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Content.Client.Resources;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.StationRecords;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Access;

public sealed class AccessOverlay : Overlay
{
	private const string TextFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";

	private const int TextFontSize = 12;

	private readonly IEntityManager _entityManager;

	private readonly SharedTransformSystem _transformSystem;

	private readonly Font _font;

	public override OverlaySpace Space => (OverlaySpace)2;

	public AccessOverlay(IEntityManager entityManager, IResourceCache resourceCache, SharedTransformSystem transformSystem)
	{
		_entityManager = entityManager;
		_transformSystem = transformSystem;
		_font = resourceCache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		EntityQueryEnumerator<AccessReaderComponent, TransformComponent> val = _entityManager.EntityQueryEnumerator<AccessReaderComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		AccessReaderComponent accessReaderComponent = default(AccessReaderComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref accessReaderComponent, ref val3))
		{
			stringBuilder.Clear();
			EntityStringRepresentation val4 = _entityManager.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(val2));
			stringBuilder.AppendLine(((EntityStringRepresentation)(ref val4)).Prototype);
			stringBuilder.Append("UID: ");
			stringBuilder.Append(((EntityStringRepresentation)(ref val4)).Uid.Id);
			stringBuilder.Append(", NUID: ");
			stringBuilder.Append(((EntityStringRepresentation)(ref val4)).Nuid.Id);
			stringBuilder.AppendLine();
			if (!accessReaderComponent.Enabled)
			{
				stringBuilder.AppendLine("-Disabled");
				continue;
			}
			if (accessReaderComponent.AccessLists.Count > 0)
			{
				int num = 0;
				foreach (HashSet<ProtoId<AccessLevelPrototype>> accessList in accessReaderComponent.AccessLists)
				{
					num++;
					foreach (ProtoId<AccessLevelPrototype> item in accessList)
					{
						stringBuilder.Append("+Set ");
						stringBuilder.Append(num);
						stringBuilder.Append(": ");
						stringBuilder.Append(item.Id);
						stringBuilder.AppendLine();
					}
				}
			}
			else
			{
				stringBuilder.AppendLine("+Unrestricted");
			}
			foreach (StationRecordKey accessKey in accessReaderComponent.AccessKeys)
			{
				stringBuilder.Append("+Key ");
				stringBuilder.Append(accessKey.OriginStation);
				stringBuilder.Append(": ");
				stringBuilder.Append(accessKey.Id);
				stringBuilder.AppendLine();
			}
			foreach (ProtoId<AccessLevelPrototype> denyTag in accessReaderComponent.DenyTags)
			{
				stringBuilder.Append("-Tag ");
				stringBuilder.AppendLine(denyTag.Id);
			}
			string text = stringBuilder.ToString();
			Vector2 vector = args.ViewportControl.WorldToScreen(_transformSystem.GetWorldPosition(val3));
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector, text, Color.Gold);
		}
	}
}
