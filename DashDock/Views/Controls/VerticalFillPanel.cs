using System.Windows;
using System.Windows.Controls;
using DashDock.ViewModels;

namespace DashDock.Views.Controls
{
    public class VerticalFillPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var totalHeight = 0.0;
            var maxWidth = 0.0;

            foreach (UIElement child in InternalChildren)
            {
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
                totalHeight += child.DesiredSize.Height;
                maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
            }

            var resultWidth = double.IsPositiveInfinity(availableSize.Width) ? maxWidth : availableSize.Width;
            var resultHeight = double.IsPositiveInfinity(availableSize.Height) ? totalHeight : availableSize.Height;

            return new Size(resultWidth, resultHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleChildren = InternalChildren.OfType<FrameworkElement>()
                .Where(c => c.Visibility != Visibility.Collapsed)
                .ToList();

            if (visibleChildren.Count == 0)
            {
                return finalSize;
            }

            // NoteAreaViewModel または ImageGalleryAreaViewModel を持つ表示中要素のうち、末尾のものを探す
            var fillTarget = visibleChildren.LastOrDefault(c =>
                c.DataContext is NoteAreaViewModel or ImageGalleryAreaViewModel);

            var nonFillHeight = visibleChildren
                .Where(c => c != fillTarget)
                .Sum(c => c.DesiredSize.Height);

            var currentY = 0.0;

            foreach (var child in visibleChildren)
            {
                var childHeight = child.DesiredSize.Height;

                if (child == fillTarget)
                {
                    // 残りの高さを埋める
                    childHeight = Math.Max(childHeight, finalSize.Height - currentY - nonFillHeightAfter(visibleChildren, child));
                }

                child.Arrange(new Rect(0, currentY, finalSize.Width, childHeight));
                currentY += childHeight;
            }

            return finalSize;
        }

        private static double nonFillHeightAfter(System.Collections.Generic.List<FrameworkElement> children, FrameworkElement current)
        {
            var index = children.IndexOf(current);
            if (index < 0)
            {
                return 0.0;
            }

            return children.Skip(index + 1).Sum(c => c.DesiredSize.Height);
        }
    }
}