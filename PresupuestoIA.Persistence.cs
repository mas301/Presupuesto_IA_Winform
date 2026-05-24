using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PresupuestoIA
{
    public partial class PresupuestoIA
    {
        private void LoadBudgetTree()
        {
            ReloadBudgetFromTablesAndRecalculate();
        }

        private void ReloadBudgetFromTablesAndRecalculate()
        {
            var items = Datos.ObtenerRecursosPresupuesto(EmpresaId, PresupuestoId);
            var catalogResources = Datos.ObtenerRecursos(EmpresaId);
            LoadBudgetTree(items, catalogResources);
        }

        private void LoadBudgetTree(List<RecursoPresupuestoDto> items, List<RecursoDto> catalogResources)
        {
            if (items == null)
                items = new List<RecursoPresupuestoDto>();

            suppressPersistence = true;
            treeList1.BeginUnboundLoad();
            var nodeById = new Dictionary<int, TreeListNode>();
            try
            {
                treeList1.Nodes.Clear();

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    bool isPartida = string.Equals(
                        resourceTypePolicy.NormalizeTypeName(item.TipoRecursoId),
                        "Partida",
                        StringComparison.OrdinalIgnoreCase);

                    TreeListNode parentNode = null;
                    if (item.RecursoxPresupuestoPadreId.HasValue)
                        nodeById.TryGetValue(item.RecursoxPresupuestoPadreId.Value, out parentNode);

                    TreeListNode node = treeList1.AppendNode(null, parentNode);
                    node.SetValue(columnTipoRecurso, item.TipoRecursoId);
                    node.SetValue(columnRecurso, item.RecursoId.HasValue ? (object)item.RecursoId.Value : null);
                    node.SetValue(columnUnidad, item.UnidadId.HasValue ? (object)item.UnidadId.Value : null);
                    node.SetValue(columnTipoCalculo, item.TipoCalculoId.HasValue ? (object)item.TipoCalculoId.Value : null);
                    node.SetValue(columnHorasJornal, item.HorasJornal.HasValue ? (object)item.HorasJornal.Value : null);
                    node.SetValue(columnRendimiento, isPartida ? null : (item.Rendimiento.HasValue ? (object)item.Rendimiento.Value : null));
                    node.SetValue(columnCantidad, item.Cantidad.HasValue ? (object)item.Cantidad.Value : null);
                    node.SetValue(columnPesoUnitario, item.PesoUnitario.HasValue ? (object)item.PesoUnitario.Value : null);
                    node.SetValue(columnDiasDuracion, item.DiasDuracion.HasValue ? (object)item.DiasDuracion.Value : null);
                    node.SetValue(columnCantidadTotal, item.CantidadTotal.HasValue ? (object)item.CantidadTotal.Value : null);
                    node.SetValue(columnValorUnitario, item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : null);
                    node.SetValue(columnValorTotal, item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : null);
                    node.SetValue(columnAlias, item.Alias);

                    if (isPartida)
                        SetPartidaCalculationData(node, item.Rendimiento, null);

                    nodeById[item.RecursoxPresupuestoId] = node;
                }

                treeListItemService.AssignItemNumbers();
                RefreshPartidaCalculationDataFromCatalog(catalogResources);
            }
            finally
            {
                treeList1.EndUnboundLoad();
                suppressPersistence = false;
            }

            // Restaurar estado expandido/colapsado despues de EndUnboundLoad para que tome efecto.
            suppressPersistence = true;
            try
            {
                var itemsById = new Dictionary<int, RecursoPresupuestoDto>();
                for (int i = 0; i < items.Count; i++)
                    itemsById[items[i].RecursoxPresupuestoId] = items[i];

                foreach (KeyValuePair<int, TreeListNode> entry in nodeById)
                {
                    if (!entry.Value.HasChildren)
                        continue;
                    if (itemsById.TryGetValue(entry.Key, out RecursoPresupuestoDto data))
                        entry.Value.Expanded = data.Expandido;
                }
            }
            finally
            {
                suppressPersistence = false;
            }

            RecalculateNumericRules();
            UpdatePendingSaveIndicator(false);
        }

        private void RefreshPartidaCalculationDataFromCatalog(List<RecursoDto> catalogResources)
        {
            if (catalogResources == null || catalogResources.Count == 0)
                return;

            var resourcesById = new Dictionary<int, RecursoDto>();
            for (int i = 0; i < catalogResources.Count; i++)
            {
                RecursoDto resource = catalogResources[i];
                if (!resourcesById.ContainsKey(resource.RecursoId))
                    resourcesById.Add(resource.RecursoId, resource);
            }

            foreach (TreeListNode node in EnumerateAllNodes())
            {
                int? resourceId = ToNullableInt(node.GetValue(columnRecurso));
                if (!resourceId.HasValue)
                {
                    node.SetValue(columnIndependiente, false);
                    continue;
                }

                if (!resourcesById.TryGetValue(resourceId.Value, out RecursoDto resource))
                {
                    node.SetValue(columnIndependiente, false);
                    continue;
                }

                node.SetValue(columnIndependiente, resource.Independiente);

                if (!resourceTypePolicy.IsPartida(node))
                    continue;

                SetPartidaCalculationData(node, resource.Rendimiento, resource.RendimientoEquipos);
            }
        }

        private void SaveBudgetTree()
        {
            if (suppressPersistence)
                return;

            var items = new List<RecursoPresupuestoDto>();
            var partidaItems = new List<RecursoPartidaDto>();
            int nextId = 1;
            for (int i = 0; i < treeList1.Nodes.Count; i++)
                CollectBudgetItems(treeList1.Nodes[i], null, 0, i + 1, items, ref nextId);

            CollectPartidaItems(partidaItems);

            Datos.GuardarRecursosPresupuesto(EmpresaId, PresupuestoId, items);
            Datos.GuardarRecursosPartida(EmpresaId, partidaItems);
            hasPendingAutoSaveChanges = false;
            lastAutoSaveUtc = DateTime.UtcNow;
            UpdatePendingSaveIndicator(false);
        }

        private void btnSaveBudgetDetail_Click(object sender, EventArgs e)
        {
            try
            {
                SaveBudgetTree();
                MessageBox.Show("El detalle del presupuesto fue grabado correctamente.", "Grabar detalle", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                string message = ex.GetBaseException().Message;
                MessageBox.Show(
                    "No se pudo grabar el detalle del presupuesto.\n\n" + message,
                    "Error al grabar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CollectBudgetItems(
            TreeListNode node,
            int? parentClientId,
            int level,
            int order,
            List<RecursoPresupuestoDto> items,
            ref int nextId)
        {
            int currentClientId = nextId++;
            items.Add(new RecursoPresupuestoDto
            {
                EmpresaId = EmpresaId,
                PresupuestoId = PresupuestoId,
                Alias = ToStringOrEmpty(node.GetValue(columnAlias)),
                RecursoxPresupuestoId = currentClientId,
                RecursoxPresupuestoPadreId = parentClientId,
                Orden = order,
                Nivel = level,
                TipoRecursoId = ToInt(node.GetValue(columnTipoRecurso)),
                RecursoId = ToNullableInt(node.GetValue(columnRecurso)),
                UnidadId = ToNullableInt(node.GetValue(columnUnidad)),
                TipoCalculoId = ToNullableInt(node.GetValue(columnTipoCalculo)),
                HorasJornal = ToNullableDecimal(node.GetValue(columnHorasJornal)),
                Rendimiento = resourceTypePolicy.IsPartida(node)
                    ? GetPartidaRendimientoManoObra(node)
                    : ToNullableDecimal(node.GetValue(columnRendimiento)),
                Cantidad = ToNullableDecimal(node.GetValue(columnCantidad)),
                PesoUnitario = ToNullableDecimal(node.GetValue(columnPesoUnitario)),
                DiasDuracion = ToNullableDecimal(node.GetValue(columnDiasDuracion)),
                CantidadTotal = ToNullableDecimal(node.GetValue(columnCantidadTotal)),
                ValorUnitario = ToNullableDecimal(node.GetValue(columnValorUnitario)),
                ValorTotal = ToNullableDecimal(node.GetValue(columnValorTotal)),
                Expandido = node.Expanded
            });

            for (int i = 0; i < node.Nodes.Count; i++)
                CollectBudgetItems(node.Nodes[i], currentClientId, level + 1, i + 1, items, ref nextId);
        }

        private static string ToStringOrEmpty(object value)
        {
            return value == null ? string.Empty : Convert.ToString(value);
        }

        private static int ToInt(object value)
        {
            if (value == null)
                return 0;

            if (value is int intValue)
                return intValue;

            int parsed;
            return int.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private static int? ToNullableInt(object value)
        {
            if (value == null)
                return null;

            if (value is int intValue)
                return intValue;

            int parsed;
            return int.TryParse(Convert.ToString(value), out parsed) ? (int?)parsed : null;
        }

        private static decimal? ToNullableDecimal(object value)
        {
            if (value == null)
                return null;

            if (value is decimal decimalValue)
                return decimalValue;

            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? (decimal?)parsed : null;
        }

        private static decimal ToDecimal(object value)
        {
            decimal? parsed = ToNullableDecimal(value);
            return parsed.HasValue ? parsed.Value : 0m;
        }

        private void CollectPartidaItems(List<RecursoPartidaDto> items)
        {
            if (items == null)
                return;

            var processedPartidaIds = new HashSet<int>();
            foreach (TreeListNode node in EnumerateAllNodes())
            {
                if (!resourceTypePolicy.IsPartida(node))
                    continue;

                int? partidaId = ToNullableInt(node.GetValue(columnRecurso));
                if (!partidaId.HasValue || !processedPartidaIds.Add(partidaId.Value))
                    continue;

                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    TreeListNode child = node.Nodes[i];
                    int? recursoId = ToNullableInt(child.GetValue(columnRecurso));
                    if (!recursoId.HasValue)
                        continue;

                    items.Add(new RecursoPartidaDto
                    {
                        EmpresaId = EmpresaId,
                        PartidaId = partidaId.Value,
                        RecursoId = recursoId.Value,
                        TipoCalculoId = ToNullableInt(child.GetValue(columnTipoCalculo)),
                        UnidadId = ToNullableInt(child.GetValue(columnUnidad)),
                        Rendimiento = resourceTypePolicy.IsPartida(child)
                            ? GetPartidaRendimientoManoObra(child)
                            : ToNullableDecimal(child.GetValue(columnRendimiento)),
                        Cantidad = ToNullableDecimal(child.GetValue(columnCantidad)),
                        PesoUnitario = ToNullableDecimal(child.GetValue(columnPesoUnitario)),
                        DiasDuracion = ToNullableDecimal(child.GetValue(columnDiasDuracion)),
                        CantidadTotal = ToNullableDecimal(child.GetValue(columnCantidadTotal)) ?? 0m,
                        Orden = i + 1
                    });
                }
            }
        }
    }
}
