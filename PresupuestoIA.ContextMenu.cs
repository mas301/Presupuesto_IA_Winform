using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PresupuestoIA
{
    public partial class PresupuestoIA
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
            ReplicateFocusedPartidaStructureIfNeeded();
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuAddBelow_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            treeListItemService.AddBelow(treeList1.FocusedNode);
            ReplicateFocusedPartidaStructureIfNeeded();
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

            ReplicateFocusedPartidaStructureIfNeeded();
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuDeleteItem_Click(object sender, EventArgs e)
        {
            ShowSharedPartidaWarningIfNeeded(treeList1.FocusedNode, allowPartidaRootTarget: false);
            TreeListNode focusedBefore = treeList1.FocusedNode;
            TreeListNode partidaBefore = focusedBefore != null && focusedBefore.ParentNode != null
                ? FindContainingPartidaOrSelf(focusedBefore.ParentNode)
                : null;
            treeListItemService.Delete(focusedBefore);
            ReplicatePartidaStructureToPeers(partidaBefore);
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
            TreeListNode partidaBefore = selectedNode.ParentNode != null
                ? FindContainingPartidaOrSelf(selectedNode.ParentNode)
                : null;
            treeListItemService.Cut(selectedNode, data => clipboardData = data);
            ReplicatePartidaStructureToPeers(partidaBefore);
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

            ReplicateFocusedPartidaStructureIfNeeded();
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

            ReplicateFocusedPartidaStructureIfNeeded();
            RecalculateNumericRules();
            MarkPendingAutoSave();
        }

        private void menuMoveRight_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveRight(treeList1.FocusedNode))
            {
                ReplicateFocusedPartidaStructureIfNeeded();
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuMoveLeft_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveLeft(treeList1.FocusedNode))
            {
                ReplicateFocusedPartidaStructureIfNeeded();
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuMoveUp_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveUp(treeList1.FocusedNode))
            {
                ReplicateFocusedPartidaStructureIfNeeded();
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void menuMoveDown_Click(object sender, EventArgs e)
        {
            if (treeListItemService.MoveDown(treeList1.FocusedNode))
            {
                ReplicateFocusedPartidaStructureIfNeeded();
                RecalculateNumericRules();
                MarkPendingAutoSave();
            }

            UpdateMoveActionsState(treeList1.FocusedNode);
        }

        private void ReplicateFocusedPartidaStructureIfNeeded()
        {
            TreeListNode partida = FindContainingPartidaOrSelf(treeList1.FocusedNode);
            if (partida == null)
                return;

            ReplicatePartidaStructureToPeers(partida);
        }

        private void menuModifyResource_Click(object sender, EventArgs e)
        {
            TreeListNode selectedNode = treeList1.FocusedNode;
            if (selectedNode == null)
                return;

            EditResourceWithForm(selectedNode, true, true);
        }

        private void ResourceEditorService_ResourceCreationRequested(object sender, ResourceCreationRequestedEventArgs e)
        {
            if (e == null || e.Node == null || string.IsNullOrWhiteSpace(e.TypedText))
                return;

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
                e.TipoRecursoId,
                null,
                null,
                null,
                string.Empty,
                null,
                null,
                null,
                null,
                false,
                false,
                true,
                false))
            {
                form.PreloadResourceName(e.TypedText);

                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                if (!form.SelectedTipoRecursoId.HasValue || string.IsNullOrWhiteSpace(form.RecursoTexto))
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
                        form.RendimientoEquipos,
                        form.DiasDuracion,
                        form.HorasJornal,
                        form.Independiente);

                    resourceNamesById[createdResourceId] = form.RecursoTexto;
                    resourceUnitIdsByResourceId[createdResourceId] = form.SelectedUnidadId;
                    resourceCalculationTypeIdsByResourceId[createdResourceId] = form.SelectedTipoCalculoId;
                    ConfigureColumnEditors();
                    e.CreatedResourceId = createdResourceId;
                }
                catch (System.Data.DataException ex)
                {
                    MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
                null,
                null,
                false,
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
                        form.RendimientoEquipos,
                        form.DiasDuracion,
                        form.HorasJornal,
                        form.Independiente);

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
                UpdateMoveActionsState(treeList1.FocusedNode);
            }
            catch (System.Data.DataException ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPropiedades_Click(object sender, EventArgs e)
        {
            using (var form = new Propiedades(this))
            {
                form.ShowDialog(this);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeList1 == null)
                    return;

                if (!treeList1.IsPrintingAvailable)
                {
                    MessageBox.Show(this, "La funcionalidad de impresión no está disponible.", "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                treeList1.ShowPrintPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.GetBaseException().Message, "Error al imprimir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeList1 == null)
                    return;

                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Exportar a Excel";
                    dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls";
                    dialog.FilterIndex = 1;
                    dialog.FileName = "Presupuesto_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    if (dialog.FilterIndex == 2)
                    {
                        var xlsOptions = new DevExpress.XtraPrinting.XlsExportOptionsEx
                        {
                            ExportType = DevExpress.Export.ExportType.WYSIWYG,
                            SheetName = "Presupuesto"
                        };
                        treeList1.ExportToXls(dialog.FileName, xlsOptions);
                    }
                    else
                    {
                        var xlsxOptions = new DevExpress.XtraPrinting.XlsxExportOptionsEx
                        {
                            ExportType = DevExpress.Export.ExportType.WYSIWYG,
                            SheetName = "Presupuesto"
                        };
                        treeList1.ExportToXlsx(dialog.FileName, xlsxOptions);
                    }

                    var result = MessageBox.Show(this,
                        "Exportación completada correctamente.\r\n¿Desea abrir el archivo?",
                        "Exportar",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.GetBaseException().Message, "Error al exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    preloadFromNode ? ToStringOrEmpty(selectedNode.GetValue(columnAlias)) : string.Empty,
                preloadFromNode && catalogResource != null ? catalogResource.Rendimiento : null,
                preloadFromNode && catalogResource != null ? catalogResource.RendimientoEquipos : null,
                preloadFromNode && catalogResource != null ? catalogResource.DiasDuracion : null,
                preloadFromNode && catalogResource != null ? catalogResource.HorasJornal : null,
                preloadFromNode && catalogResource != null ? catalogResource.Independiente : false,
                !preloadFromNode,
                false,
                preloadFromNode))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                int? originalResourceId = ToNullableInt(selectedNode.GetValue(columnRecurso));
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
                            form.RendimientoEquipos,
                            form.DiasDuracion,
                            form.HorasJornal,
                            form.Independiente);

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
                            form.RendimientoEquipos,
                            form.DiasDuracion,
                            form.HorasJornal,
                            form.Independiente);

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

                            int? nodeResourceId = ToNullableInt(node.GetValue(columnRecurso));
                            if (!nodeResourceId.HasValue || nodeResourceId.Value != originalResourceId.Value)
                                continue;

                            ApplyResourceEditsToNode(node, form, false, originalResourceId.Value);
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

        private RecursoDto ResolveCatalogResource(TreeListNode node, List<RecursoDto> resources)
        {
            int? resourceId = node == null ? (int?)null : ToNullableInt(node.GetValue(columnRecurso));
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
            node.SetValue(columnTipoRecurso, form.SelectedTipoRecursoId.HasValue ? (object)form.SelectedTipoRecursoId.Value : null);
            node.SetValue(columnRecurso,
                resourceIdOverride.HasValue
                    ? (object)resourceIdOverride.Value
                    : (form.SelectedRecursoId.HasValue ? (object)form.SelectedRecursoId.Value : null));
            node.SetValue(columnUnidad, form.SelectedUnidadId.HasValue ? (object)form.SelectedUnidadId.Value : null);
            node.SetValue(columnTipoCalculo, form.SelectedTipoCalculoId.HasValue ? (object)form.SelectedTipoCalculoId.Value : null);
            if (includeAlias)
                node.SetValue(columnAlias, form.Alias ?? string.Empty);

            if (resourceTypePolicy.IsPartida(node))
            {
                SetPartidaCalculationData(node, form.RendimientoManoObra, form.RendimientoEquipos);
            }
            else
            {
                node.SetValue(columnRendimiento, form.RendimientoManoObra.HasValue ? (object)form.RendimientoManoObra.Value : null);
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

            string selectedType = resourceTypePolicy.NormalizeTypeName(targetNode.GetValue(columnTipoRecurso));
            TreeListNode targetParent = targetNode.ParentNode;
            TreeListNode newParentOnIndent = GetPreviousSibling(targetNode);
            TreeListNode newParentOnOutdent = targetParent == null ? null : targetParent.ParentNode;
            TreeListNode[] nodes = new[] { targetNode };

            bool canPasteHere = clipboardData != null
                && resourceTypePolicy.HasAssignedResourceType(targetNode)
                && resourceTypePolicy.CanBePlacedUnder(targetParent, resourceTypePolicy.NormalizeTypeName(TreeListItemService.GetClipboardValue(clipboardData, ColumnNames.TipoRecurso)));
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

                string selectedType = resourceTypePolicy.NormalizeTypeName(targetNode.GetValue(columnTipoRecurso));
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

            int? partidaId = ToNullableInt(partidaNode.GetValue(columnRecurso));
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

                int? nodePartidaId = ToNullableInt(node.GetValue(columnRecurso));
                if (nodePartidaId.HasValue && nodePartidaId.Value == partidaId)
                    count++;
            }

            return count;
        }
    }
}
