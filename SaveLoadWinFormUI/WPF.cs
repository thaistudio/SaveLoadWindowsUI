using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Control = System.Windows.Controls.Control;

namespace SaveLoadUI
{
    public class WPF
    {
        static XmlNode checkboxesNode = null;
        static XmlNode comboboxesNode = null;
        static XmlNode textboxesNode = null;
        static XmlNode buttonsNode = null;
        static XmlNode radiobuttonsNode = null;

        static string checkboxesNodeName = "Checkboxes";
        static string comboboxesNodeName = "Comboboxes";
        static string textboxesNodeName = "Textboxes";
        static string buttonsNodeName = "Buttons";
        static string radiobuttonsNodeName = "Radiobuttons";

        static string xmlPath = @"//WindowsForm/";

        public static void WriteErrors(string path, List<string> errors)
        {
            File.WriteAllLines(path, errors);
        }

        /// <summary>
        /// Check if a node path exists
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="groupboxNodeName"></param>
        /// <param name="nodeName"></param>
        /// <param name="HasGroupBox"></param>
        /// <returns></returns>
        static bool NodeExists(XmlDocument xmlDoc, string groupboxNodeName, string nodeName, bool HasGroupBox)
        {
            string firstChildName = ((XmlNode)xmlDoc).FirstChild.Name;
            if (HasGroupBox)
            {
                if (xmlDoc.SelectSingleNode("/" + firstChildName + "/" + groupboxNodeName + "/" + nodeName) == null) return true;
                else return false;
            }
            else
            {
                if (xmlDoc.SelectSingleNode("/" + firstChildName + "/" + nodeName) == null) return true;
                else return false;
            }
        }

        /// <summary>
        /// This is where the real saving action happens. Enclose the code into this method to avoid repitition in SaveWinFormUI()
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="control"></param>
        /// <param name="HasGroupBox"></param>
        /// <param name="groupboxNode"></param>
        static void SaveAction(XmlDocument xmlDoc, Control control, bool HasGroupBox, XmlNode groupboxNode)
        {
            string groupboxNodeName = null;
            if (groupboxNode != null) groupboxNodeName = groupboxNode.Name;

            if (control is CheckBox)
            {
                if (NodeExists(xmlDoc, groupboxNodeName, checkboxesNodeName, HasGroupBox)) checkboxesNode = AddNode(xmlDoc, checkboxesNodeName, null, null, null, groupboxNode); // Create this node only once
                CheckBox cb = (CheckBox)control;
                XmlNode checkboxNode = AddNode(xmlDoc, cb.Name, cb.IsChecked.ToString(), "Text", cb.Content.ToString(), checkboxesNode);
            }

            // Save labels
            //if (control is Label)
            //{
            //    if (NodeExists(xmlDoc, groupboxNodeName, labelsNodeName, HasGroupBox)) labelsNode = AddNode(xmlDoc, labelsNodeName, null, null, null, groupboxNode);
            //    Label lb = (Label)control;
            //    XmlNode labelNode = AddNode(xmlDoc, lb.Name, lb.Content.ToString(), null, null, labelsNode);
            //}

            // Save comboboxes
            if (control is ComboBox)
            {
                if (NodeExists(xmlDoc, groupboxNodeName, comboboxesNodeName, HasGroupBox)) comboboxesNode = AddNode(xmlDoc, comboboxesNodeName, null, null, null, groupboxNode);

                ComboBox cbb = (ComboBox)control;
                XmlNode comboboxNode = AddNode(xmlDoc, cbb.Name, null, null, null, comboboxesNode);
                foreach (var item in cbb.Items)
                {

                    XmlNode cbContentNode = AddNode(xmlDoc, "Content", item.ToString(), null, null, comboboxNode);
                }
                XmlNode comboboxSelectedTextNode = AddNode(xmlDoc, "SelectedText", cbb.Text, null, null, comboboxNode);
            }

            // Save textboxes
            if (control is TextBox)
            {
                if (NodeExists(xmlDoc, groupboxNodeName, textboxesNodeName, HasGroupBox)) textboxesNode = AddNode(xmlDoc, textboxesNodeName, null, null, null, groupboxNode);

                TextBox tb = (TextBox)control;
                XmlNode textboxNode = AddNode(xmlDoc, tb.Name, tb.Text, null, null, textboxesNode);
            }

            // Save buttons
            if (control is Button)
            {
                if (NodeExists(xmlDoc, groupboxNodeName, buttonsNodeName, HasGroupBox)) buttonsNode = AddNode(xmlDoc, buttonsNodeName, null, null, null, groupboxNode);

                Button bt = (Button)control;
                XmlNode buttonNode = AddNode(xmlDoc, bt.Name, bt.Content.ToString(), null, null, buttonsNode);
            }

            // Save radio buttons
            if (control is RadioButton)
            {
                if (NodeExists(xmlDoc, groupboxNodeName, radiobuttonsNodeName, HasGroupBox)) radiobuttonsNode = AddNode(xmlDoc, radiobuttonsNodeName, null, null, null, groupboxNode);

                RadioButton rbt = (RadioButton)control;
                XmlNode radiobuttonNode = AddNode(xmlDoc, rbt.Name, rbt.IsChecked.ToString(), null, null, radiobuttonsNode);
            }
        }


