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
        public Object[] keys;   //Student names
        public int keyCount;    // Number of student names in the tree node
        public BPlusTreeNode parentNode;    //Parent of the current node
        public BPlusTreeNode leftSibling;   //left sibling of the current node
        public BPlusTreeNode rightSibling;  //right sibling of the current node

        public BPlusTreeNode() 
        {
		    this.keyCount = 0;
		    this.parentNode = null;
		    this.leftSibling = null;
		    this.rightSibling = null;
	    }

        /// <summary>
        /// Returns the number of keys in a node.
        /// </summary>
        /// <returns></returns>
        public int getKeyCount()
        {
            return this.keyCount;
        }
   
        /// <summary>
        /// Returns the key at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String getKey(int index)
        {
            return (String)this.keys[index];
        }

        public void setKey(int index, String key)
        {
            this.keys[index] = key;
        }

        /// <summary>
        /// Returns the parent node of the current node.
        /// </summary>
        /// <returns></returns>
        public BPlusTreeNode getParent()
        {
            return this.parentNode;
        }

        /// <summary>
        /// Sets the specified node as the parent of the current node.
        /// </summary>
        /// <param name="parent"></param>
        public void setParent(BPlusTreeNode parent)
        {
            this.parentNode = parent;
        }

        /// <summary>
        /// There are two types of B+ tree nodes
        /// 1. Internal Node
        /// 2. Leaf Node
        /// </summary>
        /// <returns>The type of leaf node</returns>
        public abstract BplusTreeNodeType getNodeType();

        /// <summary>
        /// Search a key on the current node. If the key is found then return its position, else return -1 for a leaf node. 
        /// Return the child node index which should contain the key for a internal node.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract int search(string key);

        /// <summary>
        /// Checks the Overflow condition. Overflow condition occurs if the node is full. There is no space to enter a new key.
        /// </summary>
        /// <returns></returns>
        public bool doesNodeOverflow()
        {
            return this.getKeyCount() == this.keys.Length;
        }

        /// <summary>
        /// Handles if the Node is full and we try to insert an additional key in the node.
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

            // References to the Sibling nodes are maintained after splitting the node.
            newNode.setLeftSibling(this);
            newNode.setRightSibling(this.rightSibling);
            if (this.getRightSibling() != null)
                this.getRightSibling().setLeftSibling(newNode);
            this.setRightSibling(newNode);

            return this.getParent().pushKeyToParent(pushUpKey, this, newNode);
        }

        /// <summary>
        /// Splits the node in case of Overflow
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the left sibling if current node has a left sibling and their parent is same.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the right sibling node if current node has a right sibling and their parent is same.
        /// </summary>
        /// <returns></returns>
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
        /// Used for listing all the leaf node keys by the List() function. The parent need not be same in this case.
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

            // If can not borrow a key from any sibling, then merge with the sibling
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

        protected abstract void performChildrenTransfer(BPlusTreeNode borrowerNode, BPlusTreeNode lenderNode, int borrowIndex);

        public abstract String transferFromSibling(String dropKey, BPlusTreeNode siblingNode, int borrowIndex);

        protected abstract BPlusTreeNode performChildrenMerge(BPlusTreeNode leftChildNode, BPlusTreeNode rightChildNode);


    }
}
