using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;

namespace DevExpressTreeListDemo
{
    internal sealed class ResourceTypePolicy
    {
        private readonly HashSet<string> validResourceTypes;

        private static readonly HashSet<string> TypesThatRequirePartidaParent = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Materia Prima",
            "Mano de Obra",
            "Equipos",
            "Herramientas",
            "Subcontratos",
            "Servicios"
        };

        private static readonly HashSet<string> TypesThatCannotHaveChildren = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Materia Prima",
            "Mano de Obra",
            "Equipos",
            "Herramientas",
            "Contratos",
            "Servicios"
        };

        private static readonly HashSet<string> TypesForbiddenUnderPartida = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Subpresupuesto"
        };

        public ResourceTypePolicy(IEnumerable<string> validResourceTypes = null)
        {
            this.validResourceTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (validResourceTypes == null)
                return;

            foreach (string resourceType in validResourceTypes)
            {
                string normalized = NormalizeTypeName(resourceType);
                if (!string.IsNullOrEmpty(normalized))
                    this.validResourceTypes.Add(normalized);
            }
        }

        public string NormalizeTypeName(object value)
        {
            return (value == null ? string.Empty : Convert.ToString(value)).Trim();
        }

        public bool IsPartida(TreeListNode node)
        {
            if (node == null)
                return false;

            return string.Equals(NormalizeTypeName(node.GetValue(1)), "Partida", StringComparison.OrdinalIgnoreCase);
        }

        public bool CanParentAcceptChildren(TreeListNode parentNode)
        {
            if (parentNode == null)
                return true;

            return !TypesThatCannotHaveChildren.Contains(NormalizeTypeName(parentNode.GetValue(1)));
        }

        public bool CanBePlacedUnder(TreeListNode parentNode, string childTypeName)
        {
            if (!CanParentAcceptChildren(parentNode))
                return false;

            if (IsPartida(parentNode) && TypesForbiddenUnderPartida.Contains(childTypeName))
                return false;

            if (TypesThatRequirePartidaParent.Contains(childTypeName))
                return IsPartida(parentNode);

            return true;
        }

        public bool HasAssignedResourceType(TreeListNode node)
        {
            if (node == null)
                return false;

            string value = NormalizeTypeName(node.GetValue(1));
            return !string.IsNullOrEmpty(value) && validResourceTypes.Contains(value);
        }

        public bool IsTypeAllowedForNode(TreeListNode node, string candidateType)
        {
            if (!CanBePlacedUnder(node == null ? null : node.ParentNode, candidateType))
                return false;

            if (node != null && node.Nodes.Count > 0 && TypesThatCannotHaveChildren.Contains(candidateType))
                return false;

            if (node != null && HasRestrictedDirectChild(node) && !string.Equals(candidateType, "Partida", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        public DataTable BuildAllowedResourceTypesTable(TreeListNode node, DataTable allResourceTypesTable)
        {
            var filtered = allResourceTypesTable.Clone();
            for (int i = 0; i < allResourceTypesTable.Rows.Count; i++)
            {
                DataRow row = allResourceTypesTable.Rows[i];
                string candidateType = NormalizeTypeName(row["TipoRecurso"]);

                if (IsTypeAllowedForNode(node, candidateType))
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private bool HasRestrictedDirectChild(TreeListNode node)
        {
            if (node == null)
                return false;

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (TypesThatRequirePartidaParent.Contains(NormalizeTypeName(node.Nodes[i].GetValue(1))))
                    return true;
            }

            return false;
        }
    }
}