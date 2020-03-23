using System;
using System.Windows;
using System.Windows.Threading;

namespace MathVis
{
    public static class ExtensionMethods
    {
        public static void ForceRedraw(this UIElement element)
        {
            static void EmptyAction()
            {
            }

            element.Dispatcher.Invoke(DispatcherPriority.Render, (Action) EmptyAction);
        }
    }
}