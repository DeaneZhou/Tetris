using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tetris.Models
{
    public partial class ImageItem : ObservableObject
    {
        [ObservableProperty]
        private ImageSource _imageSource;

        [ObservableProperty]
        private double _opacity;
        public double Left { get; set; }
        public double Top { get; set; }
    }
}
