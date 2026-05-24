using System.Collections.Generic;

namespace PresupuestoIA
{
    internal sealed class NodeClipboardData
    {
        public Dictionary<string, object> Values { get; set; }

        public PartidaCalculationData PartidaCalculationData { get; set; }

        public List<NodeClipboardData> Children { get; set; }
    }
}