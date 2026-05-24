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

            if (isPartida)
            {
                node.SetValue(columnDiasDuracion, null);
            }

            if (allowsCalculationDetail)
            {
                decimal horasJornal = horasJornalPresupuestoActual;
                node.SetValue(columnHorasJornal, horasJornal);
                decimal cantidad = ToDecimal(node.GetValue(columnCantidad));
                decimal rendimiento = ResolveRendimientoForCalculationType2Or3(node);
                decimal cantidadCalculada = rendimiento == 0m ? 0m : (horasJornal * cantidad) / rendimiento;

                node.SetValue(columnRendimiento, rendimiento);
                node.SetValue(columnCantidadTotal, cantidadCalculada);
            }
            else if (IsCalculationType8(node) && !isPartida)
            {
                decimal horasJornal = horasJornalPresupuestoActual;
                node.SetValue(columnHorasJornal, horasJornal);
                node.SetValue(columnRendimiento, null);

                decimal? partidaDiasDuracion = GetPartidaDiasDuracionFromCatalog(node);
                node.SetValue(columnDiasDuracion, partidaDiasDuracion);

                decimal cantidad = ToDecimal(node.GetValue(columnCantidad));
                decimal diasDuracion = partidaDiasDuracion ?? 0m;
                node.SetValue(columnCantidadTotal, cantidad * horasJornal * diasDuracion);
            }
            else
            {
                node.SetValue(columnHorasJornal, null);

                if (!isPartida)
                {
                    node.SetValue(columnRendimiento, null);
                }

                if (IsCalculationType5(node))
                {
                    decimal cantidad = ToDecimal(node.GetValue(columnCantidad));
                    decimal pesoUnitario = ToDecimal(node.GetValue(columnPesoUnitario));
                    node.SetValue(columnCantidadTotal, cantidad * pesoUnitario);
                }
            }

            decimal partidasTotalInSubtree = 0m;
            for (int i = 0; i < node.Nodes.Count; i++)
                partidasTotalInSubtree += RecalculateNode(node.Nodes[i], descendantOrSelfOfPartida);

            if (isPartida)
            {
                decimal manoDeObraTotal = SumManoDeObraTotal(node);
                ApplyCalculationType4Rules(node, manoDeObraTotal);

                bool partidaIsType7 = IsCalculationType7(node);

                if (partidaIsType7)
                {
                    decimal childrenQuantityTotal = 0m;
                    for (int i = 0; i < node.Nodes.Count; i++)
                        childrenQuantityTotal += ToDecimal(node.Nodes[i].GetValue(columnCantidadTotal));
                    node.SetValue(columnPesoUnitario, childrenQuantityTotal);

                    decimal cantidad = ToDecimal(node.GetValue(columnCantidad));
                    decimal cantidadTotal = cantidad * childrenQuantityTotal;
                    node.SetValue(columnCantidadTotal, cantidadTotal);

                    decimal unitValue = ToDecimal(node.GetValue(columnValorUnitario));
                    decimal totalValue = CalculateTotalValue(node, cantidadTotal, unitValue);
                    node.SetValue(columnValorTotal, totalValue);
                    partidasTotalInSubtree += totalValue;
                }
                else
                {
                    decimal unitValue = 0m;
                    for (int i = 0; i < node.Nodes.Count; i++)
                        unitValue += ToDecimal(node.Nodes[i].GetValue(columnValorTotal));

                    decimal quantity = ToDecimal(node.GetValue(columnCantidadTotal));
                    decimal totalValue = CalculateTotalValue(node, quantity, unitValue);

                    node.SetValue(columnValorUnitario, unitValue);
                    node.SetValue(columnValorTotal, totalValue);
                    partidasTotalInSubtree += totalValue;
                }
            }
            else if (hasPartidaAncestor)
            {
                decimal quantity = ToDecimal(node.GetValue(columnCantidadTotal));
                decimal unitValue = ToDecimal(node.GetValue(columnValorUnitario));
                node.SetValue(columnValorTotal, CalculateTotalValue(node, quantity, unitValue));
            }

            if (!hasPartidaAncestor && !isPartida)
                node.SetValue(columnValorTotal, partidasTotalInSubtree);

            if (resourceTypePolicy.IsSubpresupuesto(node))
            {
                decimal childrenQuantityTotal = 0m;
                for (int i = 0; i < node.Nodes.Count; i++)
                    childrenQuantityTotal += ToDecimal(node.Nodes[i].GetValue(columnCantidadTotal));
                node.SetValue(columnCantidadTotal, childrenQuantityTotal);
            }

            return partidasTotalInSubtree;
        }

        private void ApplyCalculationType4Rules(TreeListNode node, decimal manoDeObraTotal)
        {
            if (node == null)
                return;

            if (IsCalculationType4(node))
            {
                decimal quantity = ToDecimal(node.GetValue(columnCantidadTotal));
                node.SetValue(columnValorUnitario, manoDeObraTotal);
                node.SetValue(columnValorTotal, CalculateTotalValue(node, quantity, manoDeObraTotal));
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
            string nodeType = resourceTypePolicy.NormalizeTypeName(node.GetValue(columnTipoRecurso));
            if (string.Equals(nodeType, "Mano de Obra", StringComparison.OrdinalIgnoreCase))
                total += ToDecimal(node.GetValue(columnValorTotal));

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

            int? calculationType = ToNullableInt(node.GetValue(columnTipoCalculo));
            if (!calculationType.HasValue)
                return 0m;

            TreeListNode partidaNode = FindContainingPartida(node);
            if (partidaNode == null)
                return 0m;

            if (calculationType.Value == 2)
                return GetPartidaRendimientoManoObra(partidaNode) ?? 0m;

            if (calculationType.Value == 3)
                return GetPartidaRendimientoEquipos(partidaNode) ?? 0m;

            return ToDecimal(node.GetValue(columnRendimiento));
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

        private decimal? GetPartidaDiasDuracionFromCatalog(TreeListNode node)
        {
            TreeListNode partidaNode = FindContainingPartida(node);
            if (partidaNode == null || resourcesById == null)
                return null;

            int? partidaResourceId = ToNullableInt(partidaNode.GetValue(columnRecurso));
            if (!partidaResourceId.HasValue)
                return null;

            if (resourcesById.TryGetValue(partidaResourceId.Value, out RecursoDto partidaResource))
                return partidaResource.DiasDuracion;

            return null;
        }
    }
}
