using System;
using System.Collections.Generic;
using System.Text;

namespace BPT.Implementation
{
    public class BPlusTreeInternalNode: BPlusTreeNode
    {
        protected static int INTERNALORDER =2; //Maximum number of keys an Internal Node can contain.
	    protected Object[] children;    //Number of children an internal node has.

        public BPlusTreeInternalNode()
        {
            this.keys = new Object[INTERNALORDER + 1];
            this.children = new Object[INTERNALORDER + 2];
	    }

        /// <summary>
        /// Returns the child node of the current node at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
	    public BPlusTreeNode getChild(int index) {
		    return (BPlusTreeNode)this.children[index];
	    }

	    public void setChild(int index, BPlusTreeNode child) {
		    this.children[index] = child;
		    if (child != null)
			    child.setParent(this);
	    }
	    
	    public override BplusTreeNodeType getNodeType() {
		    return BplusTreeNodeType.InternalNode;
	    }

	    /// <summary>
	    /// Returns the index of the specified key.
	    /// </summary>
	    /// <param name="key"></param>
	    /// <returns></returns>
	    public override int search(String key) {
		    int index = 0;
		    for (index = 0; index < this.getKeyCount(); ++index) {
                int cmp = String.Compare(this.getKey(index), key, StringComparison.OrdinalIgnoreCase);
			    if (cmp == 0) {
				    return index + 1;
			    }
			    else if (cmp > 0) {
				    return index;
			    }
		    }
		
		    return index;
	    }

        /// <summary>
        /// Below code is used for Insertion Operation
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="leftChild"></param>
        /// <param name="rightChild"></param>
        private void insertAt(int index, String key, BPlusTreeNode leftChild, BPlusTreeNode rightChild)
        {
            // move space for the new key
            for (int i = this.getKeyCount() + 1; i > index; --i)
            {
                this.setChild(i, this.getChild(i - 1));
            }
            for (int i = this.getKeyCount(); i > index; --i)
            {
                this.setKey(i, this.getKey(i - 1));
            }

            // insert the new key
            this.setKey(index, key);
            this.setChild(index, leftChild);
            this.setChild(index + 1, rightChild);
            this.keyCount += 1;
        }

        /// <summary>
        /// Below code is used to split a internal node in case of Overflow, the middle key is pushed to parent node.
        /// </summary>
        /// <returns></returns>
	    protected override BPlusTreeNode splitNode() 
        {
		    int midIndex = this.getKeyCount() / 2;
		
		    BPlusTreeInternalNode newNode = new BPlusTreeInternalNode();
		    for (int i = midIndex + 1; i < this.getKeyCount(); ++i) {
			    newNode.setKey(i - midIndex - 1, this.getKey(i));
			    this.setKey(i, null);
		    }
		    for (int i = midIndex + 1; i <= this.getKeyCount(); ++i) {
			    newNode.setChild(i - midIndex - 1, this.getChild(i));
			    newNode.getChild(i - midIndex - 1).setParent(newNode);
			    this.setChild(i, null);
		    }
		    this.setKey(midIndex, null);
		    newNode.keyCount = this.getKeyCount() - midIndex - 1;
		    this.keyCount = midIndex;
		
		    return newNode;
	    }

        /// <summary>
        /// Finds the target position for the new key to insert and checks whether the current node needs to be split.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="leftChild"></param>
        /// <param name="rightNode"></param>
        /// <returns></returns>
        protected override BPlusTreeNode pushKeyToParent(String key, BPlusTreeNode leftChild, BPlusTreeNode rightNode)
        {
		    int index = this.search(key);		
		    this.insertAt(index, key, leftChild, rightNode);
		    if (this.doesNodeOverflow()) {
                return this.handleOverflow();
		    }
		    else {
			    return this.getParent() == null ? this : null;
		    }
	    }
	
