// Decompiled with JetBrains decompiler
// Type: Content.Client.Sprite.ContentSpriteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Sprite;

public sealed class ContentSpriteSystem : EntitySystem
{
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
  private ContentSpriteSystem.ContentSpriteControl _control = new ContentSpriteSystem.ContentSpriteControl();
  public static readonly ResPath Exports = new ResPath("/Exports");

  public virtual void Initialize()
  {
    base.Initialize();
    this._resManager.UserData.CreateDir(ContentSpriteSystem.Exports);
    ((Control) this._ui.RootControl).AddChild((Control) this._control);
    this.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.GetVerbs), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    foreach ((IRenderTexture Texture, Direction Direction, EntityUid Entity, bool IncludeId, TaskCompletionSource Tcs) queuedTexture in this._control.QueuedTextures)
      queuedTexture.Tcs.SetCanceled();
    this._control.QueuedTextures.Clear();
    ((Control) this._ui.RootControl).RemoveChild((Control) this._control);
  }

  public async Task Export(EntityUid entity, bool includeId = true, CancellationToken cancelToken = default (CancellationToken))
  {
    // ISSUE: unable to decompile the method.
  }

  public async Task Export(
    EntityUid entity,
    Direction direction,
    bool includeId = true,
    CancellationToken cancelToken = default (CancellationToken))
  {
    ContentSpriteSystem contentSpriteSystem = this;
    SpriteComponent spriteComponent;
    if (!contentSpriteSystem._timing.IsFirstTimePredicted || !contentSpriteSystem.TryComp<SpriteComponent>(entity, ref spriteComponent))
      return;
    Vector2i vector2i = Vector2i.Zero;
    foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
    {
      if (allLayer.Visible)
        vector2i = Vector2i.ComponentMax(vector2i, allLayer.PixelSize);
    }
    if (((Vector2i) ref vector2i).Equals(Vector2i.Zero))
      return;
    IRenderTexture renderTarget = contentSpriteSystem._clyde.CreateRenderTarget(new Vector2i(vector2i.X, vector2i.Y), new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "export");
    TaskCompletionSource completionSource = new TaskCompletionSource((object) cancelToken);
    contentSpriteSystem._control.QueuedTextures.Enqueue((renderTarget, direction, entity, includeId, completionSource));
    await completionSource.Task;
  }

  private void GetVerbs(GetVerbsEvent<Verb> ev)
  {
    if (!this._adminManager.IsAdmin())
      return;
    EntityUid target = ev.Target;
    Verb verb = new Verb()
    {
      Text = this.Loc.GetString("export-entity-verb-get-data-text"),
      Category = VerbCategory.Debug,
      Act = (Action) (async () =>
      {
        try
        {
          await this.Export(target);
        }
        catch (Exception ex)
        {
          this._runtimeLog.LogException(ex, "ContentSpriteSystem.Export");
        }
      })
    };
    ev.Verbs.Add(verb);
  }

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
      IoCManager.InjectDependencies<ContentSpriteSystem.ContentSpriteControl>(this);
      this._sawmill = this._logMan.GetSawmill("sprite.export");
    }

    protected virtual void Draw(DrawingHandleScreen handle)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ContentSpriteSystem.ContentSpriteControl.\u003C\u003Ec__DisplayClass6_0 cDisplayClass60 = new ContentSpriteSystem.ContentSpriteControl.\u003C\u003Ec__DisplayClass6_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.handle = handle;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      base.Draw(cDisplayClass60.handle);
      (IRenderTexture Texture, Direction Direction, EntityUid Entity, bool IncludeId, TaskCompletionSource Tcs) result;
      while (this.QueuedTextures.TryDequeue(out result))
      {
        if (!result.Tcs.Task.IsCanceled)
        {
          try
          {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ContentSpriteSystem.ContentSpriteControl.\u003C\u003Ec__DisplayClass6_1 cDisplayClass61 = new ContentSpriteSystem.ContentSpriteControl.\u003C\u003Ec__DisplayClass6_1();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass61.CS\u0024\u003C\u003E8__locals1 = cDisplayClass60;
            MetaDataComponent metaDataComponent;
            if (this._entManager.TryGetComponent<MetaDataComponent>(result.Entity, ref metaDataComponent))
            {
              string entityName = metaDataComponent.EntityName;
              // ISSUE: reference to a compiler-generated field
              cDisplayClass61.result = result;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated method
              ((DrawingHandleBase) cDisplayClass61.CS\u0024\u003C\u003E8__locals1.handle).RenderInRenderTarget((IRenderTarget) result.Texture, new Action(cDisplayClass61.\u003CDraw\u003Eb__0), new Color?(Color.Transparent));
              if (result.IncludeId)
              {
                ResPath resPath = ResPath.op_Division(ContentSpriteSystem.Exports, $"{entityName}-{result.Direction}-{result.Entity}.png");
                // ISSUE: reference to a compiler-generated field
                cDisplayClass61.fullFileName = resPath;
              }
              else
              {
                ResPath resPath = ResPath.op_Division(ContentSpriteSystem.Exports, $"{entityName}-{result.Direction}.png");
                // ISSUE: reference to a compiler-generated field
                cDisplayClass61.fullFileName = resPath;
              }
              // ISSUE: method pointer
              ((IRenderTarget) result.Texture).CopyPixelsToMemory<Rgba32>(new CopyPixelsDelegate<Rgba32>((object) cDisplayClass61, __methodptr(\u003CDraw\u003Eb__1)), new UIBox2i?());
              // ISSUE: reference to a compiler-generated field
              this._sawmill.Info($"Saved screenshot to {cDisplayClass61.fullFileName}");
              result.Tcs.SetResult();
            }
          }
          catch (Exception ex)
          {
            ((IDisposable) result.Texture).Dispose();
            if (!string.IsNullOrEmpty(ex.StackTrace))
              this._sawmill.Fatal(ex.StackTrace);
            result.Tcs.SetException(ex);
          }
        }
      }
    }
  }
}
