using UnityEngine;

namespace Console
{
    public class MonoConsole : MonoBehaviour
    {
        [SerializeField] private ConsoleUiView _view;

        private Console _console;

        private void Start()
        {
            _console = new Console(_view);
        }
    }
}