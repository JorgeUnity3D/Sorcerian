using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Kapibara.ConnectSlots
{
    public class SlotsLifetimeScope : LifetimeScope
    {
        [SerializeField] private BoardConfigData _boardConfigData;
        [SerializeField] private SlotSpritesData _slotSpritesData;
        
        protected override void Configure(IContainerBuilder builder)
        {
            //Data
            builder.RegisterInstance(_boardConfigData.Data).As<BoardConfig>();
            builder.RegisterInstance(_slotSpritesData.Data).As<SlotSprites>();
            Board board = new Board(new Slot[_boardConfigData.Data.Rows, _boardConfigData.Data.Columns]);
            builder.RegisterInstance(board).As<Board>();
            
            //Scene Monobehaviours
            builder.RegisterComponentInHierarchy<BoardView>();
            //builder.RegisterComponentInHierarchy<SlotView>().AsSelf();

            //Services
            builder.Register<BoardController>(Lifetime.Singleton).AsSelf().As<IStartable>();
            builder.Register<BoardDestroyer>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardGenerator>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardGravity>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardInputHandler>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardMatcher>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardRefiller>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardSwapper>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardDebugger>(Lifetime.Singleton).AsSelf();

            //Controller
        }
    }
}
            