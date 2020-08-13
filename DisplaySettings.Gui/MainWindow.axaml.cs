using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DisplaySettings.Gui
{
    public class MainWindow : Window
    {
        private class Resolution : IEquatable<Resolution>, IComparable<Resolution>
        {
            public int Width { get; set; }
            public int Height { get; set; }


            public bool Equals(Resolution other)
            {
                return Width == other.Width && Height == other.Height;
            }

            public override int GetHashCode()
            {

                return HashCode.Combine(Width, Height);
            }

            public int CompareTo(Resolution other)
            {
                return (Width * Height).CompareTo(other.Width * other.Height);
            }

            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            LoadModes();
        }

        private void LoadModes()
        {
            var primaryDisplayIndex = DisplayInformation.FindPrimaryDisplayIndex();
            var currentMode = DisplaySettings.GetDisplaySettings(primaryDisplayIndex).Mode;
            var modes = DisplaySettings.EnumerateAllDisplayModes(primaryDisplayIndex);

            var resolutions = modes.Select(x => new Resolution { Width = x.Width, Height = x.Height }).Distinct().ToList();
            var refreshRates = modes.Select(x => x.RefreshRate).Distinct().ToList();
            refreshRates.Sort();

            var resolutionComboxBox = this.Find<ComboBox>("resolutionComboBox");
            resolutionComboxBox.Items = resolutions;
            resolutionComboxBox.SelectedIndex = resolutions.FindIndex(x => x.Width == currentMode.Width && x.Height == currentMode.Height);

            var refreshRateComboxBox = this.Find<ComboBox>("refreshRateComboBox");
            refreshRateComboxBox.Items = refreshRates;
            refreshRateComboxBox.SelectedIndex = refreshRates.FindIndex(x => x == currentMode.RefreshRate);
        }
    }
}
