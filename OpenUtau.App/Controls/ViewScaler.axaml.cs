﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ReactiveUI;

namespace OpenUtau.App.Controls {
    public partial class ViewScaler : UserControl {
        public static readonly DirectProperty<ViewScaler, double> MaxProperty =
            AvaloniaProperty.RegisterDirect<ViewScaler, double>(
                nameof(Max),
                o => o.Max,
                (o, v) => o.Max = v);
        public static readonly DirectProperty<ViewScaler, double> MinProperty =
            AvaloniaProperty.RegisterDirect<ViewScaler, double>(
                nameof(Min),
                o => o.Min,
                (o, v) => o.Min = v);
        public static readonly DirectProperty<ViewScaler, double> ValueProperty =
            AvaloniaProperty.RegisterDirect<ViewScaler, double>(
                nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v);

        public double Max {
            get => max;
            set => SetAndRaise(MaxProperty, ref max, value);
        }
        public double Min {
            get => min;
            set => SetAndRaise(MinProperty, ref min, value);
        }
        public double Value {
            get => value_;
            set => SetAndRaise(ValueProperty, ref value_, value);
        }

        private double max;
        private double min;
        private double value_;

        private Path path;

        public ViewScaler() {
            InitializeComponent();
            path = this.FindControl<Path>("Path");
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);
            if (!change.IsEffectiveValueChange) {
                return;
            }
            if (change.Property == MaxProperty || change.Property == MinProperty || change.Property == ValueProperty) {
                UpdatePath();
            }
        }

        private void UpdatePath() {
            double offset = 7 * Math.Log(ViewConstants.TrackHeightMax / Value, 2) / Math.Log(ViewConstants.TrackHeightMax / ViewConstants.TrackHeightMin, 2);
            double size = offset < 4 ? 4 : 8 - offset;
            if (double.IsNaN(offset) || double.IsNaN(size) ||
                double.IsInfinity(offset) || double.IsInfinity(size)) return;
            path.Data = Geometry.Parse(FormattableString.Invariant(
                $"M {8 - size} {offset + size} L 8 {offset} L {8 + size} {offset + size} M {8 - size} {16 - size - offset} L 8 {16 - offset} L {8 + size} {16 - size - offset}"));
        }
    }
}
