using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.CrashLand;
using Content.Shared.ParaDrop;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.StatusIcon;

public sealed class StatusIconOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly StatusIconSystem _statusIcon;

	private readonly ShaderInstance _unshadedShader;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	public override OverlaySpace Space => (OverlaySpace)8;

	internal StatusIconOverlay()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<StatusIconOverlay>(this);
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_statusIcon = _entity.System<StatusIconSystem>();
		_unshadedShader = _prototype.Index<ShaderPrototype>(UnshadedShader).Instance();
		_xformQuery = _entity.GetEntityQuery<TransformComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		Matrix3x2 value = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)((eye != null) ? eye.Rotation : default(Angle))));
		AllEntityQueryEnumerator<StatusIconComponent, SpriteComponent, TransformComponent, MetaDataComponent> val = _entity.AllEntityQueryEnumerator<StatusIconComponent, SpriteComponent, TransformComponent, MetaDataComponent>();
		EntityUid val2 = default(EntityUid);
		StatusIconComponent statusIconComponent = default(StatusIconComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		MetaDataComponent val5 = default(MetaDataComponent);
		while (val.MoveNext(ref val2, ref statusIconComponent, ref val3, ref val4, ref val5))
		{
			if (val4.MapID != args.MapId || !val3.Visible)
			{
				continue;
			}
			Box2 val6 = (Box2)(((_003F?)statusIconComponent.Bounds) ?? _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3))));
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val4, _xformQuery);
			Box2 val7 = ((Box2)(ref val6)).Translated(worldPosition);
			if (!((Box2)(ref val7)).Intersects(ref args.WorldAABB))
			{
				continue;
			}
			List<StatusIconData> statusIcons = _statusIcon.GetStatusIcons(val2, val5);
			if (statusIcons.Count == 0)
			{
				continue;
			}
			Matrix3x2 value2 = Matrix3Helpers.CreateTranslation(worldPosition);
			Matrix3x2 matrix3x = Matrix3x2.Multiply(value, value2);
			((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			statusIcons.Sort();
			foreach (StatusIconData item in statusIcons)
			{
				if (!_statusIcon.IsVisible(Entity<MetaDataComponent>.op_Implicit((val2, val5)), item))
				{
					continue;
				}
				TimeSpan realTime = _timing.RealTime;
				Texture frame = _sprite.GetFrame(item.Icon, realTime, true);
				float y;
				float x;
				if (item.LocationPreference == StatusIconLocationPreference.Left || (item.LocationPreference == StatusIconLocationPreference.None && num <= num2))
				{
					float num5 = num3 + frame.Height;
					val7 = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
					if (num5 > ((Box2)(ref val7)).Height * 32f)
					{
						break;
					}
					if (item.Layer == StatusIconLayer.Base)
					{
						num3 += frame.Height;
						num++;
					}
					y = (((Box2)(ref val6)).Height + val3.Offset.Y) / 2f - (float)(num3 - item.Offset) / 32f;
					x = (0f - (((Box2)(ref val6)).Width + val3.Offset.X)) / 2f;
					if (_entity.HasComponent<CrashLandingComponent>(val2) || _entity.HasComponent<ParaDroppingComponent>(val2))
					{
						y = 0.25f + val3.Offset.Y;
					}
				}
				else
				{
					float num6 = num4 + frame.Height;
					val7 = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val2, val3)));
					if (num6 > ((Box2)(ref val7)).Height * 32f)
					{
						break;
					}
					if (item.Layer == StatusIconLayer.Base)
					{
						num4 += frame.Height;
						num2++;
					}
					y = (((Box2)(ref val6)).Height + val3.Offset.Y) / 2f - (float)(num4 - item.Offset) / 32f;
					x = (((Box2)(ref val6)).Width + val3.Offset.X) / 2f - (float)frame.Width / 32f;
					if (_entity.HasComponent<CrashLandingComponent>(val2) || _entity.HasComponent<ParaDroppingComponent>(val2))
					{
						y = 0.25f + val3.Offset.Y;
					}
				}
				if (item.IsShaded)
				{
					((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
				}
				else
				{
					((DrawingHandleBase)worldHandle).UseShader(_unshadedShader);
				}
				Vector2 vector = new Vector2(x, y);
				((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
			}
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
			Matrix3x2 identity = Matrix3x2.Identity;
			((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		}
	}
}
