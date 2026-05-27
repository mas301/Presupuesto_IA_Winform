using DevExpress.XtraTreeList.Nodes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

namespace PresupuestoIA
{
    public partial class PresupuestoIA
    {
        private void treeList1_CustomColumnDisplayText(object sender, DevExpress.XtraTreeList.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Node == null || e.Column == null)
                return;

            bool allowsCalculationDetail = IsCalculationType2Or3(e.Node);
            bool isPartida = resourceTypePolicy.IsPartida(e.Node);
            bool isCalculationType8 = IsCalculationType8(e.Node);
            bool isCalculationType9 = IsCalculationType9(e.Node);
            bool isCalculationType10 = IsCalculationType10(e.Node);
            if (!allowsCalculationDetail
                && !isCalculationType8
                && !isCalculationType9
                && !isCalculationType10
                && e.Column == columnHorasJornal)
            {
                e.DisplayText = string.Empty;
                return;
            }

            if (e.Column == columnRendimiento
                && !isCalculationType9
                && !isCalculationType10
                && (!allowsCalculationDetail || isPartida))
            {
                e.DisplayText = string.Empty;
                return;
            }

            if (!resourceTypePolicy.IsSubpresupuesto(e.Node))
            {
                if (e.Column == columnUnidad)
                {
                    int? unitId = ToNullableInt(e.Value);
                    if (unitId.HasValue && unitDisplayNamesById != null && unitDisplayNamesById.TryGetValue(unitId.Value, out string unitDisplayName))
                        e.DisplayText = unitDisplayName;
                }

                return;
            }

            if (e.Column == columnValorUnitario)
                e.DisplayText = string.Empty;

            if (e.Column == columnUnidad)
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
            bool isCalculationType5 = IsCalculationType5(e.Node);
            bool isCalculationType7 = IsCalculationType7(e.Node);
            bool isCalculationType8 = IsCalculationType8(e.Node);
            bool isCalculationType9 = IsCalculationType9(e.Node);
            bool isCalculationType10 = IsCalculationType10(e.Node);
            bool isSubpresupuesto = resourceTypePolicy.IsSubpresupuesto(e.Node);
            bool isReadOnlyCell = !e.Column.OptionsColumn.AllowEdit
                || e.Column == columnValorTotal
                || e.Column == columnHorasJornal
                || e.Column == columnRendimiento
                || (e.Column == columnTipoCalculo && (isPartida || isSubpresupuesto))
                || (e.Column == columnCantidadTotal && isSubpresupuesto)
                || (e.Column == columnValorUnitario && isSubpresupuesto)
                || (e.Column == columnValorUnitario && isPartida && !isCalculationType7)
                || (e.Column == columnCantidadTotal && isPartida && isCalculationType7)
                || (e.Column == columnValorUnitario && isCalculationType4)
                || (e.Column == columnCantidadTotal && allowsCalculationDetail)
                || (e.Column == columnCantidadTotal && isCalculationType5)
                || (e.Column == columnCantidadTotal && isCalculationType8)
                || (e.Column == columnCantidadTotal && isCalculationType9)
                || (e.Column == columnCantidadTotal && isCalculationType10)
                || (e.Column == columnCantidad && !isCalculationType5 && !(isPartida && isCalculationType7) && !allowsCalculationDetail && !isCalculationType8 && !isCalculationType9 && !isCalculationType10)
                || (e.Column == columnPesoUnitario && !isCalculationType5)
                || (e.Column == columnDiasDuracion && !isPartida);

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

            var column = treeList1.FocusedColumn;
            bool allowsCalculationDetail = IsCalculationType2Or3(treeList1.FocusedNode);
            bool isCalculationType4 = IsCalculationType4(treeList1.FocusedNode);
            bool isSubpresupuesto = resourceTypePolicy.IsSubpresupuesto(treeList1.FocusedNode);
            bool isPartida = resourceTypePolicy.IsPartida(treeList1.FocusedNode);

            if (column == columnValorTotal)
            {
                e.Cancel = true;
                return;
            }

            if (isSubpresupuesto && (column == columnCantidadTotal || column == columnValorUnitario))
            {
                e.Cancel = true;
                return;
            }

