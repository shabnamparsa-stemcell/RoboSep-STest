using System;
using System.Collections.Generic;
using System.Text;
using Tesla.Common.ProtocolCommand;
using Tesla.Common.Separator;

namespace Tesla.DataAccess
{
    public partial class TopUpMixTransSepTransCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }
        public void SetVolumeType3(VolumeType volumeType,double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume3 relVol = new relativeVolume3();
                relVol.proportion = (decimal)rel_vol;
                this.Item2 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume3 absVol = new absoluteVolume3();
                absVol.value_uL = abs_vol.ToString();
                this.Item2 = absVol;
            }
        }

        public void SetVolumeType4(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume4 relVol = new relativeVolume4();
                relVol.proportion = (decimal)rel_vol;
                this.Item3 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume4 absVol = new absoluteVolume4();
                absVol.value_uL = abs_vol.ToString();
                this.Item3 = absVol;
            }
        }
    }

    public partial class TopUpMixTransCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }
        public void SetVolumeType3(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume3 relVol = new relativeVolume3();
                relVol.proportion = (decimal)rel_vol;
                this.Item2 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume3 absVol = new absoluteVolume3();
                absVol.value_uL = abs_vol.ToString();
                this.Item2 = absVol;
            }
        }

    }


    public partial class TopUpTransSepTransCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }
        public void SetVolumeType3(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume3 relVol = new relativeVolume3();
                relVol.proportion = (decimal)rel_vol;
                this.Item2 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume3 absVol = new absoluteVolume3();
                absVol.value_uL = abs_vol.ToString();
                this.Item2 = absVol;
            }
        }

    }

    public partial class TopUpTransCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }

    }


    public partial class ResusMixSepTransCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }
        public void SetVolumeType3(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume3 relVol = new relativeVolume3();
                relVol.proportion = (decimal)rel_vol;
                this.Item2 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume3 absVol = new absoluteVolume3();
                absVol.value_uL = abs_vol.ToString();
                this.Item2 = absVol;
            }
        }

    }

    public partial class ResusMixCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }

    }
    public partial class MixTransCommand
    {

        public void SetVolumeType(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume relVol = new relativeVolume();
                relVol.proportion = (decimal)rel_vol;
                this.Item = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume absVol = new absoluteVolume();
                absVol.value_uL = abs_vol.ToString();
                this.Item = absVol;
            }
        }
        public void SetVolumeType2(VolumeType volumeType, double rel_vol, int abs_vol)
        {
            if (volumeType == VolumeType.Relative)
            {
                relativeVolume2 relVol = new relativeVolume2();
                relVol.proportion = (decimal)rel_vol;
                this.Item1 = relVol;
            }
            else if (volumeType == VolumeType.Absolute)
            {
                absoluteVolume2 absVol = new absoluteVolume2();
                absVol.value_uL = abs_vol.ToString();
                this.Item1 = absVol;
            }
        }

    }

}
