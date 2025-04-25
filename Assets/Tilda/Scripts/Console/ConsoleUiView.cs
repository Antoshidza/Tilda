using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable PossibleMultipleEnumeration

namespace Tilda
{
    public class ConsoleUiView : MonoBehaviour
    {
        private const int SuggestionsCount = 5;
        private const string SuggestionSelectedClass = "suggestion-selected";
        
        [SerializeField] private UIDocument _document;
        private ListView _logsList;
        private TextField _inputField;
        private VisualElement _suggestionsRoot;
        private readonly List<string> _logs = new();
        private readonly List<string> _submitted = new();
        private readonly List<string> _suggestions = new();
        private int _restorePos;
        private int _suggestionsPos;

        public IEnumerable<string> CommandNames;

        public bool IsVisible
        {
            get => _document.rootVisualElement.visible;
            set => _document.rootVisualElement.visible = value;
        }

        public event Action<string> InputSubmitted;

        private void Start()
        {
            _logsList = _document.rootVisualElement.Q<ListView>();
            
            _inputField = _document.rootVisualElement.Q<TextField>(); 
            _inputField.textSelection.cursorColor = Color.white;
            _inputField.textSelection.selectionColor = Color.white;
            _inputField.RegisterValueChangedCallback(context =>
            {
                _suggestionsRoot.Clear();
                _suggestions.Clear();
                _suggestionsPos = 0;
                _suggestionsRoot.visible = false;
                if (context.newValue == string.Empty) return;
                var commands = CommandNames
                    .Where(command => command.Contains(context.newValue, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(command => command.IndexOf(context.newValue, StringComparison.OrdinalIgnoreCase))
                    .Take(SuggestionsCount); 
                if(!commands.Any() || commands.Contains(context.newValue)) return;
                _suggestionsRoot.visible = true;
                foreach (var command in commands.Take(SuggestionsCount))
                {
                    var matchIndex = command.IndexOf(context.newValue, StringComparison.OrdinalIgnoreCase);
                    var label = new Label(command.Insert(matchIndex, "<b>").Insert(matchIndex + context.newValue.Length + 3, "</b>"));
                    label.AddToClassList("suggestion");
                    _suggestionsRoot.Add(label);
                    _suggestions.Add(command);
                }
                _suggestionsRoot[0].AddToClassList(SuggestionSelectedClass);
            });
            
            _logsList.itemsSource = _logs;
            _logsList.bindItem = (root, itemIndex) => root.Q<Label>().text = _logs[itemIndex];
            _logsList.Q<ScrollView>().verticalScroller.value = _logsList.Q<ScrollView>().verticalScroller.highValue;

            _suggestionsRoot = _document.rootVisualElement.Q("suggestions");
        }

        private void Update()
        {
            UpdateInput();

            // prevent losing focus when pressing anything
            if (Input.anyKey)
                _inputField.Focus();
            
            UpdateApplySuggestion();
            UpdateSuggestionSelection();
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
            if (_submitted.Count <= 0 || _suggestions.Count > 0) return;
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

        private void UpdateApplySuggestion()
        {
            if(!Input.GetKeyDown(KeyCode.Tab) || _suggestions.Count == 0) return;
            _inputField.value = _suggestions[_suggestionsPos];
        }

        private void UpdateSuggestionSelection()
        {
            if(_suggestions.Count == 0) return;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _suggestionsPos = (_suggestionsPos + 1) % _suggestions.Count;
                SelectSuggestion();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _suggestionsPos = (_suggestionsPos - 1 + _suggestions.Count) % _suggestions.Count;
                SelectSuggestion();
            }

            return;

            void SelectSuggestion()
            {
                foreach (var visualElement in _suggestionsRoot.Children())
                    visualElement.RemoveFromClassList(SuggestionSelectedClass);
                _suggestionsRoot[_suggestionsPos].AddToClassList(SuggestionSelectedClass);
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