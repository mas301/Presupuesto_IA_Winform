using DevExpress.XtraTreeList.Nodes;
using System;

namespace DevExpressTreeListDemo
{
    public partial class MainForm
    {
        private void RecalculateNumericRules()
        {
            RefreshHorasJornalPresupuesto();

            suppressPersistence = true;
            treeList1.BeginUnboundLoad();
            try
            {
                for (int i = 0; i < treeList1.Nodes.Count; i++)
                    RecalculateNode(treeList1.Nodes[i], false);
            }
            finally
            {
                treeList1.EndUnboundLoad();
                suppressPersistence = false;
            }
        }

        private decimal RecalculateNode(TreeListNode node, bool hasPartidaAncestor)
        {
            bool isPartida = resourceTypePolicy.IsPartida(node);
            bool allowsCalculationDetail = IsCalculationType2Or3(node);
            bool descendantOrSelfOfPartida = hasPartidaAncestor || isPartida;

            if (allowsCalculationDetail)
            {
                decimal horasJornal = horasJornalPresupuestoActual;
                node.SetValue(HoursPerDayColumnIndex, horasJornal);
                decimal cuadrilla = ToDecimal(node.GetValue(CrewColumnIndex));
                decimal rendimiento = ResolveRendimientoForCalculationType2Or3(node);
                decimal cantidadCalculada = rendimiento == 0m ? 0m : (horasJornal * cuadrilla) / rendimiento;

                node.SetValue(PerformanceColumnIndex, rendimiento);
                node.SetValue(QuantityColumnIndex, cantidadCalculada);
            }
            else
            {
                node.SetValue(HoursPerDayColumnIndex, null);

                if (!isPartida)
                {
                    node.SetValue(PerformanceColumnIndex, null);
                    node.SetValue(CrewColumnIndex, null);
                }
            }

            decimal partidasTotalInSubtree = 0m;
            for (int i = 0; i < node.Nodes.Count; i++)
                partidasTotalInSubtree += RecalculateNode(node.Nodes[i], descendantOrSelfOfPartida);

            if (isPartida)
            {
                decimal manoDeObraTotal = SumManoDeObraTotal(node);
                ApplyCalculationType4Rules(node, manoDeObraTotal);

                decimal unitValue = 0m;
                for (int i = 0; i < node.Nodes.Count; i++)
                    unitValue += ToDecimal(node.Nodes[i].GetValue(TotalValueColumnIndex));

                decimal quantity = ToDecimal(node.GetValue(QuantityColumnIndex));
                decimal totalValue = CalculateTotalValue(node, quantity, unitValue);

                node.SetValue(UnitValueColumnIndex, unitValue);
                node.SetValue(TotalValueColumnIndex, totalValue);
                partidasTotalInSubtree += totalValue;
            }
            else if (hasPartidaAncestor)
            {
                decimal quantity = ToDecimal(node.GetValue(QuantityColumnIndex));
                decimal unitValue = ToDecimal(node.GetValue(UnitValueColumnIndex));
                node.SetValue(TotalValueColumnIndex, CalculateTotalValue(node, quantity, unitValue));
            }

            if (!hasPartidaAncestor && !isPartida)
                node.SetValue(TotalValueColumnIndex, partidasTotalInSubtree);

            return partidasTotalInSubtree;
        }

        private void ApplyCalculationType4Rules(TreeListNode node, decimal manoDeObraTotal)
        {
            if (node == null)
                return;

            if (IsCalculationType4(node))
            {
                decimal quantity = ToDecimal(node.GetValue(QuantityColumnIndex));
                node.SetValue(UnitValueColumnIndex, manoDeObraTotal);
                node.SetValue(TotalValueColumnIndex, CalculateTotalValue(node, quantity, manoDeObraTotal));
            }

            for (int i = 0; i < node.Nodes.Count; i++)
                ApplyCalculationType4Rules(node.Nodes[i], manoDeObraTotal);
        }

        private decimal SumManoDeObraTotal(TreeListNode partidaNode)
        {
            if (partidaNode == null)
                return 0m;

            decimal total = 0m;
            for (int i = 0; i < partidaNode.Nodes.Count; i++)
                total += SumManoDeObraTotalRecursive(partidaNode.Nodes[i]);

            return total;
        }

        private decimal SumManoDeObraTotalRecursive(TreeListNode node)
        {
            if (node == null)
                return 0m;

            decimal total = 0m;
            string nodeType = resourceTypePolicy.NormalizeTypeName(node.GetValue(ResourceTypeColumnIndex));
            if (string.Equals(nodeType, "Mano de Obra", StringComparison.OrdinalIgnoreCase))
                total += ToDecimal(node.GetValue(TotalValueColumnIndex));

            for (int i = 0; i < node.Nodes.Count; i++)
                total += SumManoDeObraTotalRecursive(node.Nodes[i]);

            return total;
        }

        private decimal CalculateTotalValue(TreeListNode node, decimal quantity, decimal unitValue)
        {
            if (IsCalculationType4(node))
                return (quantity * unitValue) / 100m;

            return quantity * unitValue;
        }

        private decimal ResolveRendimientoForCalculationType2Or3(TreeListNode node)
        {
            if (node == null)
                return 0m;

            int? calculationType = ToNullableInt(node.GetValue(CalculationTypeColumnIndex));
            if (!calculationType.HasValue)
                return 0m;

            TreeListNode partidaNode = FindContainingPartida(node);
            if (partidaNode == null)
                return 0m;

            if (calculationType.Value == 2)
                return GetPartidaRendimientoManoObra(partidaNode) ?? 0m;

            if (calculationType.Value == 3)
                return GetPartidaRendimientoEquipos(partidaNode) ?? 0m;

            return ToDecimal(node.GetValue(PerformanceColumnIndex));
        }

        private TreeListNode FindContainingPartida(TreeListNode node)
        {
            TreeListNode current = node == null ? null : node.ParentNode;
            while (current != null)
            {
                if (resourceTypePolicy.IsPartida(current))
                    return current;

                current = current.ParentNode;
            }

            return null;
        }
    }
}
