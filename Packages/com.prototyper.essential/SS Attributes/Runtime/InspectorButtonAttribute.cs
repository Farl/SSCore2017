namespace SS
{
    public class InspectorButtonAttribute : UnityEngine.PropertyAttribute
    {
        public string label;
        public string method;
        public string[] labels;
        public string[] methods;

        public InspectorButtonAttribute(string label, string method)
        {
            this.label = label;
            this.method = method;
        }

        public InspectorButtonAttribute(string[] labels, string[] methods)
        {
            this.labels = labels;
            this.methods = methods;
        }
    }
}
