﻿using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Controls;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Document _document;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _document = new Document(Main, new ScrollBarDrawer(SideScroll));
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
            {
                if (c == '\b' || c== '\r')
                    return;

                _document.InsertChar(c);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                case Key.Delete:
                    _document.DeleteChar();
                    break;
                case Key.Enter:
                    _document.AddLine();
                    break;
                case Key.Left:
                    _document.MoveCursor(Direction.Left);
                    break;
                case Key.Up:
                    _document.MoveCursor(Direction.Up);
                    break;
                case Key.Right:
                    _document.MoveCursor(Direction.Right);
                    break;
                case Key.Down:
                    _document.MoveCursor(Direction.Down);
                    break;
                default:
                    return;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _document.SaveFile(dialog.FileName);
            title.Text = dialog.FileName;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = "txt";

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _document.LoadFile(dialog.FileName);
            title.Text = dialog.FileName;
        }

        private void SideScroll_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            _document.MoveDisplay((int)Math.Floor(mousePosition.Y / parent.ActualHeight * 100));
        }
    }
}