            if (column == columnHorasJornal)
            {
                e.Cancel = true;
                return;
            }

            if (column == columnRendimiento)
            {
                e.Cancel = true;
                return;
            }

            if (column == columnTipoCalculo)
            {
                e.Cancel = true;
                return;
            }

            if (column == columnValorUnitario && resourceTypePolicy.IsPartida(treeList1.FocusedNode) && !IsCalculationType7(treeList1.FocusedNode))
                e.Cancel = true;

            if (column == columnCantidadTotal && resourceTypePolicy.IsPartida(treeList1.FocusedNode) && IsCalculationType7(treeList1.FocusedNode))
                e.Cancel = true;

            if (column == columnValorUnitario && isCalculationType4)
                e.Cancel = true;

            if (allowsCalculationDetail && column == columnCantidadTotal)
                e.Cancel = true;

            if (IsCalculationType5(treeList1.FocusedNode) && column == columnCantidadTotal)
                e.Cancel = true;

            if (!IsCalculationType5(treeList1.FocusedNode)
                && !(isPartida && IsCalculationType7(treeList1.FocusedNode))
                && !allowsCalculationDetail
                && !IsCalculationType8(treeList1.FocusedNode)
                && !IsCalculationType9(treeList1.FocusedNode)
                && !IsCalculationType10(treeList1.FocusedNode)
                && column == columnCantidad)
                e.Cancel = true;

            if (column == columnCantidadTotal && IsCalculationType8(treeList1.FocusedNode))
                e.Cancel = true;

            if (column == columnCantidadTotal && IsCalculationType9(treeList1.FocusedNode))
                e.Cancel = true;

            if (column == columnCantidadTotal && IsCalculationType10(treeList1.FocusedNode))
                e.Cancel = true;

            if (column == columnDiasDuracion && !isPartida)
                e.Cancel = true;

            if (!IsCalculationType5(treeList1.FocusedNode) && column == columnPesoUnitario)
                e.Cancel = true;
        }

        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            if (suppressPersistence)
                return;

            if (e.Node == null || e.Column == null)
                return;

            var column = e.Column;

            if (column == columnTipoRecurso)
            {
                suppressPersistence = true;
                try
                {
                    e.Node.SetValue(columnRecurso, null);
                    e.Node.SetValue(columnUnidad, null);
                    e.Node.SetValue(columnTipoCalculo, null);
                }
                finally
                {
                    suppressPersistence = false;
                }
            }

