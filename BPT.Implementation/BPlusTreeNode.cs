using System;
using System.Collections.Generic;
using System.Text;

namespace BPT.Implementation
{
    public enum BplusTreeNodeType
    {
        InternalNode,
        LeafNode
    }
    public abstract class BPlusTreeNode
    {
        public Object[] keys;
        public int keyCount;
        public BPlusTreeNode parentNode;
        public BPlusTreeNode leftSibling;
        public BPlusTreeNode rightSibling;

        public BPlusTreeNode() 
        {
		    this.keyCount = 0;
		    this.parentNode = null;
		    this.leftSibling = null;
		    this.rightSibling = null;
	    }
        public int getKeyCount()
        {
            return this.keyCount;
        }
   
        public String getKey(int index)
        {
            return (String)this.keys[index];
        }

        public void setKey(int index, String key)
        {
            this.keys[index] = key;
        }

        public BPlusTreeNode getParent()
        {
            return this.parentNode;
        }

        public void setParent(BPlusTreeNode parent)
        {
            this.parentNode = parent;
        }

        public abstract BplusTreeNodeType getNodeType();

        /// <summary>
        /// Search a key on the current node. If the key is found then return its position, else return -1 for a leaf node. 
        /// Return the child node index which should contain the key for a internal node.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract int search(string key);

        /// <summary>
        /// Checks the Overflow condition.
        /// </summary>
        /// <returns></returns>
        public bool doesNodeOverflow()
        {
            return this.getKeyCount() == this.keys.Length;
        }

        /// <summary>
        /// Handles the overflow
        /// </summary>
        /// <returns></returns>
        public BPlusTreeNode handleOverflow()
        {
            int middleIndex = this.getKeyCount() / 2;
            String pushUpKey = this.getKey(middleIndex);

            BPlusTreeNode newNode = this.splitNode();

            if (this.getParent() == null)
            {
                this.setParent(new BPlusTreeInternalNode());
            }
            newNode.setParent(this.getParent());

            // maintain links of sibling nodes
            newNode.setLeftSibling(this);
            newNode.setRightSibling(this.rightSibling);
            if (this.getRightSibling() != null)
                this.getRightSibling().setLeftSibling(newNode);
            this.setRightSibling(newNode);

            // push up a key to parent internal node
            return this.getParent().pushKeyToParent(pushUpKey, this, newNode);
        }

        protected abstract BPlusTreeNode splitNode();

        protected abstract BPlusTreeNode pushKeyToParent(String key, BPlusTreeNode leftChild, BPlusTreeNode rightNode);

        /// <summary>
        /// Checks the Underflow condition for deletion operation. This condition is different than the normal B + tree.
        /// The underflow occurs when there are zero keys in the node.
        /// </summary>
        /// <returns></returns>
        public bool doesNodeUnderflow()
        {
            return this.getKeyCount() == 0;
        }

        /// <summary>
        /// Checks whether the sibling node can lend a key in case of underflow. The condition is different than normal B+ tree. 
        /// The sibling node can lend a key if it has more than one key.
        /// </summary>
        /// <returns></returns>
        public bool checkIfLendAKey()
        {
            return this.getKeyCount() > 1;
        }

        public BPlusTreeNode getLeftSibling()
        {
            if (this.leftSibling != null && this.leftSibling.getParent() == this.getParent())
                return this.leftSibling;
            return null;
        }

        public void setLeftSibling(BPlusTreeNode sibling)
        {
            this.leftSibling = sibling;
        }

        public BPlusTreeNode getRightSibling()
        {
            if (this.rightSibling != null && this.rightSibling.getParent() == this.getParent())
                return this.rightSibling;
            return null;
        }

        public void setRightSibling(BPlusTreeNode silbling)
        {
            this.rightSibling = silbling;
        }

        /// <summary>
        /// Gets the right leaf node key by the traversing using the last pointer (11th pointer) of the leaf node.
        /// Used for listing all the leaf node keys by the List() function.
        /// </summary>
        /// <returns></returns>
        public BPlusTreeNode getNextRightLeafNode()
        {
            if (this.rightSibling != null)
                return this.rightSibling;
            return null;
        }

        /// <summary>
        /// Handles th Underflow condition for the deletion operation. Borrows a key from left sibling or right sibling 
        /// if they can lend a key. 
        /// </summary>
        /// <returns></returns>
        public BPlusTreeNode handleUnderflow()
        {
            if (this.getParent() == null)
                return null;

            BPlusTreeNode leftSibling = this.getLeftSibling();
            if (leftSibling != null && leftSibling.checkIfLendAKey())
            {
                this.getParent().performChildrenTransfer(this, leftSibling, leftSibling.getKeyCount() - 1);
                return null;
            }

            BPlusTreeNode rightSibling = this.getRightSibling();
            if (rightSibling != null && rightSibling.checkIfLendAKey())
            {
                this.getParent().performChildrenTransfer(this, rightSibling, 0);
                return null;
            }

            // Can not borrow a key from any sibling, then do fusion with sibling
            if (leftSibling != null)
            {
                return this.getParent().performChildrenMerge(leftSibling, this);
            }
            else
            {
                return this.getParent().performChildrenMerge(this, rightSibling);
            }
        }
        public abstract void mergeWithSibling(String dropKey, BPlusTreeNode rightSibling);

        protected abstract void performChildrenTransfer(BPlusTreeNode borrower, BPlusTreeNode lender, int borrowIndex);

        public abstract String transferFromSibling(String dropKey, BPlusTreeNode sibling, int borrowIndex);

        protected abstract BPlusTreeNode performChildrenMerge(BPlusTreeNode leftChild, BPlusTreeNode rightChild);


    }
}
