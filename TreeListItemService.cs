using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;

namespace DevExpressTreeListDemo
{
    internal sealed class TreeListItemService
    {
        private const int ValueIndexItemCode = 0;
        private const int ValueIndexResourceType = 1;
        private const int ValueIndexResource = 2;
        private const int ValueIndexUnidad = 3;
        private const int ValueIndexCalculationType = 4;
        private const int ValueIndexHoursPerDay = 5;
        private const int ValueIndexPerformance = 6;
        private const int ValueIndexCrew = 7;
        private const int ValueIndexQuantity = 8;
        private const int ValueIndexUnitValue = 9;
        private const int ValueIndexTotalValue = 10;
        private const int ValueIndexAlias = 11;

        private static readonly object[] NewNodeDefaults =
            { string.Empty, null, null, null, null, null, null, null, null, null, null, string.Empty };

        private readonly TreeList treeList;
        private ResourceTypePolicy policy;
        private int itemNumberStart = 1;

        public TreeListItemService(TreeList treeList, ResourceTypePolicy policy)
        {
            this.treeList = treeList;
            this.policy = policy;
        }

        public void SetPolicy(ResourceTypePolicy policy)
        {
            this.policy = policy;
        }

        public int ItemNumberStart
        {
            get => itemNumberStart;
            set => itemNumberStart = value < 0 ? 0 : value;
        }

        public void AddRootItem()
        {
            RunUnbound(() => treeList.AppendNode(CreateNewNodeValues(), null));
        }

        public void AddAbove(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return;

            TreeListNode parent = selectedNode.ParentNode;
            if (!policy.CanParentAcceptChildren(parent))
                return;

            RunUnbound(() =>
            {
                int selIndex = treeList.GetNodeIndex(selectedNode);
                TreeListNode newNode = treeList.AppendNode(CreateNewNodeValues(), parent);
                treeList.SetNodeIndex(newNode, selIndex);
                treeList.FocusedNode = newNode;
            });
        }

        public void AddBelow(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return;

            TreeListNode parent = selectedNode.ParentNode;
            if (!policy.CanParentAcceptChildren(parent))
                return;

            RunUnbound(() =>
            {
                int selIndex = treeList.GetNodeIndex(selectedNode);
                TreeListNode newNode = treeList.AppendNode(CreateNewNodeValues(), parent);
                treeList.SetNodeIndex(newNode, selIndex + 1);
                treeList.FocusedNode = newNode;
            });
        }

        public bool AddSubItem(TreeListNode selectedNode, out string validationMessage)
        {
            validationMessage = null;

            if (selectedNode == null)
                return false;

            if (!policy.CanParentAcceptChildren(selectedNode))
                return false;

            if (!policy.HasAssignedResourceType(selectedNode))
            {
                validationMessage = "El nodo padre debe tener asignado un Tipo Recurso antes de agregar un subitem.";
                return false;
            }

            RunUnbound(() =>
            {
                TreeListNode newNode = treeList.AppendNode(CreateNewNodeValues(), selectedNode);
                selectedNode.Expanded = true;
                treeList.FocusedNode = newNode;
            });

            return true;
        }

        public void Delete(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return;

            RunUnbound(() => selectedNode.Remove());
        }

        public NodeClipboardData CaptureNode(TreeListNode node)
        {
            NodeClipboardData data = new NodeClipboardData
            {
                Values = new object[]
                {
                    node.GetValue(ValueIndexItemCode),
                    node.GetValue(ValueIndexResourceType),
                    node.GetValue(ValueIndexResource),
                    node.GetValue(ValueIndexUnidad),
                    node.GetValue(ValueIndexCalculationType),
                    node.GetValue(ValueIndexHoursPerDay),
                    node.GetValue(ValueIndexPerformance),
                    node.GetValue(ValueIndexCrew),
                    node.GetValue(ValueIndexQuantity),
                    node.GetValue(ValueIndexUnitValue),
                    node.GetValue(ValueIndexTotalValue),
                    node.GetValue(ValueIndexAlias)
                },
                Children = new System.Collections.Generic.List<NodeClipboardData>()
            };

            for (int i = 0; i < node.Nodes.Count; i++)
                data.Children.Add(CaptureNode(node.Nodes[i]));

            return data;
        }

        public void Cut(TreeListNode selectedNode, Action<NodeClipboardData> setClipboard)
        {
            if (selectedNode == null)
                return;

            setClipboard(CaptureNode(selectedNode));
            RunUnbound(() => selectedNode.Remove());
        }

        public bool Paste(TreeListNode targetNode, NodeClipboardData data, bool below, out string validationMessage)
        {
            validationMessage = null;

            if (data == null)
                return false;

            if (targetNode != null && !policy.HasAssignedResourceType(targetNode))
            {
                validationMessage = "El nodo destino debe tener asignado un Tipo Recurso antes de pegar una fila.";
                return false;
            }

            TreeListNode targetParent = targetNode == null ? null : targetNode.ParentNode;
            if (!policy.CanBePlacedUnder(targetParent, policy.NormalizeTypeName(data.Values[ValueIndexResourceType])))
                return false;

            RunUnbound(() =>
            {
                NodeClipboardData safeData = CloneClipboardData(data);
                TreeListNode newNode = AppendNodeFromClipboard(targetParent, safeData);

                if (targetNode != null)
                {
                    int targetIndex = treeList.GetNodeIndex(targetNode);
                    treeList.SetNodeIndex(newNode, below ? targetIndex + 1 : targetIndex);
                }

                treeList.FocusedNode = newNode;
                treeList.ExpandAll();
            });

            return true;
        }

