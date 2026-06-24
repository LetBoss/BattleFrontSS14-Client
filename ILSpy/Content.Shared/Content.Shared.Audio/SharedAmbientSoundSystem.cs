using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Shared.Audio;

public abstract class SharedAmbientSoundSystem : EntitySystem
{
	private EntityQuery<AmbientSoundComponent> _query;

	public override void Initialize()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AmbientSoundComponent, ComponentGetState>((ComponentEventRefHandler<AmbientSoundComponent, ComponentGetState>)GetCompState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AmbientSoundComponent, ComponentHandleState>((ComponentEventRefHandler<AmbientSoundComponent, ComponentHandleState>)HandleCompState, (Type[])null, (Type[])null);
		_query = ((EntitySystem)this).GetEntityQuery<AmbientSoundComponent>();
	}

	public virtual void SetAmbience(EntityUid uid, bool value, AmbientSoundComponent? ambience = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref ambience, false) && ambience.Enabled != value)
		{
			ambience.Enabled = value;
			QueueUpdate(uid, ambience);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)ambience, (MetaDataComponent)null);
		}
	}

	public virtual void SetRange(EntityUid uid, float value, AmbientSoundComponent? ambience = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref ambience, false) && !MathHelper.CloseToPercent(ambience.Range, value, 1E-05))
		{
			ambience.Range = value;
			QueueUpdate(uid, ambience);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)ambience, (MetaDataComponent)null);
		}
	}

	protected virtual void QueueUpdate(EntityUid uid, AmbientSoundComponent ambience)
	{
	}

	public virtual void SetVolume(EntityUid uid, float value, AmbientSoundComponent? ambience = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref ambience, false) && !MathHelper.CloseToPercent(ambience.Volume, value, 1E-05))
		{
			ambience.Volume = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)ambience, (MetaDataComponent)null);
		}
	}

	public virtual void SetSound(EntityUid uid, SoundSpecifier sound, AmbientSoundComponent? ambience = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_query.Resolve(uid, ref ambience, false) && ambience.Sound != sound)
		{
			ambience.Sound = sound;
			QueueUpdate(uid, ambience);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)ambience, (MetaDataComponent)null);
		}
	}

	private void HandleCompState(EntityUid uid, AmbientSoundComponent component, ref ComponentHandleState args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is AmbientSoundComponentState state)
		{
			SetAmbience(uid, state.Enabled, component);
			SetRange(uid, state.Range, component);
			SetVolume(uid, state.Volume, component);
			SetSound(uid, state.Sound, component);
		}
	}

	private void GetCompState(EntityUid uid, AmbientSoundComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new AmbientSoundComponentState
		{
			Enabled = component.Enabled,
			Range = component.Range,
			Volume = component.Volume,
			Sound = component.Sound
		};
	}
}
