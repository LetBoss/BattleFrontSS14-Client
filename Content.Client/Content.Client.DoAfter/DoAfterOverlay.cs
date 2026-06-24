using System;
using System.Numerics;
using Content.Client.UserInterface.Systems;
using Content.Shared._RMC14.Stealth;
using Content.Shared.DoAfter;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.DoAfter;

public sealed class DoAfterOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	private readonly IEntityManager _entManager;

	private readonly IGameTiming _timing;

	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly MetaDataSystem _meta;

	private readonly ProgressColorSystem _progressColor;

	private readonly SharedContainerSystem _container;

	private readonly SpriteSystem _sprite;

	private readonly Texture _barTexture;

	private readonly ShaderInstance _unshadedShader;

	private const float FlashTime = 0.125f;

	private const float StartX = 2f;

	private const float EndX = 22f;

	public override OverlaySpace Space => (OverlaySpace)8;

	public DoAfterOverlay(IEntityManager entManager, IPrototypeManager protoManager, IGameTiming timing, IPlayerManager player)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		_entManager = entManager;
		_timing = timing;
		_player = player;
		_transform = _entManager.EntitySysManager.GetEntitySystem<SharedTransformSystem>();
		_meta = _entManager.EntitySysManager.GetEntitySystem<MetaDataSystem>();
		_container = _entManager.EntitySysManager.GetEntitySystem<SharedContainerSystem>();
		_progressColor = _entManager.System<ProgressColorSystem>();
		_sprite = _entManager.System<SpriteSystem>();
		Rsi val = new Rsi(new ResPath("/Textures/Interface/Misc/progress_bar.rsi"), "icon");
		_barTexture = _entManager.EntitySysManager.GetEntitySystem<SpriteSystem>().Frame0((SpriteSpecifier)(object)val);
		_unshadedShader = protoManager.Index<ShaderPrototype>(UnshadedShader).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		Angle val = ((eye != null) ? eye.Rotation : Angle.Zero);
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		Vector2 vector = new Vector2(1f, 1f);
		Matrix3x2 value = Matrix3Helpers.CreateScale(ref vector);
		Matrix3x2 value2 = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-val));
		TimeSpan curTime = _timing.CurTime;
		Box2 val2 = ((Box2)(ref args.WorldAABB)).Enlarged(5f);
		ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
		EntityUid? val3 = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		EntityQuery<MetaDataComponent> entityQuery2 = _entManager.GetEntityQuery<MetaDataComponent>();
		AllEntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent, SpriteComponent, TransformComponent> val4 = _entManager.AllEntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent, SpriteComponent, TransformComponent>();
		EntityUid val5 = default(EntityUid);
		ActiveDoAfterComponent activeDoAfterComponent = default(ActiveDoAfterComponent);
		DoAfterComponent doAfterComponent = default(DoAfterComponent);
		SpriteComponent val6 = default(SpriteComponent);
		TransformComponent val7 = default(TransformComponent);
		EntityActiveInvisibleComponent entityActiveInvisibleComponent = default(EntityActiveInvisibleComponent);
		Box2 val10 = default(Box2);
		while (val4.MoveNext(ref val5, ref activeDoAfterComponent, ref doAfterComponent, ref val6, ref val7))
		{
			if (val7.MapID != args.MapId || doAfterComponent.DoAfters.Count == 0)
			{
				continue;
			}
			Vector2 worldPosition = _transform.GetWorldPosition(val7, entityQuery);
			if (!((Box2)(ref val2)).Contains(worldPosition, true))
			{
				continue;
			}
			EntityUid val8 = val5;
			EntityUid? val9 = val3;
			if (!val9.HasValue || val8 != val9.GetValueOrDefault())
			{
				((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
			}
			else
			{
				((DrawingHandleBase)worldHandle).UseShader(_unshadedShader);
			}
			MetaDataComponent component = entityQuery2.GetComponent(val5);
			TimeSpan timeSpan = (component.EntityPaused ? (curTime - _meta.GetPauseTime(val5, component)) : curTime);
			Matrix3x2 value3 = Matrix3Helpers.CreateTranslation(worldPosition);
			Matrix3x2 value4 = Matrix3x2.Multiply(value, value3);
			Matrix3x2 matrix3x = Matrix3x2.Multiply(value2, value4);
			((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
			float num = 0f;
			bool flag = _container.IsEntityOrParentInContainer(val5, component, val7);
			foreach (Content.Shared.DoAfter.DoAfter value5 in doAfterComponent.DoAfters.Values)
			{
				float num2 = 1f;
				if (value5.Args.Hidden || flag)
				{
					val8 = val5;
					val9 = val3;
					if (!val9.HasValue || val8 != val9.GetValueOrDefault())
					{
						continue;
					}
					num2 = 0.5f;
				}
				if (!value5.Args.ForceVisible)
				{
					if (!val6.Visible)
					{
						val8 = val5;
						val9 = val3;
						if (!val9.HasValue || val8 != val9.GetValueOrDefault())
						{
							continue;
						}
					}
					num2 = val6.Color.A;
					_entManager.GetEntityQuery<EntityActiveInvisibleComponent>().TryGetComponent(val5, ref entityActiveInvisibleComponent);
					if (entityActiveInvisibleComponent != null)
					{
						num2 = entityActiveInvisibleComponent.Opacity;
					}
				}
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((val5, val6)));
				float num3 = ((Box2)(ref localBounds)).Height / 2f + 0.05f;
				Vector2 vector2 = new Vector2((float)(-_barTexture.Width) / 2f / 32f, num3 / 1f + num / 32f * 1f);
				Texture barTexture = _barTexture;
				Color white = Color.White;
				((DrawingHandleBase)worldHandle).DrawTexture(barTexture, vector2, (Color?)((Color)(ref white)).WithAlpha(num2));
				float num4;
				Color progressColor;
				if (value5.CancelledTime.HasValue)
				{
					num4 = (float)Math.Min(1.0, (value5.CancelledTime.Value - value5.StartTime).TotalSeconds / value5.Args.Delay.TotalSeconds);
					bool flag2 = Math.Floor((timeSpan - value5.CancelledTime.Value).TotalSeconds / 0.125) % 2.0 == 0.0;
					progressColor = GetProgressColor(0f, flag2 ? num2 : 0f);
				}
				else
				{
					num4 = (float)Math.Min(1.0, (timeSpan - value5.StartTime).TotalSeconds / value5.Args.Delay.TotalSeconds);
					progressColor = GetProgressColor(num4, num2);
				}
				float x = 20f * num4 + 2f;
				((Box2)(ref val10))._002Ector(new Vector2(2f, 3f) / 32f, new Vector2(x, 4f) / 32f);
				val10 = ((Box2)(ref val10)).Translated(vector2);
				worldHandle.DrawRect(val10, progressColor, true);
				num += (float)_barTexture.Height / 1f;
			}
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	public Color GetProgressColor(float progress, float alpha = 1f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Color progressColor = _progressColor.GetProgressColor(progress);
		return ((Color)(ref progressColor)).WithAlpha(alpha);
	}
}
