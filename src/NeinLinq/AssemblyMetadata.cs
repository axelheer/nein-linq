// Source: https://github.com/dotnet/runtime/blob/master/src/libraries/Common/src/System/Reflection/AssemblyMetadataAttribute.cs

#if NET40

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    internal sealed class AssemblyMetadataAttribute : Attribute
    {
        private string m_key;
        private string m_value;

        public AssemblyMetadataAttribute(string key, string value)
        {
            m_key = key;
            m_value = value;
        }

        public string Key
        {
            get { return m_key; }
        }

        public string Value
        {
            get { return m_value;}
        }
    }
}

#endif
