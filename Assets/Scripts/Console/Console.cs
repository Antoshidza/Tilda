using System;

namespace Tilda
{
    public class Console
    {
        private readonly ConsoleUiView _view;
        private readonly Cmd _cmd = new();

        public bool Enabled
        {
            get => _view.IsVisible;
            set
            {
                _view.IsVisible = value;
                _view.enabled = value;
            }
        }

        public event Action<string> Logged; 

        public Console(ConsoleUiView view)
        {
            _cmd.AddCommand("help", _ => Log($"<b>Commands</b>:\n{_cmd.ToRichString()}"), "show all commands");
            _cmd.AddCommand("clear", _ => Clear(), "clear console logs");
            
            _view = view;
            view.InputSubmitted += input =>
            {
                Logged?.Invoke(input);
                if(!_cmd.TryHandle(input))
                    Log($"<b>\"{input}\"</b> isn't a command. Use <b>help</b> command to see all possible commands.");
            };
            view.CommandNames = _cmd.CommandNames;
        }

        public void Log(string message)
        {
            _view.Log(message);
            Logged?.Invoke(message);
        }

        public void LogWarning(string message)
        {
            _view.LogWarning(message);
            Logged?.Invoke(message);
        }

        public void LogError(string message)
        {
            _view.LogError(message);
            Logged?.Invoke(message);
        }

        public void Clear() => _view.Clear();
    }
}