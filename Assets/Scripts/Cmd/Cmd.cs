using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmd
{
    public class Cmd
    {
        public const string CommandDelimiter = " ";

        private readonly Dictionary<string, (Action<string> handler, string desc)> _handlerMap;

        public IEnumerable<string> CommandNames => _handlerMap.Keys;

        public Cmd(params (string, Action<string>)[] handlers)
        {
            _handlerMap = new(handlers.Length);
            foreach (var (commandName, handler) in handlers)
                AddCommand(commandName, handler);
        }

        public void AddCommand(in string commandName, Action<string> handler, in string description = default)
        {
            if (commandName.Contains(CommandDelimiter))
                throw new CmdException($"Command name can't contain command delimiter \"{CommandDelimiter}\". You are trying to add command with \"{commandName}\" name.");
            
            _handlerMap[commandName] = (handler, description);
        }

        public bool TryHandle(string input)
        {
            var firstDelimiterIndex = input.IndexOf(CommandDelimiter, StringComparison.Ordinal);
            var commandName = firstDelimiterIndex == -1 ? input : input[..firstDelimiterIndex];

            if (!_handlerMap.TryGetValue(commandName, out var command))
                return false;

            command.handler(firstDelimiterIndex == -1 ? default : input[(firstDelimiterIndex + 1)..]);
            
            return true;
        }

        public override string ToString() => 
            _handlerMap.Aggregate(string.Empty, (current, commands) => current + $"* {commands.Key} - {commands.Value.desc}\n");

        public string ToRichString() =>
            _handlerMap.Aggregate(string.Empty, (current, commands) => current + $"* <b>{commands.Key}</b> - {commands.Value.desc}\n");
    }
}