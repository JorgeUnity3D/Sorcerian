using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Kapibara.ConnectSlots
{
    public class SlotsLifetimeScope : LifetimeScope
    {
        [SerializeField] private BoardConfig _boardConfig;
        [SerializeField] private BoardView _boardView;
        
        protected override void Configure(IContainerBuilder builder)
        {
            /// --- datos / recursos ---
            // 1) Board
            Board board = new Board(new Slot[_boardConfig.Rows, _boardConfig.Columns]);
            builder.RegisterInstance(board).As<Board>();
            
            // 2) Board Config
            builder.RegisterInstance(_boardConfig).As<BoardConfig>();
            
            // --- MonoBehaviour views (resueltos desde jerarquía) ---
            // BoardView debe estar en la escena; lo registramos para inyección.
            builder.RegisterComponentInHierarchy(_boardView.GetType());
            //builder.RegisterComponentInHierarchy<SlotView>().AsSelf();

            // --- Servicios / Sistemas: dejamos que VContainer los construya resolviendo constructores ---
            // Tus clases tienen constructores del estilo: new BoardGenerator(board, rows, columns, sprites)
            // VContainer hará eso si hemos registrado board, rows, columns, sprites arriba.
            builder.Register<BoardDestroyer>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardGenerator>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardGravity>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardInputHandler>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardMatcher>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardRefiller>(Lifetime.Singleton).AsSelf();
            builder.Register<BoardSwapper>(Lifetime.Singleton).AsSelf();

            // --- Opcional: registrar el controller como consumidor para orchestrar (si quieres inyección allí) ---
            builder.RegisterComponentInHierarchy<BoardController>();
        }
    }
}
            