        public bool MoveRight(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return false;

            string selectedType = policy.NormalizeTypeName(selectedNode.GetValue(ValueIndexResourceType));
            TreeListNode newParent = GetPreviousSibling(selectedNode);
            if (!policy.CanBePlacedUnder(newParent, selectedType))
                return false;

            TreeListNode[] nodes = new[] { selectedNode };
            if (!treeList.CanIndentNodes(nodes))
                return false;

            RunUnbound(() =>
            {
                TreeListNode movedNode = MoveSubtree(selectedNode, newParent, null);
                newParent.Expanded = true;
                treeList.FocusedNode = movedNode;
            });

            return true;
        }

        public bool MoveLeft(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return false;

            string selectedType = policy.NormalizeTypeName(selectedNode.GetValue(ValueIndexResourceType));
            TreeListNode currentParent = selectedNode.ParentNode;
            if (currentParent == null)
                return false;

            TreeListNode newParent = currentParent.ParentNode;
            if (!policy.CanBePlacedUnder(newParent, selectedType))
                return false;

            RunUnbound(() =>
            {
                int newIndex = treeList.GetNodeIndex(currentParent) + 1;
                TreeListNode movedNode = MoveSubtree(selectedNode, newParent, newIndex);
                movedNode.Expanded = true;
                treeList.FocusedNode = movedNode;
            });

            return true;
        }

        public bool MoveUp(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return false;

            int currentIndex = treeList.GetNodeIndex(selectedNode);
            if (currentIndex <= 0)
                return false;

            RunUnbound(() =>
            {
                treeList.SetNodeIndex(selectedNode, currentIndex - 1);
                treeList.FocusedNode = selectedNode;
            });

            return true;
        }

        public bool MoveDown(TreeListNode selectedNode)
        {
            if (selectedNode == null)
                return false;

            TreeListNode parent = selectedNode.ParentNode;
            int currentIndex = treeList.GetNodeIndex(selectedNode);
            int siblingCount = parent == null ? treeList.Nodes.Count : parent.Nodes.Count;
            if (currentIndex >= siblingCount - 1)
                return false;

            RunUnbound(() =>
            {
                treeList.SetNodeIndex(selectedNode, currentIndex + 1);
                treeList.FocusedNode = selectedNode;
            });

            return true;
        }

        public void AssignItemNumbers()
        {
            for (int i = 0; i < treeList.Nodes.Count; i++)
            {
                string rootCode = (itemNumberStart + i).ToString("D2");
                AssignNodeCode(treeList.Nodes[i], rootCode);
            }
        }

        private object[] CreateNewNodeValues()
        {
            return (object[])NewNodeDefaults.Clone();
        }

        private void AssignNodeCode(TreeListNode node, string code)
        {
            node.SetValue(ValueIndexItemCode, code);

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                string childCode = code + "." + (i + 1).ToString("D2");
                AssignNodeCode(node.Nodes[i], childCode);
            }
        }

        private TreeListNode GetPreviousSibling(TreeListNode node)
        {
            if (node == null)
                return null;

            TreeListNode parent = node.ParentNode;
            int index = treeList.GetNodeIndex(node);
            if (index <= 0)
                return null;

            return parent == null ? treeList.Nodes[index - 1] : parent.Nodes[index - 1];
        }

        private TreeListNode AppendNodeFromClipboard(TreeListNode parent, NodeClipboardData data)
        {
            TreeListNode node = treeList.AppendNode((object[])data.Values.Clone(), parent);

            for (int i = 0; i < data.Children.Count; i++)
                AppendNodeFromClipboard(node, data.Children[i]);

            return node;
        }

        private TreeListNode MoveSubtree(TreeListNode sourceNode, TreeListNode newParent, int? newIndex)
        {
            NodeClipboardData data = CaptureNode(sourceNode);
            sourceNode.Remove();

            TreeListNode movedNode = AppendNodeFromClipboard(newParent, data);
            if (newIndex.HasValue)
                treeList.SetNodeIndex(movedNode, newIndex.Value);

            return movedNode;
        }

        private NodeClipboardData CloneClipboardData(NodeClipboardData source)
        {
            if (source == null)
                return null;

            NodeClipboardData clone = new NodeClipboardData
            {
                Values = (object[])source.Values.Clone(),
                Children = new System.Collections.Generic.List<NodeClipboardData>()
            };

            for (int i = 0; i < source.Children.Count; i++)
                clone.Children.Add(CloneClipboardData(source.Children[i]));

            return clone;
        }

        private void RunUnbound(Action action)
        {
            treeList.BeginUnboundLoad();
            try
            {
                action();
                AssignItemNumbers();
            }
            finally
            {
                treeList.EndUnboundLoad();
            }
        }
    }
}