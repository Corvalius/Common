using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Corvalius.Dynamic
{
    public sealed class DynamicRegistryKey : DynamicObject
    {
        private bool writable;
        private RegistrySecurity security;
        private RegistryKey key;

        public DynamicRegistryKey(RegistryHive hive, RegistryView view, RegistrySecurity rs, bool writable = false)
        {
            this.writable = writable;
            this.key = RegistryKey.OpenBaseKey(hive, view);
        }

        public DynamicRegistryKey(RegistryHive hive, RegistrySecurity rs, bool writable = false)
            : this(hive, RegistryView.Default, rs, writable)
        { }

        private DynamicRegistryKey(RegistryKey key, RegistrySecurity rs, bool writable)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            this.key = key;
            this.security = rs;
            this.writable = writable;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            string name = binder.Name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (name.ToLowerInvariant() == "values")
                {
                    result = new DynamicRegistryValues(key, writable);
                }
                else
                {
                    var subkey = this.key.OpenSubKey(name, writable);
                    if (subkey == null && writable)
                        subkey = this.key.CreateSubKey(name);

                    result = new DynamicRegistryKey(subkey, security, writable);
                }
            }

            return result != null;
        }

        public IEnumerable<KeyValuePair<string, string>> Keys
        {
            get
            {
                foreach (var name in this.key.GetValueNames())
                {
                    yield return new KeyValuePair<string, string>(name, this.key.GetValue(name, string.Empty).ToString());
                }
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.key.GetSubKeyNames()
                           .Distinct()
                           .OrderBy(x => x);
        }

        private sealed class DynamicRegistryValues : DynamicObject
        {
            private bool writable;
            private RegistryKey key;

            public DynamicRegistryValues(RegistryKey key, bool writable)
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                this.key = key;
                this.writable = writable;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                string name = binder.Name;
                if (!string.IsNullOrWhiteSpace(name) && key.GetValueNames().Contains(name))
                    result = key.GetValue(name);

                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                try
                {
                    string name = binder.Name;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        key.SetValue(name, value);

                        return true;
                    }
                }
                catch { }

                return false;
            }

            public object this[string name]
            {
                get
                {
                    return this.key.GetValue(name, string.Empty);
                }
                set
                {
                    this.key.SetValue(name, value);
                }
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return this.key.GetValueNames()
                               .Distinct()
                               .OrderBy(x => x);
            }
        }
    }
}