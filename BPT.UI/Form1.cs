using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BPT.Implementation;

namespace BPT.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BPlusTree tree = new BPlusTree();
            tree.Insert("john", new char[224]);
            tree.Insert("praj", new char[224]);
            tree.Insert("prajakta", new char[224]);
            tree.Insert("BOB", new char[224]);
            tree.Insert("zen", new char[224]);
            tree.Insert("abc", new char[224]);
            tree.Insert("aaa", new char[224]);
            tree.Insert("mno", new char[224]);
            tree.Insert("mat", new char[224]);
            List<string> snapshotList = tree.Snapshot();
            List<string> firstAndLast = tree.BFSTreeTraversal();
            List<string> keylist = tree.List();
            tree.Delete("bob");
            tree.Delete("mno");
            List<string> keylist2 = tree.List();
            tree.Insert("cat", new char[224]);
            tree.Insert("bat", new char[224]);
            tree.Insert("man", new char[224]);
            tree.Update("man", "This is a test");
            tree.Insert("joh", new char[224]);
            tree.Insert("prj", new char[224]);
            tree.Insert("zip", new char[224]);
            String result1 = tree.Search("man");
            tree.Insert("avi", new char[224]);
            tree.Insert("gun", new char[224]);
            tree.Insert("fun", new char[224]);
            List<string> keylist1 = tree.List();
            String result = tree.Search("bob");
        }
    }
}
