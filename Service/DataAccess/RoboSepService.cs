﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="STI")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
public partial class RoboSepService {
    
    private superScripts superScriptsField;
    
    private maintenanceScripts maintenanceScriptsField;
    
    private userScripts userScriptsField;
    
    private string loggingLevelField;
    
    /// <remarks/>
    public superScripts superScripts {
        get {
            return this.superScriptsField;
        }
        set {
            this.superScriptsField = value;
        }
    }
    
    /// <remarks/>
    public maintenanceScripts maintenanceScripts {
        get {
            return this.maintenanceScriptsField;
        }
        set {
            this.maintenanceScriptsField = value;
        }
    }
    
    /// <remarks/>
    public userScripts userScripts {
        get {
            return this.userScriptsField;
        }
        set {
            this.userScriptsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string LoggingLevel {
        get {
            return this.loggingLevelField;
        }
        set {
            this.loggingLevelField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="STI")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
public partial class superScripts {
    
    private script[] scriptField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("script")]
    public script[] script {
        get {
            return this.scriptField;
        }
        set {
            this.scriptField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="STI")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
public partial class script {
    
    private command[] commandField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("command")]
    public command[] command {
        get {
            return this.commandField;
        }
        set {
            this.commandField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="STI")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
public partial class command {
    
    private string descriptionField;
    
    private string serverCommandField;
    
    private bool isManualStepField;
    
    private bool isMessageOnlyField;
    
    private bool isBackStopField;
    
    private string axisNameField;
    
    private double axisCoarseField;
    
    private double axisFineField;
    
    private double axisMinField;
    
    private double axisMaxField;
    
    private string axisUnitsField;
    
    private string videoFilenameField;
    
    private string runBarcodeAppField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ServerCommand {
        get {
            return this.serverCommandField;
        }
        set {
            this.serverCommandField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool IsManualStep {
        get {
            return this.isManualStepField;
        }
        set {
            this.isManualStepField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool IsMessageOnly {
        get {
            return this.isMessageOnlyField;
        }
        set {
            this.isMessageOnlyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool IsBackStop {
        get {
            return this.isBackStopField;
        }
        set {
            this.isBackStopField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AxisName {
        get {
            return this.axisNameField;
        }
        set {
            this.axisNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double AxisCoarse {
        get {
            return this.axisCoarseField;
        }
        set {
            this.axisCoarseField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double AxisFine {
        get {
            return this.axisFineField;
        }
        set {
            this.axisFineField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double AxisMin {
        get {
            return this.axisMinField;
        }
        set {
            this.axisMinField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double AxisMax {
        get {
            return this.axisMaxField;
        }
        set {
            this.axisMaxField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string AxisUnits {
        get {
            return this.axisUnitsField;
        }
        set {
            this.axisUnitsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string VideoFilename {
        get {
            return this.videoFilenameField;
        }
        set {
            this.videoFilenameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RunBarcodeApp {
        get {
            return this.runBarcodeAppField;
        }
        set {
            this.runBarcodeAppField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="STI")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
public partial class maintenanceScripts {
    
    private script[] scriptField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("script")]
    public script[] script {
        get {
            return this.scriptField;
        }
        set {
            this.scriptField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="STI")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="STI", IsNullable=false)]
public partial class userScripts {
    
    private script[] scriptField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("script")]
    public script[] script {
        get {
            return this.scriptField;
        }
        set {
            this.scriptField = value;
        }
    }
}
