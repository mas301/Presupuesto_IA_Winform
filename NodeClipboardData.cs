using System.Collections.Generic;

namespace DevExpressTreeListDemo
{
    internal sealed class NodeClipboardData
    {
        public object[] Values { get; set; }

        public List<NodeClipboardData> Children { get; set; }
    }
}