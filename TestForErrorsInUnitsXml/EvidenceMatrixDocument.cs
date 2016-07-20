using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TestForErrorsInUnitsXml;
using static TestForErrorsInUnitsXml.PerformanceCriteria.Element;
using static TestForErrorsInUnitsXml.PerformanceCriteria;

namespace TestForErrorsInUnitsXml
{
    public class EvidenceMatrixDocument
    {
        public PerformanceCriteria PerformanceCriteria { get; set; }
        public PerformanceEvidence PerformanceEvidence { get; set; }
        public KnowledgeEvidence KnowledgeEvidence { get; set; }
        public AssessmentConditions AssessmentConditions { get; set; }

        public EvidenceMatrixDocument()
        {
            PerformanceCriteria = new PerformanceCriteria();
            PerformanceEvidence = new PerformanceEvidence();
            KnowledgeEvidence = new KnowledgeEvidence();
            AssessmentConditions = new AssessmentConditions();
        }

        public void getDataFromXml(XmlDocument elements)
        {
            foreach (XmlNode topics in elements["Matrix"].ChildNodes)
            {
                if (topics.Name.Equals("ElementsandPerformanceCriteria"))
                {
                    AddPerformanceCriteria(topics);
                }
                else
                {
                    AddEvidence(topics);
                }
            }
        }

        private void AddPerformanceCriteria(XmlNode topic)
        {
            var count = 1;
            foreach (XmlNode Element in topic["Data"])
            {
                var criteriaElement = new Element();
                criteriaElement.Name = Element["ElementTopic"].InnerXml;
                foreach (XmlNode Criteria in Element["CriteriaList"])
                {
                    var criterionElement = new Criterion();
                    criterionElement.Number = Criteria["PerformanceCriterionNumber"].InnerXml;
                    criterionElement.Data = Criteria["PerforamceCriterion"].InnerXml;
                    criteriaElement.Criteria.Add(criterionElement);                    
                }
                PerformanceCriteria.Elements.Add(criteriaElement);
                count++;
            }
        }

        private void AddEvidence(XmlNode topic)
        {
            List<ListItems> someth = new List<ListItems>();
            var heading = topic["Data"]["Heading"].InnerXml;
            var baseList = topic.FirstChild.ChildNodes.Cast<XmlNode>().Where(p => p.Name.Equals("ListData") 
            && p.Attributes.GetNamedItem("Heading").Value.Equals("Default")).ToList();
            foreach(var item in baseList)
            {
                var newItem = new ListItems();
                newItem = checkItemsInside(topic, item, newItem);
                someth.Add(newItem);
            }
            if (topic.Name.Equals("KnowledgeEvidence"))
            {
                KnowledgeEvidence.Heading = heading;
                KnowledgeEvidence.Items = someth;
            }
            else if(topic.Name.Equals("PerformanceEvidence"))
            {
                PerformanceEvidence.Heading = heading;
                PerformanceEvidence.Items = someth;
            }
            else
            {
                AssessmentConditions.Heading = heading;
                AssessmentConditions.Items = someth;
            }
        }

        private ListItems checkItemsInside(XmlNode topic, XmlNode item, ListItems newItem)
        {                       
            var count = item.Attributes.GetNamedItem("count").Value;            
            var list = topic.FirstChild.ChildNodes.Cast<XmlNode>().Where(p => p.Name.Equals("ListData")
            && p.Attributes.GetNamedItem("Heading").Value.Equals(count)).ToList();
            if (list.Count > 0)
            {
                newItem.Heading = true;
                newItem.SetItems();                
                foreach (var items in list)
                {
                    var newerItem = new ListItems();
                    newerItem = checkItemsInside(topic, items, newerItem);
                    newItem.Items.Add(newerItem);
                }
            }
            newItem.Data = item.InnerXml;
            return newItem;
        }
    }

    public class AssessmentConditions
    {
        public string Heading { get; set; }
        public List<ListItems> Items { get; set; }
        public AssessmentConditions()
        {
            Items = new List<ListItems>();
        }
    }

    public class ListItems
    {
        public bool Heading { get; set; }
        public string Data { get; set; }
        public List<ListItems> Items { get; set; }
        public void SetItems()
        {
            Items = new List<ListItems>();
        }
    }

    public class KnowledgeEvidence
    {
        public string Heading { get; set; }
        public List<ListItems> Items { get; set; }
        public KnowledgeEvidence()
        {
            Items = new List<ListItems>();
        }
    }

    public class PerformanceEvidence
    {
        public string Heading { get; set; }
        public List<ListItems> Items { get; set; }
        public PerformanceEvidence()
        {
            Items = new List<ListItems>();
        }
    }

    public class PerformanceCriteria
    {
        public List<Element> Elements { get; set; }

        public PerformanceCriteria()
        {
            Elements = new List<Element>();
        }

        public class Element
        {
            public string Name { get; set; }
            public List<Criterion> Criteria { get; set; }

            public Element()
            {
                Criteria = new List<Criterion>();
            }

            public class Criterion
            {
                public string Number { get; set; }
                public string Data { get; set; }
            }
        }
    }
}
