using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.Administration.Managers;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.ContentPack;
using Robust.Shared.Exceptions;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Sprite;

public sealed class ContentSpriteSystem : EntitySystem
{
	private sealed class ContentSpriteControl : Control
	{
		[Dependency]
		private IEntityManager _entManager;

		[Dependency]
		private ILogManager _logMan;

		[Dependency]
		private IResourceManager _resManager;

		internal Queue<(IRenderTexture Texture, Direction Direction, EntityUid Entity, bool IncludeId, TaskCompletionSource Tcs)> QueuedTextures = new Queue<(IRenderTexture, Direction, EntityUid, bool, TaskCompletionSource)>();

		private ISawmill _sawmill;

		public ContentSpriteControl()
		{
			IoCManager.InjectDependencies<ContentSpriteControl>(this);
			_sawmill = _logMan.GetSawmill("sprite.export");
		}

		protected override void Draw(DrawingHandleScreen handle)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			((Control)this).Draw(handle);
			(IRenderTexture, Direction, EntityUid, bool, TaskCompletionSource) result;
			MetaDataComponent val = default(MetaDataComponent);
			while (QueuedTextures.TryDequeue(out result))
			{
				if (result.Item5.Task.IsCanceled)
				{
					continue;
				}
				try
				{
					if (!_entManager.TryGetComponent<MetaDataComponent>(result.Item3, ref val))
					{
						continue;
					}
					string entityName = val.EntityName;
					(IRenderTexture Texture, Direction Direction, EntityUid Entity, bool IncludeId, TaskCompletionSource Tcs) result2 = result;
					((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)result.Item1, (Action)delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_0036: Unknown result type (might be due to invalid IL or missing references)
						//IL_0048: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						//IL_005a: Unknown result type (might be due to invalid IL or missing references)
						DrawingHandleScreen obj = handle;
						EntityUid item = result2.Entity;
						Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)result2.Texture).Size / 2);
						Vector2 one = Vector2.One;
						Angle? val2 = Angle.Zero;
						Direction? val3 = result2.Direction;
						obj.DrawEntity(item, vector, one, val2, default(Angle), val3, (SpriteComponent)null, (TransformComponent)null, (SharedTransformSystem)null);
					}, (Color?)Color.Transparent);
					ResPath fullFileName;
					if (result.Item4)
					{
						fullFileName = Exports / $"{entityName}-{result.Item2}-{result.Item3}.png";
					}
					else
					{
						fullFileName = Exports / $"{entityName}-{result.Item2}.png";
					}
					((IRenderTarget)result.Item1).CopyPixelsToMemory<Rgba32>((CopyPixelsDelegate<Rgba32>)delegate(Image<Rgba32> image)
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
						//IL_004b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0083: Unknown result type (might be due to invalid IL or missing references)
						if (_resManager.UserData.Exists(fullFileName))
						{
							_sawmill.Info($"Found existing file {fullFileName} to replace.");
							_resManager.UserData.Delete(fullFileName);
						}
						using Stream stream = _resManager.UserData.Open(fullFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
						ImageExtensions.SaveAsPng((Image)(object)image, stream);
					}, (UIBox2i?)null);
					_sawmill.Info($"Saved screenshot to {fullFileName}");
					result.Item5.SetResult();
				}
				catch (Exception ex)
				{
					((IDisposable)result.Item1).Dispose();
					if (!string.IsNullOrEmpty(ex.StackTrace))
					{
						_sawmill.Fatal(ex.StackTrace);
					}
					result.Item5.SetException(ex);
				}
			}
		}
	}

	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IResourceManager _resManager;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private IRuntimeLog _runtimeLog;

	private ContentSpriteControl _control = new ContentSpriteControl();

	public static readonly ResPath Exports = new ResPath("/Exports");

	public override void Initialize()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_resManager.UserData.CreateDir(Exports);
		((Control)_ui.RootControl).AddChild((Control)(object)_control);
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<Verb>>((EntityEventHandler<GetVerbsEvent<Verb>>)GetVerbs, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		foreach (var queuedTexture in _control.QueuedTextures)
		{
			queuedTexture.Tcs.SetCanceled();
		}
		_control.QueuedTextures.Clear();
		((Control)_ui.RootControl).RemoveChild((Control)(object)_control);
	}

	public async Task Export(EntityUid entity, bool includeId = true, CancellationToken cancelToken = default(CancellationToken))
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Task[] array = new Task[4];
		int num = 0;
		Direction[] array2 = new Direction[4];
		RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Direction[] array3 = (Direction[])(object)array2;
		foreach (Direction direction in array3)
		{
			array[num++] = Export(entity, direction, includeId, cancelToken);
		}
		await Task.WhenAll(array);
	}

	public async Task Export(EntityUid entity, Direction direction, bool includeId = true, CancellationToken cancelToken = default(CancellationToken))
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!_timing.IsFirstTimePredicted || !((EntitySystem)this).TryComp<SpriteComponent>(entity, ref val))
		{
			return;
		}
		Vector2i val2 = Vector2i.Zero;
		foreach (ISpriteLayer allLayer in val.AllLayers)
		{
			if (allLayer.Visible)
			{
				val2 = Vector2i.ComponentMax(val2, allLayer.PixelSize);
			}
		}
		if (!((Vector2i)(ref val2)).Equals(Vector2i.Zero))
		{
			IRenderTexture item = _clyde.CreateRenderTarget(new Vector2i(val2.X, val2.Y), new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "export");
			TaskCompletionSource taskCompletionSource = new TaskCompletionSource(cancelToken);
			_control.QueuedTextures.Enqueue((item, direction, entity, includeId, taskCompletionSource));
			await taskCompletionSource.Task;
		}
	}

	private void GetVerbs(GetVerbsEvent<Verb> ev)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!_adminManager.IsAdmin())
		{
			return;
		}
		EntityUid target = ev.Target;
		Verb item = new Verb
		{
			Text = base.Loc.GetString("export-entity-verb-get-data-text"),
			Category = VerbCategory.Debug,
			Act = async delegate
			{
				try
				{
					await Export(target);
				}
				catch (Exception ex)
				{
					_runtimeLog.LogException(ex, "ContentSpriteSystem.Export");
				}
			}
		};
		ev.Verbs.Add(item);
	}
}
