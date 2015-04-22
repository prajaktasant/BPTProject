using System;
using System.Collections.Generic;
using System.Text;

namespace BPT.Implementation
{
    public class BPlusTreeLeafNode : BPlusTreeNode
    {
        public static int LEAFNODEORDER = 2;
        public Object[] values;

        public BPlusTreeLeafNode()
        {
            this.keys = new Object[LEAFNODEORDER + 1];
            this.values = new Object[LEAFNODEORDER + 1];
        }

        public String getValue(int index)
        {
            return (String)this.values[index];
        }

        public void setValue(int index, String value)
        {
            this.values[index] = value;
        }
        public override BplusTreeNodeType getNodeType()
        {
            return BplusTreeNodeType.LeafNode;
        }

        /// <summary>
        /// Search for a key in the leaf nodes.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override int search(String key)
        {
            for (int i = 0; i < this.getKeyCount(); ++i)
            {
                int cmp = String.Compare(this.getKey(i), key, StringComparison.OrdinalIgnoreCase);
                if (cmp == 0)
                {
                    return i;
                }
                else if (cmp > 0)
                {
                    return -1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the index to insert the key and value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void insert(String key, String value)
        {
            int index = 0;
            while (index < this.getKeyCount() && String.Compare(this.getKey(index
                ), key, StringComparison.OrdinalIgnoreCase) < 0)
                ++index;
            this.insertAt(index, key, value);
        }

        /// <summary>
        /// Moves space for new key if required and inserts the key and value at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void insertAt(int index, String key, String value)
        {
            for (int i = this.getKeyCount() - 1; i >= index; --i)
            {
                this.setKey(i + 1, this.getKey(i));
                this.setValue(i + 1, this.getValue(i));
            }
            this.setKey(index, key);
            this.setValue(index, value);
            ++this.keyCount;
        }

        /// <summary>
        /// Splits the leaf node, the new key is pushed to pasent and also kept on the leaf node.
        /// </summary>
        /// <returns></returns>
        protected override BPlusTreeNode splitNode()
        {
            int midIndex = this.getKeyCount() / 2;

            BPlusTreeLeafNode newRNode = new BPlusTreeLeafNode();
            for (int i = midIndex; i < this.getKeyCount(); ++i)
            {
                newRNode.setKey(i - midIndex, this.getKey(i));
                newRNode.setValue(i - midIndex, this.getValue(i));
                this.setKey(i, null);
                this.setValue(i, null);
            }
            newRNode.keyCount = this.getKeyCount() - midIndex;
            this.keyCount = midIndex;

            return newRNode;
        }

        protected override BPlusTreeNode pushKeyToParent(String key, BPlusTreeNode leftChild, BPlusTreeNode rightNode)
        {
            throw new NotSupportedException("pushKeyToParent function is implemented only for Internal nodes");
        }

        /// <summary>
        /// Upates the confidential information with the specified value for the corressponding key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool update(String key, String value)
        {
            int index = this.search(key);
            if (index == -1)
            {
                return false;
            }
            this.setValue(index, value);
            return true;
        }

        /// <summary>
        /// Lists all the leaf node keys by traversing through next Right leaf nodes starting from the leftmost leaf node. 
        /// </summary>
        /// <returns></returns>
        public List<string> list()
        {
            List<string> keylist = new List<string>();
            BPlusTreeLeafNode nextnode = this;
            while (nextnode!= null)
            {
                for (int i = 0; i < nextnode.getKeyCount(); i++)
                {
                    keylist.Add(nextnode.getKey(i));
                }
                nextnode = (BPlusTreeLeafNode)nextnode.getNextRightLeafNode();
            }
            return keylist;
        }

        /// <summary>
        /// Finds the corresponding index for the key to delete
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool delete(String key)
        {
            int index = this.search(key);
            if (index == -1)
                return false;

            this.deleteAt(index);
            return true;
        }
        /// <summary>
        /// Deletes the key and the value at the index
        /// </summary>
        /// <param name="index"></param>
        private void deleteAt(int index)
        {
            int i = index;
            for (i = index; i < this.getKeyCount() - 1; ++i)
            {
                this.setKey(i, this.getKey(i + 1));
                this.setValue(i, this.getValue(i + 1));
            }
            this.setKey(i, null);
            this.setValue(i, null);
            --this.keyCount;
        }

        protected override void performChildrenTransfer(BPlusTreeNode borrower, BPlusTreeNode lender, int borrowIndex)
        {
            throw new NotSupportedException();
        }

        protected override BPlusTreeNode performChildrenMerge(BPlusTreeNode leftChild, BPlusTreeNode rightChild)
        {
            throw new NotSupportedException();
        }

        public override void mergeWithSibling(String sinkKey, BPlusTreeNode rightSibling)
        {
            BPlusTreeLeafNode siblingLeaf = (BPlusTreeLeafNode)rightSibling;

            int j = this.getKeyCount();
            for (int i = 0; i < siblingLeaf.getKeyCount(); ++i)
            {
                this.setKey(j + i, siblingLeaf.getKey(i));
                this.setValue(j + i, siblingLeaf.getValue(i));
            }
            this.keyCount += siblingLeaf.getKeyCount();

            this.setRightSibling(siblingLeaf.rightSibling);
            if (siblingLeaf.rightSibling != null)
                siblingLeaf.rightSibling.setLeftSibling(this);
        }


        public override String transferFromSibling(String dropKey, BPlusTreeNode sibling, int borrowIndex)
        {
            BPlusTreeLeafNode siblingNode = (BPlusTreeLeafNode)sibling;

            this.insert(siblingNode.getKey(borrowIndex), siblingNode.getValue(borrowIndex));
            siblingNode.deleteAt(borrowIndex);

            return borrowIndex == 0 ? sibling.getKey(0) : this.getKey(0);
        }

    }
      
}
