using BayesProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestsBayes
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void TestCreatingNodeWithNoEvidence()
        {
            Node node = new Node("testNode");
            //test Evidence
            Assert.IsTrue(node.Evidence == "NotPresent");
        }

        [TestMethod]
        public void TestCloningNode()
        {
            Node node = new Node("testNode");
            Node newNode = node.CloneNode();
           
            Assert.IsTrue(node != newNode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The vertex cannot be adjacent to itself!")]
        public void TestCannotAddParentHimself()
        {
            Node node = new Node("testNode");
            node.AddParent("testNode");
        }

        [TestMethod]//fail
        public void TestAddParentHimself()
        {
            Node node = new Node("testNode");
            node.AddParent("testNode");
            
            Assert.IsTrue(node.ParentsNumber == 1);
        }

        [TestMethod]//test invalidity
        public void TestIsChildOf()
        {
            Node child = new Node("childNode");
            Node parent = new Node("parentNode");
            child.AddParent("parentNode");

            Assert.IsTrue(parent.IsChidOf("childNode")==false);
        }
    }

    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void TestAddNodesToGraph()
        {
            Graph g = new Graph();
            g.AddNodes(new Node("v1"),null);
            g.AddNodes(new Node("v2"), null);
            g.AddNodes(new Node("v3"), null);
            Assert.IsTrue(g.GetNodes.Count == 3);
        }

        [TestMethod]
        public void TestKahnSorting()
        {
            Graph g = new Graph();
            g.AddNodes(new Node("v1"), null);
            g.AddNodes(new Node("v2"), null);
            g.AddNodes(new Node("v3"), new string[2]{"v1", "v2"});
            g.AddNodes(new Node("v4"), new string[1] { "v3" });
            g.AddNodes(new Node("v5"), new string[1] { "v3" });
            var sortedList = g.KahnSorting();
            var sortToStr = "";
            foreach(var i in sortedList)
            {
                sortToStr += i.NodeID;
            }
            Assert.IsTrue(sortToStr == "v1v2v3v4v5");
        }

        [TestMethod]//fail
        public void TestKahnSortingCycles()
        {
            Graph g = new Graph();
            g.AddNodes(new Node("v1"), new string[1] { "v3" });
            g.AddNodes(new Node("v2"), new string[1] { "v1" });
            g.AddNodes(new Node("v3"), new string[1] { "v2" });

            var sortedList = g.KahnSorting();
            var sortToStr = "";
            foreach (var i in sortedList)
            {
                sortToStr += i.NodeID;
            }
            Assert.IsTrue(sortToStr == "v1v2v3");
        }

        [ExpectedException(typeof(Exception), "The graph has cycles!")]
        [TestMethod]
        public void TestInvalidityKahnSortingCycles()
        {
            Graph g = new Graph();
            g.AddNodes(new Node("v1"), new string[1] { "v3" });
            g.AddNodes(new Node("v2"), new string[1] { "v1" });
            g.AddNodes(new Node("v3"), new string[1] { "v2" });

            var sortedList = g.KahnSorting();
            var sortToStr = "";
            foreach (var i in sortedList)
            {
                sortToStr += i.NodeID;
            }
            Assert.IsTrue(sortToStr == "v1v2v3");
        }
    }

    [TestClass]
    public class BayesNetworkConstructTests
    {
        [TestMethod]
        public void TestReadNetwork()
        {
            BayesNetwork bayesNetwork = new BayesNetwork("test.txt");
            Assert.IsTrue(bayesNetwork.getNetworkGraph.GetNodes.Count != 0);
        }

        [TestMethod]//fail
        public void TestReadBadNetwork()
        {
            BayesNetwork bayesNetwork = new BayesNetwork("badNet.txt");
            Assert.IsTrue(bayesNetwork.getNetworkGraph.GetNodes.Count != 0);
        }

        [ExpectedException(typeof(System.ArgumentOutOfRangeException), @"Index was out of range. Must be non-negative and less than the size of the collection")]
        [TestMethod]
        public void TestInvalidityReadBadNetwork()
        {
            BayesNetwork bayesNetwork = new BayesNetwork("badNet.txt");
            Assert.IsTrue(bayesNetwork.getNetworkGraph.GetNodes.Count != 0);
        }

        [ExpectedException(typeof(System.IO.FileNotFoundException), @"Could not find file 'D:\Andrei\Facultate\AN4SEM1\IA\Project-AI\TestsBayes\bin\Debug\badNet.txt")]
        [TestMethod]
        public void TestReadNetworkFromInexistentFile()
        {
            BayesNetwork bayesNetwork = new BayesNetwork("badPath.txt");
            Assert.IsTrue(bayesNetwork.getNetworkGraph.GetNodes.Count != 0);
        }
    }

    [TestClass]
    public class AlghoritmTests
    {
        private InferenceByEnumeration inf = new InferenceByEnumeration(new BayesNetwork("test.txt"));

        [TestMethod]
        public void Test1QueryNode()
        {

            var p = inf.EnumerationAsk("Fever");
            //P(F-Yes) = 0.1245
            Assert.IsTrue(p[0]==0.1245);
        }

        [TestMethod]
        public void Test2QueryNode()
        {

            var p = inf.EnumerationAsk("Fever");
            //P(F-No) = 0.8755
            Assert.IsTrue(p[1] == 0.8755);
        }

        [TestMethod]
        public void Test3QueryNode()
        {

            var p = inf.EnumerationAsk("Fever");
            double sum = 0.0;
            foreach(var i in p)
            {
                sum += i;
            }
            //Test P(Yes)+P(No) = 1
            Assert.IsTrue(sum == 1.00);
        }

        [TestMethod]
        public void Test4QueryNode()
        {
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Anorexia", "Yes");
            var p = inf.EnumerationAsk("Flu");
            
            //Test P(Yes) = 0.396281443674809
            Assert.IsTrue(p[0] == 0.39628144367480855);
        }

        [TestMethod]//fail
        public void Test5QueryNode() 
        {
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Anorexia", "Yes");
            var p = inf.EnumerationAsk("Flu");

            //Test P(No) = 0.396281443674809
            Assert.IsTrue(p[1] == 0.396281443674809);
        }

        [TestMethod]//Invalidity
        public void Test6QueryNode() 
        {
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Anorexia", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fever", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Flu", "Yes");
            var p = inf.EnumerationAsk("Fever");

            //Test P(Yes) != 0
            Assert.IsTrue(p[0] != 0);
        }

        [TestMethod]
        public void Test7QueryNode()
        {
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "No");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Abscess", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Flu", "No");
            var p = inf.EnumerationAsk("Anorexia");

            //Test P(No) != 0.842857142857143
            Assert.IsTrue(p[1] == 0.84285714285714275);
        }

        [TestMethod]//Invalidity
        public void Test8QueryNode()
        {
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "No");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Abscess", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Flu", "No");
            var p = inf.EnumerationAsk("Anorexia");

            //Test P(Yes) == 3
            Assert.IsTrue(p[0] != 3);
        }

        [TestMethod]//fail
        public void Test9QueryNode()
        {

            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "No");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Abscess", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Flu", "No");
            var p = inf.EnumerationAsk("Anorexia");
            double sum = 0.0;
            foreach (var i in p)
            {
                sum += i;
            }
            
            Assert.IsTrue(sum > 1.00);
        }

        [TestMethod]//Invalidity
        public void Test10QueryNode()
        {

            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Fatigue", "No");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Abscess", "Yes");
            inf.Netowrk.getNetworkGraph.SetNodeEvidence("Flu", "No");
            var p = inf.EnumerationAsk("Fever");
            double sum = 0.0;
            foreach (var i in p)
            {
                sum += i;
            }
            
            Assert.IsTrue(!(sum < 1.00));
        }

    }
}
