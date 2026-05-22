using DevExpress.XtraTreeList.Nodes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

namespace DevExpressTreeListDemo
{
    public partial class MainForm
    {
        private void treeList1_CustomColumnDisplayText(object sender, DevExpress.XtraTreeList.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Node == null || e.Column == null)
                return;

            bool allowsCalculationDetail = IsCalculationType2Or3(e.Node);
            bool isPartida = resourceTypePolicy.IsPartida(e.Node);
            int columnIndex = GetColumnIndex(e.Column);
            if (!allowsCalculationDetail
                && columnIndex == HoursPerDayColumnIndex)
            {
                e.DisplayText = string.Empty;
                return;
            }

            if ((columnIndex == PerformanceColumnIndex || columnIndex == CrewColumnIndex)
                && (!allowsCalculationDetail || isPartida))
            {
                e.DisplayText = string.Empty;
                return;
            }

            if (!resourceTypePolicy.IsSubpresupuesto(e.Node))
            {
                if (columnIndex == UnitColumnIndex)
                {
                    int? unitId = ToNullableInt(e.Value);
                    if (unitId.HasValue && unitDisplayNamesById != null && unitDisplayNamesById.TryGetValue(unitId.Value, out string unitDisplayName))
                        e.DisplayText = unitDisplayName;
                }

                return;
            }

            if (columnIndex == QuantityColumnIndex || columnIndex == UnitValueColumnIndex)
                e.DisplayText = string.Empty;

            if (columnIndex == UnitColumnIndex)
            {
                e.DisplayText = string.Empty;
                return;
            }

        }

