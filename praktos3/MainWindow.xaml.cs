using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
 
namespace praktos3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> paths = new List<string>();
        private bool pause = true;
        private int index = 0;
        private bool repeating = false;
        private bool israndom = false;
        List<string> buff_paths = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void play_and_stop_Click(object sender, RoutedEventArgs e)
        {
            if (pause)
            {
                play_and_stop.Content = "⏸";
                media.Pause();
                pause = false;
            }
            else
            {
                play_and_stop.Content = "▶";
                media.Play();
                pause = true;
            }
        }
        private void Start()
        {
            if (paths.Count == 0)
            {
                MessageBox.Show("Не выбрана папка!");
            }
            else
            {
                if (index > paths.Count - 1)
                {
                    index = 0;
                }
                song_name.Content = paths[index].Split("\\").Last().Split(".mp3")[0];
                media.Source = new Uri(paths[index]);
                media.Play();
                media.Volume = 0.7;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button original = (Button)e.OriginalSource;
            if (original.Name == "choose")
            {
                paths.Clear();
                var dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                };
                var res = dialog.ShowDialog();
                if (res == CommonFileDialogResult.Ok)
                {
                    string[] files = Directory.GetFiles(dialog.FileName);
                    List<string> sorted = new List<string>();
                    foreach (string file in files)
                    {
                        if (file.Contains("mp3"))
                        {
                            paths.Add(file);
                            var a = file.Split("\\");
                            sorted.Add(a[a.Length - 1]);
                        }
                    }
                    sorted.Sort();
                    songs.ItemsSource = sorted;
                    Start();
                }
                else
                {
                    MessageBox.Show("Вы выбрали неверную папку!");
                }
            }
            else if (original.Name == "previous")
            {
                if (paths.Count == 0)
                {
                    MessageBox.Show("Не выбрана папка!");
                    return;
                }
                if (index > 0)
                {
                    media.Stop();
                    index--;
                    Start();
                }
                else
                {
                    media.Stop();
                    index = paths.Count - 1;
                    Start();
                }
            }
            else if (original.Name == "next")
            {
                if (paths.Count == 0)
                {
                    MessageBox.Show("Не выбрана папка!");
                    return;
                }
                if (index < paths.Count - 1)
                {
                    media.Stop();
                    index++;
                    Start();
                }
                else if (index > paths.Count - 1)
                {
                    media.Stop();
                    index = 0;
                    Start();
                }
            }
            else if (original.Name == "repeat")
            {
                if (!repeating)
                {
                    repeating = true;
                }
                else
                {
                    repeating = false;
                }
            }
            else if (original.Name == "random")
            {
                if (paths.Count == 0)
                {
                    MessageBox.Show("Не выбрана папка!");
                    return;
                }
                if (!israndom)
                {
                    israndom = true;
                    buff_paths = paths;
                    Random RND = new Random();
                    for (int i = 0; i < paths.Count; i++)
                    {
                        string tmp = paths[0];
                        paths.RemoveAt(0);
                        paths.Insert(RND.Next(paths.Count), tmp);
                    }
                }
                else
                {
                    israndom = false;
                    paths = buff_paths;
                }
            }
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!repeating)
            {
                index++;
                Start();
            }
            else
            {
                Start();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(slider.Value));
        }
        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (media.NaturalDuration.TimeSpan.Seconds < 10)
            {
                total_second.Content = media.NaturalDuration.TimeSpan.Minutes + ":" + "0" + media.NaturalDuration.TimeSpan.Seconds;
            }
            else
            {
                total_second.Content = media.NaturalDuration.TimeSpan.Minutes + ":" + media.NaturalDuration.TimeSpan.Seconds;
            }
            slider.Value = 0;
            slider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
            slider.Delay = 1;
            slider.TickFrequency = 1;

            DispatcherTimer dt = new DispatcherTimer(); 
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += ticktimer;
            dt.Start();
        }
        private void ticktimer(object sender, EventArgs e)
        {
            now_second.Content = TimeSpan.FromTicks((long)slider.Value).Minutes + ":" + TimeSpan.FromTicks((long)slider.Value).Seconds;
            slider.Value = media.Position.Ticks;
        }

        private void songs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(songs.SelectedItem != null)
            {
                int findIndex = 0;
                foreach (var item in paths)
                {
                    if (item.Contains(songs.SelectedItem.ToString()))
                    {
                        findIndex = paths.IndexOf(item);
                    }
                }
                index = findIndex;
                song_name.Content = paths[findIndex].Split("\\").Last().Split(".mp3")[0];
                media.Stop();
                media.Source = new Uri(paths[findIndex]);
                media.Play();
                media.Volume = 0.7;
                songs.SelectedItem = null;
            }
        }
    }
}
