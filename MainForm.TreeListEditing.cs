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
                    int? resourceId = ToNullableInt(e.Node.GetValue(ResourceColumnIndex));
                    if (resourceId.HasValue
                        && resourceUnitIdsByResourceId != null
                        && resourceUnitIdsByResourceId.TryGetValue(resourceId.Value, out int? unitId)
                        && unitId.HasValue)
                    {
                        e.Node.SetValue(UnitColumnIndex, unitId.Value);
                    }
                    else
                    {
                        e.Node.SetValue(UnitColumnIndex, null);
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

    }
}
