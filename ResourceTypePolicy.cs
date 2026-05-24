using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;

namespace DevExpressTreeListDemo
{
    internal sealed class ResourceTypePolicy
    {
        private const string ResourceTypeFieldName = "TipoRecurso";
        private const string TypePartida = "Partida";
        private const string TypeMateriaPrima = "Materia Prima";
        private const string TypeManoDeObra = "Mano de Obra";
        private const string TypeEquipos = "Equipos";
        private const string TypeHerramientas = "Herramientas";
        private const string TypeSubcontratos = "Subcontratos";
        private const string TypeServicios = "Servicios";
        private const string TypeContratos = "Contratos";
        private const string TypeSubpresupuesto = "Subpresupuesto";
        private const string ResourceTypeColumnName = "TipoRecurso";

        private readonly HashSet<string> validResourceTypes;
        private readonly Dictionary<int, string> resourceTypeNamesById;

        private static readonly HashSet<string> TypesThatRequirePartidaParent = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            TypeMateriaPrima,
            TypeManoDeObra,
            TypeEquipos,
            TypeHerramientas,
            TypeSubcontratos,
            TypeServicios
        };

        private static readonly HashSet<string> TypesThatCannotHaveChildren = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            TypeMateriaPrima,
            TypeManoDeObra,
            TypeEquipos,
            TypeHerramientas,
            TypeContratos,
            TypeServicios
        };

        private static readonly HashSet<string> TypesForbiddenUnderPartida = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            TypeSubpresupuesto
        };

        public ResourceTypePolicy(IEnumerable<string> validResourceTypes = null, IDictionary<int, string> resourceTypeNamesById = null)
        {
            this.validResourceTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            this.resourceTypeNamesById = new Dictionary<int, string>();

            if (resourceTypeNamesById != null)
            {
                foreach (var item in resourceTypeNamesById)
                    this.resourceTypeNamesById[item.Key] = item.Value;
            }

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
            if (value == null)
                return string.Empty;

            if (value is int id && resourceTypeNamesById.TryGetValue(id, out string typeNameById))
                return typeNameById;

            string text = Convert.ToString(value);
            if (int.TryParse(text, out int parsedId) && resourceTypeNamesById.TryGetValue(parsedId, out string typeNameByParsedId))
                return typeNameByParsedId;

            return text == null ? string.Empty : text.Trim();
        }

        public bool IsPartida(TreeListNode node)
        {
            if (node == null)
                return false;

            return string.Equals(NormalizeTypeName(GetResourceTypeValue(node)), TypePartida, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSubpresupuesto(TreeListNode node)
        {
            if (node == null)
                return false;

            return string.Equals(NormalizeTypeName(GetResourceTypeValue(node)), TypeSubpresupuesto, StringComparison.OrdinalIgnoreCase);
        }

        public bool CanParentAcceptChildren(TreeListNode parentNode)
        {
            if (parentNode == null)
                return true;

            return !TypesThatCannotHaveChildren.Contains(NormalizeTypeName(GetResourceTypeValue(parentNode)));
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

            string value = NormalizeTypeName(GetResourceTypeValue(node));
            return !string.IsNullOrEmpty(value) && validResourceTypes.Contains(value);
        }

        public bool IsTypeAllowedForNode(TreeListNode node, string candidateType)
        {
            if (!CanBePlacedUnder(node == null ? null : node.ParentNode, candidateType))
                return false;

            if (node != null && node.Nodes.Count > 0 && TypesThatCannotHaveChildren.Contains(candidateType))
                return false;

            if (node != null && HasRestrictedDirectChild(node) && !string.Equals(candidateType, TypePartida, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        public DataTable BuildAllowedResourceTypesTable(TreeListNode node, DataTable allResourceTypesTable)
        {
            var filtered = allResourceTypesTable.Clone();
            for (int i = 0; i < allResourceTypesTable.Rows.Count; i++)
            {
                DataRow row = allResourceTypesTable.Rows[i];
                string candidateType = NormalizeTypeName(row[ResourceTypeColumnName]);

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
                if (TypesThatRequirePartidaParent.Contains(NormalizeTypeName(GetResourceTypeValue(node.Nodes[i]))))
                    return true;
            }

            return false;
        }

        private static object GetResourceTypeValue(TreeListNode node)
        {
            if (node == null || node.TreeList == null)
                return null;

            var column = node.TreeList.Columns[ResourceTypeFieldName];
            return column == null ? null : node.GetValue(column);
        }
    }
}