        static void LoadComboboxAction(XmlDocument xmlDoc, Control control, string groupBoxNodeName = null)
        {
            if (control is ComboBox)
            {
                ComboBox cbb = (ComboBox)control;

                if (cbb.HasItems) return; // Do not load for combo box that is already loaded

                string cbbName = cbb.Name;

                // Load contents
                string cbbPath;
                if (groupBoxNodeName == null) cbbPath = xmlPath + comboboxesNodeName + "/" + cbbName + "/Content";
                else cbbPath = xmlPath + groupBoxNodeName + "/" + comboboxesNodeName + "/" + cbbName + "/Content";
                XmlNodeList comboBoxNodes = xmlDoc.SelectNodes(cbbPath);

                if (comboBoxNodes != null)
                {
                    cbb.Items.Clear();
                    foreach (XmlNode node in comboBoxNodes)
                        cbb.Items.Add(node.InnerText);
                }

                // Load selected item
                if (groupBoxNodeName == null) cbbPath = xmlPath + comboboxesNodeName + "/" + cbbName + "/SelectedText";
                else cbbPath = xmlPath + groupBoxNodeName + "/" + comboboxesNodeName + "/" + cbbName + "/SelectedText";
                XmlNode comboBoxNode = xmlDoc.SelectSingleNode(cbbPath);

                if (comboBoxNode != null)
                {
                    FindComboBoxItemByText(cbb, comboBoxNode.InnerText);
                }
            }
        }


        static void LoadAction(XmlDocument xmlDoc, Control control, string groupBoxNodeName = null)
        {
            if (control is CheckBox)
            {
                CheckBox cb = (CheckBox)control;
                string cbName = cb.Name;

                string cbPath;
                if (groupBoxNodeName == null) cbPath = xmlPath + checkboxesNodeName + "/" + cbName;
                else cbPath = xmlPath + groupBoxNodeName + "/" + checkboxesNodeName + "/" + cbName;
                XmlNode checkBoxNode = xmlDoc.SelectSingleNode(cbPath);

                if (checkBoxNode.InnerText == "True") cb.IsChecked = true;
                else cb.IsChecked = false;
            }

            // Load comboboxes
            if (control is ComboBox)
            {
                ComboBox cbb = (ComboBox)control;
                string cbbName = cbb.Name;

                string cbbPath;
                if (groupBoxNodeName == null) cbbPath = xmlPath + comboboxesNodeName + "/" + cbbName + "/SelectedText";
                else cbbPath = xmlPath + groupBoxNodeName + "/" + comboboxesNodeName + "/" + cbbName + "/SelectedText";
                XmlNode comboBoxNode = xmlDoc.SelectSingleNode(cbbPath);

                if (comboBoxNode != null)
                {
                    FindComboBoxItemByText(cbb, comboBoxNode.InnerText);
                }
            }

            // Save textboxes
            if (control is TextBox)
            {
                TextBox tb = (TextBox)control;
                string tbName = tb.Name;

                string tbPath;
                if (groupBoxNodeName == null) tbPath = xmlPath + textboxesNodeName + "/" + tbName;
                else tbPath = xmlPath + groupBoxNodeName + "/" + textboxesNodeName + "/" + tbName;
                XmlNode textBoxNode = xmlDoc.SelectSingleNode(tbPath);

                tb.Text = textBoxNode.InnerText;
            }

            // Save buttons
            if (control is Button)
            {
                Button bt = (Button)control;
                string btName = bt.Name;

                string btPath;
                if (groupBoxNodeName == null) btPath = xmlPath + buttonsNodeName + "/" + btName;
                else btPath = xmlPath + groupBoxNodeName + "/" + buttonsNodeName + "/" + btName;
                XmlNode buttonNode = xmlDoc.SelectSingleNode(btPath);

                bt.Content = buttonNode.InnerText;
            }

            // Save radio buttons
            if (control is RadioButton)
            {
                RadioButton rb = (RadioButton)control;
                string rbName = rb.Name;

                string rbPath;
                if (groupBoxNodeName == null) rbPath = xmlPath + radiobuttonsNodeName + "/" + rbName;
                else rbPath = xmlPath + groupBoxNodeName + "/" + radiobuttonsNodeName + "/" + rbName;
                XmlNode radioButtonNode = xmlDoc.SelectSingleNode(rbPath);

                rb.IsChecked = Convert.ToBoolean(radioButtonNode.InnerText);
            }
        }

