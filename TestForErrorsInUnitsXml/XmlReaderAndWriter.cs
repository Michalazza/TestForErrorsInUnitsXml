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
            //Create 2 documents from the 2 links
            var criteriaData = new XmlDocument();
            var assessmentData = new XmlDocument();
            criteriaData.Load(xmlCriteria);
            assessmentData.Load(xmlAssessment);
            //Add both link data together
            criteriaData["AuthorIT"]["Objects"].InnerXml += assessmentData["AuthorIT"]["Objects"].InnerXml;
            //Select the nodes I need
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
                        headingNodes.AppendChild(GetListData(textNodes, createDocument));
                    }

                }
                root.AppendChild(headingNodes);
            }
            createDocument.AppendChild(root);
            return createDocument;
        }

        public XmlNode GetTableData(XmlNode textElement, XmlDocument unitXmlDocument)
        {
            XmlNode data = unitXmlDocument.CreateElement("Data");

            //Each Row
            foreach (XmlNode row in textElement)
            {
                if (row != row.ParentNode.FirstChild && row != row.ParentNode.FirstChild.NextSibling)
                {
                    XmlNode element = unitXmlDocument.CreateElement("Element");
                    //Each Column                        
                    foreach (XmlNode column in row)
                    {
                        if (column.Name.Equals("td") && !column.InnerText.Equals(""))
                        {
                            //Create Element with its list
                            XmlNode elementTopic = unitXmlDocument.CreateElement("ElementTopic");
                            XmlNode criteriaList = unitXmlDocument.CreateElement("CriteriaList");
                            //Data Inside the Column
                            foreach (XmlNode dataInsideColumn in column)
                            {
                                //Checks if its in the first column          
                                if (column == column.ParentNode.FirstChild)
                                {
                                    elementTopic.InnerXml = column.FirstChild.InnerXml;
                                    element.AppendChild(elementTopic);
                                }
                                //If not in first column gets all the data inside
                                else
                                {
                                    XmlNode criterion = unitXmlDocument.CreateElement("Criterion");
                                    XmlNode performanceCriterionNumber = unitXmlDocument.CreateElement("PerformanceCriterionNumber");
                                    XmlNode performanceCriterion = unitXmlDocument.CreateElement("PerforamceCriterion");

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

        public XmlNode GetListData(XmlNode textXml, XmlDocument unitXmlDocument)
        {
            //ICTNWK522
            //ICTWHS204
            //Set up Nodes
            XmlNode data = unitXmlDocument.CreateElement("Data");
            //Making heading node from first child
            XmlNode heading = unitXmlDocument.CreateElement("Heading");
            heading.InnerXml = textXml.FirstChild.InnerXml;
            data.AppendChild(heading);
            //Create a list of nodes excluding first entry and notes
            List<XmlNode> listData = textXml.ChildNodes.Cast<XmlNode>().Where(p => !p.InnerXml.Equals("")
                && !p.Attributes.GetNamedItem("id").Value.Equals("4")).ToList();
            //Get notes
            List<XmlNode> notes = textXml.ChildNodes.Cast<XmlNode>().Where(p =>
                !p.InnerXml.Equals("")
                && p.Attributes.GetNamedItem("id").Value.Equals("4")
                && !p.Equals(p.ParentNode.FirstChild)).ToList();

            //Make a list of ids so I can order them
            List<string> listOfId = new List<string>();
            foreach (var item in listData)
            {
                if (!listOfId.Contains(item.Attributes.GetNamedItem("id").Value))
                {
                    listOfId.Add(item.Attributes.GetNamedItem("id").Value);
                }
            }
            //Set a Default heading
            var currentHeading = "Default";

            foreach (var item in listData)
            {
                //Create Node
                XmlNode dataList = unitXmlDocument.CreateElement("ListData");
                //Create Attributes
                XmlAttribute id = unitXmlDocument.CreateAttribute("id");
                XmlAttribute count = unitXmlDocument.CreateAttribute("count");
                XmlAttribute listHeading = unitXmlDocument.CreateAttribute("Heading");
                //Get the current Id
                var currentId = item.Attributes.GetNamedItem("id").Value;
                //Set previous Id and check if it can be something
                var previousId = "";
                if (item.PreviousSibling != null)
                {
                    previousId = item.PreviousSibling.Attributes.GetNamedItem("id").Value;
                }
                //Set value to id
                id.Value = listOfId.IndexOf(currentId).ToString();
                //goes through 
                count.Value = listData.IndexOf(item).ToString();       
                //Checks to see if there is another entry         
                if (item.NextSibling != null
                    && !item.NextSibling.Attributes.GetNamedItem("id").Equals("4"))
                {
                    //Checks to see if the next child is of not the same id and is of greater value
                    var nextId = item.NextSibling.Attributes.GetNamedItem("id").Value;
                    if (!currentId.Equals(nextId)
                        && listOfId.IndexOf(currentId) < listOfId.IndexOf(nextId))
                    {
                        //Creates new heading
                        listHeading.Value = currentHeading;
                        currentHeading = count.Value;                       
                    }
                    else
                    {
                        //Current heading
                        listHeading.Value = currentHeading;                                                
                    }
                }
                //If last element
                else
                {
                    listHeading.Value = currentHeading;
                }
                //Checks to see previous child is of greater value and then add to previous heading
                if (!currentId.Equals("13") && listOfId.IndexOf(currentId) < listOfId.IndexOf(previousId))
                {
                    var something = false;
                    for (int index = (listData.IndexOf(item) - 1); something == false; index--)
                    {
                        var checkHeading = listData[index].Attributes.GetNamedItem("id").Value;
                        if (listOfId.IndexOf(currentId) > listOfId.IndexOf(checkHeading))
                        {
                            something = true;
                            listHeading.Value = index.ToString();
                            currentHeading = index.ToString();
                        }
                    }
                }
                //And if its the main id then just Default
                if (currentId.Equals("13"))
                {
                    listHeading.Value = "Default";
                }                
                dataList.InnerXml = item.InnerXml;
                //Add attributes to item
                dataList.Attributes.Append(listHeading);
                dataList.Attributes.Append(id);
                dataList.Attributes.Append(count);
                //Add item to list
                data.AppendChild(dataList);
            }

            foreach (var item in notes)
            {
                XmlNode note = unitXmlDocument.CreateElement("Note");
                note.InnerXml = item.InnerXml;
                data.AppendChild(note);
            }

            return data;
        }
    }
}
