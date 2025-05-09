﻿using UnityEngine;

namespace Tilda
{
    public class MonoConsole : MonoBehaviour
    {
        [SerializeField] private ConsoleUiView _view;
        [SerializeField] private bool _showAtStart;
        [SerializeField] private KeyCode _onOffKey;

        private Console _console;

        private void Awake()
        {
            _console = new Console(_view)
            {
                Enabled = _showAtStart
            };
            _view.Initialize();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(_onOffKey)) return;
            _console.Enabled = !_console.Enabled;
        }
    }
}