        static Type GetBaseType(Type item, string propertyName)
        {
            Type itemBaseType = item.BaseType;
            if (itemBaseType != null && itemBaseType.GetProperty(propertyName) == null) return item;
            if (itemBaseType == null) return item;
            else itemBaseType = GetBaseType(itemBaseType, propertyName);

            return itemBaseType;
        }

        static void FindComboBoxItemByText(ComboBox cb, string toBeComparedText)
        {
            int index = 0;

            ItemCollection cbContent = cb.Items;

            // Get displayed property
            string displayedProperty = cb.DisplayMemberPath;

            // Find the item that displayedProperty matches toBeComparedText
            foreach (object item in cbContent)
            {
                Type itemType = GetBaseType(item.GetType(), displayedProperty);
                object propertyValue;
                if (string.IsNullOrEmpty(displayedProperty)) propertyValue = item.ToString(); // item is string
                else propertyValue = itemType.GetProperty(displayedProperty).GetValue(item); // item is a class
                if (propertyValue != null)
                {
                    if (propertyValue.ToString() == toBeComparedText)
                    {
                        index = cb.Items.IndexOf(item);
                        break;
                    }
                }
            }
            cb.SelectedIndex = index;
        }

        public static void LoadComboBoxItemsSource(Window window, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(path)) return; // Make sure there is something to load

            xmlDoc.Load(path);

            List<Control> allControls = new List<Control>();
            List<FrameworkElement> listElem = GetFrameworkElments(window);
            GetControls(listElem, allControls);

            foreach (Control control in allControls)
            {
                // Actions if there are groupboxes
                if (control is GroupBox)
                {
                    GroupBox groupBox = (GroupBox)control;
                    foreach (Control gbControl in GetControlsInGroupBox(groupBox))
                        LoadComboboxAction(xmlDoc, gbControl, groupBox.Name);
                }

                // Non-groupboxes
                LoadAction(xmlDoc, control);
            }
        }

        public static void LoadWinFormUI(Window winForm, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(path)) return; // Make sure there is something to load

            xmlDoc.Load(path);

            List<Control> allControls = new List<Control>();
            List<FrameworkElement> listElem = GetFrameworkElments(winForm);
            GetControls(listElem, allControls);