            if (column == columnRecurso)
            {
                suppressPersistence = true;
                try
                {
                    object rawResourceValue = e.Node.GetValue(columnRecurso);
                    int? resourceId = ToNullableInt(rawResourceValue);
                    if (!resourceId.HasValue)
                    {
                        resourceId = ResolveResourceIdFromNodeValue(e.Node, rawResourceValue);
                        if (resourceId.HasValue)
                            e.Node.SetValue(columnRecurso, resourceId.Value);
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
                        e.Node.SetValue(columnUnidad, unitId.Value);
                    }
                    else
                    {
                        e.Node.SetValue(columnUnidad, null);
                    }

                    if (calculationTypeId.HasValue)
                    {
                        e.Node.SetValue(columnTipoCalculo, calculationTypeId.Value);
                    }
                    else
                    {
                        e.Node.SetValue(columnTipoCalculo, null);
                    }

                    bool independiente = false;
                    if (resourceId.HasValue
                        && resourcesById != null
                        && resourcesById.TryGetValue(resourceId.Value, out RecursoDto independienteResource))
                    {
                        independiente = independienteResource.Independiente;
                    }
                    e.Node.SetValue(columnIndependiente, independiente);

                    if (resourceTypePolicy.IsPartida(e.Node))
                    {
                        if (resourceId.HasValue
                            && resourcesById != null
                            && resourcesById.TryGetValue(resourceId.Value, out RecursoDto resource))
                        {
                            SetPartidaCalculationData(e.Node, resource.Rendimiento, resource.RendimientoEquipos);
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

            if (column == columnValorUnitario)
                ReplicateManualUnitValueToSameResources(e.Node);

            if (column == columnCantidadTotal
                || column == columnCantidad
                || column == columnPesoUnitario)
                ReplicatePartidaManualValuesByResourceId(e.Node, column);

            if (column == columnTipoRecurso
                || column == columnRecurso
                || column == columnTipoCalculo
                || column == columnHorasJornal
                || column == columnRendimiento
                || column == columnCantidad
                || column == columnPesoUnitario
                || column == columnCantidadTotal
                || column == columnValorUnitario)
            {
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }
        }

        private bool IsPartidaIndependiente(TreeListNode node)
        {
            if (node == null || !resourceTypePolicy.IsPartida(node))
                return false;

            object value = node.GetValue(columnIndependiente);
            return value is bool flag && flag;
        }

        private bool IsNodeIndependiente(TreeListNode node)
        {
            if (node == null)
                return false;

            object value = node.GetValue(columnIndependiente);
            return value is bool flag && flag;
        }

        private void ReplicatePartidaManualValuesByResourceId(TreeListNode sourceNode, DevExpress.XtraTreeList.Columns.TreeListColumn column)
        {
            if (sourceNode == null || column == null)
                return;

            TreeListNode sourcePartida = FindContainingPartidaOrSelf(sourceNode);
            if (sourcePartida == null)
                return;

            // La cantidad del nodo Partida no se replica entre partidas iguales.
            if (column == columnCantidadTotal && sourceNode == sourcePartida)
                return;

            // Las partidas marcadas como Independiente no propagan cambios.
            if (IsPartidaIndependiente(sourcePartida))
                return;

            // Un recurso hijo marcado como Independiente tampoco propaga sus valores.
            if (sourceNode != sourcePartida && IsNodeIndependiente(sourceNode))
                return;

            int? sourcePartidaResourceId = ToNullableInt(sourcePartida.GetValue(columnRecurso));
            if (!sourcePartidaResourceId.HasValue)
                return;

            List<int> relativePath = BuildRelativePath(sourcePartida, sourceNode);
            object value = sourceNode.GetValue(column);

            suppressPersistence = true;
            try
            {
                foreach (TreeListNode node in EnumerateAllNodes())
                {
                    if (node == sourcePartida || !resourceTypePolicy.IsPartida(node))
                        continue;

                    int? targetPartidaResourceId = ToNullableInt(node.GetValue(columnRecurso));
                    if (!targetPartidaResourceId.HasValue || targetPartidaResourceId.Value != sourcePartidaResourceId.Value)
                        continue;

                    // Las partidas Independiente tampoco reciben cambios desde otras.
                    if (IsPartidaIndependiente(node))
                        continue;

                    TreeListNode targetNode = ResolveNodeByRelativePath(node, relativePath);
                    if (targetNode == null)
                        continue;

                    // Un recurso destino Independiente no acepta el cambio replicado.
                    if (targetNode != node && IsNodeIndependiente(targetNode))
                        continue;

                    targetNode.SetValue(column, value);
                }
            }
            finally
            {
                suppressPersistence = false;
            }
        }

        private void ReplicatePartidaStructureToPeers(TreeListNode sourcePartida)
        {
            if (sourcePartida == null || !resourceTypePolicy.IsPartida(sourcePartida))
                return;

            if (IsPartidaIndependiente(sourcePartida))
                return;

            int? sourceResourceId = ToNullableInt(sourcePartida.GetValue(columnRecurso));
            if (!sourceResourceId.HasValue)
                return;

            var childData = new List<NodeClipboardData>();
            for (int i = 0; i < sourcePartida.Nodes.Count; i++)
                childData.Add(treeListItemService.CaptureNode(sourcePartida.Nodes[i]));

            suppressPersistence = true;
            treeList1.BeginUnboundLoad();
            try
            {
                foreach (TreeListNode node in EnumerateAllNodes())
                {
                    if (node == sourcePartida || !resourceTypePolicy.IsPartida(node))
                        continue;

                    int? targetId = ToNullableInt(node.GetValue(columnRecurso));
                    if (!targetId.HasValue || targetId.Value != sourceResourceId.Value)
                        continue;

                    if (IsPartidaIndependiente(node))
                        continue;

                    while (node.Nodes.Count > 0)
                        node.Nodes[0].Remove();

                    for (int i = 0; i < childData.Count; i++)
                        treeListItemService.AppendCapturedSubtree(node, childData[i]);

                    node.Expanded = true;
                }
            }
            finally
            {
                treeList1.EndUnboundLoad();
                suppressPersistence = false;
            }

            treeListItemService.AssignItemNumbers();
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

            // Un recurso marcado como Independiente no propaga su ValorUnitario.
            if (IsNodeIndependiente(sourceNode))
                return;

            int? resourceId = ToNullableInt(sourceNode.GetValue(columnRecurso));
            if (!resourceId.HasValue)
                return;

            object unitValue = sourceNode.GetValue(columnValorUnitario);

            suppressPersistence = true;
            try
            {
                foreach (TreeListNode node in EnumerateAllNodes())
                {
                    if (node == sourceNode || IsUnitValueAutomaticallyCalculated(node))
                        continue;

                    // Los nodos Independiente tampoco reciben el cambio.
                    if (IsNodeIndependiente(node))
                        continue;

                    int? nodeResourceId = ToNullableInt(node.GetValue(columnRecurso));
                    if (!nodeResourceId.HasValue || nodeResourceId.Value != resourceId.Value)
                        continue;

                    node.SetValue(columnValorUnitario, unitValue);
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

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && (calculationType.Value == 2 || calculationType.Value == 3);
        }

        private bool IsCalculationType4(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 4;
        }

        private bool IsCalculationType5(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 5;
        }

        private bool IsCalculationType6(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 6;
        }

        private bool IsCalculationType7(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 7;
        }

        private bool IsCalculationType8(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 8;
        }

        private bool IsCalculationType9(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 9;
        }

        private bool IsCalculationType10(TreeListNode node)
        {
            if (node == null)
                return false;

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            return calculationType.HasValue && calculationType.Value == 10;
        }

        private int? ResolveResourceIdFromNodeValue(TreeListNode node, object resourceValue)
        {
            if (resourcesById == null || resourcesById.Count == 0)
                return null;

            string typedText = Convert.ToString(resourceValue);
            if (string.IsNullOrWhiteSpace(typedText))
                return null;

            typedText = typedText.Trim();
            int? tipoRecursoId = ToNullableInt(node == null ? null : node.GetValue(columnTipoRecurso));

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
                        child = treeList1.AppendNode(null, partidaNode);
                    }

                    int? tipoRecursoId = null;
                    if (resourcesById != null && resourcesById.TryGetValue(template.RecursoId, out RecursoDto resource))
                        tipoRecursoId = resource.TipoRecursoId;

                    int? previousResourceId = ToNullableInt(child.GetValue(columnRecurso));
                    object previousUnitValue = child.GetValue(columnValorUnitario);

                    child.SetValue(columnTipoRecurso, tipoRecursoId.HasValue ? (object)tipoRecursoId.Value : null);
                    child.SetValue(columnRecurso, template.RecursoId);
                    child.SetValue(columnUnidad, template.UnidadId.HasValue ? (object)template.UnidadId.Value : null);
                    child.SetValue(columnTipoCalculo, template.TipoCalculoId.HasValue ? (object)template.TipoCalculoId.Value : null);
                    child.SetValue(columnHorasJornal, null);
                    child.SetValue(columnRendimiento, template.Rendimiento.HasValue ? (object)template.Rendimiento.Value : null);
                    child.SetValue(columnCantidad, template.Cantidad.HasValue ? (object)template.Cantidad.Value : null);
                    child.SetValue(columnPesoUnitario, template.PesoUnitario.HasValue ? (object)template.PesoUnitario.Value : null);
                    child.SetValue(columnCantidadTotal, template.CantidadTotal);

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

                    child.SetValue(columnValorUnitario, unitValueToApply);
                    child.SetValue(columnValorTotal, null);
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

                int? resourceId = ToNullableInt(node.GetValue(columnRecurso));
                decimal? unitValue = ToNullableDecimal(node.GetValue(columnValorUnitario));
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
