﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=1.1.4322.2032.
// 
namespace Tesla.DataAccess {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="STI")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
    public class RoboSepUser {
        
        /// <remarks/>
        public string UserName;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ProtocolFile")]
        public string[] ProtocolFile;
    }
}
