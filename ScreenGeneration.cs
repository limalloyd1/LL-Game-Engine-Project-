using System;
using System.Windows;
using System.Windows.Input;

namespace ScreenNameSpace
{
    public class screenClass
    {
        private Window window;

        public void createScreen()
        {
            window = new Window();
            window.Title = "My Game Window";
            window.Width = 800;
            window.Height = 600;

            // event handler for key presses
            window.KeyDown += Window_KeyDown;

            window.Show();
        }

        // Inquire: what type of method is an event handler?
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                closeScreen();
            }
        }

        public void closeScreen()
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}