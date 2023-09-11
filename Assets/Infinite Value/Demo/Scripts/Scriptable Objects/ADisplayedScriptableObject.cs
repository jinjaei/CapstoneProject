using UnityEngine;
using InspectorAttribute;
using System.Linq;

/*
 * Abstract class that will be inherited by both Upgrade and Income.
 * This will create a readable name from the file name and define an image and description to use when displaying
 * the object in game.
 * 
 */
namespace IV_Demo
{
    public class ADisplayedScriptableObject : ScriptableObject
    {
        // editor fields
        [Header("Display")]
        [Tooltip("Name of this object (This is the filename without digits at the start).")]
        [ReadOnly] [SerializeField] string _name;
        // public fields
        [Tooltip("Description of this object displayed in the game.")]
        [TextArea(2, 16)] public string description;
        [Tooltip("Sprite used in the game to represent this object.")]
        [PreviewSprite] public Sprite image;

        // public properties
        public new string name => _name;

        // unity
        protected virtual void OnValidate()
        {
            _name = new string(base.name.Where((c) => c != '_').ToArray());

            int i = 0;
            for (; i < _name.Length && char.IsDigit(_name[i]); i++);
            _name = _name.Substring(i);

            for (i = 0; i < _name.Length && char.IsWhiteSpace(_name[i]); i++);
            _name = _name.Substring(i);
        }
    }
}