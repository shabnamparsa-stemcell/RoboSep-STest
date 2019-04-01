using System;
using System.Collections.Generic;
using System.Text;

using Tesla.OperatorConsoleControls;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.Common;




namespace GUI_Console
{
    public class ProtocolInfo: ICloneable
    {
        protected QuadrantId initialQuadrant;
        protected string protocolName;
        protected double sampleVolumeMl;
        protected bool markAsCancelled;

        protected int NumOfQuadrantRequired;
 

        public ProtocolInfo(ProtocolInfo protocolInfo)
        {
            if (protocolInfo != null)
            {
                this.initialQuadrant = protocolInfo.initialQuadrant;
                this.protocolName = protocolInfo.protocolName;
                this.sampleVolumeMl = protocolInfo.sampleVolumeMl;
                this.NumOfQuadrantRequired = protocolInfo.NumOfQuadrantRequired;
                this.markAsCancelled = protocolInfo.markAsCancelled;
            }
        }
        public ProtocolInfo(QuadrantId initialQuadrant, string protocolName, double Vol, int QuadrantRequired, bool markAsCancelled)
        {
            this.protocolName = protocolName;
            this.initialQuadrant = initialQuadrant;
            this.sampleVolumeMl = Vol;
            this.NumOfQuadrantRequired = QuadrantRequired;
            this.markAsCancelled = markAsCancelled;
        }
        public QuadrantId Quadrant
        {
            get
            {
                return this.initialQuadrant;
            }
        }
        public string ProtocolName
        {
            get
            {
                return this.protocolName;
            }
        }
        public double SampleVolumeUl
        {
            get
            {
                return this.sampleVolumeMl;
            }
        }
                
        public int NumberOfQuadrantRequired
        {
            get
            {
                return this.NumOfQuadrantRequired;
            }
        }

        public bool MarkAsCancelled
        {
            get
            {
                return this.markAsCancelled;
            }
        }

        public virtual Object Clone()
        {
            return MemberwiseClone();
        }
    }








}
