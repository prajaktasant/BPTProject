﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BPT.Implementation
{
    public class BPlusTree
    {
        private BPlusTreeNode rootnode;
        int record;

        public BPlusTree()
        {
            this.rootnode = new BPlusTreeLeafNode();
            record = 0;
        }

        /// <summary>
        /// Inserts a student name if not present in the B plus tree and should be less than 32 characters.
        /// Also, attaches 224 bytes of confidential information that initially contains 224 blank characters.
        /// </summary>
        /// <param name="key">Student name</param>
        /// <param name="val">Array of 224 Empty charaters</param>
        public void Insert(String key, char[] val)
        {
            if (key.Length <= 32)
            {
                BPlusTreeLeafNode leafnode = this.findTargetLeafNode(key);
                int index = leafnode.search(key);
                String value = new String(val);
                if (index == -1)
                {
                    leafnode.insert(key, value);
                    record++;
                    if (leafnode.doesNodeOverflow())
                    {
                        BPlusTreeNode n = leafnode.handleOverflow();
                        if (n != null)
                            this.rootnode = n;
                    }
                }

                else
                {
                    throw new Exception(key + " already present. Cannot insert same key");
                }
            }

            else
            {
                throw new Exception(key + " cannot be inserted. Key cannot have more than 32 characters");
            }

        }

        /// <summary>
        /// Given a key, searches and returns the corresponding value
        /// </summary>
        /// <param name="key">student name</param>
        /// <returns>Confidential Information if Present else empty string</returns>
        public DataBlock Search(String key)
        {

            BPlusTreeLeafNode leafnode = this.findTargetLeafNode(key);
            int index = leafnode.search(key);
            if (index == -1)
            {
                throw new Exception(key + " not found");
            }
            else
            {
                //return leafnode.getValue(index);
                return (DataBlock)leafnode.getValue(index);
            }
        }

        /// <summary>
        /// Deletes the specified student record. It waits until there are zero keys in the node and then performs 
        /// balancing of the tree.
        /// </summary>
        /// <param name="key"></param>
        public void Delete(String key)
        {
            BPlusTreeLeafNode leafnode = this.findTargetLeafNode(key);
            int index = leafnode.search(key);
            if (index != -1)
            {
                if (leafnode.delete(key))
                {
                    record--;
                    if (leafnode.isNodeEmpty())
                    {
                        BPlusTreeNode n = leafnode.balanceTreeAfterDelete();
                        if (n != null)
                            this.rootnode = n;
                    }
                }
            }
            else
            {
                throw new Exception(key + " not found");
            }
        }

        /// <summary>
        /// Updates the information(value) for the given key(Student name). The size of the information(value) cannot be more than
        /// 224 characters.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Update(String key, String value)
        {
            if (value.Length <= 224)
            {
                BPlusTreeLeafNode leafnode = this.findTargetLeafNode(key);
                if (!leafnode.update(key, value))
                    throw new Exception("Unable to Update value for key: " + key);
            }
            else
            {
                throw new Exception("Updatation value cannot have more than 224 characters");
            }
        }

        /// <summary>
        /// Lists all the student names(keys) using the last pointer(11th Pointer for this project) of the leaf nodes.
        /// </summary>
        /// <returns></returns>
        public List<string> List()
        {
            BPlusTreeLeafNode leafnode = this.findLeftMostLeafNode();
            return leafnode.list();
        }

        /// <summary>
        /// Used to give a summary of the tree structure which includes:
        /// 1. Total number of records in all the leaf nodes.
        /// 2. Number of Blocks occupied.
        /// 3. Depth of the tree.
        /// 4. First and last keys of all the internal B+ tree nodes(excludes the leaf nodes).
        /// </summary>
        /// <returns></returns>
        public List<string> Snapshot()
        {
            List<string> snapshotList = new List<string>();
            snapshotList.Add(getNumberOfRecords().ToString());
            snapshotList.Add(getNumberOfBlocks().ToString());
            snapshotList.Add(getDepth().ToString());
            return snapshotList;
        }

        /// <summary>
        /// Returns the depth of the tree starting from the root node. Root Node is considered at depth 1.
        /// </summary>
        /// <returns></returns>
        private int getDepth()
        {
            BPlusTreeNode node = this.rootnode;
            int count = 1; // Root node at depth 1
            while (node.getNodeType() == BplusTreeNodeType.InternalNode)
            {
                node = ((BPlusTreeInternalNode)node).getChild(0);
                count++;
            }
            return count;
        }

        /// <summary>
        /// Return the total number of records in the leaf nodes(table).
        /// </summary>
        /// <returns></returns>
        private int getNumberOfRecords()
        {
            return record;
        }

        /// <summary>
        /// Returns the number of blocks occupied.
        /// </summary>
        /// <returns></returns>
        private double getNumberOfBlocks()
        {
            BPlusTreeLeafNode leafnode = this.findLeftMostLeafNode();
            double blocks = leafnode.calculateNumberOfBlocks();
            return blocks;
        }

        /// <summary>
        /// Gets the left most leaf node. Used for listing all the leaf node records starting with the leftmost leaf node.
        /// </summary>
        /// <returns></returns>
        private BPlusTreeLeafNode findLeftMostLeafNode()
        {
            BPlusTreeNode node = this.rootnode;
            while (node.getNodeType() == BplusTreeNodeType.InternalNode)
            {
                node = ((BPlusTreeInternalNode)node).getChild(0);
            }
            return (BPlusTreeLeafNode)node;
        }

        /// <summary>
        /// Finds the Target leaf node where a key has to be inserted, deleted, Updated or Search.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private BPlusTreeLeafNode findTargetLeafNode(string key)
        {
            BPlusTreeNode node = this.rootnode;
            while (node.getNodeType() == BplusTreeNodeType.InternalNode)
            {
                node = ((BPlusTreeInternalNode)node).getChild(node.search(key));
            }
            return (BPlusTreeLeafNode)node;
        }

        /// <summary>
        /// Used to get the first and last keys of all internal B+ tree nodes. 
        /// Breadth First Search Algorithm is used for tree traversal.
        /// </summary>
        public List<string> BFSTreeTraversal()
        {
            if (!(this.rootnode.getNodeType() == BplusTreeNodeType.LeafNode))
            {
                Queue<BPlusTreeInternalNode> q = new Queue<BPlusTreeInternalNode>();
                List<string> firstAndLast = new List<string>();
                int j = 0;
                q.Enqueue((BPlusTreeInternalNode)this.rootnode);
                {
                    while (q.Count > 0)
                    {
                        BPlusTreeInternalNode n = q.Dequeue();
                        firstAndLast.Add("B plus tree Internal Node : " + j);
                        j++;
                        String first = n.getKey(0);
                        firstAndLast.Add("First Key: " + first);
                        String last = n.getKey(n.getKeyCount() - 1);
                        firstAndLast.Add("Last Key: " + last + "\n");
                        for (int i = 0; i < n.getKeyCount() + 1; i++)
                        {
                            if (n.getChild(i) != null && n.getChild(i).getNodeType() == BplusTreeNodeType.InternalNode)
                                q.Enqueue((BPlusTreeInternalNode)n.getChild(i));
                        }
                    }
                }
                return firstAndLast;
            }
            else
            {
                throw new Exception("There are no Internal B + Tree Nodes. Only Root Node in the tree");
            }
        }
    }
}

