using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Console
{
    public class ConsoleUiView : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        private ListView _logsList;
        private TextField _inputField;
        private VisualElement _suggestions;
        private readonly List<string> _logs = new();
        private readonly List<string> _submitted = new();
        private int _restorePos;

        public IEnumerable<string> CommandNames; 

        public event Action<string> InputSubmitted;

        private void Start()
        {
            _logsList = _document.rootVisualElement.Q<ListView>();
            
            _inputField = _document.rootVisualElement.Q<TextField>(); 
            _inputField.textSelection.cursorColor = Color.white;
            _inputField.textSelection.selectionColor = Color.white;
            _inputField.RegisterValueChangedCallback(context =>
            {
                _suggestions.Clear();
                _suggestions.visible = false;
                if (context.newValue == string.Empty) return;
                var commands = CommandNames
                    .Where(command => command.Contains(context.newValue, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(command => command.IndexOf(context.newValue, StringComparison.OrdinalIgnoreCase))
                    .Take(5); 
                if(!commands.Any() || commands.Contains(context.newValue)) return;
                _suggestions.visible = true;
                foreach (var command in commands.Take(5))
                {
                    var matchIndex = command.IndexOf(context.newValue, StringComparison.OrdinalIgnoreCase);
                    var label = new Label(command.Insert(matchIndex, "<b>").Insert(matchIndex + context.newValue.Length + 3, "</b>"));
                    label.AddToClassList("suggestion");
                    _suggestions.Add(label);
                }
            });
            
            _logsList.itemsSource = _logs;
            _logsList.bindItem = (root, itemIndex) => root.Q<Label>().text = _logs[itemIndex];
            _logsList.Q<ScrollView>().verticalScroller.value = _logsList.Q<ScrollView>().verticalScroller.highValue;

            _suggestions = _document.rootVisualElement.Q("suggestions");
        }

        private void Update()
        {
            UpdateInput();

            // prevent losing focus when pressing anything
            if (Input.anyKey)
                _inputField.Focus();
            
            UpdateSubmitRestore();
        }

        public void Clear()
        {
            _logs.Clear();
            _logsList.Rebuild();
        }

        private void UpdateInput()
        {
            if (!Input.GetKeyUp(KeyCode.Return) || _inputField.value == string.Empty) return;
            // LogInternal(_inputField.value, true);
            _submitted.Add(_inputField.value);
            InputSubmitted?.Invoke(_inputField.value);
        }

        private void UpdateSubmitRestore()
        {
            if (_submitted.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _restorePos = (_restorePos + 1) % _submitted.Count;
                    _inputField.value = _submitted[^(_restorePos + 1)];
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _restorePos = (_restorePos - 1 + _submitted.Count) % _submitted.Count;
                    _inputField.value = _submitted[^(_restorePos + 1)];
                }
                else if (Input.anyKeyDown) 
                    _restorePos = 0;
            }
        }

        private void LogInternal(string message, bool submitted = false)
        {
            _logs.Add(message);
            _logsList.Rebuild();
            
            if(submitted)
                _logsList.ScrollToItem(_logs.Count - 1);
        }

        public void Log(string message) => LogInternal(message);
        public void LogWarning(string message) => LogInternal(message.Colored(Color.yellow));
        public void LogError(string message) => LogInternal(message.Colored(Color.red));
    }
}