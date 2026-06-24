using System;
using System.Numerics;
using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Camera;
using Content.Shared.Ghost;
using Content.Shared.Input;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Systems;

public abstract class SharedContentEyeSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class RequestTargetZoomEvent : EntityEventArgs
	{
		public Vector2 TargetZoom;

		public bool IgnoreLimit;
	}

	[Serializable]
	[NetSerializable]
	public sealed class RequestPvsScaleEvent(float scale) : EntityEventArgs
	{
		public float Scale = scale;
	}

	[Serializable]
	[NetSerializable]
	public sealed class RequestEyeEvent : EntityEventArgs
	{
		public readonly bool DrawFov;

		public readonly bool DrawLight;

		public RequestEyeEvent(bool drawFov, bool drawLight)
		{
			DrawFov = drawFov;
			DrawLight = drawLight;
		}
	}

	[Dependency]
	private ISharedAdminManager _admin;

	public const AdminFlags EyeFlag = AdminFlags.Debug;

	public const float ZoomMod = 1.5f;

	public static readonly Vector2 DefaultZoom = Vector2.One;

	public static readonly Vector2 MinZoom = DefaultZoom * (float)Math.Pow(1.5, -3.0);

	[Dependency]
	private SharedEyeSystem _eye;

	public override void Initialize()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ContentEyeComponent, ComponentStartup>((ComponentEventHandler<ContentEyeComponent, ComponentStartup>)OnContentEyeStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestTargetZoomEvent>((EntitySessionEventHandler<RequestTargetZoomEvent>)OnContentZoomRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestPvsScaleEvent>((EntitySessionEventHandler<RequestPvsScaleEvent>)OnPvsScale, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestEyeEvent>((EntitySessionEventHandler<RequestEyeEvent>)OnRequestEye, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.ZoomIn, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(ZoomIn), (StateInputCmdDelegate)null, false, true)).Bind(ContentKeyFunctions.ZoomOut, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(ZoomOut), (StateInputCmdDelegate)null, false, true)).Bind(ContentKeyFunctions.ResetZoom, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(ResetZoom), (StateInputCmdDelegate)null, false, true))
			.Register<SharedContentEyeSystem>();
		((EntitySystem)this).Log.Level = (LogLevel)2;
		((EntitySystem)this).UpdatesOutsidePrediction = true;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<SharedContentEyeSystem>();
	}

	private void ResetZoom(ICommonSession? session)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ContentEyeComponent eye = default(ContentEyeComponent);
		if (((EntitySystem)this).TryComp<ContentEyeComponent>((session != null) ? session.AttachedEntity : ((EntityUid?)null), ref eye))
		{
			ResetZoom(session.AttachedEntity.Value, eye);
		}
	}

	private void ZoomOut(ICommonSession? session)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ContentEyeComponent eye = default(ContentEyeComponent);
		if (((EntitySystem)this).TryComp<ContentEyeComponent>((session != null) ? session.AttachedEntity : ((EntityUid?)null), ref eye))
		{
			SetZoom(session.AttachedEntity.Value, eye.TargetZoom * 1.5f, ignoreLimits: false, eye);
		}
	}

	private void ZoomIn(ICommonSession? session)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ContentEyeComponent eye = default(ContentEyeComponent);
		if (((EntitySystem)this).TryComp<ContentEyeComponent>((session != null) ? session.AttachedEntity : ((EntityUid?)null), ref eye))
		{
			SetZoom(session.AttachedEntity.Value, eye.TargetZoom / 1.5f, ignoreLimits: false, eye);
		}
	}

	private Vector2 Clamp(Vector2 zoom, ContentEyeComponent component)
	{
		return Vector2.Clamp(zoom, MinZoom, component.MaxZoom);
	}

	public void SetZoom(EntityUid uid, Vector2 zoom, bool ignoreLimits = false, ContentEyeComponent? eye = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ContentEyeComponent>(uid, ref eye, false))
		{
			eye.TargetZoom = (ignoreLimits ? zoom : Clamp(zoom, eye));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)eye, (MetaDataComponent)null);
		}
	}

	private void OnContentZoomRequest(RequestTargetZoomEvent msg, EntitySessionEventArgs args)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		bool ignoreLimit = msg.IgnoreLimit && _admin.HasAdminFlag(((EntitySessionEventArgs)(ref args)).SenderSession, AdminFlags.Debug);
		ContentEyeComponent content = default(ContentEyeComponent);
		if (((EntitySystem)this).TryComp<ContentEyeComponent>(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity, ref content))
		{
			SetZoom(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, msg.TargetZoom, ignoreLimit, content);
		}
	}

	private void OnPvsScale(RequestPvsScaleEvent ev, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid uid = attachedEntity.GetValueOrDefault();
			if (_admin.HasAdminFlag(((EntitySessionEventArgs)(ref args)).SenderSession, AdminFlags.Debug))
			{
				_eye.SetPvsScale(Entity<EyeComponent>.op_Implicit(uid), ev.Scale);
			}
		}
	}

	private void OnRequestEye(RequestEyeEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid player = attachedEntity.GetValueOrDefault();
			EyeComponent eyeComp = default(EyeComponent);
			if ((((EntitySystem)this).HasComp<GhostComponent>(player) || _admin.IsAdmin(player)) && ((EntitySystem)this).TryComp<EyeComponent>(player, ref eyeComp))
			{
				_eye.SetDrawFov(player, msg.DrawFov, eyeComp);
				_eye.SetDrawLight(Entity<EyeComponent>.op_Implicit((player, eyeComp)), msg.DrawLight);
			}
		}
	}

	private void OnContentEyeStartup(EntityUid uid, ContentEyeComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent eyeComp = default(EyeComponent);
		if (((EntitySystem)this).TryComp<EyeComponent>(uid, ref eyeComp))
		{
			_eye.SetZoom(uid, component.TargetZoom, eyeComp);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void ResetZoom(EntityUid uid, ContentEyeComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_eye.SetPvsScale(Entity<EyeComponent>.op_Implicit(uid), 1f);
		SetZoom(uid, DefaultZoom, ignoreLimits: false, component);
	}

	public void SetMaxZoom(EntityUid uid, Vector2 value, ContentEyeComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ContentEyeComponent>(uid, ref component, true))
		{
			component.MaxZoom = value;
			component.TargetZoom = Clamp(component.TargetZoom, component);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void UpdateEyeOffset(Entity<EyeComponent> eye)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		GetEyeOffsetAttemptEvent evAttempt = default(GetEyeOffsetAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<GetEyeOffsetAttemptEvent>(Entity<EyeComponent>.op_Implicit(eye), ref evAttempt, false);
		if (evAttempt.Cancelled)
		{
			_eye.SetOffset(Entity<EyeComponent>.op_Implicit(eye), Vector2.Zero, Entity<EyeComponent>.op_Implicit(eye));
			return;
		}
		GetEyeOffsetEvent ev = default(GetEyeOffsetEvent);
		((EntitySystem)this).RaiseLocalEvent<GetEyeOffsetEvent>(Entity<EyeComponent>.op_Implicit(eye), ref ev, false);
		GetEyeOffsetRelayedEvent evRelayed = new GetEyeOffsetRelayedEvent();
		((EntitySystem)this).RaiseLocalEvent<GetEyeOffsetRelayedEvent>(Entity<EyeComponent>.op_Implicit(eye), ref evRelayed, false);
		_eye.SetOffset(Entity<EyeComponent>.op_Implicit(eye), ev.Offset + evRelayed.Offset, Entity<EyeComponent>.op_Implicit(eye));
	}

	public void UpdatePvsScale(EntityUid uid, ContentEyeComponent? contentEye = null, EyeComponent? eye = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ContentEyeComponent>(uid, ref contentEye, true) && ((EntitySystem)this).Resolve<EyeComponent>(uid, ref eye, true))
		{
			GetEyePvsScaleAttemptEvent evAttempt = default(GetEyePvsScaleAttemptEvent);
			((EntitySystem)this).RaiseLocalEvent<GetEyePvsScaleAttemptEvent>(uid, ref evAttempt, false);
			if (evAttempt.Cancelled)
			{
				_eye.SetPvsScale(Entity<EyeComponent>.op_Implicit((uid, eye)), 1f);
				return;
			}
			GetEyePvsScaleEvent ev = default(GetEyePvsScaleEvent);
			((EntitySystem)this).RaiseLocalEvent<GetEyePvsScaleEvent>(uid, ref ev, false);
			GetEyePvsScaleRelayedEvent evRelayed = new GetEyePvsScaleRelayedEvent();
			((EntitySystem)this).RaiseLocalEvent<GetEyePvsScaleRelayedEvent>(uid, ref evRelayed, false);
			_eye.SetPvsScale(Entity<EyeComponent>.op_Implicit((uid, eye)), 1f + ev.Scale + evRelayed.Scale);
		}
	}
}
