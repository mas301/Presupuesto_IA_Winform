using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    public partial class MainForm
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
            try
            {
                treeList1.Nodes.Clear();

                var nodeById = new Dictionary<int, TreeListNode>();
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

                    TreeListNode node = treeList1.AppendNode(
                        new object[]
                        {
                            string.Empty,
                            item.TipoRecursoId,
                            item.RecursoId.HasValue ? (object)item.RecursoId.Value : null,
                            item.UnidadId.HasValue ? (object)item.UnidadId.Value : null,
                            item.TipoCalculoId.HasValue ? (object)item.TipoCalculoId.Value : null,
                            item.HorasJornal.HasValue ? (object)item.HorasJornal.Value : null,
                            isPartida ? null : (item.Rendimiento.HasValue ? (object)item.Rendimiento.Value : null),
                            isPartida ? null : (item.Cuadrilla.HasValue ? (object)item.Cuadrilla.Value : null),
                            item.Cantidad.HasValue ? (object)item.Cantidad.Value : null,
                            item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : null,
                            item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : null,
                            item.Alias
                        },
                        parentNode);

                    if (isPartida)
                        SetPartidaCalculationData(node, item.Rendimiento, item.Cuadrilla);

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
                if (!resourceTypePolicy.IsPartida(node))
                    continue;

                int? resourceId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                if (!resourceId.HasValue)
                    continue;

                if (!resourcesById.TryGetValue(resourceId.Value, out RecursoDto resource))
                    continue;

                SetPartidaCalculationData(node, resource.Rendimiento, resource.Cuadrilla);
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
                Alias = ToStringOrEmpty(node.GetValue(AliasColumnIndex)),
                RecursoxPresupuestoId = currentClientId,
                RecursoxPresupuestoPadreId = parentClientId,
                Orden = order,
                Nivel = level,
                TipoRecursoId = ToInt(node.GetValue(ResourceTypeColumnIndex)),
                RecursoId = ToNullableInt(node.GetValue(ResourceColumnIndex)),
                UnidadId = ToNullableInt(node.GetValue(UnitColumnIndex)),
                TipoCalculoId = ToNullableInt(node.GetValue(CalculationTypeColumnIndex)),
                HorasJornal = ToNullableDecimal(node.GetValue(HoursPerDayColumnIndex)),
                Rendimiento = resourceTypePolicy.IsPartida(node)
                    ? GetPartidaRendimientoManoObra(node)
                    : ToNullableDecimal(node.GetValue(PerformanceColumnIndex)),
                Cuadrilla = resourceTypePolicy.IsPartida(node)
                    ? GetPartidaRendimientoEquipos(node)
                    : ToNullableDecimal(node.GetValue(CrewColumnIndex)),
                Cantidad = ToNullableDecimal(node.GetValue(QuantityColumnIndex)),
                ValorUnitario = ToNullableDecimal(node.GetValue(UnitValueColumnIndex)),
                ValorTotal = ToNullableDecimal(node.GetValue(TotalValueColumnIndex))
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

                int? partidaId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                if (!partidaId.HasValue || !processedPartidaIds.Add(partidaId.Value))
                    continue;

                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    TreeListNode child = node.Nodes[i];
                    int? recursoId = ToNullableInt(child.GetValue(ResourceColumnIndex));
                    if (!recursoId.HasValue)
                        continue;

                    items.Add(new RecursoPartidaDto
                    {
                        EmpresaId = EmpresaId,
                        PartidaId = partidaId.Value,
                        RecursoId = recursoId.Value,
                        TipoCalculoId = ToNullableInt(child.GetValue(CalculationTypeColumnIndex)),
                        UnidadId = ToNullableInt(child.GetValue(UnitColumnIndex)),
                        Rendimiento = resourceTypePolicy.IsPartida(child)
                            ? GetPartidaRendimientoManoObra(child)
                            : ToNullableDecimal(child.GetValue(PerformanceColumnIndex)),
                        Cuadrilla = resourceTypePolicy.IsPartida(child)
                            ? GetPartidaRendimientoEquipos(child)
                            : ToNullableDecimal(child.GetValue(CrewColumnIndex)),
                        Cantidad = ToNullableDecimal(child.GetValue(QuantityColumnIndex)) ?? 0m,
                        Orden = i + 1
                    });
                }
            }
        }
    }
}
