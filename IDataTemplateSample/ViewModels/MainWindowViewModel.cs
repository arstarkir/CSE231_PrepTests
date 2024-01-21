using IDataTemplateSample.Models;
using ReactiveUI;
using System;

namespace IDataTemplateSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private ShapeType _SelectedShape;

        /// <summary>
        /// Gets or sets the selected ShapeType
        /// </summary>
        public ShapeType SelectedShape
        {
            get { return _SelectedShape; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShape, value); }
        }

        /// <summary>
        ///  Gets an array of all available ShapeTypes
        /// </summary>
        public ShapeType[] AvailableShapes { get; } = Enum.GetValues<ShapeType>();
    }
}
