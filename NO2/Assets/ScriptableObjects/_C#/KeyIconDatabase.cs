using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "KeyIconDatabase", menuName = "Scriptable Objects/KeyIconDatabase")]
public class KeyIconDatabase : ScriptableObject
{
    private const string EmptyKey = "empty";

    [System.Serializable]
    public class KeyIconTuple
    {
        public string bindingPath; // ej: "<Keyboard>/a", "<Keyboard>/leftShift"
        public Sprite iconIdle;
        public Sprite iconPressed;
    }
    [SerializeField] private KeyIconTuple[] keyIcons;
    private Dictionary<string, Sprite> _dictionaryIdle;
    private Dictionary<string, Sprite> _dictionaryPressed;

    private void BuildDictionariesIfNeeded()
    {
        if (_dictionaryIdle != null) return;

        _dictionaryIdle = new Dictionary<string, Sprite>();
        _dictionaryPressed = new Dictionary<string, Sprite>();
        foreach (var tuple in keyIcons)
        {
            _dictionaryIdle[tuple.bindingPath] = tuple.iconIdle;
            _dictionaryPressed[tuple.bindingPath] = tuple.iconPressed;
        }
    }

    public Sprite GetIcon(string effectivePath)
    {
        BuildDictionariesIfNeeded();

        if (!_dictionaryIdle.TryGetValue(effectivePath, out Sprite sprite) || sprite == null)
            _dictionaryIdle.TryGetValue(EmptyKey, out sprite);

        return sprite;
    }
    public Sprite GetIconPressed(string effectivePath)
    {
        BuildDictionariesIfNeeded();

        if (!_dictionaryPressed.TryGetValue(effectivePath, out Sprite sprite) || sprite == null)
            _dictionaryPressed.TryGetValue(EmptyKey, out sprite);

        return sprite;
    }
}