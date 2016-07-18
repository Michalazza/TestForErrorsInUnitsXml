using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestForErrorsInUnitsXml
{
    public class XmlReaderAndWriter
    {
        public List<XmlNode> CombiningXmlLinksData(string xmlCriteria, string xmlAssessment)
        {
            var criteriaData = new XmlDocument();
            var assessmentData = new XmlDocument();
            criteriaData.Load(xmlCriteria);
            assessmentData.Load(xmlAssessment);
            criteriaData["AuthorIT"]["Objects"].InnerXml += assessmentData["AuthorIT"]["Objects"].InnerXml;

            var combinedData = criteriaData.GetElementsByTagName("Topic")
                .Cast<XmlNode>()
                .Where(x => x["Object"]["Description"].InnerText.Equals("Elements and Performance Criteria")
                || x["Object"]["Description"].InnerText.Equals("Performance Evidence")
                || x["Object"]["Description"].InnerText.Equals("Knowledge Evidence")
                || x["Object"]["Description"].InnerText.Equals("Assessment Conditions")).ToList();

            return combinedData;
        }

        public XmlDocument AddXmlDataToUnit(string xmlCriteria, string xmlAssessment)
        {
            var topicList = CombiningXmlLinksData(xmlCriteria, xmlAssessment);

            XmlDocument createDocument = new XmlDocument();
            XmlNode root = createDocument.CreateElement("Matrix");
            foreach (XmlNode topicNodes in topicList)
            {
                //Title of each Element
                XmlNode headingNodes = createDocument.CreateElement(topicNodes["Object"]["Description"].InnerText.Replace(" ", ""));
                //Checks for Data inside for each topic
                XmlNode textNodes = topicNodes["Text"];
                //Sees if there is any data
                if (textNodes.ChildNodes.Count >= 1)
                {
                    //Checks content of each child elementand then adds it
                    if (textNodes.FirstChild.Name.Equals("table"))
                    {
                        headingNodes.AppendChild(GetTableData(textNodes.FirstChild, createDocument));
                    }
                    else
                    {
                        //headingNodes.AppendChild(GetListData(textNodes, createDocument));
                    }

                }
                root.AppendChild(headingNodes);
            }
            createDocument.AppendChild(root);
            return createDocument;
        }

        public XmlNode GetTableData(XmlNode textElement, XmlDocument xmlDocument)
        {
            XmlNode data = xmlDocument.CreateElement("Data");

            //Each Row
            foreach (XmlNode row in textElement)
            {

                if (row != row.ParentNode.FirstChild && row != row.ParentNode.FirstChild.NextSibling)
                {
                    XmlNode element = xmlDocument.CreateElement("Elements");
                    //Each Column                        
                    foreach (XmlNode column in row)
                    {
                        if (column.Name.Equals("td") && !column.InnerText.Equals(""))
                        {
                            //Create Element with its list
                            XmlNode elementTopic = xmlDocument.CreateElement("ElementTopic");
                            XmlNode criteriaList = xmlDocument.CreateElement("CriteriaList");
                            //Data Inside the Column
                            foreach (XmlNode dataInsideColumn in column)
                            {
                                //Checks if its in the first column          
                                if (column == column.ParentNode.FirstChild)
                                {
                                    elementTopic.InnerXml = column.InnerXml;
                                    element.AppendChild(elementTopic);
                                }
                                //If not in first column gets all the data inside
                                else
                                {
                                    XmlNode criterion = xmlDocument.CreateElement("Criterion");
                                    XmlNode performanceCriterionNumber = xmlDocument.CreateElement("PerformanceCriterionNumber");
                                    XmlNode performanceCriterion = xmlDocument.CreateElement("PerforamceCriterion");

                                    var split = dataInsideColumn.InnerXml.IndexOf(' ');
                                    performanceCriterionNumber.InnerXml = dataInsideColumn.InnerXml.Substring(0, split);
                                    performanceCriterion.InnerXml = dataInsideColumn.InnerXml.Substring(split);

                                    criterion.AppendChild(performanceCriterionNumber);
                                    criterion.AppendChild(performanceCriterion);
                                    criteriaList.AppendChild(criterion);
                                    //Checks if its the last list item so it will add list to the topic
                                    if (dataInsideColumn == column.LastChild && column == column.ParentNode.LastChild)
                                    {
                                        element.AppendChild(criteriaList);
                                    }
                                }
                            }
                        }
                    }
                    data.AppendChild(element);
                }

            }
            return data;
        }

        public XmlNode GetListData(XmlNode textXml, XmlDocument xmlDocument)
        {
            XmlNode data = xmlDocument.CreateElement("Data");
            var count = 0;
            var id = "";
            List<string> listId = new List<string>();
            var previousId = "";
            var lists = new List<XmlNode>();
            //var listData = textXml.ChildNodes.Cast<XmlNode>().Where(p => !p.InnerText.Equals("") && !p.Attributes.GetNamedItem("id").Value.Equals("4")).ToList();
            var listData = textXml.ChildNodes.Cast<XmlNode>().Where(p => !p.InnerText.Equals("")).ToList();
            foreach (XmlNode textElement in listData)
            {

                XmlNode heading = xmlDocument.CreateElement("Heading");
                if (textElement.Equals(textElement.ParentNode.FirstChild))
                {
                    XmlNode list = xmlDocument.CreateElement("List");
                    lists.Add(list);
                    previousId = id;
                    heading.InnerText = textElement.InnerText;
                    XmlAttribute headingId = xmlDocument.CreateAttribute("id");
                    //heading.Value = count.ToString();
                    heading.Attributes.Append(headingId);
                    id = textElement.Attributes.GetNamedItem("id").Value;
                    lists[count].AppendChild(heading);
                    listId.Add(id);

                }
                else if (textElement.Name.Equals("p") && !textElement.InnerText.Equals(""))
                {
                    if (!textElement.Attributes.GetNamedItem("id").Value.Equals(id) && !textElement.NextSibling.Attributes.GetNamedItem("id").Value.Equals(id))
                    {
                        count++;
                        XmlNode list = xmlDocument.CreateElement("List");
                        lists.Add(list);
                        previousId = id;
                        heading.InnerText = textElement.InnerText;
                        XmlAttribute headingId = xmlDocument.CreateAttribute("id");
                        heading.Value = count.ToString();
                        heading.Attributes.Append(headingId);
                        id = textElement.Attributes.GetNamedItem("id").Value;
                        lists[count].AppendChild(heading);
                    }
                    else if (textElement.Attributes.GetNamedItem("id").Value.Equals("13"))
                    {

                    }

                }
            }
            return null;
        }

    }
}
