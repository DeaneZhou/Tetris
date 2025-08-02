using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tetris.Helper;
using Tetris.Models;

namespace Tetris.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
        };
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;
        public ObservableCollection<ImageItem> ImageControls { get; } =
            new ObservableCollection<ImageItem>();

        private GameState gameState = new GameState();

        [ObservableProperty]
        public string scoreText;

        [ObservableProperty]
        private string _finalScoreText;

        [ObservableProperty]
        private bool _isGameOver;

        [ObservableProperty]
        private ImageSource nextImage;

        [ObservableProperty]
        private ImageSource holdImage;

        public MainViewModel()
        {
            SetupGameCanvas(gameState.GameGrid);
        }

        private void SetupGameCanvas(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    ImageItem imageItem = new ImageItem
                    {
                        Top = (r - 2) * 25 + 10, // Adjusted for the offset
                        Left = c * 25,
                        ImageSource = tileImages[0] // Set to empty tile
                    };

                    ImageControls.Add(imageItem);
                }
            }
        }

        [RelayCommand]
        private async void CanvasLoaded()
        {
            await GameLoop();
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }
            IsGameOver = true;
            FinalScoreText = $"Score: {gameState.Score}";
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    int sid = r * 10 + c;
                    ImageControls[sid].Opacity = 1.0; // Reset opacity
                    ImageControls[sid].ImageSource = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                int sid = p.Row * 10 + p.Column;
                ImageControls[sid].Opacity = 1.0; // Reset opacity
                ImageControls[sid].ImageSource = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText = $"Score: {gameState.Score}";
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage = blockImages[0];
            }
            else
            {
                HoldImage = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();
            foreach (Position p in block.TilePositions())
            {
                int sid = (p.Row + dropDistance) * 10 + p.Column;
                ImageControls[sid].Opacity = 0.25;
                ImageControls[sid].ImageSource = tileImages[block.Id];
            }
        }

        [RelayCommand]
        private void KeyDown(KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        [RelayCommand]
        private async void RestartGame()
        {
            gameState = new GameState();
            IsGameOver = false;
            FinalScoreText = $"Score: {gameState.Score}";
            SetupGameCanvas(gameState.GameGrid);
            await GameLoop();
        }
    }
}
