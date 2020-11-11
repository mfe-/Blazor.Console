﻿using Console.Extensions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wpf.Console
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _stringWriterRedirect.Dispose();
            _stringReaderRedirect.Dispose();
        }
        StringWriterRedirect _stringWriterRedirect;
        StringReaderRedirect _stringReaderRedirect;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _stringWriterRedirect = new StringWriterRedirect() { StringWriterRedirectTaskFunc = WriteLine };
            System.Console.SetOut(_stringWriterRedirect);
            _stringReaderRedirect = new StringReaderRedirect(Read, null);
            System.Console.SetIn(_stringReaderRedirect);


            Thread t = new Thread(
            () =>
            {
                ConsoleApp1.Program.Main(null);
                MessageBox.Show("ConsoleApp finished.");
            });
            t.IsBackground = true;
            t.Name = "console";
            t.Start();


        }
        SynchronizationContext SynchronizationContext = SynchronizationContext.Current;
        public Task WriteLine(string consoleInput)
        {
            string readLineText = consoleInput;
            _StringBuilder.AppendLine(readLineText);
            if (SynchronizationContext.Current != this.SynchronizationContext)
            {
                //because outputTextBox is handled by a diffrent thread use synchronizationcontext to post operation on original thread
                this.SynchronizationContext.Post(new SendOrPostCallback((outputTextB) =>
                {
                    if (outputTextB is TextBlock textBlock)
                    {
                        outputTextBox.Text = _StringBuilder.ToString();
                    }
                }), outputTextBox);
            }
            else
            {
                outputTextBox.Text = _StringBuilder.ToString();
            }
            return Task.CompletedTask;
        }
        TaskCompletionSource<string> StringTaskCompletionSource = new TaskCompletionSource<string>();
        public async Task<string> Read()
        {
            StringTaskCompletionSource = new TaskCompletionSource<string>();
            var t = StringTaskCompletionSource.Task.ConfigureAwait(false);

            string r = await t;
            StringTaskCompletionSource = null;
            return r;
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StringTaskCompletionSource?.SetResult(inputTextBox.Text);
                inputTextBox.Text = String.Empty;
            }
        }
    }
}
