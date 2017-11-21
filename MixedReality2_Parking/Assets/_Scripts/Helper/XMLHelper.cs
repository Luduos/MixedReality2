using System;
using System.Xml;
using UnityEngine;

public static class XMLHelper{

    /// <summary>
    /// Used to read an attribute out of an xml attribute collection
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="name">name of the attribute</param>
    /// <param name="attributes">given attributes</param>
    /// <returns></returns>
    public static T GetAttribute<T>(string name, XmlAttributeCollection attributes)
    {
        XmlAttribute attribute = attributes[name];
        if (null != attribute)
        {
            string attributeValue = attribute.Value;
            try
            {
                return (T)Convert.ChangeType(attributeValue, typeof(T));
            }
            catch (FormatException e1)
            {
                // trying to cut away trailing "m" in string
                attributeValue = attributeValue.Substring(0, attributeValue.Length - 2);
                try
                {
                    return (T)Convert.ChangeType(attributeValue, typeof(T));
                }
                catch (FormatException e2)
                {
                    Debug.Log("Error: " + e2.Message + ". Tried to convert: \"" + attributeValue + "\" to a \"" + typeof(T) + "\". Using default value.");
                }
            }
        }
        return default(T);
    }
}
