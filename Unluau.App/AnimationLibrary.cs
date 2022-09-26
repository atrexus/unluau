using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Unluau.App
{
    public class AnimationLibrary
    {
        private Storyboard StoryBoard { get; set; }
        private IEasingFunction EasingFunction { get; set; }

        public AnimationLibrary()
        {

            EasingFunction = new QuadraticEase
            {
                EasingMode = EasingMode.EaseInOut
            };
        }

        public void SolidColorBrushAnimation(Brush Object, Color fromColor, Color toColor, double Miliseconds = 1000)
        {
            ColorAnimation Animation = new ColorAnimation();

            Animation.To = toColor;
            Animation.From = fromColor;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            Object = new SolidColorBrush(fromColor);
            Object.BeginAnimation(SolidColorBrush.ColorProperty, Animation);
        }

        public void LinearGradientColorAnimation(Brush Object, Color fromStart, Color fromEnd, Color toStart, Color toEnd, double Miliseconds = 1000)
        {
            LinearGradientBrush brush = (LinearGradientBrush)Object;
            ColorAnimation firstAnimation = new ColorAnimation();

            firstAnimation.To = toStart;
            firstAnimation.From = fromStart;
            firstAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            ColorAnimation secondAnimtion = new ColorAnimation();

            secondAnimtion.To = toEnd;
            secondAnimtion.From = fromEnd;
            secondAnimtion.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));


            brush = new LinearGradientBrush(fromStart, fromEnd, brush.StartPoint, brush.EndPoint);
            brush.GradientStops[0].BeginAnimation(GradientStop.ColorProperty, firstAnimation);
            brush.GradientStops[1].BeginAnimation(GradientStop.ColorProperty, secondAnimtion);
        }

        public void FadeIn(DependencyObject Object, double from = 0.0, double to = 1.0, double Miliseconds = 1000)
        {
            StoryBoard = new Storyboard();

            // Define all of our properies for our animation
            DoubleAnimation Animation = new DoubleAnimation();
            Animation.From = from;
            Animation.To = to;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            // Set our storyboard properties
            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath("Opacity", to));

            // Add the animation to the local StoryBoard and call it
            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }

        public void SizeChange(Border Object, double Width, double Height, double Miliseconds = 1000)
        {
            StoryBoard = new Storyboard();

            DoubleAnimation Animation = new DoubleAnimation();
            Animation.From = Object.Width;
            Animation.To = Width;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            DoubleAnimation Animation2 = new DoubleAnimation();
            Animation.From = Object.Height;
            Animation.To = Height;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(FrameworkElement.WidthProperty));

            Storyboard.SetTarget(Animation2, Object);
            Storyboard.SetTargetProperty(Animation2, new PropertyPath(FrameworkElement.HeightProperty));

            StoryBoard.Children.Add(Animation);
            StoryBoard.Children.Add(Animation2);

            StoryBoard.Begin();
        }

        public void FadeOut(DependencyObject Object, double Miliseconds = 1000)
        {
            StoryBoard = new Storyboard();

            // Define all of our properies for our animation
            DoubleAnimation Animation = new DoubleAnimation();
            Animation.From = 1.0;
            Animation.To = 0.0;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            // Set our storyboard properties
            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath("Opacity", 0));

            // Add the animation to the local StoryBoard and call it
            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }

        public void ObjectShift(DependencyObject Object, Thickness TFrom, Thickness TTo, double Miliseconds = 1000)
        {
            StoryBoard = new Storyboard();

            ThicknessAnimation Animation = new ThicknessAnimation();

            Animation.From = TFrom;
            Animation.To = TTo;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));
            Animation.EasingFunction = EasingFunction;

            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(FrameworkElement.MarginProperty));

            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }

        public void RotateObject(DependencyObject Object, double Miliseconds = 1000, int Angle = 360)
        {
            StoryBoard = new Storyboard();

            DoubleAnimation Animation = new DoubleAnimation();
            Animation.From = 0;
            Animation.To = Angle;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }

        public void CornerRadius(Rectangle Object, int To, double Miliseconds = 1000)
        {
            StoryBoard = new Storyboard();

            DoubleAnimation Animation = new DoubleAnimation();
            Animation.From = Object.RadiusX;
            Animation.To = To;
            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Rectangle.RadiusXProperty));

            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();

            Storyboard SotoryBoard1 = new Storyboard();

            DoubleAnimation Animation2 = new DoubleAnimation();
            Animation2.From = Object.RadiusY;
            Animation2.To = To;
            Animation2.Duration = new Duration(TimeSpan.FromMilliseconds(Miliseconds));

            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Rectangle.RadiusYProperty));

            SotoryBoard1.Children.Add(Animation);
            SotoryBoard1.Begin();
        }
    }
}