        private void treeList1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node == null)
                return;

            bool isPartida = resourceTypePolicy.IsPartida(e.Node);
            bool allowsCalculationDetail = IsCalculationType2Or3(e.Node);
            bool isCalculationType4 = IsCalculationType4(e.Node);
            bool isSubpresupuesto = resourceTypePolicy.IsSubpresupuesto(e.Node);
            bool isReadOnlyCell = !e.Column.OptionsColumn.AllowEdit
                || GetColumnIndex(e.Column) == TotalValueColumnIndex
                || GetColumnIndex(e.Column) == HoursPerDayColumnIndex
                || GetColumnIndex(e.Column) == PerformanceColumnIndex
                || (GetColumnIndex(e.Column) == CalculationTypeColumnIndex && (isPartida || isSubpresupuesto))
                || (GetColumnIndex(e.Column) == CrewColumnIndex && (!allowsCalculationDetail || isPartida))
                || (GetColumnIndex(e.Column) == QuantityColumnIndex && isSubpresupuesto)
                || (GetColumnIndex(e.Column) == UnitValueColumnIndex && isSubpresupuesto)
                || (GetColumnIndex(e.Column) == UnitValueColumnIndex && isPartida)
                || (GetColumnIndex(e.Column) == UnitValueColumnIndex && isCalculationType4)
                || (GetColumnIndex(e.Column) == QuantityColumnIndex && allowsCalculationDetail);

            if (isReadOnlyCell)
                e.Appearance.BackColor = Color.WhiteSmoke;

            if (isPartida)
            {
                e.Appearance.FontStyleDelta = FontStyle.Bold;
                e.Appearance.ForeColor = Color.DarkGreen;
                return;
            }

            if (resourceTypePolicy.IsSubpresupuesto(e.Node))
                e.Appearance.FontStyleDelta = FontStyle.Bold;
        }

        private void treeList1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (treeList1.FocusedNode == null || treeList1.FocusedColumn == null)
                return;

            int columnIndex = GetColumnIndex(treeList1.FocusedColumn);
            bool allowsCalculationDetail = IsCalculationType2Or3(treeList1.FocusedNode);
            bool isCalculationType4 = IsCalculationType4(treeList1.FocusedNode);
            bool isSubpresupuesto = resourceTypePolicy.IsSubpresupuesto(treeList1.FocusedNode);
            bool isPartida = resourceTypePolicy.IsPartida(treeList1.FocusedNode);

            if (columnIndex == TotalValueColumnIndex)
            {
                e.Cancel = true;
                return;
            }

            if (isSubpresupuesto && (columnIndex == QuantityColumnIndex || columnIndex == UnitValueColumnIndex))
            {
                e.Cancel = true;
                return;
            }

            if (columnIndex == HoursPerDayColumnIndex)
            {
                e.Cancel = true;
                return;
            }

            if (columnIndex == PerformanceColumnIndex)
            {
                e.Cancel = true;
                return;
            }

            if (columnIndex == CalculationTypeColumnIndex)
            {
                e.Cancel = true;
                return;
            }

            if (columnIndex == CrewColumnIndex && (!allowsCalculationDetail || isPartida))
            {
                e.Cancel = true;
                return;
            }

            if (columnIndex == UnitValueColumnIndex && resourceTypePolicy.IsPartida(treeList1.FocusedNode))
                e.Cancel = true;

            if (columnIndex == UnitValueColumnIndex && isCalculationType4)
                e.Cancel = true;

            if (allowsCalculationDetail && columnIndex == QuantityColumnIndex)
                e.Cancel = true;
        }

        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            if (suppressPersistence)
                return;

            if (e.Node == null || e.Column == null)
                return;

            int columnIndex = GetColumnIndex(e.Column);

            if (columnIndex == ResourceTypeColumnIndex)
            {
                suppressPersistence = true;
                try
                {
                    e.Node.SetValue(ResourceColumnIndex, null);
                    e.Node.SetValue(UnitColumnIndex, null);
                    e.Node.SetValue(CalculationTypeColumnIndex, null);
                }
                finally
                {
                    suppressPersistence = false;
                }
            }

            if (columnIndex == ResourceColumnIndex)
            {
                suppressPersistence = true;
                try
                {
                    object rawResourceValue = e.Node.GetValue(ResourceColumnIndex);
                    int? resourceId = ToNullableInt(rawResourceValue);
                    if (!resourceId.HasValue)
                    {
                        resourceId = ResolveResourceIdFromNodeValue(e.Node, rawResourceValue);
                        if (resourceId.HasValue)
                            e.Node.SetValue(ResourceColumnIndex, resourceId.Value);
                    }

                    int? unitId = null;
                    if (resourceId.HasValue
                        && resourceUnitIdsByResourceId != null
                        && resourceUnitIdsByResourceId.TryGetValue(resourceId.Value, out int? foundUnitId))
                    {
                        unitId = foundUnitId;
                    }

                    int? calculationTypeId = null;
                    if (resourceId.HasValue
                        && resourceCalculationTypeIdsByResourceId != null
                        && resourceCalculationTypeIdsByResourceId.TryGetValue(resourceId.Value, out int? foundCalculationTypeId))
                    {
                        calculationTypeId = foundCalculationTypeId;
                    }

                    if (unitId.HasValue)
                    {
                        e.Node.SetValue(UnitColumnIndex, unitId.Value);
                    }
                    else
                    {
                        e.Node.SetValue(UnitColumnIndex, null);
                    }

                    if (calculationTypeId.HasValue)
                    {
                        e.Node.SetValue(CalculationTypeColumnIndex, calculationTypeId.Value);
                    }
                    else
                    {
                        e.Node.SetValue(CalculationTypeColumnIndex, null);
                    }

                    if (resourceTypePolicy.IsPartida(e.Node))
                    {
                        if (resourceId.HasValue
                            && resourcesById != null
                            && resourcesById.TryGetValue(resourceId.Value, out RecursoDto resource))
                        {
                            SetPartidaCalculationData(e.Node, resource.Rendimiento, resource.Cuadrilla);
                            SyncPartidaChildrenFromTemplate(e.Node, resourceId.Value);
                        }
                        else
                        {
                            SetPartidaCalculationData(e.Node, null, null);
                            SyncPartidaChildrenFromTemplate(e.Node, null);
                        }
                    }
                }
                finally
                {
                    suppressPersistence = false;
                }
            }

            if (columnIndex == UnitValueColumnIndex)
                ReplicateManualUnitValueToSameResources(e.Node);

            if (columnIndex == CrewColumnIndex || columnIndex == QuantityColumnIndex)
                ReplicatePartidaManualValuesByResourceId(e.Node, columnIndex);

            if (columnIndex == ResourceTypeColumnIndex
                || columnIndex == ResourceColumnIndex
                || columnIndex == CalculationTypeColumnIndex
                || columnIndex == HoursPerDayColumnIndex
                || columnIndex == PerformanceColumnIndex
                || columnIndex == CrewColumnIndex
                || columnIndex == QuantityColumnIndex
                || columnIndex == UnitValueColumnIndex)
            {
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }
        }

        private int GetColumnIndex(DevExpress.XtraTreeList.Columns.TreeListColumn column)
        {
            if (column == null || treeList1 == null || treeList1.Columns == null)
                return -1;

            return treeList1.Columns.IndexOf(column);
        }

        private void ReplicatePartidaManualValuesByResourceId(TreeListNode sourceNode, int columnIndex)
        {
            if (sourceNode == null)
                return;

            TreeListNode sourcePartida = FindContainingPartidaOrSelf(sourceNode);
            if (sourcePartida == null)
                return;

            // La cantidad del nodo Partida no se replica entre partidas iguales.
            if (columnIndex == QuantityColumnIndex && sourceNode == sourcePartida)
                return;

            int? sourcePartidaResourceId = ToNullableInt(sourcePartida.GetValue(ResourceColumnIndex));
            if (!sourcePartidaResourceId.HasValue)
                return;

            List<int> relativePath = BuildRelativePath(sourcePartida, sourceNode);
            object value = sourceNode.GetValue(columnIndex);

            suppressPersistence = true;
            try
            {
                foreach (TreeListNode node in EnumerateAllNodes())
                {
                    if (node == sourcePartida || !resourceTypePolicy.IsPartida(node))
                        continue;

                    int? targetPartidaResourceId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                    if (!targetPartidaResourceId.HasValue || targetPartidaResourceId.Value != sourcePartidaResourceId.Value)
                        continue;

                    TreeListNode targetNode = ResolveNodeByRelativePath(node, relativePath);
                    if (targetNode == null)
                        continue;

                    targetNode.SetValue(columnIndex, value);
                }
            }
            finally
            {
                suppressPersistence = false;
            }
        }

        private TreeListNode FindContainingPartidaOrSelf(TreeListNode node)
        {
            TreeListNode current = node;
            while (current != null)
            {
                if (resourceTypePolicy.IsPartida(current))
                    return current;

                current = current.ParentNode;
            }

            return null;
        }

        private static List<int> BuildRelativePath(TreeListNode partidaNode, TreeListNode node)
        {
            var path = new List<int>();
            TreeListNode current = node;
            while (current != null && current != partidaNode)
            {
                TreeListNode parent = current.ParentNode;
                if (parent == null)
                    return new List<int>();

                path.Add(parent.Nodes.IndexOf(current));
                current = parent;
            }

            path.Reverse();
            return path;
        }

        private static TreeListNode ResolveNodeByRelativePath(TreeListNode partidaNode, List<int> relativePath)
        {
            TreeListNode current = partidaNode;
            for (int i = 0; i < relativePath.Count; i++)
            {
                int index = relativePath[i];
                if (current == null || index < 0 || index >= current.Nodes.Count)
                    return null;

                current = current.Nodes[index];
            }

            return current;
        }

        private void ReplicateManualUnitValueToSameResources(TreeListNode sourceNode)
        {
            if (sourceNode == null || IsUnitValueAutomaticallyCalculated(sourceNode))
                return;

            int? resourceId = ToNullableInt(sourceNode.GetValue(ResourceColumnIndex));
            if (!resourceId.HasValue)
                return;

            object unitValue = sourceNode.GetValue(UnitValueColumnIndex);

            suppressPersistence = true;
            try
            {
                foreach (TreeListNode node in EnumerateAllNodes())
                {
                    if (node == sourceNode || IsUnitValueAutomaticallyCalculated(node))
                        continue;

                    int? nodeResourceId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                    if (!nodeResourceId.HasValue || nodeResourceId.Value != resourceId.Value)
                        continue;

                    node.SetValue(UnitValueColumnIndex, unitValue);
                }
            }
            finally
            {
                suppressPersistence = false;
            }
        }

        private bool IsUnitValueAutomaticallyCalculated(TreeListNode node)
        {
            return node != null
                && (resourceTypePolicy.IsPartida(node)
                    || resourceTypePolicy.IsSubpresupuesto(node)
                    || IsCalculationType4(node));
        }

        private bool IsCalculationType2Or3(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(CalculationTypeColumnIndex));
            return calculationType.HasValue && (calculationType.Value == 2 || calculationType.Value == 3);
        }

        private bool IsCalculationType4(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(CalculationTypeColumnIndex));
            return calculationType.HasValue && calculationType.Value == 4;
        }

        private int? ResolveResourceIdFromNodeValue(TreeListNode node, object resourceValue)
        {
            if (resourcesById == null || resourcesById.Count == 0)
                return null;

            string typedText = Convert.ToString(resourceValue);
            if (string.IsNullOrWhiteSpace(typedText))
                return null;

            typedText = typedText.Trim();
            int? tipoRecursoId = ToNullableInt(node == null ? null : node.GetValue(ResourceTypeColumnIndex));

            int? matchedId = null;
            foreach (KeyValuePair<int, RecursoDto> item in resourcesById)
            {
                RecursoDto resource = item.Value;
                if (resource == null)
                    continue;

                if (!string.Equals(resource.Recurso ?? string.Empty, typedText, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (tipoRecursoId.HasValue && resource.TipoRecursoId != tipoRecursoId.Value)
                    continue;

                if (matchedId.HasValue)
                    return null;

                matchedId = item.Key;
            }

            return matchedId;
        }

        private void SyncPartidaChildrenFromTemplate(TreeListNode partidaNode, int? partidaResourceId)
        {
            if (partidaNode == null || !resourceTypePolicy.IsPartida(partidaNode))
                return;

            List<RecursoPartidaDto> templateRows = partidaResourceId.HasValue
                ? Datos.ObtenerRecursosPartida(EmpresaId, partidaResourceId.Value)
                : new List<RecursoPartidaDto>();
            Dictionary<int, decimal> sharedUnitValuesByResourceId = BuildEditableUnitValuesOutsidePartida(partidaNode);

            treeList1.BeginUnboundLoad();
            try
            {
                while (partidaNode.Nodes.Count > templateRows.Count)
                    partidaNode.Nodes[partidaNode.Nodes.Count - 1].Remove();

                for (int i = 0; i < templateRows.Count; i++)
                {
                    RecursoPartidaDto template = templateRows[i];

                    TreeListNode child;
                    if (i < partidaNode.Nodes.Count)
                    {
                        child = partidaNode.Nodes[i];
                    }
                    else
                    {
                        child = treeList1.AppendNode(new object[]
                        {
                            string.Empty,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            string.Empty
                        }, partidaNode);
                    }

                    int? tipoRecursoId = null;
                    if (resourcesById != null && resourcesById.TryGetValue(template.RecursoId, out RecursoDto resource))
                        tipoRecursoId = resource.TipoRecursoId;

                    int? previousResourceId = ToNullableInt(child.GetValue(ResourceColumnIndex));
                    object previousUnitValue = child.GetValue(UnitValueColumnIndex);

                    child.SetValue(ResourceTypeColumnIndex, tipoRecursoId.HasValue ? (object)tipoRecursoId.Value : null);
                    child.SetValue(ResourceColumnIndex, template.RecursoId);
                    child.SetValue(UnitColumnIndex, template.UnidadId.HasValue ? (object)template.UnidadId.Value : null);
                    child.SetValue(CalculationTypeColumnIndex, template.TipoCalculoId.HasValue ? (object)template.TipoCalculoId.Value : null);
                    child.SetValue(HoursPerDayColumnIndex, null);
                    child.SetValue(PerformanceColumnIndex, template.Rendimiento.HasValue ? (object)template.Rendimiento.Value : null);
                    child.SetValue(CrewColumnIndex, template.Cuadrilla.HasValue ? (object)template.Cuadrilla.Value : null);
                    child.SetValue(QuantityColumnIndex, template.Cantidad);

                    object unitValueToApply = null;
                    if (!IsUnitValueAutomaticallyCalculated(child)
                        && sharedUnitValuesByResourceId.TryGetValue(template.RecursoId, out decimal sharedUnitValue))
                    {
                        unitValueToApply = sharedUnitValue;
                    }
                    else if (!IsUnitValueAutomaticallyCalculated(child)
                        && previousResourceId.HasValue
                        && previousResourceId.Value == template.RecursoId
                        && previousUnitValue != null)
                    {
                        unitValueToApply = previousUnitValue;
                    }

                    child.SetValue(UnitValueColumnIndex, unitValueToApply);
                    child.SetValue(TotalValueColumnIndex, null);
                }
            }
            finally
            {
                treeList1.EndUnboundLoad();
            }

            partidaNode.Expanded = true;
            treeListItemService.AssignItemNumbers();
        }

        private Dictionary<int, decimal> BuildEditableUnitValuesOutsidePartida(TreeListNode excludedPartidaNode)
        {
            var result = new Dictionary<int, decimal>();
            foreach (TreeListNode node in EnumerateAllNodes())
            {
                if (node == null || node == excludedPartidaNode || IsDescendantOf(node, excludedPartidaNode))
                    continue;

                if (IsUnitValueAutomaticallyCalculated(node))
                    continue;

                int? resourceId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                decimal? unitValue = ToNullableDecimal(node.GetValue(UnitValueColumnIndex));
                if (!resourceId.HasValue || !unitValue.HasValue)
                    continue;

                if (!result.ContainsKey(resourceId.Value))
                    result.Add(resourceId.Value, unitValue.Value);
            }

            return result;
        }

        private static bool IsDescendantOf(TreeListNode node, TreeListNode ancestor)
        {
            TreeListNode current = node == null ? null : node.ParentNode;
            while (current != null)
            {
                if (current == ancestor)
                    return true;

                current = current.ParentNode;
            }

            return false;
        }

    }
}
