﻿using ClassLibrary;
using Client;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
namespace Client
{
    /// <summary>
    /// Логика взаимодействия для VideoPage.xaml
    /// </summary>
    public partial class VideoPage : Page
    {
        public VideoPage(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            Canvas canvas = FindName("canvas2") as Canvas;
            canvas.Children.Add(rectangleContainer);

            ListBoxForResponce.ItemsSource = OpenVideos;
            localDrawer = new Drawer(rectangleContainer, VideoImage, _window);
        }
        
        MainWindow _window;

        private readonly Canvas rectangleContainer = new();
        public readonly Drawer localDrawer;

        public ObservableCollection<string> OpenVideos { get; } = new ObservableCollection<string>();
        private List<VideoController> _videoControllers = [];
        VideoController currentVideoController;

        private string filepath;

        MainWindow _window;

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController?.Play();
        }

        private void MediaPauseButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController?.Pause();
        }

        private void MediaStopButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController?.Stop();
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController?.Rewind();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController?.NextFrame();
        }
        private void ShowInfoButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController?.ShowInfo();
        }
        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            currentVideoController?.GetSliderValue(e.NewValue);
        }
        private void UploadMediaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                filepath = FileHandler.OpenFile("Media");
                if (filepath != null)
                {
                    currentVideoController = new(filepath, VideoImage, MediaSlider, _window);
                    MediaSlider.Value = 0;
                    _videoControllers.Add(currentVideoController);
                    OpenVideos.Add($"{_videoControllers.Count}. {currentVideoController.shortName}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Media file opening error.");
                MessageBox.Show($"Media file opening error: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            currentVideoController.GetProcessedVideo();

        }

        private async void HealthCheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (await MainWindow.apiClient.CheckHealthAsync($"{ConnectionWindow.ConnectionUri}health"))
            {
                MessageBox.Show("Yes");
            }
        }
        private void VideoBox_SourceUpdated(object sender, RoutedEventArgs e)
        {
            localDrawer.CalculateScale();
            localDrawer.ClearRectangles();
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Image uploaded.");
        }
        private void ListBoxForResponce_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxForResponce.SelectedIndex != -1)
            {
                currentVideoController = _videoControllers[ListBoxForResponce.SelectedIndex];
                currentVideoController.SetFirstFrame();
            }
            else
            {
                currentVideoController = null;
                VideoImage.Source = null;
                _window.activyVideoPage.localDrawer.ClearRectangles();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if(ListBoxForResponce.Items.Count == 1)
            {
                ListBoxForResponce.SelectedIndex = -1;
                _videoControllers.Clear();
                OpenVideos.Clear();
                return;
            }
            ListBoxForResponce.SelectedIndex--;
            _videoControllers.RemoveAt(ListBoxForResponce.SelectedIndex+1);
            OpenVideos.RemoveAt(ListBoxForResponce.SelectedIndex+1);
        }
    }
}