	/// <summary>
    /// Deletes a key at a specified index and adjusts the node.
	/// </summary>
	/// <param name="index"></param>
	private void deleteAt(int index) {
		int i = 0;
		for (i = index; i < this.getKeyCount() - 1; ++i) {
			this.setKey(i, this.getKey(i + 1));
			this.setChild(i + 1, this.getChild(i + 2));
		}
		this.setKey(i, null);
		this.setChild(i + 1, null);
		--this.keyCount;
	}
	
    /// <summary>
    /// Borrows a key either from Right Sibling or Left Sibling.
    /// </summary>
    /// <param name="borrower"></param>
    /// <param name="lender"></param>
    /// <param name="borrowIndex"></param>
	protected override void performChildrenTransfer(BPlusTreeNode borrower, BPlusTreeNode lenderNode, int borrowIndex) 
    {
		int borrowerChildIndex = 0;
		while (borrowerChildIndex < this.getKeyCount() + 1 && this.getChild(borrowerChildIndex) != borrower)
			++borrowerChildIndex;
		
		if (borrowIndex == 0) {
			String pushUpKey = borrower.transferFromSibling(this.getKey(borrowerChildIndex), lenderNode, borrowIndex);
			this.setKey(borrowerChildIndex, pushUpKey);
		}
		else {
			String pushUpKey = borrower.transferFromSibling(this.getKey(borrowerChildIndex - 1), lenderNode, borrowIndex);
            this.setKey(borrowerChildIndex - 1, pushUpKey);
		}
	}
	
	protected override BPlusTreeNode performChildrenMerge(BPlusTreeNode leftChild, BPlusTreeNode rightChild)
    {
		int index = 0;
		while (index < this.getKeyCount() && this.getChild(index) != leftChild)
			++index;
		String dropKey = this.getKey(index);		
		leftChild.mergeWithSibling(dropKey, rightChild);		
		this.deleteAt(index);
				if (this.doesNodeUnderflow()) {
			if (this.getParent() == null) {
				if (this.getKeyCount() == 0) {
					leftChild.setParent(null);
					return leftChild;
				}
				else {
					return null;
				}
			}			
			return this.handleUnderflow();
		}
		
		return null;
	}
	
	
	public override void mergeWithSibling(String sinkKey, BPlusTreeNode rightSibling)
    {
		BPlusTreeInternalNode rightSiblingNode = (BPlusTreeInternalNode)rightSibling;
		
		int j = this.getKeyCount();
		this.setKey(j++, sinkKey);
		
		for (int i = 0; i < rightSiblingNode.getKeyCount(); ++i) {
			this.setKey(j + i, rightSiblingNode.getKey(i));
		}
		for (int i = 0; i < rightSiblingNode.getKeyCount() + 1; ++i) {
			this.setChild(j + i, rightSiblingNode.getChild(i));
		}
		this.keyCount += 1 + rightSiblingNode.getKeyCount();
		
		this.setRightSibling(rightSiblingNode.rightSibling);
		if (rightSiblingNode.rightSibling != null)
			rightSiblingNode.rightSibling.setLeftSibling(this);
	}
	
	public override String transferFromSibling(String dropKey, BPlusTreeNode sibling, int borrowIndex) {
        BPlusTreeInternalNode siblingNode = (BPlusTreeInternalNode)sibling;
		
		String upKey = null;
		if (borrowIndex == 0) {
			int index = this.getKeyCount();
			this.setKey(index, dropKey);
			this.setChild(index + 1, siblingNode.getChild(borrowIndex));			
			this.keyCount += 1;
			
			upKey = siblingNode.getKey(0);
			siblingNode.deleteAt(borrowIndex);
		}
		else {
			this.insertAt(0, dropKey, siblingNode.getChild(borrowIndex + 1), this.getChild(0));
			upKey = siblingNode.getKey(borrowIndex);
			siblingNode.deleteAt(borrowIndex);
		}
		
		return pushUpKey;
	}

    public BPlusTreeNode lenderNode { get; set; }

    public string pushUpKey { get; set; }
    }
}
