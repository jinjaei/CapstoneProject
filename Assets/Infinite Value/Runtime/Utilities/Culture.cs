using System;
using System.Globalization;
using UnityEngine;

namespace InfiniteValue
{
    /// <summary> 
    /// This <see langword="class"/> lets you use a <see cref="CultureInfo"/> as a field that will apear in the inspector and be serialized.
    /// </summary>
    [Serializable] public class Culture : ISerializationCallbackReceiver
    {
        // custom types
        public enum Type
        {
            InvariantCulture,
            CurrentCulture,
            SpecificCulture,
        }

        // editor fields
        [SerializeField] Type type = 0;
        [SerializeField] string name = default;

        // public fields

        /// <summary> Use this property to get the <see cref="CultureInfo"/> from this instance. </summary> 
        public CultureInfo info { get; private set; }

        // constructors
        public Culture(Type type)
        {
            this.type = type;

            SetCultureInfo();
        }

        public Culture(string name)
        {
            this.type = Type.SpecificCulture;
            this.name = name;

            SetCultureInfo();
        }

        // private methods
        void SetCultureInfo()
        {
            switch (type)
            {
                case Type.InvariantCulture: info = CultureInfo.InvariantCulture; return;
                case Type.CurrentCulture: info = CultureInfo.CurrentCulture; return;
                case Type.SpecificCulture: info = CultureInfo.GetCultureInfo(name); return;
            }
        }

        // implicit casts
        public static implicit operator CultureInfo(Culture c) => c.info;

        // serialization (interface implementation)
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            SetCultureInfo();
        }
    }
}