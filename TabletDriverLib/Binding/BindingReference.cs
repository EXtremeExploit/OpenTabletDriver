using TabletDriverLib.Plugins;

namespace TabletDriverLib.Binding
{
    public class BindingReference
    {
        public BindingReference()
        {
        }

        public BindingReference(string bindingPath, string bindingProperty = null) : this()
        {
            Binding = new PluginReference(bindingPath);
            BindingProperty = bindingProperty;
        }

        public PluginReference Binding { private set; get; }
        public string BindingProperty { set; get; }

        public static readonly BindingReference None = new BindingReference
        {
            Binding = null,
            BindingProperty = null
        };

        public static BindingReference FromString(string full)
        {
            if (string.IsNullOrWhiteSpace(full))
                return None;
            
            return new BindingReference(
                BindingTools.GetBindingPath(full),
                BindingTools.GetBindingProperty(full)
            );
        }

        public override string ToString()
        {
            return this.Equals(None) ? null : BindingTools.GetBindingString(Binding.Path, BindingProperty);
        }

        public string ToDisplayString()
        {
            return this.Equals(None) ? null : BindingTools.GetBindingString(Binding.ToString(), BindingProperty);
        }
    }
}