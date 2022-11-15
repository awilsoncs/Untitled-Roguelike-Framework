using System;
using JetBrains.Annotations;
using URF.Common.Logging;
using URF.Common.Persistence;
using URF.Server.FieldOfView;
using URF.Server.GameState;
using URF.Server.Pathfinding;
using URF.Server.RandomGeneration;

namespace URF.Server {
  public class PluginBundle {

    [NotNull]
    public IRandomGenerator Random { get; }

    [NotNull]
    public IFieldOfView FieldOfView { get; }

    [NotNull]
    public ILogging Logging { get; }

    [NotNull]
    public IPathfinding Pathfinding { get; }
    
    [NotNull]
    public IEntityFactory EntityFactory { get; }

    [NotNull]
    public PersistentStorage PersistentStorage { get; }
    
    public PluginBundle(
      [NotNull] IRandomGenerator random,
      [NotNull] IFieldOfView fieldOfView,
      [NotNull] ILogging logging,
      [NotNull] IPathfinding pathfinding,
      [NotNull] IEntityFactory entityFactory,
      [NotNull] PersistentStorage persistentStorage
    ) {
      Random = random ?? throw new ArgumentNullException(nameof(random));
      FieldOfView = fieldOfView ?? throw new ArgumentNullException(nameof(fieldOfView));
      Logging = logging ?? throw new ArgumentNullException(nameof(logging));
      Pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));
      EntityFactory = entityFactory;
      PersistentStorage = persistentStorage;
    }

  }
}