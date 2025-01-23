using UnityEngine;

namespace Tilda
{
    public class MonoConsole : MonoBehaviour
    {
        [SerializeField] private ConsoleUiView _view;

        private Console _console;

        private void Start()
        {
            _console = new Tilda.Console(_view);
        }
    }
}