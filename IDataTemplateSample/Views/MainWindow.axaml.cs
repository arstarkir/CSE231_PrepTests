using Avalonia.Controls;
using Avalonia.Controls.Utils;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CSE231_PrepTests;
using CSE231_prepTests_A.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace IDataTemplateSample.Views
{
    public partial class MainWindow : Window 
    {
        //Question + Test info
        List<TestInfo> tests = new List<TestInfo>();
        TestInfo curTest = null;
        Question curQ = null;
        ObservableCollection<TestInfo> TestInfoGrid { get; set; }
        public MainWindow()       
        {
            InitializeComponent();
            findSaves();
            TestInfoGrid = new ObservableCollection<TestInfo>(tests);
        }
        void findSaves()
        {

        }
        private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = GetTopLevel(this);
            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Text File",
                AllowMultiple = true
            });

            foreach (var file in files)
            {
                TestInfo tempTest = new TestInfo();

                string fileContent;
                using (StreamReader reader = new StreamReader(file.Path.AbsolutePath.ToString()))
                {
                    fileContent = reader.ReadToEnd();
                }
                try
                {
                    tempTest.DissectTest(file.Path.AbsolutePath.ToString(), fileContent);
                    tests.Add(tempTest);
                    updateTable(tempTest);
                }
                catch
                {
                    
                }
            }
        }
        void updateTable(TestInfo tempTest)
        {
            TestInfoGrid.Add(tempTest);
        }

    }
}