            foreach (Control control in allControls)
            {
                // Actions if there are groupboxes
                if (control is GroupBox)
                {
                    GroupBox groupBox = (GroupBox)control;
                    foreach (Control gbControl in GetControlsInGroupBox(groupBox)) LoadAction(xmlDoc, gbControl, groupBox.Name);
                }

                // Non-groupboxes
                LoadAction(xmlDoc, control);
            }
        }

        /// <summary>
        /// Save Windows Form UI to a Xml File
        /// </summary>
        /// <param name="winForm">the form to save</param>
        /// <param name="path">saving path</param>
        public static void SaveWinFormUI(Window winForm, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode winFormNode = AddNode(xmlDoc, "WindowsForm", null, "Name", winForm.Name);
            XmlAttribute winFormAttribute = xmlDoc.CreateAttribute("HostMemberId");
            winFormNode.Attributes.Append(winFormAttribute);

            List<Control> allControls = new List<Control>();
            List<FrameworkElement> listElem = GetFrameworkElments(winForm);
            GetControls(listElem, allControls);

            foreach (Control control in allControls.OrderBy(x => x.GetType().Name))
            {
                // If there are groupboxes
                if (control is GroupBox)
                {
                    GroupBox gb = (GroupBox)control;
                    if (gb.Content == null) continue;
                    XmlNode groupboxNode = AddNode(xmlDoc, gb.Name, null, "Name", gb.Content.ToString(), winFormNode);

                    foreach (Control gbControl in GetControlsInGroupBox(gb)) SaveAction(xmlDoc, gbControl, true, groupboxNode);
                }

                // Non-groupboxes
                SaveAction(xmlDoc, control, false, winFormNode);
            }

            string directory = GetDirectoryFromPath(path);
            var v = Directory.Exists(directory);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            xmlDoc.Save(path);
        }

        public static string GetDirectoryFromPath(string path)
        {
            string fileName = @"\" + path.Split('\\').LastOrDefault();
            return path.Substring(0, path.Length - fileName.Length);
        }

        static List<FrameworkElement> GetFrameworkElments(Window window)
        {
            Panel panel = (Panel)window.Content;
            UIElementCollection elements = panel.Children;
            return elements.Cast<FrameworkElement>().ToList();
        }

        static void GetControls(List<FrameworkElement> listElem, List<Control> allControls)
        {
            foreach (FrameworkElement element in listElem)
            {
                if (element is Panel)
                {
                    Panel panel = (Panel)element;
                    UIElementCollection elements = panel.Children;
                    List<FrameworkElement> listGridElem = elements.Cast<FrameworkElement>().ToList();
                    GetControls(listGridElem, allControls);
                }
                if (element is Control) allControls.Add((Control)element);
            }
        }

        static IEnumerable<Control> GetControlsInGroupBox(GroupBox groupBox)
        {
            object content = groupBox.Content;
            List<Control> controls = new List<Control>();
            if (content == null) return controls;
            if (content is Control) controls.Add((Control)content);
            try
            {
                Panel panel = (Panel)groupBox.Content;
                UIElementCollection elements = panel.Children;
                List<FrameworkElement> listElem = elements.Cast<FrameworkElement>().ToList();
                return listElem.OfType<Control>();
            }
            catch (Exception)
            {
                return controls;
            }
        }


        #region XMLHelper

        static XmlNode AddNode(XmlDocument xmlDoc, string newNodeName,
            string innerText = null,
            string attributeName = null, string attributeValue = null,
            XmlNode parentNode = null)
        {
            XmlNode newNode = xmlDoc.CreateElement(newNodeName);

            // Add innertext
            if (innerText != null)
            {
                newNode.InnerText = innerText;
            }

            // Add if attribute if there is any
            if (attributeName != null)
            {
                XmlAttribute newNodeAttribute = xmlDoc.CreateAttribute(attributeName);
                if (attributeValue != null) newNodeAttribute.Value = attributeValue;
                newNode.Attributes.Append(newNodeAttribute);
            }


            if (parentNode != null) // Append newNode to parentNode
            {
                parentNode.AppendChild(newNode);
            }
            else // Append newNode to xmlDoc
            {
                xmlDoc.AppendChild(newNode);
            }

            return newNode;
        }

        public static void AddAttribute(XmlDocument xmlDoc, string attributeName, string attributeVal, XmlNode node)
        {
            XmlAttribute xmlAttribute = xmlDoc.CreateAttribute(attributeName);
            xmlAttribute.Value = attributeVal;
            node.Attributes.Append(xmlAttribute);
        }
        #endregion
    }
}
