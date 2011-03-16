//  Koffeinfrei Minus Share
//  Copyright (C) 2011  Alexis Reigel
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Koffeinfrei.Base.Wpf
{
    /// <summary>
    /// An animating gif image control
    /// </summary>
    /// <remarks>credits to http://stackoverflow.com/questions/210922/how-do-i-get-an-animated-gif-to-work-in-wpf/1134340#1134340</remarks>
    internal class GsAnimatedGifImage : Image
    {
        public Uri Uri { get; set; }

        public int FrameIndex
        {
            get { return (int) GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof (int), typeof (GsAnimatedGifImage), new UIPropertyMetadata(0, ChangingFrameIndex));

        private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GsAnimatedGifImage image = (GsAnimatedGifImage) obj;
            image.Source = image.decoder.Frames[(int) ev.NewValue];
            image.InvalidateVisual();
        }

        private GifBitmapDecoder decoder;
        private Int32Animation animation;
        private bool animationInProgress;

        protected override void OnInitialized(EventArgs e)
        {
            decoder = new GifBitmapDecoder(Uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            animation = new Int32Animation(0, decoder.Frames.Count - 1,
                                           new Duration(new TimeSpan(0, 0, 0, decoder.Frames.Count / 10, (int) ((decoder.Frames.Count / 10.0 - decoder.Frames.Count / 10) * 1000))))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            Source = decoder.Frames[0];
        }

        protected override void OnRender(DrawingContext context)
        {
            base.OnRender(context);
            if (!animationInProgress)
            {
                BeginAnimation(FrameIndexProperty, animation);
                animationInProgress = true;
            }
        }
    }
}