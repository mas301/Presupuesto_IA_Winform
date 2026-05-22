using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    public partial class MainForm
    {
        private void treeList1_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            e.Allow = false;
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            UpdateMoveActionsState(e.Node);
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            TreeListHitInfo hitInfo = treeList1.CalcHitInfo(e.Location);

            if (treeList1.Nodes.Count == 0 && (hitInfo.Node == null || hitInfo.HitInfoType != HitInfoType.Cell))
            {
                emptyContextMenu.Show(treeList1, e.Location);
                return;
            }

            if (treeList1.Nodes.Count > 0 && hitInfo.HitInfoType == HitInfoType.Cell && hitInfo.Node != null)
            {
                treeList1.FocusedNode = hitInfo.Node;
                treeList1.FocusedColumn = hitInfo.Column;
                UpdateContextMenuVisibility(hitInfo.Node);
                treeContextMenu.Show(treeList1, e.Location);
                return;
            }

            UpdateMoveActionsState(null);
        }

        private void menuAddRootItem_Click(object sender, EventArgs e)
        {
            treeListItemService.AddRootItem();
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuAddAbove_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            treeListItemService.AddAbove(treeList1.FocusedNode);
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuAddBelow_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            treeListItemService.AddBelow(treeList1.FocusedNode);
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuAddSubItem_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: true);
            if (!treeListItemService.AddSubItem(treeList1.FocusedNode, out string validationMessage))
            {
                if (!string.IsNullOrEmpty(validationMessage))
                    MessageBox.Show(validationMessage, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuDeleteItem_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            treeListItemService.Delete(treeList1.FocusedNode);
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuCopyItem_Click(object sender, EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            clipboardData = treeListItemService.CaptureNode(selectedNode);
        }

        private void menuCutItem_Click(object sender, EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            ShowSharedPartidaWarningIfNeeded(selectedNode, allowPartidaRootTarget: false);
            treeListItemService.Cut(selectedNode, data => clipboardData = data);
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuPasteItem_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            if (!treeListItemService.Paste(treeList1.FocusedNode, clipboardData, false, out string validationMessage))
            {
                if (!string.IsNullOrEmpty(validationMessage))
                    MessageBox.Show(validationMessage, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuPasteItemBelow_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            if (!treeListItemService.Paste(treeList1.FocusedNode, clipboardData, true, out string validationMessage))
            {
                if (!string.IsNullOrEmpty(validationMessage))
                    MessageBox.Show(validationMessage, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuMoveRight_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveRight(treeList1.FocusedNode))
            {
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuMoveLeft_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveLeft(treeList1.FocusedNode))
            {
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuMoveUp_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveUp(treeList1.FocusedNode))
            {
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuMoveDown_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveDown(treeList1.FocusedNode))
            {
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuModifyResource_Click(object sender, EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            EditResourceWithForm(selectedNode, true, true);
        }

        private void btnCreateResource_Click(object sender, EventArgs e)
        {
            List<TipoRecursoDto> resourceTypes;
            List<RecursoDto> resources;
            List<UnidadDto> units;
            List<TipoCalculoDto> calculationTypes;
            try
            {
                resourceTypes = Datos.ObtenerTiposRecurso(EmpresaId, true);
                resources = Datos.ObtenerRecursos(EmpresaId);
                units = Datos.ObtenerUnidades();
                calculationTypes = Datos.ObtenerTiposCalculo(EmpresaId);
            }
            catch (System.Data.DataException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var form = new ResourceEditForm(
                resourceTypes,
                resources,
                units,
                calculationTypes,
                null,
                null,
                null,
                null,
                string.Empty,
                null,
                null,
                true,
                true,
                false))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    int createdResourceId = Datos.CrearRecurso(
                        EmpresaId,
                        form.SelectedTipoRecursoId.Value,
                        form.RecursoTexto,
                        form.SelectedUnidadId,
                        form.SelectedTipoCalculoId,
                        form.RendimientoManoObra,
                        form.RendimientoEquipos);

                    resourceNamesById[createdResourceId] = form.RecursoTexto;
                    resourceUnitIdsByResourceId[createdResourceId] = form.SelectedUnidadId;
                    resourceCalculationTypeIdsByResourceId[createdResourceId] = form.SelectedTipoCalculoId;
                    ConfigureColumnEditors();
                    MessageBox.Show("Recurso creado correctamente.", "Crear recurso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (System.Data.DataException ex)
                {
                    MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRecalculateAll_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigureColumnEditors();
                var catalogResources = Datos.ObtenerRecursos(EmpresaId);
                RefreshPartidaCalculationDataFromCatalog(catalogResources);
                RecalculateNumericRules();
                MarkPendingAutoSave();
                treeList1.ExpandAll();
                UpdateMoveActionsState(treeList1.FocusedNode);
            }
            catch (System.Data.DataException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPropiedades_Click(object sender, EventArgs e)
        {
            using (var form = new Propiedades(
                GrabacionAutomatica,
                InicioNumeracion,
                ColumnaTipoCalculoVisible,
                ColumnaRendimientoVisible,
                ColumnaHorasJornalVisible,
                ColumnaCuadrillaVisible))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    GrabacionAutomatica = form.GrabacionAutomatica;
                    InicioNumeracion = form.InicioNumeracion;
                    ColumnaTipoCalculoVisible = form.ColumnaTipoCalculoVisible;
                    ColumnaRendimientoVisible = form.ColumnaRendimientoVisible;
                    ColumnaHorasJornalVisible = form.ColumnaHorasJornalVisible;
                    ColumnaCuadrillaVisible = form.ColumnaCuadrillaVisible;
                }
            }
        }

        private void EditResourceWithForm(TreeListNode selectedNode, bool replicateToSameResource, bool preloadFromNode)
        {
            if (selectedNode == null)
                return;

            List<TipoRecursoDto> resourceTypes;
            List<RecursoDto> resources;
            List<UnidadDto> units;
            List<TipoCalculoDto> calculationTypes;
            RecursoDto catalogResource;
            try
            {
                resourceTypes = Datos.ObtenerTiposRecurso(EmpresaId, true);
                resources = Datos.ObtenerRecursos(EmpresaId);
                units = Datos.ObtenerUnidades();
                calculationTypes = Datos.ObtenerTiposCalculo(EmpresaId);
                catalogResource = ResolveCatalogResource(selectedNode, resources);
            }
            catch (System.Data.DataException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var form = new ResourceEditForm(
                resourceTypes,
                resources,
                units,
                calculationTypes,
                preloadFromNode && catalogResource != null ? (int?)catalogResource.TipoRecursoId : null,
                preloadFromNode && catalogResource != null ? (int?)catalogResource.RecursoId : null,
                preloadFromNode && catalogResource != null ? catalogResource.UnidadId : null,
                preloadFromNode && catalogResource != null ? catalogResource.TipoCalculoId : null,
                preloadFromNode ? ToStringOrEmpty(selectedNode.GetValue(AliasColumnIndex)) : string.Empty,
                preloadFromNode && catalogResource != null ? catalogResource.Rendimiento : null,
                preloadFromNode && catalogResource != null ? catalogResource.Cuadrilla : null,
                !preloadFromNode,
                false,
                preloadFromNode))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                int? originalResourceId = ToNullableInt(selectedNode.GetValue(ResourceColumnIndex));
                int? resourceIdOverride = null;
                bool shouldReplicateToSameResource = replicateToSameResource;

                if (form.CreateNewResourceRequested)
                {
                    try
                    {
                        int createdResourceId = Datos.CrearRecurso(
                            EmpresaId,
                            form.SelectedTipoRecursoId.Value,
                            form.RecursoTexto,
                            form.SelectedUnidadId,
                            form.SelectedTipoCalculoId,
                            form.RendimientoManoObra,
                            form.RendimientoEquipos);

                        resourceNamesById[createdResourceId] = form.RecursoTexto;
                        resourceUnitIdsByResourceId[createdResourceId] = form.SelectedUnidadId;
                        ConfigureColumnEditors();
                        resourceIdOverride = createdResourceId;
                        shouldReplicateToSameResource = false;
                    }
                    catch (System.Data.DataException ex)
                    {
                        MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (originalResourceId.HasValue && form.SelectedTipoRecursoId.HasValue)
                {
                    try
                    {
                        Datos.ActualizarRecurso(
                            EmpresaId,
                            originalResourceId.Value,
                            form.SelectedTipoRecursoId.Value,
                            form.RecursoTexto,
                            form.SelectedUnidadId,
                            form.SelectedTipoCalculoId,
                            form.RendimientoManoObra,
                            form.RendimientoEquipos);

                        resourceIdOverride = originalResourceId.Value;
                        resourceNamesById[originalResourceId.Value] = form.RecursoTexto;
                        resourceUnitIdsByResourceId[originalResourceId.Value] = form.SelectedUnidadId;
                        resourceCalculationTypeIdsByResourceId[originalResourceId.Value] = form.SelectedTipoCalculoId;
                        ConfigureColumnEditors();
                    }
                    catch (System.Data.DataException ex)
                    {
                        MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                suppressPersistence = true;
                try
                {
                    ApplyResourceEditsToNode(selectedNode, form, true, resourceIdOverride);

                    if (shouldReplicateToSameResource && originalResourceId.HasValue)
                    {
                        foreach (TreeListNode node in EnumerateAllNodes())
                        {
                            if (node == selectedNode)
                                continue;

                            int? nodeResourceId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                            if (!nodeResourceId.HasValue || nodeResourceId.Value != originalResourceId.Value)
                                continue;

                            ApplyResourceEditsToNode(node, form, false, null);
                        }
                    }
                }
                finally
                {
                    suppressPersistence = false;
                }

                RecalculateNumericRules();
                MarkPendingAutoSave();
            }
        }

        private static RecursoDto ResolveCatalogResource(TreeListNode node, List<RecursoDto> resources)
        {
            int? resourceId = node == null ? (int?)null : ToNullableInt(node.GetValue(ResourceColumnIndex));
            if (!resourceId.HasValue || resources == null)
                return null;

            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i].RecursoId == resourceId.Value)
                    return resources[i];
            }

            return null;
        }

        private void ApplyResourceEditsToNode(TreeListNode node, ResourceEditForm form, bool includeAlias, int? resourceIdOverride)
        {
            node.SetValue(ResourceTypeColumnIndex, form.SelectedTipoRecursoId.HasValue ? (object)form.SelectedTipoRecursoId.Value : null);
            node.SetValue(ResourceColumnIndex,
                resourceIdOverride.HasValue
                    ? (object)resourceIdOverride.Value
                    : (form.SelectedRecursoId.HasValue ? (object)form.SelectedRecursoId.Value : null));
            node.SetValue(UnitColumnIndex, form.SelectedUnidadId.HasValue ? (object)form.SelectedUnidadId.Value : null);
            node.SetValue(CalculationTypeColumnIndex, form.SelectedTipoCalculoId.HasValue ? (object)form.SelectedTipoCalculoId.Value : null);
            if (includeAlias)
                node.SetValue(AliasColumnIndex, form.Alias ?? string.Empty);

            if (resourceTypePolicy.IsPartida(node))
            {
                SetPartidaCalculationData(node, form.RendimientoManoObra, form.RendimientoEquipos);
            }
            else
            {
                node.SetValue(PerformanceColumnIndex, form.RendimientoManoObra.HasValue ? (object)form.RendimientoManoObra.Value : null);
                node.SetValue(CrewColumnIndex, form.RendimientoEquipos.HasValue ? (object)form.RendimientoEquipos.Value : null);
            }
        }

        private IEnumerable<TreeListNode> EnumerateAllNodes()
        {
            for (int i = 0; i < treeList1.Nodes.Count; i++)
            {
                foreach (TreeListNode node in EnumerateNodeRecursive(treeList1.Nodes[i]))
                    yield return node;
            }
        }

        private IEnumerable<TreeListNode> EnumerateNodeRecursive(TreeListNode node)
        {
            if (node == null)
                yield break;

            yield return node;

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                foreach (TreeListNode child in EnumerateNodeRecursive(node.Nodes[i]))
                    yield return child;
            }
        }

        private void UpdateContextMenuVisibility(TreeListNode targetNode)
        {
            if (targetNode == null)
            {
                UpdateMoveActionsState(null);
                return;
            }

            string selectedType = resourceTypePolicy.NormalizeTypeName(targetNode.GetValue(ResourceTypeValueIndex));
            TreeListNode targetParent = targetNode.ParentNode;
            TreeListNode newParentOnIndent = GetPreviousSibling(targetNode);
            TreeListNode newParentOnOutdent = targetParent == null ? null : targetParent.ParentNode;
            TreeListNode[] nodes = new[] { targetNode };

            bool canPasteHere = clipboardData != null
                && resourceTypePolicy.HasAssignedResourceType(targetNode)
                && resourceTypePolicy.CanBePlacedUnder(targetParent, resourceTypePolicy.NormalizeTypeName(clipboardData.Values[ResourceTypeValueIndex]));
            menuPasteItem.Visible = canPasteHere;
            menuPasteItemBelow.Visible = canPasteHere;
            menuAddSubItem.Visible = resourceTypePolicy.CanParentAcceptChildren(targetNode) && resourceTypePolicy.HasAssignedResourceType(targetNode);

            menuMoveRight.Visible = treeList1.CanIndentNodes(nodes) && resourceTypePolicy.CanBePlacedUnder(newParentOnIndent, selectedType);
            menuMoveLeft.Visible = treeList1.CanOutdentNodes(nodes) && resourceTypePolicy.CanBePlacedUnder(newParentOnOutdent, selectedType);

            UpdateMoveActionsState(targetNode);
        }

        private void UpdateMoveActionsState(TreeListNode targetNode)
        {
            bool canMoveUp = false;
            bool canMoveDown = false;
            bool canMoveRight = false;
            bool canMoveLeft = false;

            if (targetNode != null)
            {
                TreeListNode parent = targetNode.ParentNode;
                int currentIndex = treeList1.GetNodeIndex(targetNode);
                int siblingCount = parent == null ? treeList1.Nodes.Count : parent.Nodes.Count;

                canMoveUp = currentIndex > 0;
                canMoveDown = currentIndex >= 0 && currentIndex < siblingCount - 1;

                string selectedType = resourceTypePolicy.NormalizeTypeName(targetNode.GetValue(ResourceTypeValueIndex));
                TreeListNode[] nodes = new[] { targetNode };
                TreeListNode newParentOnIndent = GetPreviousSibling(targetNode);
                TreeListNode newParentOnOutdent = parent == null ? null : parent.ParentNode;

                canMoveRight = newParentOnIndent != null
                    && treeList1.CanIndentNodes(nodes)
                    && resourceTypePolicy.CanBePlacedUnder(newParentOnIndent, selectedType);

                canMoveLeft = parent != null
                    && resourceTypePolicy.CanBePlacedUnder(newParentOnOutdent, selectedType);
            }

            if (btnMoveUp != null)
                btnMoveUp.Enabled = canMoveUp;
            if (btnMoveDown != null)
                btnMoveDown.Enabled = canMoveDown;
            if (btnMoveRight != null)
                btnMoveRight.Enabled = canMoveRight;
            if (btnMoveLeft != null)
                btnMoveLeft.Enabled = canMoveLeft;
        }

        private TreeListNode GetPreviousSibling(TreeListNode node)
        {
            if (node == null)
                return null;

            TreeListNode parent = node.ParentNode;
            int index = treeList1.GetNodeIndex(node);
            if (index <= 0)
                return null;

            return parent == null ? treeList1.Nodes[index - 1] : parent.Nodes[index - 1];
        }

        private void ShowSharedPartidaWarningIfNeeded(TreeListNode targetNode, bool allowPartidaRootTarget)
        {
            if (targetNode == null)
                return;

            TreeListNode partidaNode = FindContainingPartidaOrSelf(targetNode);
            if (partidaNode == null || !resourceTypePolicy.IsPartida(partidaNode))
                return;

            if (!allowPartidaRootTarget && targetNode == partidaNode)
                return;

            int? partidaId = ToNullableInt(partidaNode.GetValue(ResourceColumnIndex));
            if (!partidaId.HasValue)
                return;

            int usageCount = CountPartidaUsagesInBudget(partidaId.Value);
            if (usageCount <= 1)
                return;

            if (!Datos.ExisteRecursosPartida(EmpresaId, partidaId.Value))
                return;

            MessageBox.Show(
                "Cualquier cambio que Usted realice sobre esta partida se replicará en todos los lugares de este presupuesto donde se utilice esta misma partida y en el maestro de Partidas, pero si solo desea cambiar esta partida, primero cambie de partida con una duplicada creada con otro nombre.",
                "Advertencia",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private int CountPartidaUsagesInBudget(int partidaId)
        {
            int count = 0;
            foreach (TreeListNode node in EnumerateAllNodes())
            {
                if (!resourceTypePolicy.IsPartida(node))
                    continue;

                int? nodePartidaId = ToNullableInt(node.GetValue(ResourceColumnIndex));
                if (nodePartidaId.HasValue && nodePartidaId.Value == partidaId)
                    count++;
            }

            return count;
        }
    }
}
